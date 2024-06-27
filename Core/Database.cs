using LevelsRanksApi;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace LevelsRanks;

public class Database
{
    private static string? _connectionString;
    private static string? _tableName;
    private readonly ILogger<Database> _logger;
    private readonly LevelsRanks _plugin;

    public Database(LevelsRanks plugin, string? connectionString, string? tableName, ILogger<Database> logger)
    {
        _plugin = plugin;
        _connectionString = connectionString;
        _tableName = tableName;
        _logger = logger;
    }

    public async Task<Dictionary<string, int>> GetCurrentRanksAsync()
    {
        var ranks = new Dictionary<string, int>();

        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var commandText = $"SELECT `steam`, `rank` FROM `{_tableName}`";
            await using var command = new MySqlCommand(commandText, connection);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var steamId = reader.GetString("steam");
                var rank = reader.GetInt32("rank");
                ranks[steamId] = rank;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetCurrentRanksAsync: {ex}");
        }

        return ranks;
    }

    public async Task UpdateUsersInDbWithRetry(IEnumerable<User> users)
    {
        const int maxRetries = 3;
        var retryCount = 0;

        while (retryCount < maxRetries)
            try
            {
                await UpdateUsersInDb(users);
                return;
            }
            catch (MySqlException ex) when (ex.Number == 1213)
            {
                retryCount++;
                if (retryCount == maxRetries) throw;
                await Task.Delay(1000);
            }
    }

    public async Task<List<User>> GetAllUsers()
    {
        var users = new List<User>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var commandText = $"SELECT * FROM `{_tableName}`";
        await using var command = new MySqlCommand(commandText, connection);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            users.Add(new User
            {
                SteamId = reader.GetString("steam"),
                Name = reader.GetString("name"),
                Value = reader.GetInt32("value"),
                Rank = reader.GetInt32("rank"),
                Kills = reader.GetInt32("kills"),
                Deaths = reader.GetInt32("deaths"),
                Shoots = reader.GetInt32("shoots"),
                Hits = reader.GetInt32("hits"),
                Headshots = reader.GetInt32("headshots"),
                Assists = reader.GetInt32("assists"),
                RoundWin = reader.GetInt32("round_win"),
                RoundLose = reader.GetInt32("round_lose"),
                Playtime = reader.GetInt32("playtime"),
                LastConnect = reader.GetInt32("lastconnect")
            });

        return users;
    }

    public async Task CreateTable()
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var commandText = $@"
                CREATE TABLE IF NOT EXISTS `{_tableName}` (
                    `steam` VARCHAR(22) PRIMARY KEY,
                    `name` VARCHAR(32),
                    `value` INT NOT NULL DEFAULT 0,
                    `rank` INT NOT NULL DEFAULT 0,
                    `kills` INT NOT NULL DEFAULT 0,
                    `deaths` INT NOT NULL DEFAULT 0,
                    `shoots` INT NOT NULL DEFAULT 0,
                    `hits` INT NOT NULL DEFAULT 0,
                    `headshots` INT NOT NULL DEFAULT 0,
                    `assists` INT NOT NULL DEFAULT 0,
                    `round_win` INT NOT NULL DEFAULT 0,
                    `round_lose` INT NOT NULL DEFAULT 0,
                    `playtime` INT NOT NULL DEFAULT 0,
                    `lastconnect` INT NOT NULL DEFAULT 0
                );";

        await using var command = new MySqlCommand(commandText, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<User?> GetUserFromDb(string steamId)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var commandText = $"SELECT * FROM `{_tableName}` WHERE `steam` = @steamId";
        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@steamId", steamId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return new User
            {
                SteamId = reader.GetString("steam"),
                Name = reader.GetString("name"),
                Value = reader.GetInt32("value"),
                Rank = reader.GetInt32("rank"),
                Kills = reader.GetInt32("kills"),
                Deaths = reader.GetInt32("deaths"),
                Shoots = reader.GetInt32("shoots"),
                Hits = reader.GetInt32("hits"),
                Headshots = reader.GetInt32("headshots"),
                Assists = reader.GetInt32("assists"),
                RoundWin = reader.GetInt32("round_win"),
                RoundLose = reader.GetInt32("round_lose"),
                Playtime = reader.GetInt32("playtime"),
                LastConnect = reader.GetInt32("lastconnect")
            };

        return null;
    }

    public async Task AddUserToDb(User user)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var commandText = $@"
                INSERT INTO `{_tableName}` (`steam`, `name`, `value`, `rank`, `kills`, `deaths`, `shoots`, `hits`, `headshots`, `assists`, `round_win`, `round_lose`, `playtime`, `lastconnect`)
                VALUES (@steam, @name, @value, @rank, @kills, @deaths, @shoots, @hits, @headshots, @assists, @round_win, @round_lose, @playtime, @lastconnect);";

        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@steam", user.SteamId);
        command.Parameters.AddWithValue("@name", user.Name);
        command.Parameters.AddWithValue("@value", user.Value);
        command.Parameters.AddWithValue("@rank", user.Rank);
        command.Parameters.AddWithValue("@kills", user.Kills);
        command.Parameters.AddWithValue("@deaths", user.Deaths);
        command.Parameters.AddWithValue("@shoots", user.Shoots);
        command.Parameters.AddWithValue("@hits", user.Hits);
        command.Parameters.AddWithValue("@headshots", user.Headshots);
        command.Parameters.AddWithValue("@assists", user.Assists);
        command.Parameters.AddWithValue("@round_win", user.RoundWin);
        command.Parameters.AddWithValue("@round_lose", user.RoundLose);
        command.Parameters.AddWithValue("@playtime", user.Playtime);
        command.Parameters.AddWithValue("@lastconnect", user.LastConnect);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<int?> GetPlayerRankAsync(string steamId)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var commandText = $"SELECT `rank` FROM `{_tableName}` WHERE `steam` = @steamId";
            await using var command = new MySqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@steamId", steamId);

            var rank = await command.ExecuteScalarAsync();
            return rank != null ? (int?)Convert.ToInt32(rank) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetPlayerRankAsync: {ex}");
            return null;
        }
    }

    public async Task UpdatePlayerRankAsync(string steamId, int newRank)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var commandText = $"UPDATE `{_tableName}` SET `rank` = @newRank WHERE `steam` = @steamId";
            await using var command = new MySqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@newRank", newRank);
            command.Parameters.AddWithValue("@steamId", steamId);

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in UpdatePlayerRankAsync: {ex}");
        }
    }

    public static async Task UpdateUsersInDb(IEnumerable<User> users)
    {
        if (!users.Any()) return;

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            foreach (var user in users)
            {
                var commandText = $@"
                        UPDATE `{_tableName}` SET 
                        `name` = @name, 
                        `value` = @value, 
                        `rank` = @rank, 
                        `kills` = @kills, 
                        `deaths` = @deaths, 
                        `shoots` = @shoots, 
                        `hits` = @hits, 
                        `headshots` = @headshots, 
                        `assists` = @assists, 
                        `round_win` = @round_win, 
                        `round_lose` = @round_lose, 
                        `playtime` = @playtime, 
                        `lastconnect` = @lastconnect 
                        WHERE `steam` = @steam;";

                await using var command = new MySqlCommand(commandText, connection, transaction);
                command.Parameters.AddWithValue("@steam", user.SteamId);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@value", user.Value);
                command.Parameters.AddWithValue("@rank", user.Rank);
                command.Parameters.AddWithValue("@kills", user.Kills);
                command.Parameters.AddWithValue("@deaths", user.Deaths);
                command.Parameters.AddWithValue("@shoots", user.Shoots);
                command.Parameters.AddWithValue("@hits", user.Hits);
                command.Parameters.AddWithValue("@headshots", user.Headshots);
                command.Parameters.AddWithValue("@assists", user.Assists);
                command.Parameters.AddWithValue("@round_win", user.RoundWin);
                command.Parameters.AddWithValue("@round_lose", user.RoundLose);
                command.Parameters.AddWithValue("@playtime", user.Playtime);
                command.Parameters.AddWithValue("@lastconnect", user.LastConnect);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<User?> GetUserByNameAsync(string name)
    {
        User? user = null;

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = $"SELECT * FROM `{_tableName}` WHERE name LIKE @Name LIMIT 1";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", "%" + name + "%");

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                user = new User
                {
                    SteamId = reader.GetString("steam"),
                    Name = reader.GetString("name"),
                    Value = reader.GetInt32("value"),
                    Rank = reader.GetInt32("rank"),
                    Kills = reader.GetInt32("kills"),
                    Deaths = reader.GetInt32("deaths"),
                    Shoots = reader.GetInt32("shoots"),
                    Hits = reader.GetInt32("hits"),
                    Headshots = reader.GetInt32("headshots"),
                    Assists = reader.GetInt32("assists"),
                    RoundWin = reader.GetInt32("round_win"),
                    RoundLose = reader.GetInt32("round_lose"),
                    Playtime = reader.GetInt32("playtime"),
                    LastConnect = reader.GetInt32("lastconnect")
                };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetUserByName: {ex}");
        }

        return user;
    }


    public async Task<(int totalPlayers, int playerRank)> GetPlayerRankAndTotalPlayersAsync(string steamId)
    {
        var totalPlayers = 0;
        var playerRank = 0;

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var countQuery = $"SELECT COUNT(*) FROM `{_tableName}`";
            using var countCommand = new MySqlCommand(countQuery, connection);
            totalPlayers = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

            var rankQuery = $@"
                SELECT COUNT(*) + 1
                FROM `{_tableName}`
                WHERE value > (SELECT value FROM `{_tableName}` WHERE steam = @SteamId)";
            using var rankCommand = new MySqlCommand(rankQuery, connection);
            rankCommand.Parameters.AddWithValue("@SteamId", steamId);

            playerRank = Convert.ToInt32(await rankCommand.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetPlayerRankAndTotalPlayers: {ex}");
        }

        return (totalPlayers, playerRank);
    }

    public async Task<List<User>> GetTopPlayersByExperience(int topN)
    {
        var users = new List<User>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var commandText = $"SELECT * FROM `{_tableName}` ORDER BY `value` DESC LIMIT @topN";
        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@topN", topN);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            users.Add(new User
            {
                SteamId = reader.GetString("steam"),
                Name = reader.GetString("name"),
                Value = reader.GetInt32("value"),
                Rank = reader.GetInt32("rank"),
                Kills = reader.GetInt32("kills"),
                Deaths = reader.GetInt32("deaths"),
                Shoots = reader.GetInt32("shoots"),
                Hits = reader.GetInt32("hits"),
                Headshots = reader.GetInt32("headshots"),
                Assists = reader.GetInt32("assists"),
                RoundWin = reader.GetInt32("round_win"),
                RoundLose = reader.GetInt32("round_lose"),
                Playtime = reader.GetInt32("playtime"),
                LastConnect = reader.GetInt32("lastconnect")
            });

        return users;
    }

    public async Task<List<User>> GetTopPlayersByPlaytime(int topN)
    {
        var users = new List<User>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var commandText = $"SELECT * FROM `{_tableName}` ORDER BY `playtime` DESC LIMIT @topN";
        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@topN", topN);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            users.Add(new User
            {
                SteamId = reader.GetString("steam"),
                Name = reader.GetString("name"),
                Value = reader.GetInt32("value"),
                Rank = reader.GetInt32("rank"),
                Kills = reader.GetInt32("kills"),
                Deaths = reader.GetInt32("deaths"),
                Shoots = reader.GetInt32("shoots"),
                Hits = reader.GetInt32("hits"),
                Headshots = reader.GetInt32("headshots"),
                Assists = reader.GetInt32("assists"),
                RoundWin = reader.GetInt32("round_win"),
                RoundLose = reader.GetInt32("round_lose"),
                Playtime = reader.GetInt32("playtime"),
                LastConnect = reader.GetInt32("lastconnect")
            });

        return users;
    }

    public static string? BuildConnectionString(DatabaseConnection connection)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Database = connection.Database,
            UserID = connection.User,
            Password = connection.Password,
            Server = connection.Host,
            Port = (uint)connection.Port,
            Pooling = true,
            MinimumPoolSize = 0,
            MaximumPoolSize = 640,
            ConnectionIdleTimeout = 30
        };

        return builder.ConnectionString;
    }

    public async Task ConnectAsync()
    {
        if (_connectionString == null)
            throw new InvalidOperationException("ConnectionString is not set.");

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
    }
}