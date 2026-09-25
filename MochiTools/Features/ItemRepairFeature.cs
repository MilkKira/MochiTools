using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using MochiTools.Notify;

namespace MochiTools.Features;

public static class ItemRepairFeature
{
    private static readonly FieldInfo? MainPlayerField =
        typeof(GameWorld).GetField("MainPlayer", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void RepairCurrentPlayerItems()
    {
        if (!Singleton<GameWorld>.Instantiated)
        {
            NekoPlugin.Log.LogWarning("Durability repair skipped: GameWorld not instantiated.");
            MochiNotify.Msg("Fix failed: Did not enter the battle.");
            return;
        }

        var player = ResolveCurrentPlayer();
        if (player?.Profile?.Inventory == null)
        {
            NekoPlugin.Log.LogWarning("Durability repair skipped: current player or inventory not available.");
            MochiNotify.Msg("Fix failed: The current player or backpack was not found.");
            return;
        }

        var allPlayerItems = player.Profile.Inventory.GetPlayerItems().ToArray();
        if (allPlayerItems.Length == 0)
        {
            NekoPlugin.Log.LogInfo("Durability repair skipped: no player items found.");
            MochiNotify.Msg("Repair Failed: No repairable item found.");
            return;
        }

        var repairedCount = 0;

        foreach (var item in allPlayerItems)
        {
            var itemChanged = false;

            //耐久物品(武器、护甲)
            var repairable = item?.GetItemComponent<RepairableComponent>();
            if (repairable != null)
            {
                repairable.MaxDurability = repairable.TemplateDurability;
                repairable.Durability = repairable.MaxDurability;
                repairedCount++;
                itemChanged = true;
            }
            
            //钥匙
            var key = item?.GetItemComponent<KeyComponent>();
            if (key != null)
            {
                key.NumberOfUsages = 0;
                repairedCount++;
                itemChanged = true;
            }
            
            //医疗用品
            var medKit = item?.GetItemComponent<MedKitComponent>();
            if (medKit != null)
            {
                medKit.HpResource = medKit.MaxHpResource;
                repairedCount++;
                itemChanged = true;
            }
            
            //食物
            var foodDrink = item?.GetItemComponent<FoodDrinkComponent>();
            if (foodDrink != null)
            {
                foodDrink.HpPercent = foodDrink.MaxResource;
                repairedCount++;
                itemChanged = true;
            }

            var resource = item?.GetItemComponent<ResourceComponent>();
            if (resource != null)
            {
                resource.Value = resource.MaxResource;
                repairedCount++;
                itemChanged = true;
            }

            if (item != null && itemChanged)
            {
                item.UpdateAttributes();
                item.RaiseRefreshEvent(refreshIcon: true, checkMagazine: false);
            }
        }

        NekoPlugin.Log.LogInfo($"Durability repair completed. Updated components: {repairedCount}.");
        MochiNotify.Msg(repairedCount > 0 ? $"Item durability and resources have been fixed ({repairedCount})." : "There are no repairable item resources.");
    }

    private static IPlayer? ResolveCurrentPlayer()
    {
        var gameWorld = Singleton<GameWorld>.Instance;

        var mainPlayer = MainPlayerField?.GetValue(gameWorld) as IPlayer;
        if (mainPlayer != null)
            return mainPlayer;

        if (gameWorld.RegisteredPlayers == null)
            return null;

        return gameWorld.RegisteredPlayers.FirstOrDefault(player => player?.IsYourPlayer == true);
    }
}