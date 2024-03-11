using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Reflection;
using LevelsRanks.API;
using CounterStrikeSharp.API.Core.Capabilities;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using MySqlConnector;
using Dapper;

namespace LevelsRanksCore
{   
    public class LevelsRanksCore : BasePlugin, IPointsManager
    {
        private const string PluginAuthor = "ABKAM designed by RoadSide Romeo & Wend4r";
        private const string PluginName = "[Levels Ranks] Core";
        private const string PluginVersion = "1.0";
        private const string DbConfigFileName = "dbconfig.json";
        private DatabaseConfig? dbConfig;
        private PluginConfig config;
        private StatsConfig statsconfig;
        private PhrasesConfig phrasesConfig;

        private List<Action> _pendingActions = new List<Action>();
        private bool isActiveRoundForPoints;       
        private HashSet<string> activePlayers = new HashSet<string>();  
        private Dictionary<string, PlayerResetInfo> playerResetTimes = new Dictionary<string, PlayerResetInfo>();
        public class StatsConfig
        {
            public int PointsPerRoundWin { get; set; } = 2;
            public int PointsPerRoundLoss { get; set; } = -2; 
            public int PointsPerMVP { get; set; } = 3;
            public int PointsForSuicide { get; set; } = -6; 
            public int PointsForKill { get; set; } = 5; 
            public int PointsForDeath { get; set; } = -5;
            public int PointsForAssist { get; set; } = 1; 
            public int PointsForHeadshot { get; set; } = 1;
            public int PointsForBombDefusal { get; set; } = 2; 
            public int PointsForBombExploded { get; set; } = 2; 
            public int PointsForBombPlanting { get; set; } = 2;
            public int PointsForBombDropping { get; set; } = -2;
            public int PointsForBombPickup { get; set; } = 1;     
            public int PointsForHostageFollows { get; set; } = 2; 
            public int PointsForHostageStopsFollowing { get; set; } = -2; 
            public int PointsForHostageRescued { get; set; } = 4;    
            public int PointsForDoubleKill { get; set; } = 2;
            public int PointsForTripleKill { get; set; } = 3;
            public int PointsForDomination { get; set; } = 4;
            public int PointsForRampage { get; set; } = 5;
            public int PointsForMegaKill { get; set; } = 6;
            public int PointsForOwnage { get; set; } = 7;
            public int PointsForUltraKill { get; set; } = 8;
            public int PointsForKillingSpree { get; set; } = 9;
            public int PointsForMonsterKill { get; set; } = 10;
            public int PointsForUnstoppable { get; set; } = 11;
            public int PointsForGodLike { get; set; } = 12;
        }

        public class PhrasesConfig
        {
            public string Prefix { get; set; } = " {DarkRed}[LR]{White} ";
            public string IntroMessage { get; set; } = "Enter: {Purple}!lvl {White}- main menu";
            public string GetActivePlayerCountMsg { get; set; } = "Only {DarkRed}{CURRENT_PLAYERS} {White}players present out of {DarkRed}{MIN_PLAYERS}";
            public string PointsChangeMessage { get; set; } = "Your experience:{COLOR} {POINTS} [{SIGN}{CHANGE_POINTS} for {REASON}]";
            public string SuicideMessage { get; set; } = "suicide";
            public string SuicideMessageColor { get; set; } = "{DarkRed}";
            public string DeathMessage { get; set; } = "death";
            public string DeathMessageColor { get; set; } = "{DarkRed}";
            public string KillMessage { get; set; } = "kill";
            public string KillMessageColor { get; set; } = "{Green}";
            public string HeadshotMessage { get; set; } = "headshot";
            public string HeadshotMessageColor { get; set; } = "{Yellow}";
            public string AssistMessage { get; set; } = "assist";
            public string AssistMessageColor { get; set; } = "{Blue}";
            public string RoundWinMessage { get; set; } = "round win";
            public string RoundWinMessageColor { get; set; } = "{Green}";
            public string RoundLossMessage { get; set; } = "round loss";
            public string RoundLossMessageColor { get; set; } = "{DarkRed}";
            public string MVPMessage { get; set; } = "MVP";
            public string MVPMessageColor { get; set; } = "{Gold}";
            public string BombDefusalMessage { get; set; } = "bomb defusal";
            public string BombDefusalMessageColor { get; set; } = "{Green}";
            public string BombExplodedMessage { get; set; } = "bomb exploded";
            public string BombExplodedMessageColor { get; set; } = "{Green}";
            public string BombPlantingMessage { get; set; } = "bomb planting";
            public string BombPlantingMessageColor { get; set; } = "{Green}";
            public string BombDroppingMessage { get; set; } = "bomb dropping";
            public string BombDroppingMessageColor { get; set; } = "{DarkRed}";
            public string BombPickupMessage { get; set; } = "bomb pickup";
            public string BombPickupMessageColor { get; set; } = "{Green}";
            public string HostageFollowsMessage { get; set; } = "hostage follows";
            public string HostageFollowsMessageColor { get; set; } = "{Green}";
            public string HostageStopsFollowingMessage { get; set; } = "hostage stops following";
            public string HostageStopsFollowingMessageColor { get; set; } = "{DarkRed}";
            public string HostageRescuedMessage { get; set; } = "hostage rescued";
            public string HostageRescuedMessageColor { get; set; } = "{Blue}";
            public string DoubleKillMessage { get; set; } = "Doublekill";
            public string DoubleKillMessageColor { get; set; } = "{Purple}";
            public string TripleKillMessage { get; set; } = "Triplekill";
            public string TripleKillMessageColor { get; set; } = "{Purple}";
            public string DominationMessage { get; set; } = "Domination";
            public string DominationMessageColor { get; set; } = "{Purple}";
            public string RampageMessage { get; set; } = "Rampage";
            public string RampageMessageColor { get; set; } = "{Purple}";
            public string MegaKillMessage { get; set; } = "Megakill";
            public string MegaKillMessageColor { get; set; } = "{Purple}";
            public string OwnageMessage { get; set; } = "Ownage";
            public string OwnageMessageColor { get; set; } = "{Purple}";
            public string UltraKillMessage { get; set; } = "Ultrakill";
            public string UltraKillMessageColor { get; set; } = "{Purple}";
            public string KillingSpreeMessage { get; set; } = "Killingspree";
            public string KillingSpreeMessageColor { get; set; } = "{Purple}";
            public string MonsterKillMessage { get; set; } = "Monsterkill";
            public string MonsterKillMessageColor { get; set; } = "{Purple}";
            public string UnstoppableMessage { get; set; } = "Unstoppable";
            public string UnstoppableMessageColor { get; set; } = "{Purple}";
            public string GodLikeMessage { get; set; } = "Godlike";
            public string GodLikeMessageColor { get; set; } = "{Purple}";
            public string RankCommandMessage { get; set; } = "Rank: {Green}{RANK_NAME} {White}| Position: {Blue}{PLACE}/{TOTAL_PLAYERS} {White}| Experience: {Gold}{POINTS} {White}| Kills: {Green}{KILLS} {White}| Deaths: {DarkRed}{DEATHS} {White}| KDR: {Yellow}{KDR} {White}| Time on server: {Gold}{PLAY_TIME}";
            public string TimeFormat { get; set; } = "{0}d {1}h {2}min";
            public string TopCommandIntroMessage { get; set; } = "[ {Blue}Top Players{White} ]";
            public string TopCommandPlayerMessage { get; set; } = "{INDEX}. {Grey}{NAME} - {White}{RANK} {Grey}- {Blue}{POINTS} points";
            public string TopCommandNoDataMessage { get; set; } = "[ {DarkRed}Error{White} ] No data on top players.";
            public string TopCommandErrorMessage { get; set; } = "[ {DarkRed}Error{White} ] An error occurred while executing the command.";
            public string TopKillsCommandIntroMessage { get; set; } = "[ {Green}Top Players by Kills{White} ]";
            public string TopKillsCommandPlayerMessage { get; set; } = "{INDEX}. {Grey}{NAME} - {Green}{KILLS} kills{White}";
            public string TopKillsCommandNoDataMessage { get; set; } = "[ {DarkRed}Error{White} ] No data on top players by kills.";
            public string TopKillsCommandErrorMessage { get; set; } = "[ {DarkRed}Error{White} ] An error occurred while executing the command.";
            public string TopDeathsCommandIntroMessage { get; set; } = "[ {DarkRed}Top Players by Deaths{White} ]";
            public string TopDeathsCommandPlayerMessage { get; set; } = "{INDEX}. {Grey}{NAME}{White} - {DarkRed}{DEATHS} deaths{White}";
            public string TopDeathsCommandNoDataMessage { get; set; } = "[ {DarkRed}Error{White} ] No data on top players by deaths.";
            public string TopDeathsCommandErrorMessage { get; set; } = "[ {DarkRed}Error{White} ] An error occurred while executing the command.";
            public string TopKDRCommandIntroMessage { get; set; } = "[ {Yellow}Top Players by KDR{White} ]";
            public string TopKDRCommandPlayerMessage { get; set; } = "{INDEX}. {Grey}{NAME}{White} - {Yellow}KDR: {KDR}";
            public string TopKDRCommandNoDataMessage { get; set; } = "[ {DarkRed}Error{White} ] No data on top players by KDR.";
            public string TopKDRCommandErrorMessage { get; set; } = "[ {DarkRed}Error{White} ] An error occurred while executing the command.";
            public string TopTimeCommandIntroMessage { get; set; } = "[ {Gold}Top Players by Time on Server{White} ]";
            public string TopTimeCommandPlayerMessage { get; set; } = "{INDEX}. {Grey}{NAME} - {Gold}{TIME}{White}";
            public string TopTimeCommandNoDataMessage { get; set; } = "[ {DarkRed}Error{White} ] No data on top players by time on server.";
            public string TopTimeCommandErrorMessage { get; set; } = "[ {DarkRed}Error{White} ] An error occurred while executing the command.";
            public string TopTimeFormat { get; set; } = "{0}d {1}h {2}min";
            public string ResetStatsCooldownMessage { get; set; } = " {DarkRed}[LR]{White} You can reset your statistics only once every 3 hours.";
            public string ResetStatsSuccessMessage { get; set; } = " {DarkRed}[LR]{White} Your statistics have been reset.";
            public string RanksCommandIntroMessage { get; set; } = "[ {Gold}List of Ranks{White} ]";
            public string RanksCommandRankMessage { get; set; } = "{NAME} - {Green}{EXPERIENCE} experience{White}";
            public string RanksCommandNoDataMessage { get; set; } = "[ {DarkRed}Error{White} ] No data on ranks.";
            public string RanksCommandErrorMessage { get; set; } = "[ {DarkRed}Error{White} ] An error occurred while executing the command.";
            public string RankUpMessage { get; set; } = "Your rank has been promoted to {RANK_NAME}!";
            public string RankDownMessage { get; set; } = "Your rank has been demoted to {RANK_NAME}.";
            public string MainMenuTitle { get; set; } = "Main Menu";
            public string StatsMenuOption { get; set; } = "-> Statistics";
            public string TopPlayersMenuOption { get; set; } = "-> Top Players";
            public string RanksListMenuOption { get; set; } = "-> List of All Ranks";
            public string TopPlayersMenuTitle { get; set; } = "Top Players";
            public string TopPointsOption { get; set; } = "-> Top 10 by Experience Points";
            public string TopKillsOption { get; set; } = "-> Top 10 by Kills";
            public string TopDeathsOption { get; set; } = "-> Top 10 by Deaths";
            public string TopKDROption { get; set; } = "-> Top 10 by KDR";
            public string TopTimeOption { get; set; } = "-> Top 10 by Time";
            public string BackToMainMenuOption { get; set; } = "-> {Red}Back to Main Menu";
            public string StatsMenuTitle { get; set; } = "Statistics";
            public string MyStatsOption { get; set; } = "-> My Statistics";
            public string ResetStatsOption { get; set; } = "-> Reset Statistics";
            public string BackToMainMenuFromStatsOption { get; set; } = "-> {Red}Back to Main Menu";
            public string ResetStatsConfirmationTitle { get; set; } = "Are you sure you want to reset your statistics?";
            public string ResetStatsConfirmOption { get; set; } = "-> {Green}Yes";
            public string ResetStatsDenyOption { get; set; } = "-> {Red}No";
            public string RanksMenuTitle { get; set; } = "List of Ranks";
            public string RankMessageFormat { get; set; } = "{0} - {1} experience";
        }

        public class PluginConfig
        {
            public int MinPlayersForExperience { get; set; } = 4; 
            public bool GivePointsForBotKills { get; set; } = false;        
            public bool IsRankCommandEnabled { get; set; } = true;
            public bool EnableSpecialNicknameBonus { get; set; } = true;
            public double BonusMultiplierForSpecialNickname { get; set; } = 2; 
            public string SpecialNicknameContains { get; set; } = "example.com";                   
            public double ResetStatsCooldownHours { get; set; } = 3.0; 
            public bool EnableKillStreaks { get; set; } = true;
        } 
        public void SaveConfig(PluginConfig config, string filePath)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("# Levels Ranks Plugin Configuration");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# Minimum number of players required for experience points to be awarded");
            stringBuilder.AppendLine($"MinPlayersForExperience: {config.MinPlayersForExperience}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# Enable or disable points awarded for killing bots");
            stringBuilder.AppendLine($"GivePointsForBotKills: {config.GivePointsForBotKills.ToString().ToLower()}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# Enable or disable the !rank command");
            stringBuilder.AppendLine($"IsRankCommandEnabled: {config.IsRankCommandEnabled.ToString().ToLower()}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# Enable or disable extra experience for special nicknames");
            stringBuilder.AppendLine($"EnableSpecialNicknameBonus: {config.EnableSpecialNicknameBonus.ToString().ToLower()}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# Experience multiplier for special nicknames");
            stringBuilder.AppendLine($"BonusMultiplierForSpecialNickname: {config.BonusMultiplierForSpecialNickname}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# String to look for in a nickname to apply the multiplier");
            stringBuilder.AppendLine($"SpecialNicknameContains: \"{config.SpecialNicknameContains}\"");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# How many hours before statistics can be reset again");
            stringBuilder.AppendLine($"ResetStatsCooldownHours: {config.ResetStatsCooldownHours}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# Enable or disable kill streaks");
            stringBuilder.AppendLine("# If true, kill streaks will be tracked and award extra points");
            stringBuilder.AppendLine($"EnableKillStreaks: {config.EnableKillStreaks.ToString().ToLower()}");

            File.WriteAllText(filePath, stringBuilder.ToString());
        }
        public void SaveStatsConfig(StatsConfig config, string filePath)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("# Experience Awarding Configuration");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# Statistics Settings");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForKill), config.PointsForKill, "Points for Kill - The amount of points added to a player for killing an opponent.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForDeath), config.PointsForDeath, "Points for Death - The amount of points subtracted from a player for dying.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForHeadshot), config.PointsForHeadshot, "Points for Headshot - Additional points for killing with a headshot.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForAssist), config.PointsForAssist, "Points for Assist - The amount of points added to a player for assisting in a kill.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForSuicide), config.PointsForSuicide, "Points for Suicide - The amount of points subtracted from a player for committing suicide.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsPerRoundWin), config.PointsPerRoundWin, "Points for Round Win - The amount of points added to a player for their team winning a round.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsPerRoundLoss), config.PointsPerRoundLoss, "Points for Round Loss - The amount of points subtracted from a player for their team losing a round.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsPerMVP), config.PointsPerMVP, "Points for MVP - The amount of points added to a player for earning the MVP title of the round.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForBombPlanting), config.PointsForBombPlanting, "Points for Bomb Planting - The amount of points added to a player for successfully planting the bomb.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForBombDropping), config.PointsForBombDropping, "Points for Bomb Dropping - The amount of points subtracted from a player for dropping the bomb.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForBombPickup), config.PointsForBombPickup, "Points for Bomb Pickup - The amount of points added to a player for picking up the bomb.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForBombDefusal), config.PointsForBombDefusal, "Points for Bomb Defusal - The amount of points added to a player for defusing the bomb.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForBombExploded), config.PointsForBombExploded, "Points for Bomb Explosion - The amount of points added to a player for the bomb exploding.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForHostageFollows), config.PointsForHostageFollows, "Points for Hostage Follows - The amount of points added to a player for a hostage following them.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForHostageStopsFollowing), config.PointsForHostageStopsFollowing, "Points for Hostage Stops Following - The amount of points subtracted from a player for a hostage stops following them.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForHostageRescued), config.PointsForHostageRescued, "Points for Hostage Rescued - The amount of points added to a player for rescuing a hostage.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForDoubleKill), config.PointsForDoubleKill, "Points for DoubleKill.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForTripleKill), config.PointsForTripleKill, "Points for TripleKill.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForDomination), config.PointsForDomination, "Points for Domination.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForRampage), config.PointsForRampage, "Points for Rampage.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForMegaKill), config.PointsForMegaKill, "Points for MegaKill.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForOwnage), config.PointsForOwnage, "Points for Ownage.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForUltraKill), config.PointsForUltraKill, "Points for UltraKill.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForKillingSpree), config.PointsForKillingSpree, "Points for KillingSpree.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForMonsterKill), config.PointsForMonsterKill, "Points for MonsterKill.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForUnstoppable), config.PointsForUnstoppable, "Points for Unstoppable.");
            AppendConfigValueWithComment(stringBuilder, nameof(config.PointsForGodLike), config.PointsForGodLike, "Points for GodLike.");

            File.WriteAllText(filePath, stringBuilder.ToString());
        }
        private StatsConfig LoadStatsConfig()
        {
            var filePath = Path.Combine(ModuleDirectory, "settings_stats.yml");
            if (!File.Exists(filePath))
            {
                var defaultConfig = new StatsConfig();
                SaveStatsConfig(defaultConfig, filePath);
                return defaultConfig;
            }
            else
            {
                var deserializer = new DeserializerBuilder().Build();
                var yaml = File.ReadAllText(filePath);
                return deserializer.Deserialize<StatsConfig>(yaml);
            }
        }
        private string EscapeMessage(string message)
        {
            return message.Replace("\"", "\\\"").Replace("\n", "\\n");
        }

        private void AppendConfigValueWithComment(StringBuilder sb, string key, object value, string comment)
        {
            sb.AppendLine($"# {comment}");
            sb.AppendLine($"{key}: {value}");
        }
        private void AppendConfigValue(StringBuilder sb, string key, object value)
        {
            sb.AppendLine($"{key}: {value}");
        }  
        public List<RankConfig> LoadRanksConfig()
        {
            var filePath = Path.Combine(ModuleDirectory, "settings_ranks.yml");

            if (!File.Exists(filePath))
            {
                var defaultRanks = new List<RankConfig>
                {
                    new RankConfig { Id = 0, Name = "Silver - I", MinExperience = 0},
                    new RankConfig { Id = 1, Name = "Silver - II", MinExperience = 10},
                    new RankConfig { Id = 2, Name = "Silver - III", MinExperience = 25},
                    new RankConfig { Id = 3, Name = "Silver - IV", MinExperience = 50},
                    new RankConfig { Id = 4, Name = "Silver Elite", MinExperience = 75},
                    new RankConfig { Id = 5, Name = "Silver - Master Guardian", MinExperience = 100},
                    new RankConfig { Id = 6, Name = "Gold Nova - I", MinExperience = 150},
                    new RankConfig { Id = 7, Name = "Gold Nova - II", MinExperience = 200},
                    new RankConfig { Id = 8, Name = "Gold Nova - III", MinExperience = 300},
                    new RankConfig { Id = 9, Name = "Gold Nova - Master", MinExperience = 500},
                    new RankConfig { Id = 10, Name = "Master Guardian - I", MinExperience = 750},
                    new RankConfig { Id = 11, Name = "Master Guardian - II", MinExperience = 1000},
                    new RankConfig { Id = 12, Name = "Master Guardian - Elite", MinExperience = 1500},
                    new RankConfig { Id = 13, Name = "Distinguished Master Guardian", MinExperience = 2000},
                    new RankConfig { Id = 14, Name = "Legendary Eagle", MinExperience = 3000},
                    new RankConfig { Id = 15, Name = "Legendary Eagle Master", MinExperience = 5000},
                    new RankConfig { Id = 16, Name = "Supreme Master First Class", MinExperience = 7500},
                    new RankConfig { Id = 17, Name = "Global Elite", MinExperience = 10000}
                };
                
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var yaml = serializer.Serialize(defaultRanks);
                File.WriteAllText(filePath, yaml);

                return defaultRanks;
            }

            try
            {
                var yaml = File.ReadAllText(filePath);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var ranksConfig = deserializer.Deserialize<List<RankConfig>>(yaml);

                return ranksConfig ?? new List<RankConfig>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading rank configuration file: {ex.Message}");
                return new List<RankConfig>();
            }
        }
        public static string ConvertSteamID64ToSteamID(string steamId64)
        {
            if (ulong.TryParse(steamId64, out var communityId) && communityId > 76561197960265728)
            {
                var authServer = (communityId - 76561197960265728) % 2;
                var authId = (communityId - 76561197960265728 - authServer) / 2;
                return $"STEAM_1:{authServer}:{authId}";
            }
            return null; 
        }
        private bool isWarmup = true;
        private readonly PluginCapability<IPointsManager> _pointsManagerCapability = new PluginCapability<IPointsManager>("levelsranks");

        public override void Load(bool hotReload)
        {
            base.Load(hotReload);
            dbConfig = DatabaseConfig.ReadFromJsonFile(Path.Combine(ModuleDirectory, DbConfigFileName));
            RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
            RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
            RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
            RegisterEventHandler<EventWeaponFire>(OnWeaponFire);
            RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
            RegisterEventHandler<EventRoundMvp>(OnPlayerMVP);
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
            RegisterEventHandler<EventBombExploded>(OnBombExploded);
            RegisterEventHandler<EventBombDefused>(OnBombDefused);
            RegisterEventHandler<EventRoundAnnounceMatchStart>(OnMatchStart);
            RegisterEventHandler<EventRoundAnnounceWarmup>(OnWarmupStart);      
            RegisterEventHandler<EventBombPlanted>(OnBombPlanted);      
            RegisterEventHandler<EventBombDropped>(OnBombDropped);
            RegisterEventHandler<EventBombPickup>(OnBombPickup);
            RegisterEventHandler<EventHostageFollows>(OnHostageFollows);
            RegisterEventHandler<EventHostageStopsFollowing>(OnHostageStopsFollowing);
            RegisterEventHandler<EventHostageRescued>(OnHostageRescued);
            isActiveRoundForPoints = true; 
            CreateTable();
            CreateDbConfigIfNotExists();
            LoadRanksConfig();
            config = LoadOrCreateConfig();
            phrasesConfig = LoadPhrasesConfig();
            statsconfig = LoadStatsConfig();
    
            CreateDbConfigIfNotExists();
            dbConfig = DatabaseConfig.ReadFromJsonFile(Path.Combine(ModuleDirectory, DbConfigFileName));         
            
            Capabilities.RegisterPluginCapability(_pointsManagerCapability, () => this);
        } 
        public void SavePhrasesConfig(PhrasesConfig config, string filePath)
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine("# Plugin Message Settings");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("# Prefix before the message");
            stringBuilder.AppendLine($"Prefix: \"{EscapeMessage(config.Prefix)}\"");
            stringBuilder.AppendLine("# This message is sent at the beginning of each round");
            stringBuilder.AppendLine($"IntroMessage: \"{EscapeMessage(config.IntroMessage)}\"");
            stringBuilder.AppendLine("# Message about the number of active players");
            stringBuilder.AppendLine($"GetActivePlayerCountMsg: \"{EscapeMessage(config.GetActivePlayerCountMsg)}\"");

            stringBuilder.AppendLine("# Messages upon gaining experience");
            stringBuilder.AppendLine($"PointsChangeMessage: \"{EscapeMessage(config.PointsChangeMessage)}\"");
            stringBuilder.AppendLine("# Events");

            stringBuilder.AppendLine($"SuicideMessage: \"{EscapeMessage(config.SuicideMessage)}\"");
            stringBuilder.AppendLine($"SuicideMessageColor: \"{EscapeMessage(config.SuicideMessageColor)}\"");
            stringBuilder.AppendLine($"DeathMessage: \"{EscapeMessage(config.DeathMessage)}\"");
            stringBuilder.AppendLine($"DeathMessageColor: \"{EscapeMessage(config.DeathMessageColor)}\"");
            stringBuilder.AppendLine($"KillMessage: \"{EscapeMessage(config.KillMessage)}\"");
            stringBuilder.AppendLine($"KillMessageColor: \"{EscapeMessage(config.KillMessageColor)}\"");
            stringBuilder.AppendLine($"HeadshotMessage: \"{EscapeMessage(config.HeadshotMessage)}\"");
            stringBuilder.AppendLine($"HeadshotMessageColor: \"{EscapeMessage(config.HeadshotMessageColor)}\"");
            stringBuilder.AppendLine($"AssistMessage: \"{EscapeMessage(config.AssistMessage)}\"");
            stringBuilder.AppendLine($"AssistMessageColor: \"{EscapeMessage(config.AssistMessageColor)}\"");
            stringBuilder.AppendLine($"RoundWinMessage: \"{EscapeMessage(config.RoundWinMessage)}\"");
            stringBuilder.AppendLine($"RoundWinMessageColor: \"{EscapeMessage(config.RoundWinMessageColor)}\"");
            stringBuilder.AppendLine($"RoundLossMessage: \"{EscapeMessage(config.RoundLossMessage)}\"");
            stringBuilder.AppendLine($"RoundLossMessageColor: \"{EscapeMessage(config.RoundLossMessageColor)}\"");
            stringBuilder.AppendLine($"MVPMessage: \"{EscapeMessage(config.MVPMessage)}\"");
            stringBuilder.AppendLine($"MVPMessageColor: \"{EscapeMessage(config.MVPMessageColor)}\"");
            stringBuilder.AppendLine($"BombDefusalMessage: \"{EscapeMessage(config.BombDefusalMessage)}\"");
            stringBuilder.AppendLine($"BombDefusalMessageColor: \"{EscapeMessage(config.BombDefusalMessageColor)}\"");
            stringBuilder.AppendLine($"BombExplodedMessage: \"{EscapeMessage(config.BombExplodedMessage)}\"");
            stringBuilder.AppendLine($"BombExplodedMessageColor: \"{EscapeMessage(config.BombExplodedMessageColor)}\"");
            stringBuilder.AppendLine($"BombPlantingMessage: \"{EscapeMessage(config.BombPlantingMessage)}\"");
            stringBuilder.AppendLine($"BombPlantingMessageColor: \"{EscapeMessage(config.BombPlantingMessageColor)}\"");
            stringBuilder.AppendLine($"BombDroppingMessage: \"{EscapeMessage(config.BombDroppingMessage)}\"");
            stringBuilder.AppendLine($"BombDroppingMessageColor: \"{EscapeMessage(config.BombDroppingMessageColor)}\"");
            stringBuilder.AppendLine($"BombPickupMessage: \"{EscapeMessage(config.BombPickupMessage)}\"");
            stringBuilder.AppendLine($"BombPickupMessageColor: \"{EscapeMessage(config.BombPickupMessageColor)}\"");
            stringBuilder.AppendLine($"HostageFollowsMessage: \"{EscapeMessage(config.HostageFollowsMessage)}\"");
            stringBuilder.AppendLine($"HostageFollowsMessageColor: \"{EscapeMessage(config.HostageFollowsMessageColor)}\"");
            stringBuilder.AppendLine($"HostageStopsFollowingMessage: \"{EscapeMessage(config.HostageStopsFollowingMessage)}\"");
            stringBuilder.AppendLine($"HostageStopsFollowingMessageColor: \"{EscapeMessage(config.HostageStopsFollowingMessageColor)}\"");
            stringBuilder.AppendLine($"HostageRescuedMessage: \"{EscapeMessage(config.HostageRescuedMessage)}\"");
            stringBuilder.AppendLine($"HostageRescuedMessageColor: \"{EscapeMessage(config.HostageRescuedMessageColor)}\"");
            stringBuilder.AppendLine("# Kill Streaks");
            stringBuilder.AppendLine($"DoubleKillMessage: \"{EscapeMessage(config.DoubleKillMessage)}\"");
            stringBuilder.AppendLine($"DoubleKillMessageColor: \"{config.DoubleKillMessageColor}\"");
            stringBuilder.AppendLine($"TripleKillMessage: \"{EscapeMessage(config.TripleKillMessage)}\"");
            stringBuilder.AppendLine($"TripleKillMessageColor: \"{config.TripleKillMessageColor}\"");
            stringBuilder.AppendLine($"DominationMessage: \"{EscapeMessage(config.DominationMessage)}\"");
            stringBuilder.AppendLine($"DominationMessageColor: \"{config.DominationMessageColor}\"");
            stringBuilder.AppendLine($"RampageMessage: \"{EscapeMessage(config.RampageMessage)}\"");
            stringBuilder.AppendLine($"RampageMessageColor: \"{config.RampageMessageColor}\"");
            stringBuilder.AppendLine($"MegaKillMessage: \"{EscapeMessage(config.MegaKillMessage)}\"");
            stringBuilder.AppendLine($"MegaKillMessageColor: \"{config.MegaKillMessageColor}\"");
            stringBuilder.AppendLine($"OwnageMessage: \"{EscapeMessage(config.OwnageMessage)}\"");
            stringBuilder.AppendLine($"OwnageMessageColor: \"{config.OwnageMessageColor}\"");
            stringBuilder.AppendLine($"UltraKillMessage: \"{EscapeMessage(config.UltraKillMessage)}\"");
            stringBuilder.AppendLine($"UltraKillMessageColor: \"{config.UltraKillMessageColor}\"");
            stringBuilder.AppendLine($"KillingSpreeMessage: \"{EscapeMessage(config.KillingSpreeMessage)}\"");
            stringBuilder.AppendLine($"KillingSpreeMessageColor: \"{config.KillingSpreeMessageColor}\"");
            stringBuilder.AppendLine($"MonsterKillMessage: \"{EscapeMessage(config.MonsterKillMessage)}\"");
            stringBuilder.AppendLine($"MonsterKillMessageColor: \"{config.MonsterKillMessageColor}\"");
            stringBuilder.AppendLine($"UnstoppableMessage: \"{EscapeMessage(config.UnstoppableMessage)}\"");
            stringBuilder.AppendLine($"UnstoppableMessageColor: \"{config.UnstoppableMessageColor}\"");
            stringBuilder.AppendLine($"GodLikeMessage: \"{EscapeMessage(config.GodLikeMessage)}\"");
            stringBuilder.AppendLine($"GodLikeMessageColor: \"{config.GodLikeMessageColor}\"");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# !rank");
            stringBuilder.AppendLine($"RankCommandMessage : \"{EscapeMessage(config.RankCommandMessage)}\"");   
            stringBuilder.AppendLine($"TimeFormat: \"{EscapeMessage(config.TimeFormat)}\"");  
            stringBuilder.AppendLine(); 
            stringBuilder.AppendLine("# Main Menu");
            stringBuilder.AppendLine("# Title of the main menu.");
            stringBuilder.AppendLine($"MainMenuTitle: \"{EscapeMessage(config.MainMenuTitle)}\"");
            stringBuilder.AppendLine("# Menu option for statistics.");
            stringBuilder.AppendLine($"StatsMenuOption: \"{EscapeMessage(config.StatsMenuOption)}\"");
            stringBuilder.AppendLine("# Menu option for top players.");
            stringBuilder.AppendLine($"TopPlayersMenuOption: \"{EscapeMessage(config.TopPlayersMenuOption)}\"");
            stringBuilder.AppendLine("# Menu option for the list of ranks.");
            stringBuilder.AppendLine($"RanksListMenuOption: \"{EscapeMessage(config.RanksListMenuOption)}\"");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("# 'Top Players' Menu");
            stringBuilder.AppendLine("# Title of the 'Top Players' menu.");
            stringBuilder.AppendLine($"TopPlayersMenuTitle: \"{EscapeMessage(config.TopPlayersMenuTitle)}\"");
            stringBuilder.AppendLine("# Menu option for top 10 by experience points.");
            stringBuilder.AppendLine($"TopPointsOption: \"{EscapeMessage(config.TopPointsOption)}\"");
            stringBuilder.AppendLine("# Menu option for top 10 by kills.");
            stringBuilder.AppendLine($"TopKillsOption: \"{EscapeMessage(config.TopKillsOption)}\"");
            stringBuilder.AppendLine("# Menu option for top 10 by deaths.");
            stringBuilder.AppendLine($"TopDeathsOption: \"{EscapeMessage(config.TopDeathsOption)}\"");
            stringBuilder.AppendLine("# Menu option for top 10 by KDR.");
            stringBuilder.AppendLine($"TopKDROption: \"{EscapeMessage(config.TopKDROption)}\"");
            stringBuilder.AppendLine("# Menu option for top 10 by time.");
            stringBuilder.AppendLine($"TopTimeOption: \"{EscapeMessage(config.TopTimeOption)}\"");
            stringBuilder.AppendLine("# Option to return to the main menu.");
            stringBuilder.AppendLine($"BackToMainMenuOption: \"{EscapeMessage(config.BackToMainMenuOption)}\"");

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("# 'Statistics' Menu");
            stringBuilder.AppendLine($"StatsMenuTitle: \"{EscapeMessage(config.StatsMenuTitle)}\"");
            stringBuilder.AppendLine($"MyStatsOption: \"{EscapeMessage(config.MyStatsOption)}\"");
            stringBuilder.AppendLine($"ResetStatsOption: \"{EscapeMessage(config.ResetStatsOption)}\"");
            stringBuilder.AppendLine($"BackToMainMenuFromStatsOption: \"{EscapeMessage(config.BackToMainMenuFromStatsOption)}\"");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("# Confirmation Menu for Resetting Statistics");
            stringBuilder.AppendLine($"ResetStatsConfirmationTitle: \"{EscapeMessage(config.ResetStatsConfirmationTitle)}\"");
            stringBuilder.AppendLine($"ResetStatsConfirmOption: \"{EscapeMessage(config.ResetStatsConfirmOption)}\"");
            stringBuilder.AppendLine($"ResetStatsDenyOption: \"{EscapeMessage(config.ResetStatsDenyOption)}\"");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("# Ranks List Menu");
            stringBuilder.AppendLine($"RanksMenuTitle: \"{EscapeMessage(config.RanksMenuTitle)}\"");
            stringBuilder.AppendLine($"RankMessageFormat: \"{EscapeMessage(config.RankMessageFormat)}\"");
            stringBuilder.AppendLine();
            
            File.WriteAllText(filePath, stringBuilder.ToString());
        }

        private PhrasesConfig LoadPhrasesConfig()
        {
            var filePath = Path.Combine(ModuleDirectory, "lr_core.phrases.yml");
            if (!File.Exists(filePath))
            {
                var defaultConfig = new PhrasesConfig();
                SavePhrasesConfig(defaultConfig, filePath);
                return defaultConfig;
            }
            else
            {
                var deserializer = new DeserializerBuilder().Build();
                var yaml = File.ReadAllText(filePath);
                return deserializer.Deserialize<PhrasesConfig>(yaml);
            }
        }
        private void ClearAllPendingActions()
        {
            lock (_pendingActions)
            {
                _pendingActions.Clear();
            }
        }
        private void OnMapEnd()
        {
            ClearAllPendingActions();
            activePlayers.Clear();
        }

        private HookResult OnHostageFollows(EventHostageFollows hostageFollowsEvent, GameEventInfo info)
        {
            if (!isActiveRoundForPoints) return HookResult.Continue;

            var playerSteamId64 = hostageFollowsEvent.Userid.SteamID.ToString();
            var playerSteamId = ConvertSteamID64ToSteamID(playerSteamId64);

            if (statsconfig.PointsForHostageFollows != 0)
            {
                var pointsTask = AddOrRemovePoints(playerSteamId, statsconfig.PointsForHostageFollows, hostageFollowsEvent.Userid, phrasesConfig.HostageFollowsMessage, phrasesConfig.HostageFollowsMessageColor);
            }

            return HookResult.Continue;
        }
        private HookResult OnHostageStopsFollowing(EventHostageStopsFollowing hostageStopsFollowingEvent, GameEventInfo info)
        {
            if (!isActiveRoundForPoints) return HookResult.Continue;

            var playerSteamId64 = hostageStopsFollowingEvent.Userid.SteamID.ToString();
            var playerSteamId = ConvertSteamID64ToSteamID(playerSteamId64);

            if (statsconfig.PointsForHostageStopsFollowing != 0)
            {
                var pointsTask = AddOrRemovePoints(playerSteamId, statsconfig.PointsForHostageStopsFollowing, hostageStopsFollowingEvent.Userid, phrasesConfig.HostageStopsFollowingMessage, phrasesConfig.HostageStopsFollowingMessageColor);
            }

            return HookResult.Continue;
        }
        private HookResult OnHostageRescued(EventHostageRescued hostageRescuedEvent, GameEventInfo info)
        {
            if (!isActiveRoundForPoints) return HookResult.Continue;

            var playerSteamId64 = hostageRescuedEvent.Userid.SteamID.ToString();
            var playerSteamId = ConvertSteamID64ToSteamID(playerSteamId64);

            if (statsconfig.PointsForHostageRescued != 0)
            {
                var pointsTask = AddOrRemovePoints(playerSteamId, statsconfig.PointsForHostageRescued, hostageRescuedEvent.Userid, phrasesConfig.HostageRescuedMessage, phrasesConfig.HostageRescuedMessageColor);
            }

            return HookResult.Continue;
        }
        private HookResult OnBombPickup(EventBombPickup bombPickupEvent, GameEventInfo info)
        {
            var pickerSteamId64 = bombPickupEvent.Userid.SteamID.ToString();
            var pickerSteamId = ConvertSteamID64ToSteamID(pickerSteamId64);
            Console.WriteLine($"Bomb Pickup by {pickerSteamId}");

            if (statsconfig.PointsForBombPickup != 0)
            {
                string BombPickupMessageColor = ReplaceColorPlaceholders(phrasesConfig.BombPickupMessageColor);       
                var pointsTask = AddOrRemovePoints(pickerSteamId, statsconfig.PointsForBombPickup, bombPickupEvent.Userid, phrasesConfig.BombPickupMessage, BombPickupMessageColor);
            }    

            return HookResult.Continue;
        } 
 
        private HookResult OnBombDropped(EventBombDropped bombDroppedEvent, GameEventInfo info)
        {
            var dropperSteamId64 = bombDroppedEvent.Userid.SteamID.ToString();
            var dropperSteamId = ConvertSteamID64ToSteamID(dropperSteamId64);

            if (statsconfig.PointsForBombDropping != 0)
            {
                string BombDroppingMessageColor = ReplaceColorPlaceholders(phrasesConfig.BombDroppingMessageColor);       
                var pointsTask = AddOrRemovePoints(dropperSteamId, statsconfig.PointsForBombDropping, bombDroppedEvent.Userid, phrasesConfig.BombDroppingMessage, BombDroppingMessageColor);
            }    

            return HookResult.Continue;
        }
        private HookResult OnBombPlanted(EventBombPlanted bombPlantedEvent, GameEventInfo info)
        {
            var planterSteamId64 = bombPlantedEvent.Userid.SteamID.ToString();
            var planterSteamId = ConvertSteamID64ToSteamID(planterSteamId64);

            if (statsconfig.PointsForBombPlanting != 0)
            {
                string BombPlantingMessageColor = ReplaceColorPlaceholders(phrasesConfig.BombPlantingMessageColor);       
                var pointsTask = AddOrRemovePoints(planterSteamId, statsconfig.PointsForBombPlanting, bombPlantedEvent.Userid, phrasesConfig.BombPlantingMessage, BombPlantingMessageColor);
            }    

            return HookResult.Continue;
        }
        Dictionary<uint, PlayerIteam> g_Player = new Dictionary<uint, PlayerIteam>();
        private void OnClientConnected(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            var client = player.Index;
            g_Player[client] = new PlayerIteam
            {
                value = 0,
                valuechange = 0,
                rank = 0
            };
            if (player != null && !player.IsBot)
            {
                var steamId64 = player.SteamID.ToString();
                var steamId = ConvertSteamID64ToSteamID(steamId64); 
                var playerName = GetPlayerNickname(steamId64);
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var getdateTask = GetPlayerConnectionAsync(steamId, playerName, currentTime, client, player);

                activePlayers.Add(steamId);  
            }
        }
        private async Task GetPlayerConnectionAsync(string steamId, string playerName, long currentTime, uint client, CCSPlayerController player)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var insertQuery = $"INSERT INTO `{dbConfig.Name}` (steam, name, lastconnect) VALUES (@SteamID, @Name, @LastConnect) ON DUPLICATE KEY UPDATE lastconnect = @LastConnect, name = @Name;";
                    await connection.ExecuteAsync(insertQuery, new { SteamID = steamId, Name = playerName, LastConnect = currentTime });
            
                    string query = $"SELECT * FROM `{dbConfig.Name}` WHERE steam = @SteamId";
                    var playerdata = await connection.QueryFirstOrDefaultAsync<PlayerData>(query, new { SteamId = steamId });
            
                    if (playerdata != null)
                    {
                        UpdatePlayerData(client, playerdata, player);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPlayerConnectionAsync: {ex.Message}");
            }
        }


        private void UpdatePlayerData(uint client, PlayerData? playerdata, CCSPlayerController player)
        {
            Server.NextFrame(() => 
            {
                g_Player[client].value = (int)Math.Min(playerdata.value, int.MaxValue);
                g_Player[client].rank = (int)Math.Min(playerdata.rank, int.MaxValue);
            });    
        }
        public class PlayerIteam
        {
            public int? value { get; set; }
            public int? valuechange { get; set; }
            public int? rank { get; set; }

            public PlayerIteam()
            {
                Init();
            }

            public void Init()
            {
                value = 0;
                valuechange = 0;    
                rank = 0;
            }
        }        
        private HookResult OnRoundStart(EventRoundStart roundStartEvent, GameEventInfo info)
        {
            string introMessage = phrasesConfig.Prefix + phrasesConfig.IntroMessage;
            introMessage = ReplaceColorPlaceholders(introMessage);
            BroadcastToPlayers(introMessage);

            if (isWarmup)
            {
                isActiveRoundForPoints = false;
            }
            else
            {
                isActiveRoundForPoints = GetActivePlayerCount() >= config.MinPlayersForExperience;
            }

            if (!isActiveRoundForPoints)
            {
                string message = phrasesConfig.Prefix + phrasesConfig.GetActivePlayerCountMsg
                    .Replace("{CURRENT_PLAYERS}", activePlayers.Count.ToString()) 
                    .Replace("{MIN_PLAYERS}", config.MinPlayersForExperience.ToString());
                message = ReplaceColorPlaceholders(message); 
                BroadcastToPlayers(message); 
            }
            
            killStreaks.Clear();

            return HookResult.Continue;
        }
        private HookResult OnMatchStart(EventRoundAnnounceMatchStart matchStartEvent, GameEventInfo info)
        {
            isWarmup = false;
            return HookResult.Continue;
        }

        private HookResult OnWarmupStart(EventRoundAnnounceWarmup warmupStartEvent, GameEventInfo info)
        {
            isWarmup = true;
            return HookResult.Continue;
        }        
        private HookResult OnBombExploded(EventBombExploded eventBombPlanted, GameEventInfo info)
        { 
            var planterSteamId64 = eventBombPlanted.Userid.SteamID.ToString();
            var planterSteamId = ConvertSteamID64ToSteamID(planterSteamId64);

            if (statsconfig.PointsForBombExploded != 0)
            {
                string BombExplodedMessageColor = ReplaceColorPlaceholders(phrasesConfig.BombExplodedMessageColor);       

                var pointsTask = AddOrRemovePoints(planterSteamId, statsconfig.PointsForBombExploded, eventBombPlanted.Userid, phrasesConfig.BombExplodedMessage, BombExplodedMessageColor);         
            }    

            return HookResult.Continue;
        }
        private HookResult OnBombDefused(EventBombDefused eventBombDefused, GameEventInfo info)
        {
            var defuserSteamId64 = eventBombDefused.Userid.SteamID.ToString();
            var defuserSteamId = ConvertSteamID64ToSteamID(defuserSteamId64);

            if (statsconfig.PointsForBombDefusal != 0)
            {
                string BombDefusalMessageColor = ReplaceColorPlaceholders(phrasesConfig.BombDefusalMessageColor);  

                var pointsTask = AddOrRemovePoints(defuserSteamId, statsconfig.PointsForBombDefusal, eventBombDefused.Userid, phrasesConfig.BombDefusalMessage, BombDefusalMessageColor);
            }

            return HookResult.Continue;
        }
        private void BroadcastToPlayers(string message)
        {
            foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
            {
                if (player != null && player.IsValid && !player.IsBot && player.TeamNum != (int)CsTeam.Spectator)
                {
                    player.PrintToChat(message);
                }
            }
        }
        private PluginConfig LoadOrCreateConfig()
        {
            var filePath = Path.Combine(ModuleDirectory, "settings.yml");
            if (!File.Exists(filePath))
            {
                var defaultConfig = new PluginConfig();
                SaveConfig(defaultConfig, filePath);
                return defaultConfig;
            }
            else
            {
                var deserializer = new DeserializerBuilder().Build();
                var yaml = File.ReadAllText(filePath);
                return deserializer.Deserialize<PluginConfig>(yaml);
            }
        }
        private int GetActivePlayerCount()
        {
            return activePlayers.Count;
        }
        private async Task UpdateShootsAsync(string steamId)
        {
            try
            {            
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var updateQuery = $"UPDATE `{dbConfig.Name}` SET shoots = shoots + 1 WHERE steam = @SteamID;";
                    await connection.ExecuteAsync(updateQuery, new { SteamID = steamId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateShootsAsync: {ex.Message}");
            }                
        }

        private async Task UpdateHitsAsync(string steamId)
        {
            try
            {            
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var updateQuery = $"UPDATE `{dbConfig.Name}` SET hits = hits + 1 WHERE steam = @SteamID;";
                    await connection.ExecuteAsync(updateQuery, new { SteamID = steamId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateHitsAsync: {ex.Message}");
            }                
        }

        private HookResult OnWeaponFire(EventWeaponFire fireEvent, GameEventInfo info)
        {
            var shooterSteamId64 = fireEvent.Userid.SteamID.ToString();
            var shooterSteamId = ConvertSteamID64ToSteamID(shooterSteamId64);

            var updateTask = UpdateShootsAsync(shooterSteamId);

            return HookResult.Continue;
        }

        private HookResult OnPlayerHurt(EventPlayerHurt hurtEvent, GameEventInfo info)
        {
            if (hurtEvent.Attacker != null && IsValidPlayer(hurtEvent.Attacker))
            {
                var attackerSteamId64 = hurtEvent.Attacker.SteamID.ToString();
                var attackerSteamId = ConvertSteamID64ToSteamID(attackerSteamId64);

                var updateTask = UpdateHitsAsync(attackerSteamId);
            }

            return HookResult.Continue;
        }      
        private async Task UpdatePlayerDisconnectAsync(string steamId, long currentTime)
        {
            try
            {            
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    var playerData = await connection.QueryFirstOrDefaultAsync($"SELECT lastconnect, playtime FROM `{dbConfig.Name}` WHERE steam = @SteamID", new { SteamID = steamId });
                    
                    if (playerData != null)
                    {
                        var sessionTime = currentTime - playerData.lastconnect;
                        var newPlaytime = playerData.playtime + sessionTime;

                        var updateQuery = $"UPDATE `{dbConfig.Name}` SET playtime = @Playtime WHERE steam = @SteamID;";
                        await connection.ExecuteAsync(updateQuery, new { SteamID = steamId, Playtime = newPlaytime });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePlayerDisconnectAsync: {ex.Message}");
            }                
        }
        private HookResult OnPlayerDisconnect(EventPlayerDisconnect disconnectEvent, GameEventInfo info)
        {
            if (disconnectEvent?.Userid != null && !disconnectEvent.Userid.IsBot)
            {
                var steamId64 = disconnectEvent.Userid.SteamID.ToString();
                var steamId = ConvertSteamID64ToSteamID(steamId64);  
                var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var disconnectTask = UpdatePlayerDisconnectAsync(steamId, currentTime);

                activePlayers.Remove(steamId);

                foreach (var player in activePlayers)
                {
                    Console.WriteLine("Remaining active player: " + player);
                }
            }

            return HookResult.Continue;
        }
        private HookResult OnRoundEnd(EventRoundEnd roundEndEvent, GameEventInfo info)
        { 
            CsTeam winnerTeam = (CsTeam)roundEndEvent.Winner;

            for (int playerIndex = 0; playerIndex <= Server.MaxPlayers; playerIndex++) 
            {
                CCSPlayerController playerController = Utilities.GetPlayerFromUserid(playerIndex);

                if (playerController != null && playerController.IsValid && !playerController.IsBot && HasJoinedTeam(playerController))
                {
                    CsTeam playerTeam = (CsTeam)playerController.TeamNum;
                    var steamID = playerController.SteamID.ToString();
                    var steamId = ConvertSteamID64ToSteamID(steamID);

                    bool isWin = playerTeam == winnerTeam;

                    int pointsChange = isWin ? statsconfig.PointsPerRoundWin : statsconfig.PointsPerRoundLoss;
                    if (pointsChange != 0)
                    {                          
                        string messageColor = isWin ? ReplaceColorPlaceholders(phrasesConfig.RoundWinMessageColor) : ReplaceColorPlaceholders(phrasesConfig.RoundLossMessageColor);   
                        var pointsTask = AddOrRemovePoints(steamId, pointsChange, playerController, isWin ? phrasesConfig.RoundWinMessage : phrasesConfig.RoundLossMessage, messageColor);
                    }

                    var roundResultTask = UpdateRoundResultAsync(steamId, isWin);
                }
            }

            return HookResult.Continue;
        }

        private bool HasJoinedTeam(CCSPlayerController playerController)
        {
            if (playerController == null || !playerController.IsValid)
            {
                return false;
            }

            return playerController.TeamNum == 2 || playerController.TeamNum == 3;
        }
        private HookResult OnPlayerMVP(EventRoundMvp mvpEvent, GameEventInfo info)
        {
            var mvpPlayerSteamId64 = mvpEvent.Userid.SteamID.ToString();
            var mvpPlayerSteamId = ConvertSteamID64ToSteamID(mvpPlayerSteamId64);

            if (statsconfig.PointsPerMVP != 0)
            {
                string MVPMessageColor = ReplaceColorPlaceholders(phrasesConfig.MVPMessageColor);  

                var pointsTask = AddOrRemovePoints(mvpPlayerSteamId, statsconfig.PointsPerMVP, mvpEvent.Userid, phrasesConfig.MVPMessage, MVPMessageColor);
            }

            return HookResult.Continue;
        }
        private async Task UpdateRoundResultAsync(string steamId, bool isWin)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    string columnToUpdate = isWin ? "round_win" : "round_lose";
                    var updateQuery = $"UPDATE {dbConfig.Name} SET {columnToUpdate} = {columnToUpdate} + 1 WHERE steam = @SteamID;";
                    await connection.ExecuteAsync(updateQuery, new { SteamID = steamId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePlayerConnectionAsync: {ex.Message}");
            }                
        }
        public class KillStreakTracker
        {
            public string SteamId { get; set; }
            public int KillCount { get; set; } = 0;
            public DateTime LastKillTime { get; set; } = DateTime.MinValue;
        }
        private Dictionary<string, KillStreakTracker> killStreaks = new Dictionary<string, KillStreakTracker>();
        private void CheckAndAwardKillStreakPoints(string killerSteamId, CCSPlayerController killerPlayerController)
        {
            if (!config.EnableKillStreaks)
            {
                return; 
            }
            
            if (killStreaks.TryGetValue(killerSteamId, out KillStreakTracker tracker))
            {
                tracker.KillCount++;

                int points = 0;
                string message = "";
                string color;

                switch(tracker.KillCount)
                {
                    case 2:
                        points = statsconfig.PointsForDoubleKill;
                        message = phrasesConfig.DoubleKillMessage;
                        color = phrasesConfig.DoubleKillMessageColor;
                        break;
                    case 3:
                        points = statsconfig.PointsForTripleKill;
                        message = phrasesConfig.TripleKillMessage;
                        color = phrasesConfig.TripleKillMessageColor;
                        break;
                    case 4:
                        points = statsconfig.PointsForDomination;
                        message = phrasesConfig.DominationMessage;
                        color = phrasesConfig.DominationMessageColor;
                        break;
                    case 5:
                        points = statsconfig.PointsForRampage;
                        message = phrasesConfig.RampageMessage;
                        color = phrasesConfig.RampageMessageColor;
                        break;
                    case 6:
                        points = statsconfig.PointsForMegaKill;
                        message = phrasesConfig.MegaKillMessage;
                        color = phrasesConfig.MegaKillMessageColor;
                        break;
                    case 7:
                        points = statsconfig.PointsForOwnage;
                        message = phrasesConfig.OwnageMessage;
                        color = phrasesConfig.OwnageMessageColor;
                        break;
                    case 8:
                        points = statsconfig.PointsForUltraKill;
                        message = phrasesConfig.UltraKillMessage;
                        color = phrasesConfig.UltraKillMessageColor;
                        break;
                    case 9:
                        points = statsconfig.PointsForKillingSpree;
                        message = phrasesConfig.KillingSpreeMessage;
                        color = phrasesConfig.KillingSpreeMessageColor;
                        break;
                    case 10:
                        points = statsconfig.PointsForMonsterKill;
                        message = phrasesConfig.MonsterKillMessage;
                        color = phrasesConfig.MonsterKillMessageColor;
                        break;
                    case 11:
                        points = statsconfig.PointsForUnstoppable;
                        message = phrasesConfig.UnstoppableMessage;
                        color = phrasesConfig.UnstoppableMessageColor;
                        break;
                    case 12:
                        points = statsconfig.PointsForGodLike;
                        message = phrasesConfig.GodLikeMessage;
                        color = phrasesConfig.GodLikeMessageColor;
                        break;
                    default:
                        color = $"{ChatColors.Default}"; 
                        break;
                }

                if (points > 0 && !string.IsNullOrEmpty(message))
                {
                    AddOrRemovePoints(killerSteamId, points, killerPlayerController, message, color);
                }
            }
            else
            {
                killStreaks[killerSteamId] = new KillStreakTracker { SteamId = killerSteamId, KillCount = 1, LastKillTime = DateTime.Now };
            }
        }


        private HookResult OnPlayerDeath(EventPlayerDeath deathEvent, GameEventInfo info)
        {
            if ((deathEvent?.Userid?.IsBot ?? true) && !config.GivePointsForBotKills)
            {
                return HookResult.Continue;
            }
            
            try
            {
                var victimSteamId64 = deathEvent.Userid.SteamID.ToString();
                var victimSteamId = ConvertSteamID64ToSteamID(victimSteamId64);

                if (deathEvent.Attacker != null && deathEvent.Attacker == deathEvent.Userid)
                {
                    if (statsconfig.PointsForSuicide != 0)
                    {
                        string suicideMessageColor = ReplaceColorPlaceholders(phrasesConfig.SuicideMessageColor);
                        var pointsTask = AddOrRemovePoints(victimSteamId, statsconfig.PointsForSuicide, deathEvent.Userid, phrasesConfig.SuicideMessage, suicideMessageColor);
                    }
                }
                else
                {
                    if (statsconfig.PointsForDeath != 0)
                    {
                        string DeathMessageColor = ReplaceColorPlaceholders(phrasesConfig.DeathMessageColor);            
                        var deathPointsTask = AddOrRemovePoints(victimSteamId, statsconfig.PointsForDeath, deathEvent.Userid, phrasesConfig.DeathMessage, DeathMessageColor);
                    }
                    var updateKillsOrDeathsTask = UpdateKillsOrDeathsAsync(victimSteamId, false);

                    if (deathEvent.Attacker != null && IsValidPlayer(deathEvent.Attacker))
                    {
                        var killerSteamId64 = deathEvent.Attacker.SteamID.ToString();
                        var killerSteamId = ConvertSteamID64ToSteamID(killerSteamId64);
                        
                        CheckAndAwardKillStreakPoints(killerSteamId, deathEvent.Attacker);
                        
                        if (statsconfig.PointsForKill != 0)
                        {
                            string KillMessageColor = ReplaceColorPlaceholders(phrasesConfig.KillMessageColor);                                   
                            var killPointsTask = AddOrRemovePoints(killerSteamId, statsconfig.PointsForKill, deathEvent.Attacker, phrasesConfig.KillMessage, KillMessageColor);
                        }
                        var updateKillsTask = UpdateKillsOrDeathsAsync(killerSteamId, true);
                        
                        if (deathEvent.Headshot && statsconfig.PointsForHeadshot != 0)
                        {
                            string HeadshotMessageColor = ReplaceColorPlaceholders(phrasesConfig.HeadshotMessageColor);  
                            var headshotPointsTask = AddOrRemovePoints(killerSteamId, statsconfig.PointsForHeadshot, deathEvent.Attacker, phrasesConfig.HeadshotMessage, HeadshotMessageColor);
                            var updateHeadshotsTask = UpdateHeadshotsAsync(killerSteamId);
                        } 
                    }
                    if (deathEvent.Assister != null && IsValidPlayer(deathEvent.Assister) && statsconfig.PointsForAssist != 0)
                    {
                        var assisterSteamId64 = deathEvent.Assister.SteamID.ToString();
                        var assisterSteamId = ConvertSteamID64ToSteamID(assisterSteamId64);

                        string AssistMessageColor = ReplaceColorPlaceholders(phrasesConfig.AssistMessageColor);  
                        var assistPointsTask = AddOrRemovePoints(assisterSteamId, statsconfig.PointsForAssist, deathEvent.Assister, phrasesConfig.AssistMessage, AssistMessageColor);
                        var updateAssistsTask = UpdateAssistsAsync(assisterSteamId);
                    }                                                          
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OnPlayerDeath: " + ex.Message);
            }
            return HookResult.Continue;
        }

        private bool IsValidPlayer(CCSPlayerController player)
        {
            return player != null && player.IsValid && !player.IsBot;
        }        

        private async Task UpdateHeadshotsAsync(string steamId)
        {
            try
            {            
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var updateQuery = $"UPDATE `{dbConfig.Name}` SET headshots = headshots + 1 WHERE steam = @SteamID;";
                    await connection.ExecuteAsync(updateQuery, new { SteamID = steamId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateHeadshotsAsync: {ex.Message}");
            }                
        }

        private async Task UpdateAssistsAsync(string steamId)
        {
            try
            {            
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var updateQuery = $"UPDATE `{dbConfig.Name}` SET assists = assists + 1 WHERE steam = @SteamID;";
                    await connection.ExecuteAsync(updateQuery, new { SteamID = steamId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAssistsAsync: {ex.Message}");
            }                
        }
        private async Task UpdateKillsOrDeathsAsync(string steamId, bool isKill)
        {
            try
            {              
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    string columnToUpdate = isKill ? "kills" : "deaths";
                    var updateQuery = $"UPDATE `{dbConfig.Name}` SET `{columnToUpdate}` = `{columnToUpdate}` + 1 WHERE steam = @SteamID;";
                    await connection.ExecuteAsync(updateQuery, new { SteamID = steamId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateKillsOrDeathsAsync: {ex.Message}");
            }                
        }
        private async Task<int> UpdatePlayerPointsAsync(string steamId, int points, uint client)
        {
            int updatedPoints = 0;
            updatedPoints = (int)(g_Player[client].value + points);
            if (updatedPoints < 0) points = 0;

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        var updateQuery = $"UPDATE `{dbConfig.Name}` SET value = value + @NewPoints WHERE steam = @SteamID;";

                        await connection.ExecuteAsync(updateQuery, new { NewPoints = points, SteamID = steamId }, transaction);

                        await transaction.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePlayerPointsAsync: {ex.Message}");
            }

            return updatedPoints;
        }
        public int AddOrRemovePoints(string steamId, int points, CCSPlayerController playerController, string reason, string messageColor)
        {
            if (isWarmup)
            {
                return 0; 
            }
            
            if (GetActivePlayerCount() < config.MinPlayersForExperience)
            {
                return 0;
            }
            else
            {
                if (playerController == null || string.IsNullOrEmpty(steamId))
                {
                    return 0;
                }

                var client = playerController.Index;

                if (string.IsNullOrEmpty(steamId))
                {
                    return 0;
                }

                if (playerController != null && !playerController.IsBot)
                {
                    if (config.EnableSpecialNicknameBonus &&
                        playerController.PlayerName.Contains(config.SpecialNicknameContains,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        if (points > 0 && config.BonusMultiplierForSpecialNickname > 1)
                        {
                            points = (int)(points * config.BonusMultiplierForSpecialNickname);
                        }
                    }
                }

                int updatedPoints = 0;
                if (g_Player.ContainsKey(client))
                {
                    var currentValue = g_Player[client].value ?? 0;
                    updatedPoints = currentValue + points;
                    if (updatedPoints < 0) updatedPoints = 0;

                    g_Player[client] = new PlayerIteam { value = updatedPoints };
                }
                else
                {
                    g_Player[client] = new PlayerIteam { value = Math.Max(points, 0) };
                    updatedPoints = g_Player[client].value ?? 0;
                }



                _ = UpdatePlayerPointsAsync(steamId, points, client);

                Action chatUpdateAction = () =>
                {
                    if (playerController != null && playerController.IsValid && !playerController.IsBot)
                    {
                        string sign = points >= 0 ? "+" : "-";
                        string rawMessage = phrasesConfig.Prefix + phrasesConfig.PointsChangeMessage
                            .Replace("{COLOR}", messageColor)
                            .Replace("{POINTS}", updatedPoints.ToString())
                            .Replace("{SIGN}", sign)
                            .Replace("{CHANGE_POINTS}", Math.Abs(points).ToString())
                            .Replace("{REASON}", reason);

                        string formattedMessage = ReplaceColorPlaceholders(rawMessage);
                        playerController.PrintToChat(formattedMessage);
                    }
                };

                lock (_pendingActions)
                {
                    _pendingActions.Add(chatUpdateAction);
                }

                Server.NextFrame(() =>
                {
                    lock (_pendingActions)
                    {
                        chatUpdateAction();
                        _pendingActions.Remove(chatUpdateAction);
                    }
                });

                CheckAndUpdateRankAsync(steamId, updatedPoints);

                return updatedPoints;
            }
        }
        public class PlayerData
        {
            public int value { get; set; }
            public int rank { get; set; }
        }

        private async Task<bool> CheckAndUpdateRankAsync(string steamId, int updatedPoints)
        {
            var ranksConfig = LoadRanksConfig();

            var newRankIndex = -1;

            for (int i = 0; i < ranksConfig.Count; i++)
            {
                if (updatedPoints >= ranksConfig[i].MinExperience)
                {
                    newRankIndex = i;
                }
                else
                {
                    break;
                }
            }
    
            if (newRankIndex != -1)
            {
                var newRank = ranksConfig[newRankIndex];

                int currentRankId = await GetCurrentRankId(steamId);
                if (currentRankId != newRank.Id)
                {
                    bool isRankUpdated = await UpdatePlayerRankAsync(steamId, newRank.Id);
            
                    if (isRankUpdated)
                    {
                        bool isRankUp = newRank.Id > currentRankId;
                        NotifyPlayerOfRankChange(steamId, newRank.Name, isRankUp);
                        
                        return true;
                    }


                }
            }

            return false;
        }
        private async Task<int> GetCurrentRankId(string steamId)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var currentRankQuery = $"SELECT `rank` FROM `{dbConfig.Name}` WHERE `steam` = @SteamID;";
                    var rankId = await connection.ExecuteScalarAsync<int>(currentRankQuery, new { SteamID = steamId });
                    return rankId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetCurrentRankId] Error fetching rank for {steamId}: {ex.Message}");
                return -1; 
            }
        }


        private async Task<bool> UpdatePlayerRankAsync(string steamId, int newRankId)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    var updateRankQuery = $"UPDATE `{dbConfig.Name}` SET `rank` = @NewRankId WHERE `steam` = @SteamID;";
                    var affectedRows = await connection.ExecuteAsync(updateRankQuery, new { NewRankId = newRankId, SteamID = steamId });
                    return affectedRows > 0; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePlayerRankAsync: {ex.Message}");
                return false;
            }
        }
        private void NotifyPlayerOfRankChange(string steamId, string newRankName, bool isRankUp)
        {
            Server.NextFrame(() =>
            {
                string steamId64 = ConvertSteamIDToSteamID64(steamId);
                string message = isRankUp ? phrasesConfig.RankUpMessage.Replace("{RANK_NAME}", newRankName) 
                    : phrasesConfig.RankDownMessage.Replace("{RANK_NAME}", newRankName);

                foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
                {
                    if (player != null && player.IsValid && !player.IsBot && player.SteamID.ToString() == steamId64)
                    {
                        player.PrintToCenter(message);
                        break;
                    }
                }
            });
        }
        private string ConvertSteamIDToSteamID64(string steamID)
        {
            if (string.IsNullOrEmpty(steamID) || !steamID.StartsWith("STEAM_"))
            {
                return null;
            }

            try
            {
                string[] split = steamID.Replace("STEAM_", "").Split(':');
                long steamID64 = 76561197960265728 + Convert.ToInt64(split[2]) * 2 + Convert.ToInt64(split[1]);
                return steamID64.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConvertSteamIDToSteamID64] Ошибка при конвертации SteamID: {ex.Message}");
                return null;
            }
        }
        public RankConfig? GetCurrentRank(string steamID64)
        {
            var steamID = ConvertSteamID64ToSteamID(steamID64);
            if (steamID == null)
            {
                Console.WriteLine("Invalid SteamID64 format.");
                return null;
            }

            var ranksConfig = LoadRanksConfig();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var query = $"SELECT rank FROM {dbConfig.Name} WHERE steam = @SteamID;";
                var rankId = connection.QueryFirstOrDefault<int>(query, new { SteamID = steamID }); 

                RankConfig? defaultRank = ranksConfig.FirstOrDefault(r => r.Id == 0);
                RankConfig? currentRank = ranksConfig.FirstOrDefault(r => r.Id == rankId);

                return currentRank ?? defaultRank;
            }
        }
        private string GetPlayerNickname(string steamID)
        {
            var player = FindPlayerBySteamID(steamID);
            if (player != null)
            {
                return player.PlayerName;
            }
            return "Unkown";
        }

        private CCSPlayerController FindPlayerBySteamID(string steamID)
        {
            foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
            {
                if (player != null && player.IsValid && !player.IsBot && player.SteamID.ToString() == steamID)
                {
                    return player;
                }
            }
            return null;
        }
        private void CreateDbConfigIfNotExists()
        {
            string configFilePath = Path.Combine(ModuleDirectory, DbConfigFileName);
            if (!File.Exists(configFilePath))
            {
                var config = new DatabaseConfig
                {
                    DbHost = "YourHost",
                    DbUser = "YourUser",
                    DbPassword = "YourPassword",
                    DbName = "YourDatabase",
                    DbPort = "3306" 
                };

                string jsonConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFilePath, jsonConfig);
                Console.WriteLine("Database configuration file created.");
            }
        }
        private string ReplaceColorPlaceholders(string message)
        {
            if (message.Contains('{'))
            {
                string modifiedValue = message;
                foreach (FieldInfo field in typeof(ChatColors).GetFields())
                {
                    string pattern = $"{{{field.Name}}}";
                    if (message.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null).ToString(), StringComparison.OrdinalIgnoreCase);
                    }
                }
                return modifiedValue;
            }

            return message;
        }
        private async Task<PlayerStats> GetPlayerStatsAsync(string steamId)
        {
            var ranksConfig = LoadRanksConfig(); 

            using (var connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                var query = $@"
            SELECT p.rank, p.value as points, p.kills, p.deaths, p.playtime,
                (SELECT COUNT(DISTINCT value) FROM `{dbConfig.Name}` WHERE value > p.value) + 1 as place,
                (SELECT COUNT(DISTINCT steam) FROM `{dbConfig.Name}`) as totalPlayers
            FROM `{dbConfig.Name}` p
            WHERE p.steam = @SteamID;";
        
                var playerData = await connection.QueryFirstOrDefaultAsync(query, new { SteamID = steamId });

                if (playerData == null)
                { 
                    return null; 
                }

                var rankConfig = ranksConfig.FirstOrDefault(r => r.Id == Convert.ToInt32(playerData.rank));
                if (rankConfig == null)
                {
                    return null; 
                }
                
                var kdr = playerData.deaths > 0 ? (double)playerData.kills / playerData.deaths : playerData.kills;
                
                return new PlayerStats
                {
                    RankName = rankConfig.Name,
                    Place = Convert.ToInt32(playerData.place),
                    TotalPlayers = Convert.ToInt32(playerData.totalPlayers),
                    Points = Convert.ToInt32(playerData.points),
                    Kills = playerData.kills,
                    Deaths = playerData.deaths,
                    PlayTime = playerData.playtime,
                    KDR = kdr
                };
            }
        }


        public class PlayerResetInfo
        {
            public DateTime LastResetTime { get; set; }
        }        
        public class PlayerStats
        {
            public string RankName { get; set; }
            public int Place { get; set; }
            public int TotalPlayers { get; set; }
            public int Points { get; set; }
            public int Kills { get; set; }
            public int PlayTime { get; set; }
            public int Deaths { get; set; }
            public double KDR { get; set; }
        }

        private string FormatTime(int playTimeSeconds)
        {
            TimeSpan timePlayed = TimeSpan.FromSeconds(playTimeSeconds);
            return string.Format(phrasesConfig.TimeFormat, timePlayed.Days, timePlayed.Hours, timePlayed.Minutes);
        }
        [ConsoleCommand("lvl", "Главное меню плагина")]
        public void OnLvlCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;

            ShowMainMenu(player);
        }
        private void ShowMainMenu(CCSPlayerController player)
        {
            string mainMenuTitle = phrasesConfig.Prefix + phrasesConfig.MainMenuTitle;
            mainMenuTitle = ReplaceColorPlaceholders(mainMenuTitle); 

            ChatMenu mainMenu = new ChatMenu(mainMenuTitle);
            
            string statsMenuOption = ReplaceColorPlaceholders(phrasesConfig.StatsMenuOption);
            string topPlayersMenuOption = ReplaceColorPlaceholders(phrasesConfig.TopPlayersMenuOption);
            string ranksListMenuOption = ReplaceColorPlaceholders(phrasesConfig.RanksListMenuOption);

            mainMenu.AddMenuOption(statsMenuOption, (p, option) => ShowStatsMenu(p));
            mainMenu.AddMenuOption(topPlayersMenuOption, (p, option) => ShowTopPlayersMenu(p));
            mainMenu.AddMenuOption(ranksListMenuOption, (p, option) => OnRanksCommand(p));
    
            MenuManager.OpenChatMenu(player, mainMenu);
        }
        private void ShowTopPlayersMenu(CCSPlayerController player)
        {
            ChatMenu topPlayersMenu = new ChatMenu(ReplaceColorPlaceholders(phrasesConfig.Prefix + phrasesConfig.TopPlayersMenuTitle));
            topPlayersMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.TopPointsOption), (p, option) => OnTopCommand(p));
            topPlayersMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.TopKillsOption), (p, option) => OnTopKillsCommand(p));
            topPlayersMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.TopDeathsOption), (p, option) => OnTopDeathsCommand(p));
            topPlayersMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.TopKDROption), (p, option) => OnTopKDRCommand(p));       
            topPlayersMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.TopTimeOption), (p, option) => OnTopTimeCommand(p));    
            topPlayersMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.BackToMainMenuOption), (p, option) => ShowMainMenu(p));

            MenuManager.OpenChatMenu(player, topPlayersMenu);
        }
        private void ShowStatsMenu(CCSPlayerController player)
        {
            ChatMenu statsMenu = new ChatMenu( ReplaceColorPlaceholders(phrasesConfig.Prefix + phrasesConfig.StatsMenuTitle));
            statsMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.MyStatsOption), (p, option) => OnRankCommand(p));
            statsMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.ResetStatsOption), (p, option) => Youreallywantit(p));
            statsMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.BackToMainMenuFromStatsOption), (p, option) => ShowMainMenu(p));
    
            MenuManager.OpenChatMenu(player, statsMenu);
        }

        private void Youreallywantit(CCSPlayerController player)
        {
            ChatMenu confirmationMenu = new ChatMenu(ReplaceColorPlaceholders(phrasesConfig.Prefix + phrasesConfig.ResetStatsConfirmationTitle));
            confirmationMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.ResetStatsConfirmOption), (p, option) => OnResetStatsCommand(p));
            confirmationMenu.AddMenuOption(ReplaceColorPlaceholders(phrasesConfig.ResetStatsDenyOption), (p, option) => ShowStatsMenu(p));
            MenuManager.OpenChatMenu(player, confirmationMenu);
        }   
        public void OnRanksCommand(CCSPlayerController player)
        {
            try
            {
                var ranksConfig = LoadRanksConfig();

                if (ranksConfig.Any())
                {
                    ChatMenu ranksMenu = new ChatMenu(ReplaceColorPlaceholders(phrasesConfig.Prefix + phrasesConfig.RanksMenuTitle));

                    foreach (var rank in ranksConfig)
                    {
                        string rankMessage = string.Format(phrasesConfig.RankMessageFormat, rank.Name, rank.MinExperience);
                        ranksMenu.AddMenuOption(ReplaceColorPlaceholders(rankMessage), (p, option) => {});
                    }
                    MenuManager.OpenChatMenu(player, ranksMenu);
                }
                else
                {
                    string noDataMessage = ReplaceColorPlaceholders(phrasesConfig.RanksCommandNoDataMessage);
                    player.PrintToChat(noDataMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OnRanksCommand: " + ex.Message);
                string errorMessage = ReplaceColorPlaceholders(phrasesConfig.RanksCommandErrorMessage);
                player.PrintToChat(errorMessage);
            }
        }
        [ConsoleCommand("rank", "Check your statistics")]
        public void RankCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null) return;

            OnRankCommand(player);
        }
        public void OnRankCommand(CCSPlayerController player)
        {
            if (!config.IsRankCommandEnabled)
            {
                return;
            }

            if (player == null) return;

            var steamID64 = player.SteamID.ToString();
            var steamID = ConvertSteamID64ToSteamID(steamID64); 

            var stats = GetPlayerStatsAsync(steamID).GetAwaiter().GetResult(); 

            string message = phrasesConfig.Prefix + phrasesConfig.RankCommandMessage
                .Replace("{RANK_NAME}", stats.RankName)
                .Replace("{PLACE}", stats.Place.ToString())
                .Replace("{TOTAL_PLAYERS}", stats.TotalPlayers.ToString())
                .Replace("{POINTS}", stats.Points.ToString())
                .Replace("{KILLS}", stats.Kills.ToString())
                .Replace("{DEATHS}", stats.Deaths.ToString())
                .Replace("{KDR}", stats.KDR.ToString("F2"))
                .Replace("{PLAY_TIME}", FormatTime(stats.PlayTime));

            message = ReplaceColorPlaceholders(message);
            player.PrintToChat(message);
        }
        public void OnResetStatsCommand(CCSPlayerController player)
        {
            if (player == null) return;

            var steamId64 = player.SteamID.ToString();
            var steamId = ConvertSteamID64ToSteamID(steamId64);

            if (playerResetTimes.TryGetValue(steamId, out var resetInfo))
            {
                if ((DateTime.UtcNow - resetInfo.LastResetTime).TotalHours < config.ResetStatsCooldownHours)
                {
                    string cooldownMessage = ReplaceColorPlaceholders(phrasesConfig.ResetStatsCooldownMessage);
                    player.PrintToChat(cooldownMessage);
                    return;
                }
            }

            ResetPlayerStats(steamId);
            playerResetTimes[steamId] = new PlayerResetInfo { LastResetTime = DateTime.UtcNow };
            
            string successMessage = ReplaceColorPlaceholders(phrasesConfig.ResetStatsSuccessMessage);
            player.PrintToChat(successMessage);
        }       
        public void OnTopCommand(CCSPlayerController player)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var topPlayersQuery = @"
                        SELECT steam, name, value, `rank`
                        FROM `" + dbConfig.Name + @"`
                        ORDER BY value DESC
                        LIMIT 10;";
                    
                    var topPlayers = connection.Query(topPlayersQuery).ToList();

                    if (topPlayers.Any())
                    {
                        var ranksConfig = LoadRanksConfig(); 

                        string introMessage = ReplaceColorPlaceholders(phrasesConfig.TopCommandIntroMessage);
                        player.PrintToChat(introMessage);

                        for (int i = 0; i < topPlayers.Count; i++)
                        {
                            var topPlayerInfo = topPlayers[i];
                            var rankName = ranksConfig.FirstOrDefault(r => r.Id == topPlayerInfo.rank)?.Name ?? "Unknown Rank";
                            string playerMessage = phrasesConfig.TopCommandPlayerMessage
                                .Replace("{INDEX}", (i + 1).ToString())
                                .Replace("{NAME}", topPlayerInfo.name)
                                .Replace("{POINTS}", topPlayerInfo.value.ToString())
                                .Replace("{RANK}", rankName);
                            playerMessage = ReplaceColorPlaceholders(playerMessage);
                            player.PrintToChat(playerMessage);
                        }
                    }
                    else
                    {
                        string noDataMessage = ReplaceColorPlaceholders(phrasesConfig.TopCommandNoDataMessage);
                        player.PrintToChat(noDataMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OnTopCommand: " + ex.Message);
                string errorMessage = ReplaceColorPlaceholders(phrasesConfig.TopCommandErrorMessage);
                player.PrintToChat(errorMessage);
            }
        }
        
        public void OnTopKillsCommand(CCSPlayerController player)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var topPlayersQuery = @"
                        SELECT steam, name, kills
                        FROM `" + dbConfig.Name + @"`
                        ORDER BY kills DESC
                        LIMIT 10;";


                    var topPlayers = connection.Query(topPlayersQuery).ToList();

                    if (topPlayers.Any())
                    {
                        string introMessage = ReplaceColorPlaceholders(phrasesConfig.TopKillsCommandIntroMessage);
                        player.PrintToChat(introMessage);

                        for (int i = 0; i < topPlayers.Count; i++)
                        {
                            var topPlayerInfo = topPlayers[i];
                            string playerMessage = ReplaceColorPlaceholders(phrasesConfig.TopKillsCommandPlayerMessage)
                                .Replace("{INDEX}", (i + 1).ToString())
                                .Replace("{NAME}", topPlayerInfo.name)
                                .Replace("{KILLS}", topPlayerInfo.kills.ToString());
                            player.PrintToChat(playerMessage);
                        }
                    }
                    else
                    {
                        player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopKillsCommandNoDataMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OnTopKillsCommand: " + ex.Message);
                player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopKillsCommandErrorMessage));
            }
        }
        
        public void OnTopDeathsCommand(CCSPlayerController player)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var topPlayersQuery = $@"
                        SELECT steam, name, deaths
                        FROM `{dbConfig.Name}`
                        ORDER BY deaths DESC
                        LIMIT 10;";

                    var topPlayers = connection.Query(topPlayersQuery).ToList();

                    if (topPlayers.Any())
                    {
                        string introMessage = ReplaceColorPlaceholders(phrasesConfig.TopDeathsCommandIntroMessage);
                        player.PrintToChat(introMessage);

                        for (int i = 0; i < topPlayers.Count; i++)
                        {
                            var topPlayerInfo = topPlayers[i];
                            string playerMessage = ReplaceColorPlaceholders(phrasesConfig.TopDeathsCommandPlayerMessage)
                                .Replace("{INDEX}", (i + 1).ToString())
                                .Replace("{NAME}", topPlayerInfo.name)
                                .Replace("{DEATHS}", topPlayerInfo.deaths.ToString());
                            player.PrintToChat(playerMessage);
                        }
                    }
                    else
                    {
                        player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopDeathsCommandNoDataMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OnTopDeathsCommand: " + ex.Message);
                player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopDeathsCommandErrorMessage));
            }
        }
        public void OnTopKDRCommand(CCSPlayerController player)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var topPlayersQuery = $@"
                        SELECT steam, name, kills, deaths, 
                        CASE
                            WHEN deaths = 0 THEN kills
                            ELSE kills / deaths
                        END AS kdr
                        FROM `{dbConfig.Name}`
                        ORDER BY kdr DESC, kills DESC
                        LIMIT 10;";
                    
                    var topPlayers = connection.Query(topPlayersQuery).ToList();

                    if (topPlayers.Any())
                    {
                        string introMessage = ReplaceColorPlaceholders(phrasesConfig.TopKDRCommandIntroMessage);
                        player.PrintToChat(introMessage);

                        foreach (var topPlayerInfo in topPlayers)
                        {
                            string formattedKDR = topPlayerInfo.kdr.ToString("F2");
                            string playerMessage = phrasesConfig.TopKDRCommandPlayerMessage
                                .Replace("{INDEX}", (topPlayers.IndexOf(topPlayerInfo) + 1).ToString())
                                .Replace("{NAME}", topPlayerInfo.name)
                                .Replace("{KDR}", formattedKDR);
                            player.PrintToChat(ReplaceColorPlaceholders(playerMessage));
                        }
                    }
                    else
                    {
                        player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopKDRCommandNoDataMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OnTopKDRCommand: " + ex.Message);
                player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopKDRCommandErrorMessage));
            }
        }

        public void OnTopTimeCommand(CCSPlayerController player)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var topPlayersQuery = @"
                        SELECT steam, name, playtime
                        FROM `" + dbConfig.Name + @"`
                        ORDER BY playtime DESC
                        LIMIT 10;";

                    var topPlayers = connection.Query(topPlayersQuery).ToList();

                    if (topPlayers.Any())
                    {
                        string introMessage = ReplaceColorPlaceholders(phrasesConfig.TopTimeCommandIntroMessage);
                        player.PrintToChat(introMessage);

                        for (int i = 0; i < topPlayers.Count; i++)
                        {
                            var topPlayerInfo = topPlayers[i];
                            TimeSpan timePlayed = TimeSpan.FromSeconds(topPlayerInfo.playtime);
                            string formattedTime = string.Format(phrasesConfig.TopTimeFormat,
                                timePlayed.Days, timePlayed.Hours, timePlayed.Minutes);
                            string playerMessage = phrasesConfig.TopTimeCommandPlayerMessage
                                .Replace("{INDEX}", (i + 1).ToString())
                                .Replace("{NAME}", topPlayerInfo.name)
                                .Replace("{TIME}", formattedTime);
                            player.PrintToChat(ReplaceColorPlaceholders(playerMessage));
                        }
                    }
                    else
                    {
                        player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopTimeCommandNoDataMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in OnTopTimeCommand: " + ex.Message);
                player.PrintToChat(ReplaceColorPlaceholders(phrasesConfig.TopTimeCommandErrorMessage));
            }
        }

        private void ResetPlayerStats(string steamId)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var resetQuery = $"UPDATE `{dbConfig.Name}` SET kills = 0, deaths = 0, `value` = 0, shoots = 0, hits = 0, headshots = 0, assists = 0, round_win = 0, round_lose = 0, playtime = 0 WHERE steam = @SteamID;";
                    int affectedRows = connection.Execute(resetQuery, new { SteamID = steamId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating player stats: {ex.Message}");
            }
        }

        [ConsoleCommand("css_lvl_reload", "Reloads the configuration files")]
        public void ReloadConfigsCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                try
                {
                    config = LoadOrCreateConfig();
                    phrasesConfig = LoadPhrasesConfig();                    
                    statsconfig = LoadStatsConfig();
                    LoadRanksConfig();

                    Console.WriteLine("[LR] Configuration successfully reloaded.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LR] Error reloading configuration: {ex.Message}");
                }
            }
            else
            {
                player.PrintToChat("This command is only available from the server console.");
            }
        }
        [ConsoleCommand("css_lvl_take_points", "Takes experience points from a player. Usage: css_lvl_take_point <steamid64> <amount>")]
        public void TakePointsCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || player.IsBot) 
            {
                try
                {
                    var steamId64 = command.ArgByIndex(1);
                    var steamId = ConvertSteamID64ToSteamID(steamId64);
                    if (int.TryParse(command.ArgByIndex(2), out var amount))
                    {
                        RemovePlayerExperience(steamId, amount);
                        Console.WriteLine($"[LR] {amount} experience points were taken from player {steamId} (SteamID64: {steamId64}).");
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount of points. Make sure you have entered a number.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LR] An error occurred while deducting experience points: {ex.Message}");
                }
            }
            else
            {
                player.PrintToChat($"{ChatColors.Red}This command is only available from the server console.");
            }
        }
        private void RemovePlayerExperience(string steamId, int amount)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var updateQuery = $"UPDATE `{dbConfig.Name}` SET `value` = GREATEST(0, `value` - @Amount) WHERE steam = @SteamID;";
                connection.Execute(updateQuery, new { Amount = amount, SteamID = steamId });
            }
        }
        [ConsoleCommand("css_lvl_give_points", "Awards experience points to a player. Usage: css_lvl_give_point <steamid64> <amount>")]
        [CommandHelper(minArgs: 2, usage: "<steamid64> <amount>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void GivePointsCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || player.IsBot) 
            {
                try
                {
                    var steamId64 = command.ArgByIndex(1);
                    var steamId = ConvertSteamID64ToSteamID(steamId64);
                    if (int.TryParse(command.ArgByIndex(2), out var amount))
                    {
                        AddPlayerExperience(steamId, amount);
                        Console.WriteLine($"[LR] {amount} experience points have been awarded to player {steamId} (SteamID64: {steamId64}).");
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount of points. Ensure you have entered a number.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LR] An error occurred while adding experience points: {ex.Message}");
                }
            }
            else
            {
                player.PrintToChat($"{ChatColors.Red}This command is only available from the server console.");
            }
        }
        private void AddPlayerExperience(string steamId, int amount)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var updateQuery = $"UPDATE `{dbConfig.Name}` SET `value` = `value` + @Amount WHERE steam = @SteamID;";
                connection.Execute(updateQuery, new { Amount = amount, SteamID = steamId });
            }
        }
        [ConsoleCommand("css_lvl_reset", "Clears a player's statistics. Usage: css_lvl_reset <steamid64> <data-type>")]
        [CommandHelper(minArgs: 2, usage: "<steamid64> <data-type>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void ResetRanksCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || player.IsBot) 
            {
                try
                {
                    var steamId64 = command.ArgByIndex(1);
                    var steamId = ConvertSteamID64ToSteamID(steamId64);
                    var dataType = command.ArgByIndex(2).ToLower();

                    if (steamId == null)
                    {
                        Console.WriteLine("Incorrect SteamID64.");
                        return;
                    }

                    switch (dataType)
                    {
                        case "exp":
                            ResetPlayerExperience(steamId);
                            Console.WriteLine($"[LR] Experience and rank for player {steamId} (SteamID64: {steamId64}) have been reset.");
                            break;
                        case "stats":
                            ResetPlayerStats(steamId);
                            Console.WriteLine($"[LR] Statistics for player {steamId} (SteamID64: {steamId64}) have been reset.");
                            break;
                        case "time":
                            ResetPlayerPlaytime(steamId);
                            Console.WriteLine($"[LR] Playtime for player {steamId} (SteamID64: {steamId64}) has been reset.");
                            break;
                        default:
                            Console.WriteLine("Invalid data type. Use 'exp', 'stats', or 'time'.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LR] An error occurred while resetting statistics: {ex.Message}");
                }
            }
            else
            {
                player.PrintToChat($"{ChatColors.Red}This command is only available from the server console.");
            }
        }
        private void ResetPlayerExperience(string steamId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var resetQuery = $"UPDATE `{dbConfig.Name}` SET `value` = 0, `rank` = 1 WHERE steam = @SteamID;";
                connection.Execute(resetQuery, new { SteamID = steamId });
            }
        }

        private void ResetPlayerStats2(string steamId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var resetQuery = $"UPDATE `{dbConfig.Name}` SET kills = 0, deaths = 0, shoots = 0, hits = 0, headshots = 0, assists = 0, round_win = 0, round_lose = 0 WHERE steam = @SteamID;";
                connection.Execute(resetQuery, new { SteamID = steamId });
            }
        }

        private void ResetPlayerPlaytime(string steamId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var resetQuery = $"UPDATE `{dbConfig.Name}` SET playtime = 0 WHERE steam = @SteamID;";
                connection.Execute(resetQuery, new { SteamID = steamId });
            }
        }
        private void CreateTable()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                var createTableQuery = string.Format(SQL_CreateTable, $"{dbConfig.Name}", "", "");
                connection.Execute(createTableQuery);
            }
        }
        public DatabaseConfig GetDatabaseConfig()
        {
            return dbConfig;
        }
        public string GetConnectionString()
        {
            return this.ConnectionString;
        }
        public string ConnectionString
        {
            get
            {
                if (dbConfig?.DbHost == null || dbConfig?.DbUser == null || dbConfig?.DbPassword == null || dbConfig?.DbName == null || dbConfig?.DbPort == null)
                    throw new InvalidOperationException("Database configuration is not properly set.");
                
                return $"Server={dbConfig.DbHost};Port={dbConfig.DbPort};User ID={dbConfig.DbUser};Password={dbConfig.DbPassword};Database={dbConfig.DbName};";
            }
        }

        private const string SQL_CreateTable = "CREATE TABLE IF NOT EXISTS `{0}` ( `steam` varchar(22){1} PRIMARY KEY, `name` varchar(32){2}, `value` int NOT NULL DEFAULT 0, `rank` int NOT NULL DEFAULT 0, `kills` int NOT NULL DEFAULT 0, `deaths` int NOT NULL DEFAULT 0, `shoots` int NOT NULL DEFAULT 0, `hits` int NOT NULL DEFAULT 0, `headshots` int NOT NULL DEFAULT 0, `assists` int NOT NULL DEFAULT 0, `round_win` int NOT NULL DEFAULT 0, `round_lose` int NOT NULL DEFAULT 0, `playtime` int NOT NULL DEFAULT 0, `lastconnect` int NOT NULL DEFAULT 0);";
        public override string ModuleAuthor => PluginAuthor;
        public override string ModuleName => PluginName;
        public override string ModuleVersion => PluginVersion;
    }
}