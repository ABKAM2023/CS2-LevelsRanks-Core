using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using MenuManager;
using LevelsRanksApi;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Timer = System.Threading.Timer;

namespace LevelsRanks;

[MinimumApiVersion(80)]
public class LevelsRanks : BasePlugin
{
    public override string ModuleName => "[LevelsRanks] Core";
	public override string ModuleAuthor => "ABKAM designed by RoadSide Romeo & Wend4r";
    public override string ModuleVersion => "v1.1.0";
    public DatabaseConnection DatabaseConnection { get; set; } = null!;
    public Database Database { get; set; } = null!;
    public string? DbConnectionString = string.Empty;
    private bool IsRoundEnded;
    public bool ExperienceFromBots { get; set; }
    private string StatisticType { get; set; } = "0";
    public string? TableName { get; set; } = "lvl_base";
    public int MinPlayersCount { get; set; } = 4;
    private bool ShowSpawnMessage { get; set; } = true;
    public int ShowUsualMessage { get; set; } = 1;
    public bool BlockExpDuringWarmup { get; set; }
    public bool GiveExpOnRoundEnd { get; set; }
    public bool AllAgainstAll { get; set; }
    public bool ShowLevelUpMessage { get; set; }
    public bool ShowLevelDownMessage { get; set; }
    public bool PlaySound { get; set; }
    public string SoundLvlUp { get; set; } = null!;
    public string SoundLvlDown { get; set; } = null!;
    public bool ShowRankMessage { get; set; }
    public bool ShowRankList { get; set; }
    public string AdminMenuFlag { get; private set; } = "@lr/admin";
    public string PluginTitle { get; set; } = "Levels Ranks v1.1.0";
    public ConcurrentDictionary<string, User> OnlineUsers { get; set; } = new();
    private readonly ConcurrentDictionary<string, int> _killStreaks = new();

    public readonly List<(string Name, Action<CCSPlayerController> Action)> CustomMenuOptions = new();

    private readonly ConcurrentDictionary<string, int>
        _roundExpChanges = new();

    private ResetStatsCooldown _resetStatsCooldown = new();
    private string _resetStatsCooldownFilePath = null!;
    private bool _showResetMyStats;
    private long _resetMyStatsCooldown;
    private readonly ConcurrentDictionary<string, double> _experienceMultipliers = new();
    private readonly ConcurrentQueue<User> _userUpdateQueue = new();
    private readonly
        ConcurrentQueue<(User user, CCSPlayerController player, int expChange, string eventDescription, char color)>
        _expChangeQueue = new();

    private IMenuApi? _api;
    private readonly PluginCapability<IMenuApi?> _pluginCapability = new("menu:nfcore");
    public double _experienceMultiplier = 1.0;
    private Dictionary<string, double> _playerExperienceMultipliers = new();
    private ILevelsRanksApi? _rankapi;
    private readonly PluginCapability<ILevelsRanksApi> _rankpluginCapability = new("levels_ranks");

    private readonly ILoggerFactory LoggerFactory;

    private readonly
        Queue<(CCSPlayerController Player, string Color, int NewExp, int ExpChange, string EventDescription)>
        _messageQueue = new();

    private readonly TimeSpan batchInterval = TimeSpan.FromSeconds(5);
    private DateTime lastFlushTime = DateTime.UtcNow;

    public LevelsRanks(ILogger<LevelsRanks> logger, ILoggerFactory loggerFactory)
    {
        Logger = logger;
        LoggerFactory = loggerFactory;
    }

    public override void Load(bool hotReload)
    {
        LoadConfig();

        DbConnectionString = Database.BuildConnectionString(DatabaseConnection);

        var databaseLogger = LoggerFactory.CreateLogger<Database>();

        Database = new Database(this, DbConnectionString, TableName, databaseLogger);

        var levelsRanksApiLogger = LoggerFactory.CreateLogger<LevelsRanksApi>();
        _rankapi = new LevelsRanksApi(this, levelsRanksApiLogger);
        Capabilities.RegisterPluginCapability(_rankpluginCapability, () => _rankapi);

        Task.Run(() => Database.CreateTable());


        AddTimer(5.0f, async () => await ProcessUserUpdateQueue(), TimerFlags.REPEAT);
        AddTimer(60.0f, async () => await UpdateOnlineUserPlaytime(), TimerFlags.REPEAT);

        Task.Run(ReauthorizeOnlinePlayers);

        RegisterEventHandlers();
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _api = _pluginCapability.Get();
        if (_api == null) Console.WriteLine("MenuManager Core not found...");
    }

    private void LoadConfig()
    {
        Logger.LogInformation("Loading configuration...");
        var configDirectory = Path.Combine(Application.RootDirectory, "configs/plugins/LevelsRanks");
        var configFilePath = Path.Combine(configDirectory, "database.json");
        var settingsFilePath = Path.Combine(configDirectory, "settings_stats.json");
        var ranksFilePath = Path.Combine(configDirectory, "settings_ranks.json");
        var mainSettingsFilePath = Path.Combine(configDirectory, "settings.json");
        _resetStatsCooldownFilePath = Path.Combine(ModuleDirectory, "reset_stats_cooldown.json");
        _resetStatsCooldown = ResetStatsCooldown.Load(_resetStatsCooldownFilePath);

        DatabaseConnection = ConfigLoader<DatabaseConnection>.Load(configFilePath);

        ExperienceSettings.Initialize(Logger);
        ExperienceSettings.Load(settingsFilePath);

        RanksSettings.Load(ranksFilePath);

        if (File.Exists(mainSettingsFilePath))
        {
            var mainSettings = ConfigLoader<MainSettings>.Load(mainSettingsFilePath);
            TableName = mainSettings.lr_table;
            StatisticType = mainSettings.lr_type_statistics;
            ExperienceFromBots = mainSettings.lr_experience_from_bots == "1";
            MinPlayersCount = int.TryParse(mainSettings.lr_minplayers_count, out var minPlayers) ? minPlayers : 4;
            ShowSpawnMessage = mainSettings.lr_show_spawnmessage == "1";
            ShowUsualMessage = int.TryParse(mainSettings.lr_show_usualmessage, out var showUsualMessage)
                ? showUsualMessage
                : 1;
            BlockExpDuringWarmup = mainSettings.lr_block_warmup == "1";
            GiveExpOnRoundEnd = mainSettings.lr_giveexp_roundend == "1";
            AllAgainstAll = mainSettings.lr_allagainst_all == "1";
            ShowLevelUpMessage = mainSettings.lr_show_levelup_message == "1";
            ShowLevelDownMessage = mainSettings.lr_show_leveldown_message == "1";
            PlaySound = mainSettings.lr_sound == "1";
            SoundLvlUp = mainSettings.lr_sound_lvlup;
            SoundLvlDown = mainSettings.lr_sound_lvldown;
            _showResetMyStats = mainSettings.lr_show_resetmystats == "1";
            _resetMyStatsCooldown = long.TryParse(mainSettings.lr_resetmystats_cooldown, out var cooldown)
                ? cooldown
                : 86400;
            ShowRankMessage = mainSettings.lr_show_rankmessage == "1";
            ShowRankList = mainSettings.lr_show_ranklist == "1";
            PluginTitle = mainSettings.lr_plugin_title;
            AdminMenuFlag = mainSettings.lr_flag_adminmenu;
        }
        else
        {
            var defaultSettings = new MainSettings();
            Directory.CreateDirectory(configDirectory);
            File.WriteAllText(mainSettingsFilePath,
                JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true }));
            StatisticType = defaultSettings.lr_type_statistics;
            TableName = defaultSettings.lr_table;
            ExperienceFromBots = defaultSettings.lr_experience_from_bots == "1";
            MinPlayersCount = 4;
            ShowSpawnMessage = true;
            ShowUsualMessage = 1;
            AllAgainstAll = defaultSettings.lr_allagainst_all == "1";
            ShowLevelUpMessage = defaultSettings.lr_show_levelup_message == "1";
            ShowLevelDownMessage = defaultSettings.lr_show_leveldown_message == "1";
            PlaySound = true;
            SoundLvlUp = defaultSettings.lr_sound_lvlup;
            SoundLvlDown = defaultSettings.lr_sound_lvldown;
            _showResetMyStats = defaultSettings.lr_show_resetmystats == "1";
            _resetMyStatsCooldown = 86400;
            ShowRankMessage = defaultSettings.lr_show_rankmessage == "1";
            ShowRankList = defaultSettings.lr_show_ranklist == "1";
            PluginTitle = defaultSettings.lr_plugin_title;
            AdminMenuFlag = defaultSettings.lr_flag_adminmenu;
        }

        if (DatabaseConnection == null) throw new NullReferenceException("Database connection configuration is null.");
    }

    private void TryFlushMessages()
    {
        if (DateTime.UtcNow - lastFlushTime > batchInterval)
        {
            FlushMessagesAsync();
            lastFlushTime = DateTime.UtcNow;
        }
    }

    private async void FlushMessagesAsync()
    {
        await Task.Delay(100);
        while (_messageQueue.Count > 0)
        {
            var (player, color, newExp, expChange, eventDescription) = _messageQueue.Dequeue();
            if (player.IsValid)
            {
                SendChatMessage(player, color, newExp, expChange, eventDescription);
                await Task.Delay(50);
            }
        }
    }

    private async Task UpdateOnlineUserPlaytime()
    {
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        foreach (var onlineUser in OnlineUsers)
        {
            var user = onlineUser.Value;
            user.Playtime += (int)(currentTime - user.LastConnect);
            user.LastConnect = (int)currentTime;

            _userUpdateQueue.Enqueue(user);
        }

        await ProcessUserUpdateQueue();
    }


    public void SetExperienceMultiplier(string steamId, double multiplier)
    {
        if (multiplier < 0) return;

        _experienceMultipliers[steamId] = multiplier;
    }

    public double GetExperienceMultiplier(string steamId)
    {
        return _experienceMultipliers.TryGetValue(steamId, out var multiplier) ? multiplier : 1.0;
    }

    private int GetActivePlayersCount()
    {
        return Utilities.GetPlayers().Count(p =>
            (p.Team == CsTeam.Terrorist || p.Team == CsTeam.CounterTerrorist) && p.AuthorizedSteamID != null);
    }

    public string GetColorFromLocalizer(string colorKey)
    {
        return ReplaceColorPlaceholders(Localizer[colorKey]);
    }

    public void ApplyExperienceUpdateSync(User user, CCSPlayerController player, int expChange, string eventDescription,
        string colorKey)
    {
        if (expChange == 0) return;
        
        if ((BlockExpDuringWarmup && IsWarmupPeriod()) || (!GiveExpOnRoundEnd && IsRoundEnded)) return;


        var playersCount = GetActivePlayersCount();
        if (playersCount < MinPlayersCount) return;


        var steamId = player.SteamID.ToString();


        var multiplier = GetExperienceMultiplier(steamId);


        if (expChange > 0) expChange = (int)(expChange * multiplier);

        var newExp = user.Value + expChange;
        user.Value = newExp < 0 ? 0 : newExp;


        CheckAndUpdateRank(user);


        _userUpdateQueue.Enqueue(user);


        var color = GetColorFromLocalizer(colorKey);


        switch (ShowUsualMessage)
        {
            case 1:
                SendChatMessage(player, color, user.Value, expChange, eventDescription);
                break;
            case 2:
                AddRoundExpChange(player.AuthorizedSteamID!.SteamId64.ToString(), expChange);
                break;
        }
    }


    public void ApplyExperienceUpdateSyncWithoutLimits(User user, CCSPlayerController player, int expChange,
        string eventDescription, char color)
    {
        if (expChange == 0) return;
        
        var newExp = user.Value += expChange;
        if (newExp < 0) user.Value = newExp = 0;

        _userUpdateQueue.Enqueue(user);

        CheckAndUpdateRank(user);

        if (ShowUsualMessage == 1)
            SendChatMessage2(player, color, user.Value, expChange, eventDescription);
        else if (ShowUsualMessage == 2) AddRoundExpChange(player.AuthorizedSteamID!.SteamId64.ToString(), expChange);
    }

    public void SendChatMessage2(CCSPlayerController player, char color, int newExp, int expChange,
        string eventDescription)
    {
        var expChangeStr = expChange > 0 ? $"+{expChange}" : expChange.ToString();
        var message = Localizer["experience_message", color, newExp, expChangeStr, eventDescription];
        player.PrintToChat(ReplaceColorPlaceholders(message));
    }

    public void SendChatMessage(CCSPlayerController player, string color, int newExp, int expChange,
        string eventDescription)
    {
        var expChangeStr = expChange > 0 ? $"+{expChange}" : expChange.ToString();
        var message = Localizer["experience_message", color, newExp, expChangeStr, eventDescription];
        player.PrintToChat(ReplaceColorPlaceholders(message));
    }


    private bool IsWarmupPeriod()
    {
        var gameRulesProxy =
            Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault();
        return gameRulesProxy != null && gameRulesProxy.GameRules!.WarmupPeriod;
    }

    private async Task ProcessUserUpdateQueue()
    {
        const int batchSize = 10;

        var usersToUpdate = new List<User>();

        while (_userUpdateQueue.Count > 0 && usersToUpdate.Count < batchSize)
            if (_userUpdateQueue.TryDequeue(out var user))
                usersToUpdate.Add(user);

        if (usersToUpdate.Count > 0)
            try
            {
                await Database.UpdateUsersInDbWithRetry(usersToUpdate);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error updating users in database: {e}");
            }
    }


    private void RegisterEventHandlers()
    {
        RegisterListener<Listeners.OnClientAuthorized>((slot, id) =>
        {
            var player = Utilities.GetPlayerFromSlot(slot);

            if (player is null || !player.IsValid) return;

            Server.NextFrame(() => OnClientAuthorized(player, id));
        });

        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        RegisterEventHandler<EventWeaponFire>(OnWeaponFire);
        RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        RegisterEventHandler<EventBombPickup>(OnBombPickup);
        RegisterEventHandler<EventBombDropped>(OnBombDropped);
        RegisterEventHandler<EventBombDefused>(OnBombDefused);
        RegisterEventHandler<EventBombPlanted>(OnBombPlanted);
        RegisterEventHandler<EventHostageRescued>(OnHostageRescued);
        RegisterEventHandler<EventHostageKilled>(OnHostageKilled);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundMvp>(OnRoundMvp);
    }

    private void OnClientAuthorized(CCSPlayerController player, SteamID steamId)
    {
        var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID!.SteamId64);
        var playerName = player.PlayerName;

        Task.Run(async () =>
        {
            try
            {
                var userFromDb = await Database.GetUserFromDb(steamIdStr);
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (userFromDb == null)
                {
                    userFromDb = new User
                    {
                        SteamId = steamIdStr,
                        Name = playerName,
                        LastConnect = (int)currentTime,
                        Value = StatisticType == "1" || StatisticType == "2" ? 1000 : 0,
                        Rank = 1
                    };
                    await Database.AddUserToDb(userFromDb);
                }
                else
                {
                    userFromDb.Name = playerName;
                    userFromDb.LastConnect = (int)currentTime;
                }

                Server.NextFrame(() => { OnlineUsers[steamIdStr] = userFromDb; });
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        });
    }

    private HookResult OnPlayerConnectFull(EventPlayerConnectFull eventPlayerConnectFull, GameEventInfo gameEventInfo)
    {
        var player = eventPlayerConnectFull.Userid;
        if (player == null || player.AuthorizedSteamID == null) return HookResult.Continue;

        var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);

        if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            CheckAndUpdateRank(user);
        else
            Logger.LogWarning($"Player Online with SteamID {steamIdStr} not found in OnlineUsers.");

        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect eventPlayerDisconnect, GameEventInfo gameEventInfo)
    {
        var player = eventPlayerDisconnect.Userid;
        if (player == null || player.AuthorizedSteamID == null) return HookResult.Continue;

        var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
        if (OnlineUsers.TryRemove(steamIdStr, out var user))
        {
            var disconnectTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            user.Playtime += (int)(disconnectTime - user.LastConnect);
            user.LastConnect = (int)disconnectTime;


            _userUpdateQueue.Enqueue(user);
        }

        return HookResult.Continue;
    }

    private void CheckAndUpdateRank(User user)
    {
        Server.NextFrame(() =>
        {
            var newRank = RanksSettings.GetRankForExperience(user.Value, StatisticType);
            if (newRank != user.Rank)
            {
                var steamIdStr = user.SteamId;

                var player = Utilities.GetPlayers().FirstOrDefault(p =>
                    p.AuthorizedSteamID != null &&
                    SteamIdConverter.ConvertToSteamId(p.AuthorizedSteamID.SteamId64) == steamIdStr);

                var rankName = Localizer[$"rank_{newRank}"];

                if (player != null)
                {
                    if (newRank > user.Rank)
                    {
                        player.PrintToChat(
                            ReplaceColorPlaceholders(Localizer["rank_up_message", rankName, user.Name!]));

                        if (PlaySound) player.ExecuteClientCommand($"play {SoundLvlUp}");

                        if (ShowLevelUpMessage)
                            foreach (var otherPlayer in Utilities.GetPlayers().Where(p =>
                                         p.AuthorizedSteamID != null && p.AuthorizedSteamID.SteamId64 !=
                                         player.AuthorizedSteamID!.SteamId64))
                                otherPlayer.PrintToChat(
                                    ReplaceColorPlaceholders(Localizer["rank_up_broadcast", user.Name!, rankName]));
                    }
                    else
                    {
                        player.PrintToChat(
                            ReplaceColorPlaceholders(Localizer["rank_down_message", rankName, user.Name!]));

                        if (PlaySound) player.ExecuteClientCommand($"play {SoundLvlDown}");

                        if (ShowLevelDownMessage)
                            foreach (var otherPlayer in Utilities.GetPlayers().Where(p =>
                                         p.AuthorizedSteamID != null && p.AuthorizedSteamID.SteamId64 !=
                                         player.AuthorizedSteamID!.SteamId64))
                                otherPlayer.PrintToChat(
                                    ReplaceColorPlaceholders(Localizer["rank_down_broadcast", user.Name!, rankName]));
                    }

                    user.Rank = newRank;

                    _userUpdateQueue.Enqueue(user);
                }
                else
                {
                    Logger.LogWarning($"Player with SteamID {steamIdStr} not found.");
                }
            }
        });
    }


    private async Task ReauthorizeOnlinePlayers()
    {
        List<CCSPlayerController>? players = null;


        await Server.NextFrameAsync(() => { players = Utilities.GetPlayers().ToList(); });

        foreach (var player in players!)
        {
            if (player == null || player.AuthorizedSteamID == null) continue;

            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
            var playerName = player.PlayerName;

            try
            {
                var userFromDb = await Database.GetUserFromDb(steamIdStr);
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (userFromDb == null)
                {
                    userFromDb = new User
                    {
                        SteamId = steamIdStr,
                        Name = playerName,
                        LastConnect = (int)currentTime,
                        Value = StatisticType == "1" || StatisticType == "2" ? 1000 : 0
                    };
                    await Database.AddUserToDb(userFromDb);
                }
                else
                {
                    userFromDb.Name = playerName;
                    userFromDb.LastConnect = (int)currentTime;
                }


                await Server.NextFrameAsync(() =>
                {
                    OnlineUsers[steamIdStr] = userFromDb;
                    CheckAndUpdateRank(userFromDb);
                });
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
    private HookResult OnPlayerDeath(EventPlayerDeath eventPlayerDeath, GameEventInfo gameEventInfo)
    {
        var attacker = eventPlayerDeath.Attacker;
        var victim = eventPlayerDeath.Userid;
        var assister = eventPlayerDeath.Assister;
        var headshot = eventPlayerDeath.Headshot;

        if (attacker == null || victim == null) return HookResult.Continue;

        var victimId = victim.AuthorizedSteamID?.SteamId64;
        var attackerId = attacker.AuthorizedSteamID?.SteamId64;

        var victimSteamIdStr = victimId != null ? SteamIdConverter.ConvertToSteamId(victimId.Value) : "BOT";
        var attackerSteamIdStr = attackerId != null ? SteamIdConverter.ConvertToSteamId(attackerId.Value) : "BOT";

        if (!OnlineUsers.TryGetValue(victimSteamIdStr, out var victimUser) && victimId != null)
            return HookResult.Continue;

        if (!OnlineUsers.TryGetValue(attackerSteamIdStr, out var attackerUser) && attackerId != null)
            return HookResult.Continue;
        
        bool isSuicide = attackerSteamIdStr == victimSteamIdStr;

        if (isSuicide)
        {
            ProcessPlayerSuicide(victimUser!, victim);
        }
        else
        {
            if (attacker.IsBot && !ExperienceFromBots)
            {
                if (victimUser != null)
                    ProcessPlayerDeath(victimUser, victim, null, attacker, attackerSteamIdStr, headshot, false);
            }
            else
            {
                ProcessPlayerDeath(victimUser!, victim, attackerUser, attacker, attackerSteamIdStr, headshot, true);
            }

            if (assister != null && assister.AuthorizedSteamID != null)
            {
                var assisterId = assister.AuthorizedSteamID.SteamId64;
                var assisterSteamIdStr = SteamIdConverter.ConvertToSteamId(assisterId);

                if (OnlineUsers.TryGetValue(assisterSteamIdStr, out var assisterUser))
                {
                    assisterUser.Assists++;
                    var assistExp = ExperienceSettings.GetExperience(StatisticType, "lr_assist");
                    ApplyExperienceUpdateSync(assisterUser, assister, (int)assistExp, Localizer["assist"],
                        Localizer["assist_color"]);
                }
                else
                {
                    Logger.LogError($"Assister user not found in OnlineUsers: {assisterSteamIdStr}");
                }
            }
        }

        return HookResult.Continue;
    }
    private void ProcessPlayerSuicide(User victimUser, CCSPlayerController victim)
    {
        if (victim == null)
        {
            Logger.LogError("Victim is null in ProcessPlayerSuicide");
            return;
        }
        
        if (!ExperienceFromBots && victim.IsBot)
        {
            return;
        }

        if (victimUser != null)
        {
            victimUser.Deaths++;
            var expVictim = ExperienceSettings.GetExperience(StatisticType, "lr_suicide");
            expVictim = -Math.Abs(expVictim);
            ApplyExperienceUpdateSync(victimUser, victim, (int)expVictim, Localizer["suicide"], Localizer["suicide_color"]);
        }
    }

    private void ProcessPlayerDeath(User victimUser, CCSPlayerController victim, User? attackerUser,
        CCSPlayerController attacker, string attackerSteamIdStr, bool headshot, bool isKill)
    {
        if (victim == null || attacker == null)
        {
            Logger.LogError("Victim or attacker is null in ProcessPlayerDeath");
            return;
        }
        
        if (!ExperienceFromBots && (attacker.IsBot || victim.IsBot))
        {
            return;
        }

        if (victimUser != null)
        {
            victimUser.Deaths++;
            var expChangeDeath = CalculateExperience(attackerUser, victimUser, headshot, out var expVictim);
            expVictim = -Math.Abs(expVictim);
            ApplyExperienceUpdateSync(victimUser, victim, expVictim, Localizer["death"], Localizer["death_color"]);
        }

        if (isKill && attackerUser != null)
        {
            attackerUser.Kills++;
            var expAttacker = CalculateExperience(attackerUser, victimUser, headshot, out _);
            ApplyExperienceUpdateSync(attackerUser, attacker, expAttacker, Localizer["kill"], Localizer["kill_color"]);

            if (headshot)
            {
                var headshotExp = (int)ExperienceSettings.GetExperience(StatisticType, "lr_headshot");
                ApplyExperienceUpdateSync(attackerUser, attacker, headshotExp, Localizer["headshot"],
                    Localizer["headshot_color"]);
                attackerUser.Headshots++;
            }

            if (_killStreaks.TryGetValue(attackerSteamIdStr, out var streak))
            {
                streak++;
                _killStreaks[attackerSteamIdStr] = streak;
                if (streak >= 2)
                {
                    var streakName = Localizer[$"killstreak_{streak}"];
                    var streakExp =
                        (int)(ExperienceSettings.Experience.Special_Bonuses.TryGetValue($"lr_bonus_{streak}",
                            out var exp)
                            ? exp
                            : 0);
                    ApplyExperienceUpdateSync(attackerUser, attacker, streakExp,
                        Localizer["killstreak_bonus", streakName], Localizer["killstreak_bonus_color"]);
                }
            }
            else
            {
                _killStreaks[attackerSteamIdStr] = 1;
            }
        }
    }

    private int CalculateExperience(User? attacker, User? victim, bool headshot, out int expVictim)
    {
        var expAttacker = 0;
        expVictim = 0;

        if (attacker == null || string.IsNullOrEmpty(attacker.SteamId) || attacker.SteamId == "BOT")
        {
            if (!ExperienceFromBots || StatisticType != "0") return expAttacker;
            attacker = new User { SteamId = "BOT", Value = 0 };
        }

        if (victim == null || string.IsNullOrEmpty(victim.SteamId) || victim.SteamId == "BOT")
        {
            if (!ExperienceFromBots || StatisticType != "0") return expAttacker;
            victim = new User { SteamId = "BOT", Value = 0 };
        }

        switch (StatisticType)
        {
            case "0":
                expAttacker = (int)ExperienceSettings.GetExperience("0", "lr_kill");
                expVictim = (int)-ExperienceSettings.GetExperience("0", "lr_death");
                break;
            case "1":
            {
                var killCoefficient =
                    Math.Max(0.5, Math.Min(2.0, ExperienceSettings.GetExperience("1", "lr_killcoeff")));
                expAttacker = Math.Max(1, (int)Math.Round((float)victim.Value / attacker.Value * 5.0));
                expVictim = -Math.Max(1, (int)Math.Round(expAttacker * killCoefficient));
            }
                break;
            case "2":
            {
                expAttacker = Math.Max(2, (int)Math.Round(victim.Value / 10.0 + 2));
                expVictim = -expAttacker;
            }
                break;
        }

        return expAttacker;
    }

    private HookResult OnHostageKilled(EventHostageKilled eventHostageKilled, GameEventInfo gameEventInfo)
    {
        var player = eventHostageKilled.Userid;

        if (player != null && player.AuthorizedSteamID != null)
        {
            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                var expChange = (int)ExperienceSettings.GetExperience(StatisticType, "lr_hostagekilled");
                ApplyExperienceUpdateSync(user, player, expChange, Localizer["hostage_killed"],
                    Localizer["hostage_killed_color"]);
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnRoundEnd(EventRoundEnd eventRoundEnd, GameEventInfo gameEventInfo)
    {
        try
        {
            var winner = eventRoundEnd.Winner;

            if (ShowUsualMessage == 2)
            {
                foreach (var player in Utilities.GetPlayers())
                    if (player.AuthorizedSteamID != null)
                        SendRoundSummaryMessage(player,
                            SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64));
                _roundExpChanges.Clear();
            }

            foreach (var player in Utilities.GetPlayers())
            {
                if (player.AuthorizedSteamID == null || !player.IsValid || player.Team == CsTeam.Spectator) continue;

                var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
                if (OnlineUsers.TryGetValue(steamIdStr, out var user))
                {
                    var expChange = 0;

                    if (player.Team == (CsTeam)winner)
                    {
                        user.RoundWin++;
                        expChange = (int)ExperienceSettings.GetExperience("0", "lr_winround");
                    }
                    else
                    {
                        user.RoundLose++;
                        expChange = (int)ExperienceSettings.GetExperience("0", "lr_loseround");
                    }

                    ApplyExperienceUpdateSync(user, player, expChange,
                        player.Team == (CsTeam)winner ? Localizer["win_round"] : Localizer["lose_round"],
                        player.Team == (CsTeam)winner ? Localizer["win_round_color"] : Localizer["lose_round_color"]);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString());
        }

        IsRoundEnded = true;
        return HookResult.Continue;
    }


    private void AddRoundExpChange(string steamIdStr, int expChange)
    {
        _roundExpChanges.AddOrUpdate(steamIdStr, expChange, (key, oldValue) => oldValue + expChange);
    }

    private void SendRoundSummaryMessage(CCSPlayerController player, string convertToSteamId)
    {
        var steamId64 = player.AuthorizedSteamID!.SteamId64;
        var steamId64Str = steamId64.ToString();

        if (_roundExpChanges.TryGetValue(steamId64Str, out var totalExpChange))
        {
            Logger.LogInformation($"Sending round summary to {player.PlayerName}: {totalExpChange} points");

            if (totalExpChange > 0)
                player.PrintToChat(
                    ReplaceColorPlaceholders(Localizer["round_summary_positive", totalExpChange]));
            else if (totalExpChange < 0)
                player.PrintToChat(
                    ReplaceColorPlaceholders(Localizer["round_summary_negative", totalExpChange]));
            else
                player.PrintToChat(
                    ReplaceColorPlaceholders(Localizer["round_summary_neutral"]));
        }
        else
        {
            Logger.LogInformation($"No experience change recorded for player {steamId64Str}");
        }
    }


    private HookResult OnWeaponFire(EventWeaponFire eventWeaponFire, GameEventInfo gameEventInfo)
    {
        try
        {
            var player = eventWeaponFire.Userid;
            if (player == null || player.AuthorizedSteamID == null || !player.IsValid) return HookResult.Continue;

            var steamId64 = player.AuthorizedSteamID.SteamId64;
            var steamIdStr = SteamIdConverter.ConvertToSteamId(steamId64);

            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                user.Shoots++;
                _userUpdateQueue.Enqueue(user);
            }
            else
            {
                Logger.LogWarning($"OnWeaponFire: Player {steamIdStr} not found in OnlineUsers");
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.ToString());
        }

        return HookResult.Continue;
    }

    private HookResult OnPlayerHurt(EventPlayerHurt eventPlayerHurt, GameEventInfo gameEventInfo)
    {
        var attacker = eventPlayerHurt.Attacker;
        var victim = eventPlayerHurt.Userid;

        if (attacker != null && attacker != victim && attacker.AuthorizedSteamID != null)
        {
            var steamIdStr = SteamIdConverter.ConvertToSteamId(attacker.AuthorizedSteamID.SteamId64);
            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                user.Hits++;
                _userUpdateQueue.Enqueue(user);
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnBombPickup(EventBombPickup eventBombPickup, GameEventInfo gameEventInfo)
    {
        var player = eventBombPickup.Userid;

        if (player != null && player.AuthorizedSteamID != null && !player.IsBot)
        {
            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                var expChange = (int)ExperienceSettings.GetExperience(StatisticType, "lr_bombpickup");
                ApplyExperienceUpdateSync(user, player, expChange, Localizer["bomb_pickup"],
                    Localizer["bomb_pickup_color"]);
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnBombDropped(EventBombDropped eventBombDropped, GameEventInfo gameEventInfo)
    {
        var player = eventBombDropped.Userid;

        if (player != null && player.AuthorizedSteamID != null)
        {
            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                var expChange = (int)ExperienceSettings.GetExperience(StatisticType, "lr_bombdropped");
                Server.NextFrame(() =>
                {
                    ApplyExperienceUpdateSync(user, player, expChange, Localizer["bomb_dropped"],
                        Localizer["bomb_dropped_color"]);
                });
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnBombDefused(EventBombDefused eventBombDefused, GameEventInfo gameEventInfo)
    {
        var player = eventBombDefused.Userid;

        if (player != null && player.AuthorizedSteamID != null)
        {
            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                var expChange = (int)ExperienceSettings.GetExperience("0", "lr_bombdefused");
                ApplyExperienceUpdateSync(user, player, expChange, Localizer["bomb_defused"],
                    Localizer["bomb_defused_color"]);
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnBombPlanted(EventBombPlanted eventBombPlanted, GameEventInfo gameEventInfo)
    {
        var player = eventBombPlanted.Userid;

        if (player != null && player.AuthorizedSteamID != null)
        {
            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                var expChange = (int)ExperienceSettings.GetExperience("0", "lr_bombplanted");
                ApplyExperienceUpdateSync(user, player, expChange, Localizer["bomb_planted"],
                    Localizer["bomb_planted"]);
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnHostageRescued(EventHostageRescued eventHostageRescued, GameEventInfo gameEventInfo)
    {
        var player = eventHostageRescued.Userid;

        if (player != null && player.AuthorizedSteamID != null)
        {
            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);
            if (OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                var expChange = (int)ExperienceSettings.GetExperience("0", "lr_hostagerescued");
                ApplyExperienceUpdateSync(user, player, expChange, Localizer["hostage_rescued"],
                    Localizer["hostage_rescued_color"]);
            }
        }

        return HookResult.Continue;
    }

    public HookResult OnRoundStart(EventRoundStart eventRoundStart, GameEventInfo gameEventInfo)
    {
        var players = Utilities.GetPlayers().Count(p =>
            (p.Team == CsTeam.Terrorist || p.Team == CsTeam.CounterTerrorist) && p.AuthorizedSteamID != null);

        if (players < MinPlayersCount)
            foreach (var player in Utilities.GetPlayers().Where(p => p.AuthorizedSteamID != null))
            {
                player.PrintToChat(
                    ReplaceColorPlaceholders(Localizer["round_start_message"]));
                player.PrintToChat(
                    ReplaceColorPlaceholders(Localizer["round_start_few_players", players, MinPlayersCount]));
            }
        else if (ShowSpawnMessage)
            foreach (var player in Utilities.GetPlayers().Where(p => p.AuthorizedSteamID != null))
                player.PrintToChat(
                    ReplaceColorPlaceholders(Localizer["round_start_message"]));

        _killStreaks.Clear();
        IsRoundEnded = false;
        return HookResult.Continue;
    }

    private HookResult OnRoundMvp(EventRoundMvp eventRoundMvp, GameEventInfo gameEventInfo)
    {
        var player = eventRoundMvp.Userid;

        if (player == null || player.AuthorizedSteamID == null || !player.IsValid) return HookResult.Continue;

        var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID.SteamId64);

        if (OnlineUsers.TryGetValue(steamIdStr, out var user))
        {
            var mvpExp = ExperienceSettings.GetExperience(StatisticType, "lr_mvpround");
            ApplyExperienceUpdateSync(user, player, (int)mvpExp, Localizer["mvp_round"], Localizer["mvp_round_color"]);
        }
        else
        {
            Logger.LogError($"MVP user not found in OnlineUsers: {steamIdStr}");
        }

        return HookResult.Continue;
    }

[ConsoleCommand("css_rank", "Показать статистику игрока")]
public void HandleRankCommand(CCSPlayerController? player, CommandInfo commandInfo)
{
    if (player == null)
    {
        Server.NextFrame(() =>
        {
            commandInfo.ReplyToCommand(ReplaceColorPlaceholders(Localizer["command_player_only"]));
        });
        return;
    }

    var searchTerm = commandInfo.ArgCount < 2 ? player.PlayerName : commandInfo.GetArg(1);
    bool isSteamId = searchTerm.All(char.IsDigit);

    Task.Run(async () =>
    {
        User? user = null;
        var totalPlayers = 0;
        var playerRank = 0;
        double kdr = 0;

        try
        {
            if (isSteamId)
            {
                var steamId = searchTerm;
                user = await Database.GetUserFromDb(steamId);
            }
            else
            {
                user = await Database.GetUserByNameAsync(searchTerm);
            }

            if (user != null)
            {
                var rankAndTotalPlayers = await Database.GetPlayerRankAndTotalPlayersAsync(user.SteamId!);
                totalPlayers = rankAndTotalPlayers.totalPlayers;
                playerRank = rankAndTotalPlayers.playerRank;
                kdr = user.Deaths > 0 ? (double)user.Kills / user.Deaths : user.Kills;
            }

            var message = user == null
                ? ReplaceColorPlaceholders(Localizer["player_not_found", searchTerm])
                : ReplaceColorPlaceholders(Localizer["player_stats", user.Name!, playerRank, totalPlayers,
                    user.Value, user.Kills, user.Deaths, kdr]);

            Server.NextFrame(() =>
            {
                player.PrintToChat(message);

                if (ShowRankMessage && user != null)
                    foreach (var p in Utilities.GetPlayers().Where(p => p.AuthorizedSteamID != null && p != player))
                        p.PrintToChat(message);
            });
        }
        catch (Exception ex)
        {
            Server.NextFrame(() =>
            {
                player.PrintToChat(
                    ReplaceColorPlaceholders(Localizer["command_error"]));
                Logger.LogError($"Error in HandleRankCommand: {ex}");
            });
        }
    });
}


    [ConsoleCommand("css_lvl", "Открыть меню Levels Ranks")]
    public void OpenLevelsRanksMenu(CCSPlayerController? player, CommandInfo command)
    {
        if (player != null)
        {
            if (_api == null)
            {
                player.PrintToChat(ReplaceColorPlaceholders(Localizer["menu_api_not_found"]));
                return;
            }

            var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID!.SteamId64);
            if (!OnlineUsers.TryGetValue(steamIdStr, out var user))
            {
                player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_data_not_found"]));
                return;
            }

            var menu = _api.NewMenu(PluginTitle);

            if (AdminManager.PlayerHasPermissions(player, AdminMenuFlag))
                menu.AddMenuOption(ReplaceColorPlaceholders(Localizer["admin_panel"]),
                    (p, option) => OpenAdminPanel(p));

            menu.AddMenuOption(ReplaceColorPlaceholders(Localizer["my_stats"]),
                (p, option) => OpenUserStatsMenu(p, user));
            menu.AddMenuOption(ReplaceColorPlaceholders(Localizer["top_players"]),
                (p, option) => OpenTopPlayersMenu(p));
            if (ShowRankList)
                menu.AddMenuOption(ReplaceColorPlaceholders(Localizer["all_ranks"]),
                    (p, option) => OpenAllRanksMenu(p));


            foreach (var menuOption in CustomMenuOptions)
                menu.AddMenuOption(menuOption.Name, (p, option) => menuOption.Action(p));

            menu.Open(player);
        }
    }

    private void OpenAllRanksMenu(CCSPlayerController player)
    {
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["all_ranks"]));

        foreach (var rank in RanksSettings.Ranks.OrderBy(r => r.Key))
        {
            var experienceNeeded = rank.Value.Value0;
            var rankName = Localizer[$"rank_{rank.Key}"];
            menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["rank_item", experienceNeeded, rankName]),
                (p, option) => { });
        }

        menu?.Open(player);
    }

    private void OpenTopPlayersMenu(CCSPlayerController player)
    {
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["top_players"]));
        menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["top_10_experience"]),
            (p, option) => ShowTopPlayersByExperience(p));
        menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["top_10_activity"]),
            (p, option) => ShowTopPlayersByPlaytime(p));
        menu?.Open(player);
    }

    private async void ShowTopPlayersByExperience(CCSPlayerController player)
    {
        var topPlayers = await Database.GetTopPlayersByExperience(10);
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["top_10_experience"]));

        for (var i = 0; i < topPlayers.Count; i++)
        {
            var user = topPlayers[i];
            menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["top_player_item", i + 1, user.Value, user.Name!]),
                (p, option) => { });
        }
        
        Server.NextFrame(() => menu?.Open(player));
    }

    [ConsoleCommand("css_top", "Показать топ игроков по очкам опыта")]
    public void HandleTopCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null)
        {
            Server.NextFrame(() =>
            {
                commandInfo.ReplyToCommand(ReplaceColorPlaceholders(Localizer["command_player_only"]));
            });
            return;
        }

        Server.NextFrame(() => { ShowTopPlayersByExperience(player); });
    }

    private async void ShowTopPlayersByPlaytime(CCSPlayerController player)
    {
        var topPlayers = await Database.GetTopPlayersByPlaytime(10);
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["top_10_activity"]));

        for (var i = 0; i < topPlayers.Count; i++)
        {
            var user = topPlayers[i];
            var playtime = TimeSpan.FromSeconds(user.Playtime);
            var formattedPlaytime = $"{playtime.Days}д {playtime.Hours}ч {playtime.Minutes}м";
            menu?.AddMenuOption(
                ReplaceColorPlaceholders(Localizer["top_player_activity_item", i + 1, formattedPlaytime, user.Name!]),
                (p, option) => { });
        }

        menu?.Open(player);
    }

    private void OpenUserStatsMenu(CCSPlayerController player, User user)
    {
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["my_stats"]));
        menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["show_stats_in_chat"]),
            (p, option) => ShowUserStats(p, user));
        if (_showResetMyStats)
            menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["reset_stats"]),
                (p, option) => ResetUserStats(p, user));

        menu?.Open(player);
    }

    private void ShowUserStats(CCSPlayerController player, User user)
    {
        var playtime = TimeSpan.FromSeconds(user.Playtime);
        var formattedPlaytime = $"{playtime.Days}д {playtime.Hours}ч {playtime.Minutes}м";

        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stats_title"]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_experience", user.Value]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_kills", user.Kills]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_deaths", user.Deaths]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_headshots", user.Headshots]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_assists", user.Assists]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_shoots", user.Shoots]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_hits", user.Hits]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_round_win", user.RoundWin]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_round_lose", user.RoundLose]));
        player.PrintToChat(ReplaceColorPlaceholders(Localizer["user_stat_playtime", formattedPlaytime]));

        _api?.CloseMenu(player);
    }

    private void ResetUserStats(CCSPlayerController player, User user)
    {
        var steamIdStr = SteamIdConverter.ConvertToSteamId(player.AuthorizedSteamID!.SteamId64);
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (_resetStatsCooldown.LastResetTimestamps.TryGetValue(steamIdStr, out var lastResetTime))
            if (currentTime - lastResetTime < _resetMyStatsCooldown)
            {
                var timeLeft = TimeSpan.FromSeconds(_resetMyStatsCooldown - (currentTime - lastResetTime));
                Server.NextFrame(() =>
                {
                    player.PrintToChat(
                        ReplaceColorPlaceholders(Localizer["reset_stats_cooldown", timeLeft.Days, timeLeft.Hours,
                            timeLeft.Minutes, timeLeft.Seconds]));
                    _api?.CloseMenu(player);
                });
                return;
            }

        user.Kills = 0;
        user.Deaths = 0;
        user.Headshots = 0;
        user.Assists = 0;
        user.Shoots = 0;
        user.Hits = 0;
        user.RoundWin = 0;
        user.RoundLose = 0;
        user.Playtime = 0;


        if (StatisticType == "1" || StatisticType == "2")
            user.Value = 1000;
        else
            user.Value = 0;


        Task.Run(async () =>
        {
            try
            {
                await Database.UpdateUsersInDb(new List<User> { user });


                Server.NextFrame(() =>
                {
                    CheckAndUpdateRank(user);
                    player.PrintToChat(ReplaceColorPlaceholders(Localizer["stats_reset_success"]));
                });
            }
            catch (Exception ex)
            {
                Server.NextFrame(() =>
                {
                    player.PrintToChat(ReplaceColorPlaceholders(Localizer["stats_reset_failed"]));
                });
                Logger.LogError($"Error updating user stats in database: {ex}");
            }


            Server.NextFrame(() =>
            {
                _resetStatsCooldown.LastResetTimestamps[steamIdStr] = currentTime;
                _resetStatsCooldown.Save(_resetStatsCooldownFilePath);
                _api?.CloseMenu(player);
            });
        });
    }


    private void OpenAdminPanel(CCSPlayerController player)
    {
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["admin_panel"]));
        menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["grant_revoke_points"]),
            (p, option) => OpenGrantRevokeMenu(p));
        menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["reload_plugin_settings"]),
            (p, option) => ReloadPluginSettings(p));
        menu?.Open(player);
    }

    private void ReloadPluginSettings(CCSPlayerController player)
    {
        _api?.CloseMenu(player);

        LoadConfig();

        player.PrintToChat(
            ReplaceColorPlaceholders(Localizer["plugin_settings_reloaded"]));
    }

    private void OpenGrantRevokeMenu(CCSPlayerController player)
    {
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["grant_revoke_points"]));
        menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["grant_points"]),
            (p, option) => OpenPlayerSelectionMenu(p, true));
        menu?.AddMenuOption(ReplaceColorPlaceholders(Localizer["revoke_points"]),
            (p, option) => OpenPlayerSelectionMenu(p, false));
        menu?.Open(player);
    }

    private void OpenPlayerSelectionMenu(CCSPlayerController player, bool isGrant)
    {
        var menu = _api?.NewMenu(ReplaceColorPlaceholders(Localizer["select_player"]));
        var players = Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot).ToList();

        foreach (var targetPlayer in players)
            menu?.AddMenuOption(targetPlayer.PlayerName,
                (p, option) => OpenAmountSelectionMenu(p, targetPlayer, isGrant));

        menu?.Open(player);
    }

    private void OpenAmountSelectionMenu(CCSPlayerController player, CCSPlayerController targetPlayer, bool isGrant)
    {
        var menu = _api?.NewMenu(isGrant
            ? ReplaceColorPlaceholders(Localizer["grant_points"])
            : ReplaceColorPlaceholders(Localizer["revoke_points"]));
        var amounts = new[] { 10, 50, 100, 500 };

        foreach (var amount in amounts)
            menu?.AddMenuOption($"{amount} {ReplaceColorPlaceholders(Localizer["points"])}",
                (p, option) => AdjustPlayerPoints(p, targetPlayer, amount, isGrant));

        menu?.Open(player);
    }

    private void AdjustPlayerPoints(CCSPlayerController player, CCSPlayerController targetPlayer, int amount,
        bool isGrant)
    {
        if (OnlineUsers.TryGetValue(SteamIdConverter.ConvertToSteamId(targetPlayer.AuthorizedSteamID!.SteamId64),
                out var user))
        {
            var expChange = isGrant ? amount : -amount;
            ApplyExperienceUpdateSyncWithoutLimits(user, targetPlayer, expChange,
                isGrant ? Localizer["admin_grant_points"] : Localizer["admin_revoke_points"],
                isGrant ? ChatColors.Green : ChatColors.DarkRed);
            player.PrintToChat(
                ReplaceColorPlaceholders(Localizer[isGrant ? "points_granted" : "points_revoked", amount,
                    targetPlayer.PlayerName]));
        }
        else
        {
            player.PrintToChat(
                ReplaceColorPlaceholders(Localizer["user_data_not_found_for_player", targetPlayer.PlayerName]));
        }
    }

    public void GrantExperience(string steamIdOrSteamId64, int experience)
    {
        var steamId = steamIdOrSteamId64;

        if (ulong.TryParse(steamIdOrSteamId64, out var steamId64))
            steamId = SteamIdConverter.ConvertToSteamId(steamId64);
        else
            steamId = NormalizeSteamID(steamIdOrSteamId64);


        if (OnlineUsers.TryGetValue(steamId, out var user))
        {
            var player = Utilities.GetPlayers().FirstOrDefault(p =>
                p.AuthorizedSteamID != null &&
                SteamIdConverter.ConvertToSteamId(p.AuthorizedSteamID.SteamId64) == steamId);

            if (player != null)
            {
                Logger.LogInformation($"{experience} experience to {steamId}.");
                if (experience >= 0)
                    ApplyExperienceUpdateSyncWithoutLimits(user, player, experience, Localizer["admin_grant_points"],
                        ChatColors.Green);
                else
                    ApplyExperienceUpdateSyncWithoutLimits(user, player, experience, Localizer["admin_revoke_points"],
                        ChatColors.DarkRed);
                return;
            }
        }


        Task.Run(async () =>
        {
            var userFromDb = await Database.GetUserFromDb(steamId);
            if (userFromDb != null)
            {
                userFromDb.Value += experience;
                if (userFromDb.Value < 0) userFromDb.Value = 0;

                CheckAndUpdateRank(userFromDb);

                await Database.UpdateUsersInDb(new List<User> { userFromDb });

                Logger.LogInformation($"{experience} experience to {steamIdOrSteamId64} (offline).");
            }
            else
            {
                Logger.LogWarning($"Player offline with SteamID {steamId} not found in database.");
            }
        });
    }


    public string NormalizeSteamID(string steamId)
    {
        if (steamId.StartsWith("STEAM_0:")) steamId = "STEAM_1:" + steamId.Substring(8);
        return steamId;
    }

    [ConsoleCommand("css_lvl_reload", "Reloads the configuration files")]
    public void ReloadConfigsCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
            try
            {
                LoadConfig();
                Logger.LogInformation("Configuration successfully reloaded.");
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"Error reloading configuration: {ex.Message}");
            }
        else
            player.PrintToChat(ReplaceColorPlaceholders(Localizer["command_console_only"]));
    }

    [ConsoleCommand("css_lvl_giveexp", "Grants experience to a player by SteamID or SteamID64")]
    public void GrantExperienceCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null)
        {
            if (commandInfo.ArgCount < 3)
            {
                Logger.LogInformation("Usage: grant_experience <SteamID or SteamID64> <experience>");
                return;
            }

            var steamId = commandInfo.GetArg(1);
            if (!int.TryParse(commandInfo.GetArg(2), out var experience))
            {
                Logger.LogInformation("Invalid experience value.");
                return;
            }

            GrantExperience(steamId, experience);
        }
        else
        {
            player.PrintToChat(ReplaceColorPlaceholders(Localizer["command_console_only"]));
        }
    }

    [ConsoleCommand("css_lvl_reset", "Resets statistics for all players")]
    public void ResetStatisticsCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null)
        {
            if (commandInfo.ArgCount < 2)
            {
                Logger.LogInformation("Usage: css_lvl_reset <type>");
                Logger.LogInformation("Types: all, exp, stats");
                return;
            }

            var resetType = commandInfo.GetArg(1).ToLower();

            Task.Run(async () =>
            {
                var users = await Database.GetAllUsers();
                foreach (var user in users)
                {
                    ResetUserStatistics(user, resetType);

                    if (OnlineUsers.ContainsKey(user.SteamId!))
                        await Server.NextFrameAsync(() =>
                        {
                            var player = Utilities.GetPlayers().FirstOrDefault(p =>
                                p.AuthorizedSteamID != null &&
                                SteamIdConverter.ConvertToSteamId(p.AuthorizedSteamID.SteamId64) == user.SteamId);
                            if (player != null)
                                ApplyExperienceUpdateSyncWithoutLimits(user, player, 0,
                                    Localizer["admin_revoke_points"], ChatColors.DarkRed);
                        });
                }

                await Database.UpdateUsersInDb(users);

                Logger.LogInformation($"Statistics reset for all players with type: {resetType}");
            });
        }
        else
        {
            player.PrintToChat(ReplaceColorPlaceholders(Localizer["command_console_only"]));
        }
    }

    [ConsoleCommand("css_lvl_del", "Resets statistics for a specific player")]
    public void ResetPlayerStatisticsCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null)
        {
            if (commandInfo.ArgCount < 2)
            {
                Logger.LogInformation("Usage: css_lvl_del <SteamID or SteamID64>");
                return;
            }

            var steamId = commandInfo.GetArg(1);

            if (ulong.TryParse(steamId, out var steamId64))
                steamId = SteamIdConverter.ConvertToSteamId(steamId64);
            else
                steamId = NormalizeSteamID(steamId);

            Task.Run(async () =>
            {
                var user = await Database.GetUserFromDb(steamId);
                if (user != null)
                {
                    ResetUserStatistics(user, "all");
                    await Database.UpdateUsersInDb(new List<User> { user });

                    if (OnlineUsers.ContainsKey(user.SteamId!))
                        await Server.NextFrameAsync(() =>
                        {
                            var player = Utilities.GetPlayers().FirstOrDefault(p =>
                                p.AuthorizedSteamID != null &&
                                SteamIdConverter.ConvertToSteamId(p.AuthorizedSteamID.SteamId64) == user.SteamId);
                            if (player != null)
                                ApplyExperienceUpdateSyncWithoutLimits(user, player, 0,
                                    Localizer["admin_revoke_points"], ChatColors.DarkRed);
                        });

                    Logger.LogInformation($"Statistics reset for player: {steamId}");


                    CheckAndUpdateRank(user);
                }
                else
                {
                    Logger.LogWarning($"Player with SteamID {steamId} not found in database.");
                }
            });
        }
        else
        {
            player.PrintToChat(ReplaceColorPlaceholders(Localizer["command_console_only"]));
        }
    }

    private void ResetUserStatistics(User user, string resetType)
    {
        switch (resetType)
        {
            case "all":
                user.Value = 0;
                user.Rank = 1;
                user.Kills = 0;
                user.Deaths = 0;
                user.Shoots = 0;
                user.Hits = 0;
                user.Headshots = 0;
                user.Assists = 0;
                user.RoundWin = 0;
                user.RoundLose = 0;
                break;

            case "exp":
                user.Value = 0;
                user.Rank = 1;
                break;

            case "stats":
                user.Kills = 0;
                user.Deaths = 0;
                user.Shoots = 0;
                user.Hits = 0;
                user.Headshots = 0;
                user.Assists = 0;
                user.RoundWin = 0;
                user.RoundLose = 0;
                user.Playtime = 0;
                break;

            default:
                Logger.LogWarning($"Unknown reset type: {resetType}");
                break;
        }
    }

    public string ReplaceColorPlaceholders(string message)
    {
        if (message.Contains('{'))
        {
            var modifiedValue = message;
            foreach (var field in typeof(ChatColors).GetFields())
            {
                var pattern = $"{{{field.Name}}}";
                if (message.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null)?.ToString(),
                        StringComparison.OrdinalIgnoreCase);
            }

            return modifiedValue;
        }

        return message;
    }
}