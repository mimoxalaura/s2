using System;
using System.Collections.Generic;

namespace BackpackSurvivors.System;

public class Enums
{
	public enum InputType
	{
		Ingame,
		UI
	}

	public enum DraggableFailureReason
	{
		NotEnoughCurrency,
		DropzoneIllegal,
		PartiallyPlaceble
	}

	public enum WavePositionSpawnType
	{
		Random,
		Grouped,
		Circle,
		Line
	}

	public class Modal
	{
		public enum OpenDirection
		{
			Horizontal,
			Vertical,
			Both,
			None,
			UseGiven
		}
	}

	public enum Emotes
	{
		Attention,
		Question
	}

	public enum PlayerMessageType
	{
		Default,
		Good,
		Bad
	}

	public enum RuneSpecialEffectDestructionType
	{
		DestroyAfterTrigger,
		DestroyAfterShopEntering,
		NeverDestroy,
		DestroyAfterX
	}

	public enum CurrencySource
	{
		Drop,
		Save,
		Shop,
		PrerunSetup,
		Reward
	}

	public enum RewardType
	{
		Weapon,
		Item,
		Relic,
		TitanicSouls,
		Bag
	}

	public enum GameMenuButtonType
	{
		Resume,
		Character,
		Settings,
		BackToTown,
		ExitGame,
		Collection,
		Feedback,
		QuitGame
	}

	[Flags]
	public enum SpawnDirection
	{
		None = 0,
		NorthOfPlayer = 1,
		EastOfPlayer = 2,
		SouthOfPlayer = 4,
		WestOfPlayer = 8,
		All = -1
	}

	public enum CursorState
	{
		Default,
		Grabbing,
		CannotDoAction,
		CanActivate,
		CanDeactivate
	}

	public enum TooltipType
	{
		Default,
		Item,
		Weapon,
		Talent,
		Relic,
		Bag,
		Stat,
		Buff,
		CraftingResource,
		Inline
	}

	public enum TargetingTooltipType
	{
		UseAttackTargetingType,
		Custom,
		DontShow
	}

	public enum PlaceableRarity
	{
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary,
		Mythic,
		Unique
	}

	[Flags]
	public enum RelicSource
	{
		None = 0,
		Miniboss = 1,
		Boss = 2,
		Altar = 4,
		Unlocks = 8,
		Adventure = 0x10,
		All = -1
	}

	internal enum RelicSetupType
	{
		Once,
		OncePerLevel
	}

	public enum PlaceableType
	{
		Weapon = 0,
		Bag = 10,
		Item = 11
	}

	public enum PlaceableItemSubtype
	{
		Trinket = 0,
		Shield = 1,
		BodyArmor = 2,
		LegArmor = 3,
		Boots = 4,
		Gloves = 5,
		Amulet = 6,
		Ring = 7,
		Headwear = 8,
		Special = 9,
		None = 999
	}

	public enum PlaceableWeaponSubtype
	{
		Sword = 0,
		Hammer = 1,
		Axe = 2,
		FistWeapon = 3,
		Halberd = 4,
		Bow = 5,
		Crossbow = 6,
		Javelin = 7,
		Throwing = 8,
		Wand = 9,
		Staff = 10,
		Spellbook = 11,
		Whip = 12,
		Exotic = 13,
		Firearm = 14,
		None = 999
	}

	public enum MergeType
	{
		TwoWeapons,
		TwoItems,
		WeaponAndItem
	}

	public enum MergeResult
	{
		Weapon,
		Item
	}

	public enum ItemModifierSourceType
	{
		Talent,
		Relic,
		Bag,
		GlobalItem,
		ConnectedItem,
		ConnectedWeapon,
		ModifierOverride,
		ConditionalStatsItem,
		Buff,
		BaseCharacter,
		Formula,
		ConditionalFormula
	}

	public enum SceneType
	{
		MainMenu,
		Credits,
		Town,
		Story,
		TrainingRoom,
		FinalShop,
		CombatLevel
	}

	public enum MovieStep
	{
		Step0_Black,
		Step1_FromTheVoid,
		Step2_NoReason,
		Step3_VoidCorrupted,
		Step3_5_OrcsCorrupted_,
		Step3_8_Black,
		Step3_9_HumanCiv,
		Step4_Gateways,
		Step5_Fortresses,
		Step6_EliteSoldiers,
		Step7_HardBattle,
		Step8_FellToTheHordes,
		Step8_5_Black,
		Step9_BastionsDestroyed,
		Step10_OneRemained,
		Step11_ChildOfHumanity,
		Step12_KeyIsGiven,
		Step13_FinalFortress,
		Step13_5_Black,
		Step14_DemonHordesAwait,
		Step15_Ready,
		Step15_5_Black,
		Step6_BlackTitle
	}

	public enum RichPresenceOptions
	{
		Status_AtMainMenu,
		Status_InFortress,
		Status_InAdventure1Level1,
		Status_InAdventure1Level2,
		Status_InAdventure1Level3,
		Status_InAdventure1Level4,
		Status_InAdventure1Level5,
		Status_InAdventure2Level1,
		Status_InAdventure2Level2,
		Status_InAdventure2Level3,
		Status_InAdventure2Level4,
		Status_InAdventure2Level5,
		Status_InAdventure3Level1,
		Status_InAdventure3Level2,
		Status_InAdventure3Level3,
		Status_InAdventure3Level4,
		Status_InAdventure3Level5,
		Status_InAdventure4Level1,
		Status_InAdventure4Level2,
		Status_InAdventure4Level3,
		Status_InAdventure4Level4,
		Status_InAdventure4Level5,
		Status_StartingNewAdventure,
		Status_FinalShop,
		Status_InInfiniteModeLevel1
	}

	public enum ModalUITypes
	{
		GameMenu,
		Stats,
		Settings,
		Quests,
		Difficulty,
		Talents,
		Unlocks,
		Collection,
		Adventure,
		RelicReward,
		AdventureCompleted,
		Shop,
		Backpack,
		None,
		TrainingRoomItemSelection,
		GenericPopup,
		Demo,
		Blacksmith,
		Revive,
		Achievements
	}

	public enum SizeScaleSource
	{
		FromProjectileSize,
		FromAoeSize
	}

	public enum ProjectileMovementFreedom
	{
		AtPlayer,
		InWorld,
		AtEnemy
	}

	public enum ProjectileStartPosition
	{
		OnPlayer,
		OnTarget,
		OnPosition,
		OnCustomPosition
	}

	public enum ProjectileStartPointFlip
	{
		NoNotFlip,
		FlipWithPlayer
	}

	public enum ProjectileMovement
	{
		None,
		HeadingTowardsTarget,
		RotatingAroundStartPosition,
		Waving,
		MovingIntoAngleDirection,
		Boomerang,
		Thrown,
		MovingFacingPlayerDirection,
		MovingFacingPlayerAim
	}

	public enum ProjectileChaining
	{
		None,
		ChainToNearestEnemy
	}

	public enum ProjectileSpreading
	{
		None,
		Spread
	}

	public enum ProjectileRotation
	{
		None,
		TowardsTheEnemy,
		StableAngle,
		RotatingConstantly
	}

	public enum ProjectileFlip
	{
		None,
		FlipXBasedOnTargetPosition,
		FlipXBasedOnPlayerDirection
	}

	public enum CollectionMetrics
	{
		Weapons,
		Items,
		Bags,
		Relics,
		Recipes,
		Enemies
	}

	public enum SingleValueStatisticMetrics
	{
		CompletedRuns,
		BossesKilled,
		MiniBossesKilled,
		TimesDied,
		TotalCoinsLooted,
		TotalTitanicSoulsLooted,
		TotalExperienceGained,
		TotalWeaponsGained,
		TotalItemsCollected
	}

	public enum EnemyMovementType
	{
		None,
		FollowPlayer,
		FollowPlayerAndPause,
		FollowPlayerUntilRange,
		FollowPlayerUntilRangeAndRunAway,
		Burrowing,
		Charging,
		MoveInFixedDirection,
		PassThroughPlayerLocation,
		RunAwayFromPlayerUntilRange,
		RunAwayInRandomDirectionWhenPlayerInRange,
		Stationary,
		MoveToFixedPoint
	}

	public enum CharacterType
	{
		Unknown,
		Player,
		Enemy
	}

	[Flags]
	public enum CharacterFilter
	{
		None = 0,
		Player = 1,
		Enemy = 2,
		All = -1
	}

	public enum AttackTargetingType
	{
		AttackClosestEnemy = 0,
		AttackRandomEnemy = 1,
		TargetCursorDONOTUSE = 2,
		AttackPlayer = 3,
		AttackPlayerDirection = 4,
		AttackPlayerAim = 5,
		AttackDummy = 6,
		AttackCardinalDirection = 7,
		None = 999
	}

	public enum CardinalDirection
	{
		MovingNorth,
		MovingEast,
		MovingSouth,
		MovingWest,
		MovingNorthEast,
		MovingSouthEast,
		MovingSouthWest,
		MovingNorthWest
	}

	[Flags]
	public enum WeaponType
	{
		None = 0,
		Melee = 1,
		Ranged = 2,
		All = -1
	}

	public enum WeaponAttackTypeDurationAndPiercing
	{
		ShortAndInfinitePiercing,
		LongAndUseStatPiercing
	}

	public enum WeaponAnimationSize
	{
		Small = 0,
		Large = 1,
		None = 999
	}

	public enum WeaponAnimationType
	{
		Melee = 0,
		Spell = 1,
		Bow = 2,
		Thrown = 3,
		Firearm = 4,
		None = 999
	}

	public enum DamageCalculationType
	{
		Default,
		BasedOnWeaponDamagePercentage
	}

	public enum OverrideOrAdd
	{
		Override,
		Add
	}

	public enum AnimationType
	{
		None,
		Melee,
		Bow,
		Spell
	}

	public class AnimatorStateMachineEnums
	{
		public enum PlayerAnimationStates
		{
			None = 0,
			Spawning = 10,
			Despawning = 20,
			DamageTaken = 30,
			Dash = 40,
			Dying = 50,
			Dead = 60,
			Succes = 70,
			ShowItem = 80,
			Revive = 90,
			IdleNoWeapon = 200,
			IdleLargeMeleeWeapon = 210,
			IdleSmallMeleeWeapon = 220,
			IdleLargeSpellWeapon = 230,
			IdleSmallSpellWeapon = 240,
			IdleLargeBowWeapon = 250,
			IdleSmallBowWeapon = 260,
			IdleSmallThrownWeapon = 270,
			IdleLargeThrownWeapon = 280,
			IdleLargeFireArmWeapon = 290,
			AttackNoWeapon = 300,
			AttackLargeMeleeWeapon = 310,
			AttackSmallMeleeWeapon = 320,
			AttackLargeSpellWeapon = 330,
			AttackSmallSpellWeapon = 340,
			AttackLargeBowWeapon = 350,
			AttackSmallBowWeapon = 360,
			AttackLargeThrownWeapon = 370,
			AttackSmallThrownWeapon = 380,
			AttackLargeFireArmWeapon = 390,
			RunNoWeapon = 400,
			RunLargeMeleeWeapon = 410,
			RunSmallMeleeWeapon = 420,
			RunLargeSpellWeapon = 430,
			RunSmallSpellWeapon = 440,
			RunLargeBowWeapon = 450,
			RunSmallBowWeapon = 460,
			RunLargeThrownWeapon = 470,
			RunSmallThrownWeapon = 480,
			RunLargeFireArmWeapon = 490,
			RunAttackNoWeapon = 500,
			RunAttackLargeMeleeWeapon = 510,
			RunAttackSmallMeleeWeapon = 520,
			RunAttackLargeSpellWeapon = 530,
			RunAttackSmallSpellWeapon = 540,
			RunAttackLargeBowWeapon = 550,
			RunAttackSmallBowWeapon = 560,
			RunAttackLargeThrownWeapon = 570,
			RunAttackSmallThrownWeapon = 580,
			RunAttackLargeFireArmWeapon = 590
		}

		public enum IdleState
		{
			NoWeapon,
			SmallWeapon,
			LargeWeapon
		}
	}

	public enum AudioType
	{
		Music,
		Ambiance,
		SFX,
		Master
	}

	public enum VideoSettingsType
	{
		Resolution,
		Windowmode,
		Worlddetail,
		Bloom,
		MaxFPS,
		QualityShaders,
		CameraShake
	}

	public enum FadeStatus
	{
		FadingIn,
		FadingOut,
		None
	}

	public enum SettingsPanels
	{
		Audio,
		Video,
		Gameplay
	}

	public enum Resolutions
	{
		_1920_x_1080,
		_2560_x_1440,
		_3840_x_2160,
		_1128_x_634,
		_1280_x_720,
		_1366_x_766,
		_1600_x_900,
		_1760_x_990
	}

	public enum Windowmodes
	{
		Windowed,
		WindowedBorderless
	}

	public enum CameraShake
	{
		Enabled,
		Disabled
	}

	public enum WorldDetail
	{
		Full,
		Medium,
		Low
	}

	public enum Bloom
	{
		Enabled,
		Disabled
	}

	public enum WeatherEffects
	{
		Enabled,
		Disabled
	}

	public enum QualityShaders
	{
		Enabled,
		Disabled
	}

	public enum ShowDamageNumbers
	{
		Visible,
		Hidden
	}

	public enum ShowHealthBars
	{
		Visible,
		Hidden,
		OnlyBosses
	}

	public enum CooldownVisuals
	{
		Icon,
		Bar
	}

	public enum MinimapVisual
	{
		Visible,
		Hidden
	}

	public enum TooltipComplexity
	{
		AlwaysVisible,
		VisibleOnAlt
	}

	public enum FlashOnDamageTaken
	{
		Enabled,
		Disabled
	}

	public enum DashVisual
	{
		InUI,
		UnderPlayer,
		Both,
		None
	}

	public enum Targeting
	{
		Automatic,
		Manual,
		AutomaticButManualOnHotkey
	}

	public enum CurrencyType
	{
		Coins,
		TitanSouls
	}

	public enum CraftingResource
	{
		DemonicDiamond,
		SpiderSilk,
		DamnationAcid,
		CorruptedOre,
		HolyCrystal
	}

	public enum MinimapType
	{
		Player,
		Enemy,
		Friendly,
		Loot,
		Interactable,
		OverrideWithIcon,
		Nothing,
		Projectile
	}

	public enum DefaultAudioType
	{
		ButtonClick,
		ButtonHover,
		UIOpen,
		UIClose
	}

	public enum LevelCompletedRank
	{
		None,
		Bronze,
		Silver,
		Gold
	}

	public enum LingeringEffectType
	{
		Fire,
		PoisonPool,
		BlackHole,
		FrostField,
		Darkness,
		IceSpike
	}

	public enum LingeringEffectRotation
	{
		Default,
		Random
	}

	public enum WeaponStatType
	{
		CritChancePercentage,
		CritMultiplier,
		DamagePercentage,
		CooldownTime,
		WeaponRange,
		Penetrating,
		ExplosionSizePercentage,
		LifeDrainPercentage,
		ProjectileCount,
		ProjectileSpeed,
		ProjectileSizePercentage,
		StunChancePercentage,
		CooldownReductionPercentage,
		FlatDamage,
		ProjectileDuration
	}

	public enum ItemStatType
	{
		CritChancePercentage,
		CritMultiplier,
		Health,
		DamagePercentage,
		SpeedPercentage,
		LuckPercentage,
		CooldownTime,
		DamageReductionPercentageDONOTUSE,
		Armor,
		Spiked,
		FlatDamage,
		PickupRadiusPercentage,
		EnemyCount,
		WeaponRange,
		Penetrating,
		ExplosionSizePercentage,
		LifeDrainPercentage,
		ProjectileCount,
		ProjectileSpeed,
		ProjectileSizePercentage,
		StunChancePercentage,
		ExtraCoinChancePercentage,
		CooldownReductionPercentage,
		ExtraDash,
		DodgePercentage,
		ProjectileDuration,
		HealthRegeneration,
		MaximumCompanionCount,
		WeaponCapacity,
		DamageAgainstNormalEnemies,
		DamageAgainstEliteAndBossEnemies,
		ExperienceGainedPercentage,
		BuffDuration,
		DebuffDuration,
		Knockback
	}

	public enum ItemStatTypeGroup
	{
		Offensive,
		Defensive,
		Utility,
		Other,
		Unused,
		Hidden
	}

	public enum LootType
	{
		Coins,
		Health,
		TitanicSouls,
		Magnet,
		Other
	}

	public class Debuff
	{
		public enum DebuffType
		{
			Burn = 0,
			Poison = 1,
			Bleed = 2,
			ArmorReduction = 3,
			Slow = 4,
			Stun = 5,
			Arcing = 6,
			Slowdown = 7,
			Twilight = 8,
			Gloom = 9,
			None = 999
		}

		public enum DebuffTriggerType
		{
			Once,
			Continuous,
			Never
		}

		public enum DebuffFalloffTimeType
		{
			SetTime,
			AfterTrigger,
			Infinite
		}
	}

	public class Buff
	{
		public enum BuffTriggerType
		{
			Once,
			Continuous
		}

		public enum BuffFalloffTimeType
		{
			SetTime,
			AfterTrigger,
			Infinite
		}

		public enum BuffType
		{
			Invincibility,
			ArmorIncrease,
			DamageIncrease,
			Heal,
			MovementSpeed
		}
	}

	[Flags]
	public enum PlaceableTag : long
	{
		None = 0L,
		Physical = 1L,
		Fire = 2L,
		Cold = 4L,
		Lightning = 8L,
		Void = 0x10L,
		Poison = 0x20L,
		Energy = 0x40L,
		Holy = 0x80L,
		Trinket = 0x100L,
		Shield = 0x200L,
		BodyArmor = 0x400L,
		LegArmor = 0x800L,
		Boots = 0x1000L,
		Gloves = 0x2000L,
		Amulet = 0x4000L,
		Ring = 0x8000L,
		Headwear = 0x10000L,
		Sword = 0x20000L,
		Hammer = 0x40000L,
		Axe = 0x80000L,
		FistWeapon = 0x100000L,
		Halberd = 0x200000L,
		Bow = 0x400000L,
		Crossbow = 0x800000L,
		Javelin = 0x1000000L,
		Throwing = 0x2000000L,
		Wand = 0x4000000L,
		Staff = 0x8000000L,
		Spellbook = 0x10000000L,
		Whip = 0x20000000L,
		Exotic = 0x40000000L,
		Melee = 0x80000000L,
		Ranged = 0x100000000L,
		SpellDONOTUSE = 0x200000000L,
		Common = 0x400000000L,
		Uncommon = 0x800000000L,
		Rare = 0x1000000000L,
		Epic = 0x2000000000L,
		Legendary = 0x4000000000L,
		Mythic = 0x8000000000L,
		FireArm = 0x10000000000L,
		Special = 0x20000000000L,
		Unique = 0x40000000000L,
		Blunt = 0x80000000000L,
		Slashing = 0x100000000000L,
		Piercing = 0x200000000000L,
		MergeIngredient = 0x400000000000L
	}

	public enum PlaceableTagType
	{
		DamageType = 0,
		WeaponSubtype = 1,
		ItemSubtype = 2,
		WeaponType = 3,
		Rarity = 4,
		None = 999
	}

	[Flags]
	public enum DamageType
	{
		None = 0,
		PhysicalDONOTUSE = 1,
		Fire = 2,
		Cold = 4,
		Lightning = 8,
		Void = 0x10,
		Poison = 0x20,
		Energy = 0x200,
		Holy = 0x400,
		Blunt = 0x800,
		Slashing = 0x1000,
		Piercing = 0x2000,
		All = -1
	}

	public enum WeaponAttackMovement
	{
		NoMovement,
		DirectlyOnTarget,
		HeadingTowardsTarget,
		CirclingTransform,
		BouncingBetweenEnemies,
		WavingTowardsTarget,
		MovingNorth,
		MovingEast,
		MovingSouth,
		MovingWest,
		MovingNorthEast,
		MovingSouthEast,
		MovingSouthWest,
		MovingNorthWest,
		Lightning,
		HeadingTowardsPlayerAim,
		NoMovementWithAiming
	}

	public enum WeaponAttackForm
	{
		Projectile,
		SingleInstance
	}

	public enum WeaponAttackEffectTrigger
	{
		OnStart,
		OnFlight,
		OnHit,
		OnDestroy
	}

	public enum TooltipValueDifference
	{
		SameAsBase,
		HigherThenBase,
		LowerThenBase
	}

	public class Enemies
	{
		public enum EnemyType
		{
			Monster,
			Minion,
			Elite,
			Miniboss,
			Boss,
			EnvironmentalHazard,
			Player
		}

		public enum EnemyOnHitType
		{
			Default = 0,
			Flesh = 1,
			Bone = 2,
			Energy = 3,
			Steel = 4,
			Stone = 5,
			Wood = 6,
			None = 999
		}

		[Flags]
		public enum EnemyTriggerType
		{
			None = 0,
			Damaged = 1,
			Dead = 2,
			TimeBased = 4,
			PercentageDamaged = 8,
			PercentageHealthReached = 0x10
		}
	}

	public enum Unlockable
	{
		UnlockAltar,
		TalentTree,
		ExtraCharacter1,
		ExtraCharacter2,
		ExtraCharacter3,
		ElementalTypedBags,
		Difficulties,
		AdventurePortal,
		DailyQuests,
		Blacksmith,
		CharacterCustomization,
		Collections,
		ExtraDashes,
		ExtraRevives,
		ExtraCharacter0,
		ExtraShopOffers,
		TrainingRoom,
		BagSpace,
		RelicRerolls,
		ShopBanishes,
		ShopRerolls,
		ExtraStartingGold,
		DiscountChance
	}

	public enum CharacterClass
	{
		Warrior,
		Mage,
		Rogue,
		Necromancer
	}

	internal enum ImmunitySource
	{
		Dash,
		DamageTaken,
		LeavingShop,
		Burrowed,
		Reviving
	}

	public enum SoftPopupType
	{
		FirstKill
	}

	public class Backpack
	{
		public enum GridCellStatus
		{
			Empty,
			ContainsBag,
			ContainsItem,
			Locked
		}

		public enum ItemSizeCellType
		{
			None,
			CellContainsPlacable,
			CellContainsStar
		}

		public enum ItemRotation
		{
			Rotation0,
			Rotation90,
			Rotation180,
			Rotation270
		}

		public enum GridType
		{
			Backpack,
			Storage
		}

		public enum DraggableOwner
		{
			Shop,
			Player,
			Collection
		}

		public enum DraggableType
		{
			Bag,
			Item,
			Weapon
		}

		public enum BagSizes
		{
			_1x1,
			_1x2,
			_1x3,
			_1x4,
			_2x2,
			_2x3,
			_T,
			_Z,
			_2x5,
			_3x3
		}
	}

	public enum GenericPopupLocation
	{
		MiddleOfScreen,
		BottomOfScreen,
		TopOfScreen,
		LeftOfScreen,
		RightOfScreen
	}

	[Flags]
	public enum GenericPopupButtons
	{
		Yes = 0,
		No = 2,
		Cancel = 4,
		Ok = 8
	}

	public enum Tutorial
	{
		None,
		Town,
		Talents,
		Backpack,
		TitanicSouls
	}

	public enum AchievementEnum
	{
		None,
		TestAchievement1,
		TestAchievement2,
		TestAchievement3,
		Demo_AdventureWon
	}

	public enum SteamStat
	{
		None,
		TestSteamStat
	}

	public class BuildInfo
	{
		public enum UnityAnalyticsEnvironment
		{
			Development,
			DemoProduction,
			AnalyticsTest,
			Production
		}
	}

	public class ConditionalStats
	{
		public enum ConditionTarget
		{
			Global,
			Weapon
		}

		public enum ConditionCheckType
		{
			Minimum,
			Maximum
		}

		public enum TypeToCheckAgainst
		{
			PlaceableTagCount,
			PlaceableTypeCount,
			StatType,
			CoinAmount
		}

		public enum FormulaType
		{
			Fixed,
			Calculated,
			ComplexCalculation
		}
	}

	public static List<DamageType> GetUsedDamageTypes()
	{
		List<DamageType> unusedDamageTypes = GetUnusedDamageTypes();
		List<DamageType> list = new List<DamageType>();
		foreach (object value in Enum.GetValues(typeof(DamageType)))
		{
			if (!unusedDamageTypes.Contains((DamageType)value))
			{
				list.Add((DamageType)value);
			}
		}
		return list;
	}

	public static List<DamageType> GetUnusedDamageTypes()
	{
		return new List<DamageType>
		{
			DamageType.None,
			DamageType.PhysicalDONOTUSE
		};
	}
}
