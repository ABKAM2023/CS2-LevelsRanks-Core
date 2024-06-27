namespace LevelsRanksApi
{
    public class User
    {
        public string? SteamId { get; set; }
        public string? Name { get; set; }
        public int Value { get; set; }
        public int Rank { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Shoots { get; set; }
        public int Hits { get; set; }
        public int Headshots { get; set; }
        public int Assists { get; set; }
        public int RoundWin { get; set; }
        public int RoundLose { get; set; }
        public int Playtime { get; set; }
        public int LastConnect { get; set; }
    }
}