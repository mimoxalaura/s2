using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.ScriptableObjects.Achievement;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.CraftingResource;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.ScriptableObjects.Unlockable;
using Tymski;
using UnityEngine;

namespace BackpackSurvivors.System.Helper;

internal class GameDatabaseHelper
{
	internal static bool AllowDebugging => SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging;

	internal static TalentSO GetTalentFromId(int talentId)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableTalents.FirstOrDefault((TalentSO x) => x.Id == talentId);
	}

	internal static List<WeaponSO> GetWeapons()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableWeapons;
	}

	internal static List<MergableSO> GetMergeRecipes()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableMergables;
	}

	internal static List<AdventureSO> GetAdventures()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.Adventures;
	}

	internal static List<CraftingResourceSO> GetCraftingResources()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.CraftingResources;
	}

	internal static List<RelicSO> GetRelics()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableRelics;
	}

	internal static List<ItemSO> GetItems()
	{
		List<ItemSO> list = new List<ItemSO>();
		list.AddRange(SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableItems);
		return list;
	}

	internal static List<TalentSO> GetTalents()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableTalents;
	}

	internal static List<EnemySO> GetEnemies()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableEnemies;
	}

	internal static List<CharacterSO> GetCharacters()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableCharacters;
	}

	internal static List<BagSO> GetBags()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableBags;
	}

	internal static List<UnlockableSO> GetUnlockables()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableUnlocks;
	}

	internal static UnlockableSO GetUnlockableFromType(Enums.Unlockable unlockable)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableUnlocks.FirstOrDefault((UnlockableSO x) => x.Unlockable == unlockable);
	}

	internal static SceneReference GetSceneFromType(Enums.SceneType sceneTypes)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.Scenes[sceneTypes];
	}

	internal static BagSO GetBagById(int bagId)
	{
		BagSO bagSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableBags.FirstOrDefault((BagSO b) => b.Id == bagId);
		if (bagSO == null)
		{
			Debug.LogWarning($"Bag with id {bagId} was not found in the Game Database");
		}
		return bagSO;
	}

	internal static WeaponSO GetWeaponById(int weaponId)
	{
		WeaponSO weaponSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableWeapons.FirstOrDefault((WeaponSO w) => w.Id == weaponId);
		if (weaponSO == null)
		{
			Debug.LogWarning($"Weapon with id {weaponId} was not found in the Game Database");
		}
		return weaponSO;
	}

	internal static ItemSO GetItemById(int itemId)
	{
		ItemSO itemSO = GetItems().FirstOrDefault((ItemSO b) => b.Id == itemId);
		if (itemSO == null)
		{
			Debug.LogWarning($"Item with id {itemId} was not found in the Game Database");
		}
		return itemSO;
	}

	internal static CharacterSO GetCharacterById(int characterId)
	{
		CharacterSO characterSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableCharacters.FirstOrDefault((CharacterSO b) => b.Id == characterId);
		if (characterSO == null)
		{
			Debug.LogWarning($"Character with id {characterId} was not found in the Game Database");
		}
		return characterSO;
	}

	internal static RelicSO GetRelicById(int relicId)
	{
		RelicSO relicSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableRelics.FirstOrDefault((RelicSO r) => r.Id == relicId);
		if (relicSO == null)
		{
			Debug.LogWarning($"Relic with id {relicId} was not found in the Game Database");
		}
		return relicSO;
	}

	internal static DraggableBag GetDraggableBagPrefab()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.DraggableBagPrefab;
	}

	internal static DraggableItem GetDraggableItemPrefab()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.DraggableItemPrefab;
	}

	internal static DraggableWeapon GetDraggableWeaponPrefab()
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.DraggableWeaponPrefab;
	}

	internal static DebuffSO GetDebuffSO(Enums.Debuff.DebuffType debuffType)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.Debuffs.TryGet(debuffType, null);
	}

	internal static Color GetRarityColor(Enums.PlaceableRarity rarity)
	{
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.ItemRarityColor[rarity];
	}

	internal static AchievementSO GetAchievementSOByEnum(Enums.AchievementEnum achievementEnum)
	{
		AchievementSO achievementSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableAchievements.FirstOrDefault((AchievementSO r) => r.AchievementEnum == achievementEnum);
		if (achievementSO == null)
		{
			Debug.LogWarning($"Achievement for enum {achievementEnum} was not found in the Game Database");
		}
		return achievementSO;
	}

	internal static SteamStatSO GetSteamStatSOByEnum(Enums.SteamStat steamStat)
	{
		SteamStatSO steamStatSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableSteamStats.FirstOrDefault((SteamStatSO s) => s.SteamStat == steamStat);
		if (steamStatSO == null)
		{
			Debug.LogWarning($"SteamStat for enum {steamStatSO} was not found in the Game Database");
		}
		return steamStatSO;
	}
}
