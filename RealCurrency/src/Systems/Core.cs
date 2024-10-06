global using static RealCurrency.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace RealCurrency;

public class Core : ModSystem
{
    private Harmony HarmonyInstance => new Harmony(Mod.Info.ModID);

    public const string ModID = "realcurrency";

    public static ConfigRealCurrency ConfigRealCurrency { get; set; }

    public static Dictionary<EnumItemRenderTarget, ModelTransform> Transformations { get; protected set; } = new();
    public static Dictionary<string, int[]> Currencies { get; protected set; } = new();

    public static string GetCurrenciesAsString() => string.Join(", ", Currencies.Keys);

    public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsClient();

    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("RealCurrency.StackShape", typeof(CollectibleBehaviorStackShape));

        HarmonyInstance.PatchAll();

        api.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        ConfigRealCurrency = ModConfig.ReadConfig<ConfigRealCurrency>(api, ConfigRealCurrency.Path);

        if (api.ModLoader.IsModEnabled("configlib"))
        {
            _ = new ConfigLibCompatibility(api);
        }

        foreach (CollectibleObject obj in api.World.Collectibles)
        {
            if (obj?.Code != null && obj is ItemRustyGear)
            {
                obj.CollectibleBehaviors = obj.CollectibleBehaviors.Append(new CollectibleBehaviorStackShape(obj));
            }
        }
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        Transformations = api.Assets.TryGet(ModID + ":config/transformations.json").ToObject<Dictionary<string, ModelTransform>>().ToDictionary(x => Enum.Parse<EnumItemRenderTarget>(x.Key), x => x.Value);
        Currencies = api.Assets.TryGet(ModID + ":config/currencies.json").ToObject<Dictionary<string, int[]>>();
    }

    public override void Dispose()
    {
        Transformations.Clear();
        Currencies.Clear();

        HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
    }
}
