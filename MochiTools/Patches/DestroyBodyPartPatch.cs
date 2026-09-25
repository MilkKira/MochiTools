using EFT;
using EFT.HealthSystem;
using HarmonyLib;

namespace MochiTools.Patches;

/// <summary>
///     阻止关键身体部位被摧毁（避免黑胸黑头）
/// </summary>
[HarmonyPatch(typeof(ActiveHealthController), nameof(ActiveHealthController.DestroyBodyPart))]
public class DestroyBodyPartPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, EDamageType damageType)
    {
        // 未开启保护，直接放行
        if (NekoPlugin.EnableDeathProtection?.Value != true)
            return true;

        // 非本地玩家，不处理
        if (__instance.Player?.IsYourPlayer != true)
            return true;

        // 判断是否受保护部位
        return !IsProtectedBodyPart(bodyPart);
    }

    /// <summary>
    ///     检查身体部位是否受保护
    /// </summary>
    private static bool IsProtectedBodyPart(EBodyPart bodyPart)
    {
        // 始终保护的头胸
        if (bodyPart is EBodyPart.Head or EBodyPart.Chest)
            return true;

        // 保护腿部
        if (NekoPlugin.ProtectLegs?.Value == true)
            if (bodyPart is EBodyPart.LeftLeg or EBodyPart.RightLeg)
                return true;
        // 保护胳膊
        if (NekoPlugin.ProtectArms?.Value == true)
            if (bodyPart is EBodyPart.LeftArm or EBodyPart.RightArm)
                return true;
        // 保护胃
        if (NekoPlugin.ProtectStomach?.Value == true)
            if (bodyPart is EBodyPart.Stomach)
                return true;


        return false;
    }
}