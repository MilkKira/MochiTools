using System;
using EFT.HealthSystem;
using HarmonyLib;
using System.Linq;

namespace MochiTools.Patches
{
    [HarmonyPatch(typeof(ActiveHealthController), nameof(ActiveHealthController.ChangeHealth))]
    public class ChangeHealthPatch
    { 
        
        [HarmonyPrefix]
    private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, ref float value)
    {
        try
        {
            // 只处理本地玩家
            if (__instance.Player?.IsYourPlayer is not true)
                return true;

            // 上帝模式：完全无敌
            if (NekoPlugin.GodMode is { Value: true })
            {
                // 只处理伤害（负值）
                if (value >= 0f) return true;
                value = 0f;

                // 移除负面效果
                foreach (var temBodyPart in (EBodyPart[])Enum.GetValues(typeof(EBodyPart)))
                    __instance.RemoveNegativeEffects(temBodyPart);

                return false;
            }

            // 未开启不死模式，跳过
            if (NekoPlugin.EnableDeathProtection is not { Value: true })
                return true;

            // 判断该部位是否受保护
            if (!IsProtectedBodyPart(bodyPart))
                return true;

            // 只处理伤害（负值）
            if (value >= 0f)
                return true;

            // 应用保底血量逻辑
            return ApplyMinHealthProtection(__instance, bodyPart, ref value);
        }
        catch (Exception e)
        {
            NekoPlugin.Log.LogError($"NekoChangeHealthPatch: {e}");
            return true;
        }
    }

    /// <summary>
    ///     检查身体部位是否受保护
    /// </summary>
    private static bool IsProtectedBodyPart(EBodyPart bodyPart)
    {
        var criticalParts = new[] { EBodyPart.Head, EBodyPart.Chest };

        if (criticalParts.Contains(bodyPart))
            return true;

        // 如果开启腿部破坏，则也保护腿部
        if (NekoPlugin.ProtectLegs is { Value: true })
        {
            var legParts = new[] { EBodyPart.LeftLeg, EBodyPart.RightLeg };
            if (legParts.Contains(bodyPart))
                return true;
        }

        // 如果开启胳膊破坏，则也保护胳膊
        if (NekoPlugin.ProtectArms is { Value: true })
        {
            var armParts = new[] { EBodyPart.LeftArm, EBodyPart.RightArm };
            if (armParts.Contains(bodyPart))
                return true;
        }

        // 如果开启胃破坏，则也保护胃
        if (NekoPlugin.ProtectStomach is { Value: true })
            if (bodyPart is EBodyPart.Stomach)
                return true;

        return false;
    }

    /// <summary>
    ///     应用最低血量保护
    /// </summary>
    private static bool ApplyMinHealthProtection(
        ActiveHealthController controller,
        EBodyPart bodyPart,
        ref float value)
    {
        var health = controller.GetBodyPartHealth(bodyPart);
        var minHealth = 1f; // 保底血量

        // 计算后的血量不会低于 minHealth
        if (health.Current + value >= minHealth)
            return true;

        // 计算需要调整的伤害值
        var newValue = minHealth - health.Current;

        if (newValue <= 0f)
        {
            // 当前血量已经低于或等于保底值，完全阻挡伤害
            value = 0f;
            return false;
        }

        // 调整伤害值，使最终血量等于 minHealth
        value = newValue;
        // 继续执行，但使用修改后的 value
        return true;
    }
        
    }
}