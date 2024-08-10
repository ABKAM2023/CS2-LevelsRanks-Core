using System.Collections.Concurrent;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using LevelsRanksApi;

namespace LevelsRanks;

public class LevelsRanksApi : ILevelsRanksApi
{
    private readonly LevelsRanks _levelsRanks;
    private readonly ILogger<LevelsRanksApi> _logger;

    public LevelsRanksApi(LevelsRanks levelsRanks, ILogger<LevelsRanksApi> logger)
    {
        _levelsRanks = levelsRanks;
        _logger = logger;
    }

    public async Task ConnectAsync()
    {
        try
        {
            await _levelsRanks.Database.ConnectAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while connecting to the database.");
            throw;
        }
    }

    public string? TableName
    {
        get => _levelsRanks.TableName;
        set => _levelsRanks.TableName = value;
    }

    public string? DbConnectionString => _levelsRanks.DbConnectionString;

    public void ApplyExperienceUpdateSync(User user, CCSPlayerController player, int expChange, string eventDescription,
        string colorKey)
    {
        _levelsRanks.ApplyExperienceUpdateSync(user, player, expChange, eventDescription, colorKey);
    }

    public void ApplyExperienceUpdateSyncWithoutLimits(User user, CCSPlayerController player, int expChange,
        string eventDescription, char color)
    {
        _levelsRanks.ApplyExperienceUpdateSyncWithoutLimits(user, player, expChange, eventDescription, color);
    }

    public async Task<Dictionary<string, int>> GetCurrentRanksAsync()
    {
        return await _levelsRanks.Database.GetCurrentRanksAsync();
    }

    public ulong ConvertToSteamId64(string steamId)
    {
        return SteamIdConverter.ConvertToSteamId64(steamId);
    }

    public string ConvertToSteamId(ulong steamId64)
    {
        return SteamIdConverter.ConvertToSteamId(steamId64);
    }

    public void RegisterMenuOption(string menuOptionName, Action<CCSPlayerController> action)
    {
        _levelsRanks.CustomMenuOptions.Add((menuOptionName, action));
    }

    public void UnregisterMenuOption(string menuOptionName)
    {
        _levelsRanks.CustomMenuOptions.RemoveAll(option => option.Name == menuOptionName);
    }

    public void SetExperienceMultiplier(string steamId, double multiplier)
    {
        _levelsRanks.SetExperienceMultiplier(steamId, multiplier);
    }

    public double GetExperienceMultiplier(string steamId)
    {
        return _levelsRanks.GetExperienceMultiplier(steamId);
    }
    
    public ConcurrentDictionary<string, User> OnlineUsers => _levelsRanks.OnlineUsers;
}