using System.Text.Json;

public class ResetStatsCooldown
{
    public Dictionary<string, long> LastResetTimestamps { get; set; } = new();

    public void Save(string filePath)
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(filePath, json);
    }

    public static ResetStatsCooldown Load(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<ResetStatsCooldown>(json) ?? new ResetStatsCooldown();
        }

        return new ResetStatsCooldown();
    }
}