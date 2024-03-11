using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Collections.Generic;
using System.IO; 
using System.Text.Json; 
using MySqlConnector;
using Dapper;

namespace LevelsRanks.API
{
    public interface IPointsManager
    {
        int AddOrRemovePoints(string steamId, int points, CCSPlayerController playerController, string reason, string messageColor);
        List<RankConfig> LoadRanksConfig(); 
        RankConfig? GetCurrentRank(string steamID64); 
        string GetConnectionString();
        DatabaseConfig GetDatabaseConfig();
    }
    public class RankConfig
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int MinExperience { get; set; }
    }
    public class DatabaseConfig
    {
        public string DbHost { get; set; }
        public string DbUser { get; set; }
        public string DbPassword { get; set; }
        public string DbName { get; set; }
        public string DbPort { get; set; }
        public string Name { get; set; }
        
        public static DatabaseConfig ReadFromJsonFile(string filePath)
        {
            string jsonConfig = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<DatabaseConfig>(jsonConfig) ?? new DatabaseConfig();
        }
    }    
}