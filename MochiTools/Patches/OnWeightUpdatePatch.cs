// using Diz.Binding;
// using EFT.InventoryLogic;
// using HarmonyLib;
//
// namespace MochiTools.Patches;
//
// public class OnWeightUpdatePatch
// {
//     internal static class InventoryWeightPatchHelper
//     {
//         internal static float GetAdjustedWeight(Inventory inventory)
//         {
//             var equipmentWeight = inventory.Equipment?.GetEquipmentWeight() ?? 0f;
//             var multiplier = NekoPlugin.TotalWeightReductionPercentage.Value / 100f;
//             return equipmentWeight * multiplier;
//         }
//     }
//
//     [HarmonyPatch(typeof(Inventory), nameof(Inventory._onWeightUpdated))]
//     public static class InventoryWeightGetterPatch
//     {
//         [HarmonyPrefix]
//         private static bool Prefix(Inventory __instance, ref float __result)
//         {
//             __result = InventoryWeightPatchHelper.GetAdjustedWeight(__instance);
//             return false;
//         }
//     }
//
//     [HarmonyPatch(typeof(Inventory), nameof(Inventory.UpdateTotalWeight))]
//     public static class InventoryWeightUpdatePatch
//     {
//         [HarmonyPostfix]
//         private static void Postfix(Inventory __instance)
//         {
//             var adjustedWeight = InventoryWeightPatchHelper.GetAdjustedWeight(__instance);
//
//             __instance.TotalWeight._value = adjustedWeight;
//             __instance.TotalWeight._isDirty = false;
//             __instance.TotalWeightEliteSkill._value = adjustedWeight;
//             __instance.TotalWeightEliteSkill._isDirty = false;
//
//             if (__instance._onWeightUpdated is BindableEvent onWeightUpdated)
//                 onWeightUpdated.Invoke();
//         }
//     }
// }