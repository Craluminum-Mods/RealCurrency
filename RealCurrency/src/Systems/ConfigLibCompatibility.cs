using ConfigLib;
using ImGuiNET;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace RealCurrency;

public class ConfigLibCompatibility
{
    private const string settingPrefix = $"{ModID}:Config.Setting.";

    public ConfigLibCompatibility(ICoreAPI api)
    {
        Init(api);
    }

    private void Init(ICoreAPI api)
    {
        api.ModLoader.GetModSystem<ConfigLibModSystem>().RegisterCustomConfig(ModID, (id, buttons) => EditConfig(id, buttons, api));
    }

    private void EditConfig(string id, ControlButtons buttons, ICoreAPI api)
    {
        if (buttons.Save) ModConfig.WriteConfig(api, ConfigRealCurrency.Path, Core.ConfigRealCurrency);
        if (buttons.Restore) Core.ConfigRealCurrency = ModConfig.ReadConfig<ConfigRealCurrency>(api, ConfigRealCurrency.Path);
        if (buttons.Defaults) Core.ConfigRealCurrency = new ConfigRealCurrency(api);
        Edit(api, Core.ConfigRealCurrency, id);
    }

    private void Edit(ICoreAPI api, ConfigRealCurrency config, string id)
    {
        BuildSettings(config, id);
    }

    private void BuildSettings(ConfigRealCurrency config, string id)
    {
        ImGui.TextWrapped(Lang.Get($"{ModID}:currencies", GetCurrenciesAsString()));
        ImGui.NewLine();

        int currentCurrency = Currencies.Keys.ToList().IndexOf(config.Currency);
        if (ImGui.Combo(Lang.Get($"{ModID}:Choose currency") + "##choose-currency", ref currentCurrency, string.Join("\0", Currencies.Keys), Currencies.Count))
        {
            config.Currency = Currencies.Keys.ToArray()[currentCurrency];
        }
    }
}