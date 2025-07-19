using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HafezBot;

public class FaalResponse
{
    [JsonPropertyName("شعر")]
    public string ?  Poem { get; set; }

    [JsonPropertyName("تعبیر")]
    public string ? Interpretation { get; set; }
}
