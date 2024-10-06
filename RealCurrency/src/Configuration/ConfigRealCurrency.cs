using Newtonsoft.Json;
using System.Linq;
using Vintagestory.API.Common;

namespace RealCurrency;

public class ConfigRealCurrency : IModConfig
{
    [JsonProperty(Order = 1)]
    public string Currencies;

    [JsonProperty(Order = 2)]
    public string Currency { get; set; }

    public ConfigRealCurrency(ICoreAPI api, ConfigRealCurrency previousConfig = null)
    {
        if (previousConfig != null)
        {
            Currency = previousConfig.Currency;
        }

        Currencies = string.Join(", ", Core.Currencies.Keys);
        Currency ??= Core.Currencies.First().Key;
    }
}
