using EFT;
using HarmonyLib;

namespace MochiTools.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.LateUpdate))]
public class PlayerPatch
{
    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
        if (NekoPlugin.IsInfiniteStamina is { Value: false })
            return;

        if (!__instance.IsYourPlayer)
            return;

        //无限体力
        __instance.Physical.Stamina.UpdateStamina((float)__instance.Physical.Stamina.TotalCapacity);
        //无限手部体力
        __instance.Physical.HandsStamina.UpdateStamina((float)__instance.Physical.HandsStamina.TotalCapacity);
        //无限憋气
        __instance.Physical.Oxygen.UpdateStamina((float)__instance.Physical.Oxygen.TotalCapacity);
        
        //补充能量
        if (NekoPlugin.InfiniteEnergy is { Value: true })
        {
            __instance.ActiveHealthController.ChangeEnergy(__instance.ActiveHealthController.Energy.Maximum);
        }
        //补充水分
        if (NekoPlugin.InfiniteHydration is { Value: true })
        {
            __instance.ActiveHealthController.ChangeHydration(__instance.ActiveHealthController.Hydration.Maximum);
        }
    }
}