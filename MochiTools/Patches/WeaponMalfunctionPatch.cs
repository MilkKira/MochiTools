using EFT.InventoryLogic;
using HarmonyLib;

namespace MochiTools.Patches;

// 这个类负责禁止武器所有故障
[HarmonyPatch]
public static class WeaponMalfunctionPatch
{
    // 控制开关：true 禁止所有故障，false 恢复默认
    public static bool DisableAllMalfunctions => NekoPlugin.IgnoreFault?.Value == true;

    // 需要拦截的武器故障 getter 方法
    private static readonly string[] MalfunctionGetters = new string[]
    {
        "get_AllowJam",
        "get_AllowFeed",
        "get_AllowMisfire",
        "get_AllowSlide",
        "get_AllowOverheat"
    };

    // 自动生成所有补丁
    [HarmonyTargetMethods]
    private static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> TargetMethods()
    {
        foreach (var name in MalfunctionGetters)
        {
            var method = AccessTools.Method(typeof(Weapon), name);
            if (method != null)
                yield return method;
        }
    }

    // 前置方法，禁止故障
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        if (!DisableAllMalfunctions)
            return true;

        __result = false; // 禁止故障
        return false; // 阻止原方法执行
    }
}