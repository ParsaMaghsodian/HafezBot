using System.Text.Json;

namespace HafezBot;

public class MarketItems
{
    public string? name { get; set; }
    public string? date { get; set; }
    public string? time { get; set; }
    public string? unit { get; set; }
    public JsonElement? price { get; set; }
    public string ?symbol { get; set; }
}