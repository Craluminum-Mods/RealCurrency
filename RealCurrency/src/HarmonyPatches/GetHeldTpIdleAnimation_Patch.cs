﻿using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace RealCurrency;

[HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldTpIdleAnimation))]
public static class GetHeldTpIdleAnimation_Patch
{
    public static void Postfix(CollectibleObject __instance, ref string __result)
    {
        if (__instance is ItemRustyGear)
        {
            __result = "holdbothhands";
        }
    }
}
