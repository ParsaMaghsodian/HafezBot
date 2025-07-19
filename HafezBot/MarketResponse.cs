using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HafezBot;

public class MarketResponse
{
    [JsonPropertyName("gold")]
    public IList<MarketItems>? Gold { get; set; }
    [JsonPropertyName("currency")]
    public IList<MarketItems>? Currency { get; set; }
    [JsonPropertyName("cryptocurrency")]
    public IList<MarketItems>? CryptoCurrency { get; set; }
}
