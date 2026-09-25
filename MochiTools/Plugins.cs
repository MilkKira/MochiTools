using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MochiTools.Features;
using MochiTools.Notify;
using UnityEngine;

namespace MochiTools
{
  [BepInPlugin("com.milk.patch", "Neko", "5.0.0")]
  public class NekoPlugin : BaseUnityPlugin
  {
    public static ManualLogSource Log { get; private set; }
    
    public static ConfigEntry<bool> GodMode { get; private set; }
    
    public static ConfigEntry<KeyboardShortcut> GodModeToggle { get; private set; }
    
    public static ConfigEntry<bool> EnableDeathProtection { get; private set; }
    
    public static ConfigEntry<KeyboardShortcut> EnableDeathProtectionToggle { get; private set; }
    
    public static ConfigEntry<bool> IsInfiniteStamina { get; private set; }
    
    public static ConfigEntry<bool> ProtectArms { get; private set; }
    
    public static ConfigEntry<bool> ProtectStomach { get; private set; }
    
    public static ConfigEntry<bool> ProtectLegs { get; private set; }
    
    public static ConfigEntry<float> RecoilMultiplier { get; private set; }
    
    public static ConfigEntry<KeyboardShortcut> RepairDurabilityRequestToggle { get; private set; }
    
    public static ConfigEntry<bool> InfiniteHydration { get; private set; }
    
    public static ConfigEntry<bool> InfiniteEnergy { get; private set; }
    
    public static ConfigEntry<bool> UnlimitedAmmo { get; private set; }
    
    public static ConfigEntry<KeyboardShortcut> UnlimitedAmmoToggle { get; private set; }
    
    public static ConfigEntry<bool> IgnoreFault { get; private set; }
    
    public static ConfigEntry<KeyboardShortcut> IgnoreFaultToggle { get; private set; }
    
    public static ConfigEntry<bool> AllowLooting { get; private set; }
    
    public static ConfigEntry<bool> AllowLootingRemoteInteract { get; private set; }
    
    public static ConfigEntry<bool> AllowLootingBlockItemsInsideSecureContainer { get; private set; }
    
    public static ConfigEntry<int> TotalWeightReductionPercentage { get; private set; }
    private void Awake()
		{
			NekoPlugin.Log = base.Logger;
			NekoPlugin.GodMode = base.Config.Bind<bool>("生命系统", "GodMode", false, "免疫一切伤害捏");
			NekoPlugin.GodModeToggle = base.Config.Bind<KeyboardShortcut>("生命系统", "GodModeToggle", new KeyboardShortcut(KeyCode.F7, Array.Empty<KeyCode>()), "上帝模式切换");
			NekoPlugin.EnableDeathProtection = base.Config.Bind<bool>("生命系统", "EnableDeathProtection", false, "剩下一点血不会似掉捏");
			NekoPlugin.EnableDeathProtectionToggle = base.Config.Bind<KeyboardShortcut>("生命系统", "EnableDeathProtectionToggle", new KeyboardShortcut(KeyCode.F8, Array.Empty<KeyCode>()), "免死切换键");
			NekoPlugin.ProtectArms = base.Config.Bind<bool>("生命系统", "ProtectArms", false, "保护胳膊");
			NekoPlugin.ProtectStomach = base.Config.Bind<bool>("生命系统", "ProtectStomach", false, "保护胃部");
			NekoPlugin.ProtectLegs = base.Config.Bind<bool>("生命系统", "ProtectLegs", false, "保护腿");
			NekoPlugin.RecoilMultiplier = base.Config.Bind<float>("武器系统", "recoilMultiplier", 1f, new ConfigDescription("后坐力倍率", new AcceptableValueRange<float>(0f, 10f), Array.Empty<object>()));
			NekoPlugin.RecoilMultiplier.Value = Mathf.Clamp(NekoPlugin.RecoilMultiplier.Value, 0f, 10f);
			NekoPlugin.UnlimitedAmmo = base.Config.Bind<bool>("武器系统", "UnlimitedAmmo", false, "无穷子弹（至少装填一发）");
			NekoPlugin.UnlimitedAmmoToggle = base.Config.Bind<KeyboardShortcut>("武器系统", "UnlimitedAmmoToggle", new KeyboardShortcut(KeyCode.F9, Array.Empty<KeyCode>()), "无穷子弹切换键");
			NekoPlugin.IgnoreFault = base.Config.Bind<bool>("武器系统", "IgnoreFault", false, "无视故障");
			NekoPlugin.IgnoreFaultToggle = base.Config.Bind<KeyboardShortcut>("武器系统", "IgnoreFaultToggle", new KeyboardShortcut(KeyCode.F10, Array.Empty<KeyCode>()), "无视故障切换键");
			NekoPlugin.AllowLooting = base.Config.Bind<bool>("交互系统", "AllowLooting", false, "允许掠夺原本不可掠夺的物品");
			NekoPlugin.AllowLootingRemoteInteract = base.Config.Bind<bool>("交互系统", "AllowLootingRemoteInteract", false, "允许远程交互并绕过观察状态限制");
			NekoPlugin.AllowLootingBlockItemsInsideSecureContainer = base.Config.Bind<bool>("交互系统", "AllowLootingBlockItemsInsideSecureContainer", false, "阻止从他人的安全箱或特殊槽位掠夺物品");
			NekoPlugin.IsInfiniteStamina = base.Config.Bind<bool>("人物系统", "IsInfiniteStamina", true, "无限体力以及手部体力");
			NekoPlugin.InfiniteEnergy = base.Config.Bind<bool>("人物系统", "InfiniteEnergy", false, "满能量");
			NekoPlugin.InfiniteHydration = base.Config.Bind<bool>("人物系统", "InfiniteHydration", false, "满水分");
			NekoPlugin.RepairDurabilityRequestToggle = base.Config.Bind<KeyboardShortcut>("库存系统", "RepairDurabilityRequest", new KeyboardShortcut(KeyCode.F11, Array.Empty<KeyCode>()), "修复武器耐久");
			NekoPlugin.TotalWeightReductionPercentage = base.Config.Bind<int>("库存系统", "TotalWeightReductionPercentage", 100, new ConfigDescription("用于减少物品总重量的百分比", new AcceptableValueRange<int>(0, 100), Array.Empty<object>()));
			Harmony harmony = new Harmony("com.milk.patch");
			harmony.PatchAll();
			NekoPlugin.Log.LogInfo("Nyan Nyan Nyan ~");
		}
		private void Update()
		{
			this.CheckHotkeys();
		}
		private void CheckHotkeys()
		{
			if (NekoPlugin.GodModeToggle.Value.IsDown())
			{
				NekoPlugin.GodMode.Value = !NekoPlugin.GodMode.Value;
				MochiNotify.OnOff("DEBUG_PATTERN", NekoPlugin.GodMode.Value);
			}
			if (NekoPlugin.EnableDeathProtectionToggle.Value.IsDown())
			{
				NekoPlugin.EnableDeathProtection.Value = !NekoPlugin.EnableDeathProtection.Value;
				MochiNotify.OnOff("LOCK_A_BLOOD", NekoPlugin.EnableDeathProtection.Value);
			}
			if (NekoPlugin.UnlimitedAmmoToggle.Value.IsDown())
			{
				NekoPlugin.UnlimitedAmmo.Value = !NekoPlugin.UnlimitedAmmo.Value;
				MochiNotify.OnOff("UNLIMITED_BULLETS", NekoPlugin.UnlimitedAmmo.Value);
			}
			if (NekoPlugin.IgnoreFaultToggle.Value.IsDown())
			{
				NekoPlugin.IgnoreFault.Value = !NekoPlugin.IgnoreFault.Value;
				MochiNotify.OnOff("IGNORE_THE FAULT", NekoPlugin.IgnoreFault.Value);
			}
			if (NekoPlugin.RepairDurabilityRequestToggle.Value.IsDown())
			{
				ItemRepairFeature.RepairCurrentPlayerItems();
			}
		}
  }
}