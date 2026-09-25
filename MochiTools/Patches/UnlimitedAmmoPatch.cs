using System.Reflection;
using EFT.Ballistics;
using EFT.InventoryLogic;
using HarmonyLib;
using System;
using EFT;


namespace MochiTools.Patches;

/// <summary>
/// 每次开枪后补回子弹
/// </summary>
[HarmonyPatch(typeof(BallisticsCalculator), nameof(BallisticsCalculator.Shoot), new Type[] { typeof(Shot) })]
public class UnlimitedAmmoPatch
{
    /// <summary>
    /// Postfix：射击后执行
    /// </summary>
    [HarmonyPostfix]
    private static void Postfix(Shot shot)
    {
        // 未开启无穷子弹，跳过
        if (NekoPlugin.UnlimitedAmmo is not { Value: true })
            return;

        // 基础安全检查
        if (shot == null)
            return;

        if (!Comfort.Common.Singleton<ItemFactory>.Instantiated)
            return;

        var owner = shot.Player?.iPlayer;

        // 只对自己生效
        if (owner == null || !owner.IsYourPlayer)
            return;


        var ammo = shot.Ammo;
        var weapon = shot.Weapon as Weapon;

        if (ammo == null || weapon == null)
            return;


        var magazine = weapon.GetCurrentMagazine();


        // ======================
        // 有弹匣
        // ======================
        if (magazine != null)
        {
            // 转轮
            if (magazine is CylinderMagazine cylinder)
            {
                foreach (var slot in cylinder.Camoras)
                    slot.Add(CreateAmmo(ammo), false, true);

                return;
            }

            // 普通弹匣
            var cartridges = magazine.Cartridges;
            cartridges?.Add(CreateAmmo(ammo), false);

            return;
        }


        // ======================
        // 无弹匣（单发枪 / 栓动 / 霰弹枪）
        // ======================
        foreach (var chamber in weapon.Chambers)
            chamber.Add(CreateAmmo(ammo), false, true);
    }


    /// <summary>
    /// 创建新子弹
    /// </summary>
    private static Item CreateAmmo(Item template)
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 24);

        return Comfort.Common.Singleton<ItemFactory>.Instance.CreateItem(
            id,
            template.TemplateId,
            null);
    }
}