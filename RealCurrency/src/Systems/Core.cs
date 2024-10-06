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
    public const string ModID = "realcurrency";

    public static Dictionary<EnumItemRenderTarget, ModelTransform> Transformations { get; protected set; } = new();
    public static Dictionary<string, int[]> Currencies { get; protected set; } = new();

    public static ConfigRealCurrency ConfigRealCurrency { get; protected set; }

    public static string Currency => ConfigRealCurrency.Currency;
    public static int[] CurrencyDenominations => Currencies.GetValueSafe(Currency);

    public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsClient();

    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("RealCurrency.StackShape", typeof(CollectibleBehaviorStackShape));
        api.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        ConfigRealCurrency = ModConfig.ReadConfig<ConfigRealCurrency>(api, $"RealCurrency-{api.Side}.json");

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
    }
}
