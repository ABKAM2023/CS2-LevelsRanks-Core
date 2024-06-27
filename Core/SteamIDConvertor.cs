namespace LevelsRanks;

public static class SteamIdConverter
{
    public static ulong ConvertToSteamId64(string steamId)
    {
        var parts = steamId.Split(':');
        if (parts.Length != 3 || !parts[0].Equals("STEAM_1"))
            throw new FormatException($"The input string '{steamId}' was not in a correct format.");

        var y = ulong.Parse(parts[1]);
        var z = ulong.Parse(parts[2]);

        return z * 2 + 76561197960265728 + y;
    }

    public static string ConvertToSteamId(ulong steamId64)
    {
        var y = steamId64 % 2;
        var z = steamId64 - 76561197960265728;
        var x = (z - y) / 2;
        return $"STEAM_1:{y}:{x}";
    }
}