using HarmonyLib;

namespace MochiTools.Patches;

[HarmonyPatch(typeof(ShotEffector), nameof(ShotEffector.Process))]
public class ShotEffectorPatch
{
    [HarmonyPrefix]
    private static void Prefix(ShotEffector __instance, ref float str)
    {
        var value = NekoPlugin.RecoilMultiplier.Value;
        str *= value;
    }
}