using Newtonsoft.Json;
using System.Linq;
using Vintagestory.API.Common;

namespace RealCurrency;

public class ConfigRealCurrency : IModConfig
{
    public const string Path = "RealCurrency-Client.json";

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

        Currencies = GetCurrenciesAsString();
        Currency ??= Core.Currencies.First().Key;
    }
}
