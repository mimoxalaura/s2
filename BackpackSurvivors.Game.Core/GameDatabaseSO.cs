using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.ScriptableObjects.Achievement;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.CraftingResource;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.ScriptableObjects.Unlockable;
using BackpackSurvivors.System;
using Tymski;
using UnityEngine;

namespace BackpackSurvivors.Game.Core;

[CreateAssetMenu(fileName = "Core", menuName = "Game/Core", order = 1)]
internal class GameDatabaseSO : ScriptableObject
{
	[SerializeField]
	internal SerializableDictionaryBase<Enums.SceneType, SceneReference> Scenes;

	[SerializeField]
	internal bool AllowDebugging;

	[SerializeField]
	internal Enums.BuildInfo.UnityAnalyticsEnvironment UnityAnalyticsEnvironment;

	[SerializeField]
	internal int MaxDifficulty;

	[SerializeField]
	internal int MaxLevel;

	[SerializeField]
	internal float BackpackAffinityWeighting = 0.01f;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.DamageType, Color> DamageTypeColor;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Color> ItemRarityColor;

	[HelpBox("Materials used for the VFX when dropping / creating items in the backpack", null)]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Material> RarityVFXMaterials;

	[HelpBox("Materials for the borders in the shop offers", null)]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Material> RarityBorderMaterials;

	[HelpBox("Materials for the borders of items", null)]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Material> RarityItemBorderMaterials;

	[HelpBox("Materials for the borders in item tooltips", null)]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Material> TooltipBorderMaterials;

	[HelpBox("Materials for the borders in item tooltips - these are used as internal container borders", null)]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Material> TooltipBorderMaterialsInternal;

	[HelpBox("Materials for the visual weapon change when equiped by the player", null)]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.DamageType, Material> ElementWeaponTypeMaterials;

	[HelpBox("Materials for the weapon attacks", null)]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.DamageType, Material> ElementAttackTypeMaterials;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Sprite> ItemRarity;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableRarity, Sprite> ItemRarityBackground;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.DamageType, Sprite> DamageTypes;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.WeaponType, Sprite> WeaponTypes;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.PlaceableWeaponSubtype, Sprite> PlaceableWeaponSubtypes;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.ItemStatType, Sprite> ItemStatTypeIcons;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.Debuff.DebuffType, Sprite> DebuffIcons;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.Emotes, Sprite> EmoteIcons;

	[Header("GameMenuButton Icons")]
	[SerializeField]
	internal Sprite Resume;

	[SerializeField]
	internal Sprite Character;

	[SerializeField]
	internal Sprite Settings;

	[SerializeField]
	internal Sprite Collection;

	[SerializeField]
	internal Sprite BackToTown;

	[SerializeField]
	internal Sprite ExitGame;

	[SerializeField]
	internal Sprite QuitGame;

	[SerializeField]
	internal Sprite Feedback;

	[Header("Coin Value Icons")]
	[SerializeField]
	internal Sprite CoinValue1000;

	[SerializeField]
	internal Sprite CoinValue500;

	[SerializeField]
	internal Sprite CoinValue100;

	[SerializeField]
	internal Sprite CoinValue25;

	[SerializeField]
	internal Sprite CoinValue10;

	[SerializeField]
	internal Sprite CoinValue1;

	[SerializeField]
	internal Sprite CoinValueDefault;

	[SerializeField]
	internal Sprite TitanicSouls;

	[Header("Custom Icons")]
	[SerializeField]
	internal Sprite SkullSprite;

	[SerializeField]
	internal Sprite MergeSprite;

	[SerializeField]
	internal Sprite PowerIcon;

	[SerializeField]
	internal Sprite DefaultArrowIcon;

	[SerializeField]
	internal Sprite SpecialBossArrowIcon;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.SoftPopupType, Sprite> SoftPopupIcons;

	[SerializeField]
	internal List<AdventureSO> Adventures;

	[SerializeField]
	internal SerializableDictionaryBase<AdventureSO, bool> AdventureAvailability;

	[SerializeField]
	internal List<LevelSO> Levels;

	[SerializeField]
	internal LevelSO ShopLevel;

	[SerializeField]
	internal List<WeaponSO> AvailableWeapons;

	[SerializeField]
	internal List<ItemSO> AvailableItems;

	[SerializeField]
	internal List<ItemSO> AvailableRunes;

	[SerializeField]
	internal List<BagSO> AvailableBags;

	[SerializeField]
	internal List<RelicSO> AvailableRelics;

	[SerializeField]
	internal List<AchievementSO> AvailableAchievements;

	[SerializeField]
	internal List<SteamStatSO> AvailableSteamStats;

	[SerializeField]
	internal List<MergableSO> AvailableMergables;

	[SerializeField]
	internal List<EnemySO> AvailableEnemies;

	[SerializeField]
	internal List<CharacterSO> AvailableCharacters;

	[SerializeField]
	internal List<UnlockableSO> AvailableUnlocks;

	[SerializeField]
	internal List<TalentSO> AvailableTalents;

	[SerializeField]
	internal List<CraftingResourceSO> CraftingResources;

	[SerializeField]
	internal SerializableDictionaryBase<Enums.Debuff.DebuffType, DebuffSO> Debuffs;

	[Header("Backpack Prefabs")]
	[SerializeField]
	internal DraggableBag DraggableBagPrefab;

	[SerializeField]
	internal DraggableItem DraggableItemPrefab;

	[SerializeField]
	internal DraggableWeapon DraggableWeaponPrefab;

	[Header("Combat Prefabs")]
	[SerializeField]
	internal Transform DummyTransformPrefab;

	[SerializeField]
	internal CombatWeapon CombatWeaponPrefab;

	[SerializeField]
	internal Enemy DummyEnemy;

	[Header("Audio")]
	[SerializeField]
	internal SerializableDictionaryBase<Enums.Emotes, AudioClip> EmoteAudio;

	[SerializeField]
	internal AudioClip ShowItemAudio;

	[SerializeField]
	internal List<string> Keywords;
}
