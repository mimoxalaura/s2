using UnityEngine;

namespace BackpackSurvivors.System;

public class Constants
{
	public class ShaderNames
	{
		public const string MetalFade = "_MetalFade";
	}

	public class SceneNames
	{
		public const string TownSceneName = "4. Town";
	}

	public class VisualSceneNames
	{
		public const string FortressSceneName = "Fortress";
	}

	public class CommonGameObjectNames
	{
		public const string WorkingGameplayCanvas = "WorkingGameplayCanvas";

		public const string MinimapCanvas = "MinimapCanvas";

		public const string UICamera = "UICamera";

		public const string PortalInteractionZone = "PortalInteractionZone";

		public const string AnimationContainer = "AnimationContainer";

		public const string PortalCamera = "PortalCamera";
	}

	public class Movie
	{
		public const string Step1 = "It came from the Void...";

		public const string Step2 = "There was no communication, only destruction...";

		public const string Step3 = "Everything it touched was corrupted beyond recognition...";

		public const string Step3_5 = "Some easier then others...\r\n\r\nEven nature was corrupted";

		public const string Step3_9 = "Humanity flurished for hundreds of years";

		public const string Step4 = "";

		public const string Step5 = "Fortresses were erected to protect the Gateways and humanity";

		public const string Step6 = "Old magics forged powerfull warriors - The Aetherian Wardens";

		public const string Step7 = "In their final hours, they stood strong...";

		public const string Step8 = "...but the Wardens were not enough. Humanity fell to the Void.";

		public const string Step9 = "The bastions of humanity were destroyed.";

		public const string Step10 = "However, one fortress remained. Hidden away during the cataclysm.";

		public const string Step11 = "centuries later, the Warden bloodline continues...";

		public const string Step12 = "A chance for victory - a fortress key is crafted";

		public const string Step13 = "The final fortress has been found.";

		public const string Step14 = "The currupted hordes await...\r\nThe void awaits...";

		public const string Step15 = "Are you ready?";
	}

	public class Combat
	{
		public const float PlayerImmunityTimeAfterTakingDamage = 0.5f;

		public const float EnemyImmunityTimeAfterTakingSpikeDamage = 0.5f;
	}

	public class PlayerPrefsVariables
	{
		public const string PlayerId = "PlayerId";
	}

	public class AssetMenuPaths
	{
		public const string AssetRoot = "Game";
	}

	public class Modal
	{
		public const float OpenTweenTime = 0.3f;

		public const float CloseTweenTime = 0.1f;
	}

	public class Messages
	{
		public const string LevelBossAppeared = "LEVEL BOSS APPEARED!";
	}

	public class Save
	{
		public const int SaveSlots = 3;

		public const string FileExtension = "*.bps";

		public const string SavedSettingsFileName = "BackpackSurvivors.settings";
	}

	public class Input
	{
		public const string MouseScrollWheel = "Mouse ScrollWheel";

		public const string KeyboardControlschemeName = "Keyboard&Mouse";

		public const string GamepadControlschemeName = "Gamepad";
	}

	public class Socials
	{
		public const string Steam = "https://store.steampowered.com/app/2294780/Backpack_Survivors";

		public const string Twitter = "https://x.com/CMGameDev";

		public const string Youtube = "https://www.youtube.com/@CMGameStudio";

		public const string Discord = "https://discord.gg/xjDKSkGWhU";

		public const string Wishlist = "steam://openurl/https://store.steampowered.com/app/2294780/Backpack_Survivors#game_area_purchase";
	}

	public class ButtonTexts
	{
		public const string Resume = "Resume";

		public const string Character = "Character";

		public const string Settings = "Settings";

		public const string BackToTown = "Back to Town";

		public const string ExitGame = "Main Menu";

		public const string QuitGame = "Quit Game";

		public const string Collection = "Collection";

		public const string Feedback = "Feedback";
	}

	public class AnimationParameters
	{
		public class EnemyAnimationParameters
		{
			public const string AnimationIsMoving = "IsMoving";

			public const string AnimationOnAttack = "OnAttack";

			public const string AnimationOnHurt = "OnHurt";

			public const string AnimationOnDeath = "OnDeath";

			public const string AnimationIsDead = "IsDead";

			public const string AnimationUnderground = "Underground";

			public const string AnimationAppear = "Appear";

			public const string AnimationDisappear = "Disappear";

			public const string AnimationMoving = "Moving";

			public const string AnimationBossBeetleSpiking = "Spiking";

			public const string AnimationBossBeetleStartSpiking = "StartSpiking";

			public const string AnimationBossBeetleRolling = "Rolling";

			public const string AnimationBossBeetleStartRolling = "StartRolling";

			public const string AnimationBossBeetleFinishRolling = "FinishRolling";
		}

		public class PlayerAnimationParameters
		{
			public const string AnimationParamAttackMelee = "DoAttackMelee";

			public const string AnimationParamAttackThrow = "DoAttackThrow";

			public const string AnimationParamAttackBow = "DoAttackBow";

			public const string AnimationParamIsRunning = "IsRunning";

			public const string AnimationParamIsDead = "IsDead";

			public const string AnimationParamDoDying = "DoDying";

			public const string AnimationParamDoHit = "DoHit";

			public const string AnimationParamSpawned = "Spawned";

			public const string AnimationParamSpawning = "Spawning";

			public const string AnimationParamDespawn = "Despawn";

			public const string AnimationParamDoDash = "Dash";

			public const string AnimationParamHasWeapon = "HasWeapon";

			public const string AnimationParamHasMagicWeapon = "HasMagicWeapon";

			public const string AnimationParamLevelUp = "LevelUp";
		}

		public class CharacterImmunityAnimationParameters
		{
			public const string AnimationParamImmunitySpawned = "Spawned";

			public const string AnimationParamImmunityDroppingSoon = "DroppingSoon";
		}

		public class EffectAnimationParameters
		{
			public const string EffectAnimationActive = "Active";

			public const string EffectAnimationStart = "Start";

			public const string EffectAnimationComplete = "Complete";

			public const string EffectAnimationClosed = "Closed";
		}

		public class CurrencyAnimationParameters
		{
			public const string CurrencyAnimationReceived = "Received";

			public const string CurrencyAnimationLost = "Lost";
		}

		public class RelicRewardAnimationParameters
		{
			public const string RelicSelected = "Selected";
		}

		public class InteractableAnimationParameters
		{
			public const string InteractableAnimationActivated = "Activated";

			public const string PortalIsOpen = "IsOpen";

			public const string PortalOpen = "Open";

			public const string PortalClose = "Close";

			public const string CollectionOpen = "Open";

			public const string ChaliceGlow = "Open";
		}

		public class VendorSellingAnimationParameters
		{
			public const string VendorSelling = "VendorSelling";

			public const string RerollChest = "Closed";

			public const string Reserved = "Reserved";

			public const string Spawned = "Spawned";
		}

		public class UIAnimationParameters
		{
			public const string AnimationIsOpen = "IsOpen";

			public const string AnimationOpen = "Open";

			public const string AnimationTransitionOut = "TransitionOut";

			public const string AnimationTransitionIn = "TransitionIn";
		}
	}

	public class ModalFormAnimationParameters
	{
		public const string ModalFormAnimationOpen = "Open";
	}

	public class ProjectileVisualizationAnimationParameters
	{
		public const string ProjectileVisualizationAnimationGoing = "Going";

		public const string ProjectileVisualizationAnimationSpawn = "Spawn";

		public const string ProjectileVisualizationAnimationOnHit = "OnHit";

		public const string ProjectileVisualizationAnimationDestroying = "Destroying";
	}

	public class ObjectNames
	{
		public const string MainCamera = "Main Camera";
	}

	public class ReplacementStringForDescriptors
	{
		public const string MinDamage = "{mindamage}";

		public const string MaxDamage = "{maxdamage}";

		public const string WeaponType = "{weaponType}";
	}

	public class Descriptions
	{
		public class Debuff
		{
			public const string DebuffText = "Enemies hit are afflicted by";

			public const string StacksText = "(stacks)";
		}

		public class Override
		{
			public const string OverrideText = "damage is converted to";
		}

		public class ItemStatType
		{
			public const string CritChancePercentage = "Your chance to do critical damage on an attack.";

			public const string CritMultiplier = "The amount of extra damage when the attack critted.";

			public const string DamagePercentage = "Global damage increase - affects all attacks.";

			public const string CooldownTime = "Reduces the cooldown your items have after an attack. The cooldown cannot be reduced below 25% of the original cooldown.";

			public const string WeaponRange = "The range your weapons attack targets at.";

			public const string Penetrating = "The amount of enemies your attacks can penetrate.";

			public const string ExplosionSizePercentage = "This increases the size of your area of affect effects.";

			public const string LifeDrainPercentage = "The chance per attack to heal 1 life. (0.1s cooldown)";

			public const string ProjectileCount = "The amount of projectiles fired with each weapon attack.";

			public const string ProjectileSpeed = "The speed weapon projectiles move at.";

			public const string ProjectileSizePercentage = "The size of your weapon attacks that use projectiles.";

			public const string Health = "Your health - if you lose it all you die.";

			public const string HealthRegeneration = "Health regained every 5 seconds.";

			public const string SpeedPercentage = "The speed your character moves at.";

			public const string LuckPercentage = "Your luck, affecting the rarity of items appearing in the shop. Also affects the chance of finding Altars and usable drops from enemies.";

			public const string DamageReductionPercentage = "A percentual damage reduction. Affects all damage taken and reduces by this % amount.";

			public const string Armor = "Modifies incoming damage. Currently preventing <color={1}>{0}%</color> of incoming damage.";

			public const string ArmorNegative = "Modifies incoming damage. Currently taking <color={1}>{0}%</color> extra incoming damage.";

			public const string FlatDamage = "Increases damage done by all weapon attacks by a flat amount.";

			public const string PickupRadiusPercentage = "The radius at which your character can pickup items.";

			public const string EnemyCount = "The amount of enemies occasionally spawning additionally during an adventure.";

			public const string Spiked = "Damage returned to the attacker when your character is damaged.";

			public const string StunChancePercentage = "The chance your weapon attacks can stun the target enemy";

			public const string ExtraCoinChancePercentage = "The chance of an enemy dropping additional coins on death";

			public const string CooldownReductionPercentage = "Reduces the cooldown your items have after an attack. The cooldown cannot be reduced below 25% of the original cooldown.";

			public const string ExtraDash = "The amount of dashes your character has.";

			public const string DodgePercentage = "The chance of avoiding an enemy attack fully.";

			public const string ProjectileDuration = "The duration your weapon attack projectiles are alive before destroyed.";

			public const string MaximumCompanionCount = "The maximum amount of companions allowed.";

			public const string WeaponCapacity = "The maximum amount of weapons you can have active at the same time";
		}

		public class DamageType
		{
			public const string None = "Additional damage caused by SLASHING Weapons and Attacks.";

			public const string Physical = "Additional damage caused by PHYSICAL Weapons and Attacks";

			public const string Fire = "Additional damage caused by FIRE Weapons and Attacks";

			public const string Cold = "Additional damage caused by COLD Weapons and Attacks";

			public const string Lightning = "Additional damage caused by LIGHTNING Weapons and Attacks";

			public const string Void = "Additional damage caused by VOID Weapons and Attacks";

			public const string Poison = "Additional damage caused by POISON Weapons and Attacks";

			public const string Blunt = "Additional damage caused by BLUNT Weapons and Attacks";

			public const string Piercing = "Additional damage caused by PIERCING Weapons and Attacks";

			public const string Slashing = "Additional damage caused by SLASHING Weapons and Attacks";

			public const string Energy = "Additional damage caused by ENERGY Weapons and Attacks";

			public const string Holy = "Additional damage caused by HOLY Weapons and Attacks";
		}
	}

	public class CleanStrings
	{
		public class RuneSpecialEffectDestructionType
		{
			public const string DestroyAfterTrigger = "<color=#FF0000>Destroyed</color> after triggering";

			public const string DestroyAfterShopEntering = "<color=#FF0000>Destroyed</color> after entering the next shop";

			public const string NeverDestroy = "<color=#00A6FF>Triggers</color> <color=#FFFFFF>every</color> shop entry";

			public const string DestroyAfterX = "<color=#FF0000>Destroyed</color> after <color=#FFFFFF>[MaxTriggerCount]</color> triggers <color=#FFFFFF>([CurrentTriggerCount]/[MaxTriggerCount])</color>";
		}

		public class CraftingResource
		{
			public const string DemonicDiamond = "Demonic Diamond";

			public const string SpiderSilk = "Spider Silk";

			public const string DamnationAcid = "Damnation Acid";

			public const string CorruptedOre = "Corrupted Ore";

			public const string HolyCrystal = "Holy Crystal";
		}

		public class Currency
		{
			public const string TitanSouls = "Titan Souls";

			public const string Coins = "Gold";
		}

		public class PlaceableType
		{
			public const string Weapon = "Weapon";

			public const string Bag = "Bag";

			public const string Item = "Item";

			public const string Weapons = "Weapons";

			public const string Bags = "Bags";

			public const string Items = "Items";
		}

		public class ItemSubtype
		{
			public const string Trinket = "Trinket";

			public const string Shield = "Shield";

			public const string BodyArmor = "BodyArmor";

			public const string LegArmor = "LegArmor";

			public const string Boots = "Boots";

			public const string Gloves = "Gloves";

			public const string Amulet = "Amulet";

			public const string Ring = "Ring";

			public const string Helmet = "Headwear";
		}

		public class WeaponStatType
		{
			public const string CritChancePercentage = "Crit Chance";

			public const string CritMultiplier = "Crit Multiplier";

			public const string DamagePercentage = "Damage";

			public const string CooldownTime = "Cooldown Reduction";

			public const string WeaponRange = "Weapon Range";

			public const string Penetrating = "Penetrating";

			public const string ExplosionSizePercentage = "Area of Effect Size";

			public const string LifeDrainPercentage = "Life Drain";

			public const string ProjectileCount = "Projectile Count";

			public const string ProjectileSpeed = "Projectile Speed";

			public const string ProjectileSizePercentage = "Projectile Size";

			public const string StunChancePercentage = "Stun Chance";

			public const string CooldownReductionPercentage = "Cooldown Reduction";

			public const string FlatDamage = "Base Damage";

			public const string ProjectileDuration = "Projectile Duration";
		}

		public class ItemStatType
		{
			public const string CritChancePercentage = "Crit Chance";

			public const string CritMultiplier = "Crit Multiplier";

			public const string DamagePercentage = "Damage";

			public const string CooldownTime = "Cooldown Reduction";

			public const string WeaponRange = "Weapon Range";

			public const string Penetrating = "Penetrating";

			public const string ExplosionSizePercentage = "Area of Effect Size";

			public const string LifeDrainPercentage = "Life Drain";

			public const string ProjectileCount = "Projectile Count";

			public const string ProjectileSpeed = "Projectile Speed";

			public const string ProjectileSizePercentage = "Projectile Size";

			public const string StunChancePercentage = "Stun Chance";

			public const string CooldownReductionPercentage = "Cooldown Reduction";

			public const string FlatDamage = "Base Damage";

			public const string ProjectileDuration = "Projectile Duration";

			public const string Health = "Health";

			public const string HealthRegeneration = "Health Regeneration";

			public const string SpeedPercentage = "Speed";

			public const string LuckPercentage = "Luck";

			public const string DamageReductionPercentage = "Damage Reduction DO NOT USE";

			public const string Armor = "Armor";

			public const string PickupRadiusPercentage = "Pickup Radius";

			public const string EnemyCount = "Enemy Count";

			public const string Spiked = "Spiked";

			public const string ExtraCoinChancePercentage = "Extra Coin Chance";

			public const string ExtraDash = "Dashes";

			public const string DodgePercentage = "Dodge Chance";

			public const string MaximumCompanionCount = "Maximum Companion Count";

			public const string WeaponCapacity = "Weapon Capacity";

			public const string DamageAgainstNormalEnemies = "Damage Against weak enemies";

			public const string DamageAgainstEliteAndBossEnemies = "Damage Against strong enemies";

			public const string ExperienceGainedPercentage = "Experience Gained";

			public const string BuffDuration = "Buff Duration";

			public const string DebuffDuration = "Debuff Duration";

			public const string Knockback = "Knockback";
		}
	}

	public class ProjectileMovementStrings
	{
		public const string CirclingRange = "Circling range";
	}

	public class LootLocker
	{
		public class Leaderboards
		{
			public class DailyChallenges
			{
				public const string DailyChallengeDay1Key = "daily_challenge_day_1";

				public const string DailyChallengeDay2Key = "daily_challenge_day_2";

				public const string DailyChallengeDay3Key = "daily_challenge_day_3";

				public const string DailyChallengeDay4Key = "daily_challenge_day_4";

				public const string DailyChallengeDay5Key = "daily_challenge_day_5";

				public const string DailyChallengeDay6Key = "daily_challenge_day_6";

				public const string DailyChallengeDay7Key = "daily_challenge_day_7";

				public const string DailyChallengeDay8Key = "daily_challenge_day_8";

				public const string DailyChallengeDay9Key = "daily_challenge_day_9";

				public const string DailyChallengeDay10Key = "daily_challenge_day_10";

				public const string DailyChallengeDay11Key = "daily_challenge_day_11";

				public const string DailyChallengeDay12Key = "daily_challenge_day_12";

				public const string DailyChallengeDay13Key = "daily_challenge_day_13";

				public const string DailyChallengeDay14Key = "daily_challenge_day_14";

				public const string DailyChallengeDay15Key = "daily_challenge_day_15";

				public const string DailyChallengeDay16Key = "daily_challenge_day_16";

				public const string DailyChallengeDay17Key = "daily_challenge_day_17";

				public const string DailyChallengeDay18Key = "daily_challenge_day_18";

				public const string DailyChallengeDay19Key = "daily_challenge_day_19";

				public const string DailyChallengeDay20Key = "daily_challenge_day_20";

				public const string DailyChallengeDay21Key = "daily_challenge_day_21";

				public const string DailyChallengeDay22Key = "daily_challenge_day_22";

				public const string DailyChallengeDay23Key = "daily_challenge_day_23";

				public const string DailyChallengeDay24Key = "daily_challenge_day_24";

				public const string DailyChallengeDay25Key = "daily_challenge_day_25";

				public const string DailyChallengeDay26Key = "daily_challenge_day_26";

				public const string DailyChallengeDay27Key = "daily_challenge_day_27";

				public const string DailyChallengeDay28Key = "daily_challenge_day_28";

				public const string DailyChallengeDay29Key = "daily_challenge_day_29";

				public const string DailyChallengeDay30Key = "daily_challenge_day_30";

				public const string DailyChallengeDay31Key = "daily_challenge_day_31";
			}

			public const string MaxLevelFinishedKey = "max_level_finished";
		}
	}

	public class Commands
	{
		public class Collection
		{
			public class Weapons
			{
				public const string All = "collection.weapons.all";

				public const string None = "collection.weapons.None";

				public const string ById = "collection.weapon";
			}

			public class Items
			{
				public const string All = "collection.items.all";

				public const string None = "collection.items.None";

				public const string ById = "collection.item";
			}

			public class Bags
			{
				public const string All = "collection.bags.all";

				public const string None = "collection.bags.None";

				public const string ById = "collection.bag";
			}

			public class Relics
			{
				public const string All = "collection.relics.all";

				public const string None = "collection.relics.None";

				public const string ById = "collection.relic";
			}

			public class Enemies
			{
				public const string All = "collection.enemies.all";

				public const string None = "collection.enemies.None";

				public const string ById = "collection.enemy";
			}

			public class Recipes
			{
				public const string All = "collection.recipes.all";

				public const string None = "collection.recipes.None";

				public const string ById = "collection.recipe";
			}

			public const string All = "collection.unlock_all";

			public const string None = "collection.lock_all";
		}

		public const string Currency_Give = "currency.give";

		public const string Currency_GainCurrency = "currency.gain-currency";

		public const string Currency_LoseCurrency = "currency.lose-currency";

		public const string CraftingResource_Gain = "craftingResource.gain";

		public const string CraftingResource_Lose = "craftingResource.lose";

		public const string Settings_Open = "settings.open-modal";

		public const string Settings_Close = "settings.close-modal";

		public const string Vendor_Spawn = "vendor.spawn";

		public const string Player_Kill = "player.kill";

		public const string Player_Animation_Success = "player.animation.succes";

		public const string Player_Animation_Dead = "player.animation.dead";

		public const string Player_Animation_AttackMelee = "player.animation.melee";

		public const string Player_Animation_AttackThrow = "player.animation.throw";

		public const string Player_Animation_AttackBow = "player.animation.bow";

		public const string Player_Animation_DoHit = "player.animation.dohit";

		public const string Player_Animation_DoDamageBlocked = "player.animation.damageBlocked";

		public const string Player_Animation_SetSpawned = "player.animation.Spawn";

		public const string Player_Animation_SetRevive = "player.animation.Revive";

		public const string Player_Animation_SetDespawned = "player.animation.Despawn";

		public const string Player_Emote_Speak = "player.emote.speak";

		public const string Player_Emote_emote = "player.emote.emote";

		public const string Player_Message = "player.message";

		public const string Player_ChangeWeapon = "player.weapons.change";

		public const string Player_ChangeShield = "player.shield.change";

		public const string Player_ChangeArmor = "player.armor.change";

		public const string Player_ChangeHelmet = "player.helmet.change";

		public const string Player_ChangeGloves = "player.gloves.change";

		public const string Player_ChangeBoots = "player.boots.change";

		public const string Player_ChangeElementType = "player.element.change";

		public const string Player_LevelUp = "player.animation.levelup";

		public const string Effects_LingeringSpawn = "effects.lingering.spawn";

		public const string ReloadAndStartCombat = "combat.start";

		public const string WeaponbarsChangeVisual = "weaponbar.change";

		public const string Backpack_AddWeapon = "backpack.add.weapon";

		public const string Backpack_AddBag = "backpack.add.bag";

		public const string Backpack_AddItem = "backpack.add.item";

		public const string Backpack_Open = "backpack.open";

		public const string Backpack_Close = "backpack.close";

		public const string Backpack_Clear = "backpack.clear";

		public const string Backpack_Save = "backpack.Save";

		public const string Backpack_Load = "backpack.Load";

		public const string Backpack_ActivateSpecialEffects = "backpack.ActivateSpecialEffects";

		public const string Merge_Update = "merge.Update";

		public const string Merge_Execute = "merge.Execute";

		public const string Merge_ShowToast = "merge.ShowToast";

		public const string Stats_Open = "stats.open";

		public const string Stats_Close = "stats.close";

		public const string Stats_Recalculate = "stats.recalculate";

		public const string Relic_Add = "relics.add";

		public const string Relic_OpenReward = "relics.reward";

		public const string Relic_SetSource = "relics.SetSource";

		public const string Relic_Init = "relics.Init";

		public const string Relic_Reroll = "relics.Reroll";

		public const string Progression_Save = "progression.save";

		public const string Progression_Load = "progression.load";

		public const string Unlockable_Unlock = "unlockable.unlock";

		public const string Unlockable_Lock = "unlockable.lock";

		public const string Unlockable_Open = "unlockable.open";

		public const string Unlockable_ShowMessage = "unlockable.showMessage";

		public const string Achievements_Open = "achievements.open";

		public const string Talent_AddPoints = "talent.addpoints";

		public const string Talent_RemovePoints = "talent.removepoints";

		public const string Shop_GuaranteeBag = "shop.guaranteebag";

		public const string Shop_GuaranteeWeapon = "shop.guaranteeweapon";

		public const string Shop_GuaranteeItem = "shop.guaranteeitem";

		public const string Shop_Reroll = "shop.reroll";

		public const string Shop_Open = "shop.open";

		public const string Shop_Close = "shop.close";

		public const string Shop_TestRandomizer = "shop.testrandomizer";

		public const string Shop_TestRandomizerRarity = "shop.testrandomizerrarity";

		public const string Shop_LogContents = "shop.logContent";

		public const string GameMenu_Open = "menu.open";

		public const string GameMenu_Close = "menu.close";

		public const string Hellfire_Unlock = "hellfire.unlock";

		public const string Letterbox_Info = "letterbox.info";

		public const string Letterbox_Setup = "letterbox.setup";

		public const string Audiocontroller_Test_Left = "audiocontroller.test.left";

		public const string Audiocontroller_Test_Right = "audiocontroller.test.right";

		public const string Character_SwitchCharacter = "character.switch";

		public const string Character_AddExperience = "character.addexperience";

		public const string Adventure_OpenCompletionUI = "adventure.complete";

		public const string Level_Complete = "level.complete";

		public const string Demo_OpenUI = "Demo.OpenUI";

		public const string BossHealthUI_Init = "BossHealth.UI.Init";

		public const string GenericPopup_Create = "GenericPopup.Create";

		public const string GenericPopup_Open = "GenericPopup.Open";

		public const string GenericPopup_Close = "GenericPopup.Close";

		public const string PlayerHealthUI_Gained = "PlayerHealthUI.Gained";

		public const string PlayerHealthUI_Lost = "PlayerHealthUI.Lost";

		public const string Build_UpdateSaveGamesBuildNumber = "Build.UpdateSaveGameBuildNumber";

		public const string Enemies_Kill = "Enemies.kill";

		public const string InLevelTransition_Test = "InLevelTransition.Test";

		public const string DPS_WriteDPSLogsToFile = "DPS.WriteLogsToFile";

		public const string DPS_SaveDPSLogsToFile = "DPS.SaveLogsToFile";
	}

	public class Shop
	{
		public const string ShopLocked = "Locked";

		public const string ShopUnlocked = "Unlocked";

		public const string ShopLockedDescription = "This <u>will not</u> be rerolled when rerolling the shop";

		public const string ShopUnlockedDescription = "This <u>will</u> be rerolled when rerolling the shop";

		public const float ShopScaleSize = 1.5f;

		public const float ShopSpawnScaleDuration = 0.2f;

		public const float ShopRemoveScaleDuration = 0.4f;
	}

	public class Colors
	{
		public class HexStrings
		{
			public const string ShopItemStatTitleColor = "F8EFBA";

			public const string RarityCommonColor = "#FFFFFF";

			public const string RarityUncommonColor = "#00FF02";

			public const string RarityRareColor = "#4C70FF";

			public const string RarityEpicColor = "#E200FB";

			public const string RarityLegendaryColor = "#BC3600";

			public const string RarityMythicColor = "#FFCC00";

			public const string RarityUniqueColor = "#00FFBF";

			public static string TooltipHexStringSameAsBase = "#DE9F08";

			public static string TooltipHexStringHigherThenBase = "#52FF00";

			public static string TooltipHexStringLowerThenBase = "#CB0F00";

			public static string TooltipHexColorConditionUnsatisfiedRarityCommon = "#57514C";

			public static string TooltipHexColorConditionUnsatisfiedRarityUncommon = "#27201B";

			public static string TooltipHexColorConditionUnsatisfiedRarityRare = "#27201B";

			public static string TooltipHexColorConditionUnsatisfiedRarityEpic = "#27201B";

			public static string TooltipHexColorConditionUnsatisfiedRarityLegendary = "#27201B";

			public static string TooltipHexColorConditionUnsatisfiedRarityMythic = "#27201B";

			public static string TooltipHexColorConditionUnsatisfiedRarityUnique = "#27201B";

			public static string TooltipHexColorConditionSatisfied = "#FFFFFF";

			public static string DamageTypePhysical = "#FFFFFF";

			public static string DamageTypeFire = "#FF3B00";

			public static string DamageTypeCold = "#84FFEE";

			public static string DamageTypeLightning = "#FFEFAC";

			public static string DamageTypeVoid = "#E300BB";

			public static string DamageTypePoison = "#00FF1D";

			public static string DamageTypeEnergy = "#00DBBC";

			public static string DamageTypeHoly = "#EABC00";

			public static string DamageTypeBlunt = "#FFFFFF";

			public static string DamageTypeSlashing = "#FFFFFF";

			public static string DamageTypePiercing = "#FFFFFF";

			public static string CollectionDefault = "#842852";

			public static string CollectionAll = "#2D6600";

			public static string Green = "#52FF00";

			public static string Red = "#CB0F00";

			public static string White = "#FFFFFF";

			public static string HighlightKeyword = "#52FF00";

			public static string SellForColor = "#67FF00";

			public static string DefaultTextColor = "#FFFFFF";

			public static string DefaultKeywordColor = "#292929";

			public static string WeaponTypeMelee = "#FF9F25";

			public static string WeaponTypeRanged = "#5BFF4C";

			public static string DefaultItemType = "#FFD54C";

			public static string DefaultWeaponType = "#FFD54C";

			public static string MergeIngredientColor = "#CB67D9";
		}

		public static Color PositiveEffectColor = Color.green;

		public static Color NegativeEffectColor = Color.red;

		public static Color CoinColor = Color.yellow;

		public static Color ShopItemStatTitleColor = new Color(248f, 239f, 186f);

		public static Color ShopItemCategoryColor = new Color(248f, 239f, 186f);

		public static Color WeaponBaseDamageColor = new Color(91f, 91f, 91f);

		public static Color PlayerHealingColor = new Color(0f, 255f, 33f);

		public static Color PlayerDamagedColor = new Color(127f, 0f, 55f);

		public static Color PlayerBlockedDamageColor = new Color(127f, 127f, 127f);

		public static Color PlayerDamagedByCritColor = new Color(255f, 48f, 234f);

		public static Color EnemyDamagedColor = new Color(255f, 255f, 255f);

		public static Color EnemyDamagedByCritColor = new Color(255f, 116f, 0f);

		public static Color ValidBackpackPlacementColor = new Color(0f, 1f, 0f, 0.5f);

		public static Color InvalidBackpackPlacementColor = new Color(1f, 0f, 0f, 0.5f);

		public static Color PlayerDodgedAttackColor = Color.green;

		public static Color Poisoned = Color.green;

		public static Color Burning = Color.red;

		public static Color Bleeding = Color.black;

		public static Color ArmorReduced = Color.grey;

		public static Color Slowed = Color.blue;

		public static Color Stunned = Color.yellow;

		public static Color ButtonTextColorRegular = new Color(55f, 33f, 55f, 1f);

		public static Color ButtonTextColorDark = Color.white;
	}

	public class Tooltips
	{
		public const string RequiresAtLeast = "Requires at least";

		public const string RequiresAtMost = "Requires at most";

		public const string Coins = "coins";

		public const string Items = "items";

		public const string Item = "item";

		public const string Weapons = "weapons";

		public const string Weapon = "weapon";

		public const string Tags = "tags";

		public const string Tag = "tag";

		public const string Affects = "Affects";

		public const string NumberPlaceholder = "[NUMBER]";

		public const string CheckMarkIconSpriteSheetString = "spritesheet_YES";

		public const string CrossMarkIconSpriteSheetString = "spritesheet_NO";

		public const string TooltipPoint = "<sprite name=\"TooltipPointNormal\"> ";

		public const string TooltipPointNegative = "<sprite name=\"TooltipPointNegative\"> ";

		public const string TooltipPointPositive = "<sprite name=\"TooltipPointPositive\"> ";

		public const string PositiveStarSprite = "<sprite name=\"PositiveStar\">";

		public const string NegativeStarSprite = "<sprite name=\"NegativeStar\">";

		public const string CoinSprite = "<sprite name=\"Coin\">";

		public const string SpecialItemActivationSprite = "<sprite name=\"SpecialItemActivation\">";

		public const string SpecialItemDestructionSprite = "<sprite name=\"SpecialItemDestruction\">";

		public const string SpecialItemAfterTriggerSprite = "<sprite name=\"SpecialItemDestroyAfterTrigger\">";

		public const string SpecialItemNeverDestroySprite = "<sprite name=\"SpecialItemNeverDestroy\">";

		public const string SpecialItemDestroyAfterXSprite = "<sprite name=\"SpecialItemDestroyAfterX\">";

		public const string ItemSprite = "<sprite name=\"Item\">";

		public const string WeaponSprite = "<sprite name=\"Weapon\">";

		public const string BagSprite = "<sprite name=\"Bag\">";

		public const string YesSprite = "<sprite name=\"spritesheet_YES\">";

		public const string NoSprite = "<sprite name=\"spritesheet_NO\">";

		public const string Permanent = "Permanent";

		public const string Cooldown = "Cooldown";

		public const string Range = "Range";

		public const string FormulaTransform = "FormulaTransform";

		public const string FormulaTransformNegative = "FormulaTransformNegative";

		public const string DebuffFor = "for";

		public const string DebuffOnce = "once";

		public const string DebuffForever = "forever";

		public const string Damage = "DAMAGE";

		public const string All = "all";

		public const string ReplaceTextCurrentTriggerCount = "[CurrentTriggerCount]";

		public const string ReplaceTextMaxTriggerCount = "[MaxTriggerCount]";
	}

	public class Tutorial
	{
		public class Town
		{
			public static string Speak1 = "The <b>ancient fortress</b>! I finally found it!";

			public static string Speak2 = "It is deserted...";

			public static string Speak3 = "Wait! What is that?";

			public static string Speak4 = "I see, this is the <b>ancient gateway</b> from the stories!";

			public static string Speak5 = "And here I can use <sprite name=\"TitanSoul\"> <b>resources</b> to improve the fortress!";

			public static string Speak6 = "I should go and see where this goes - my first <b>adventure!</b>";

			public static string Speak1_1 = "That was interesting!";

			public static string Speak1_2 = "I found my first <sprite name=\"TitanSoul\"><color=#DE9F08>Titan Souls</color>!";

			public static string Speak1_3 = "I should spend them to <color=#DE9F08>upgrade</color> the <b>fortress</b>!";

			public static string Speak1_4 = "The <color=#DE9F08>Altar of Change</color>, lets activate it!";
		}

		public class Backpack
		{
			public static string Explanation0 = "Welcome to the backpack and shop!";

			public static string Explanation1 = "This is your backpack area. Its small now, but you can make it bigger. Fill it with <sprite name=\"Bag\"> bags, and fill those with <sprite name=\"Weapon\"> weapons and <sprite name=\"Item\"> items!";

			public static string Explanation2 = "This is the shop area, here you can buy <sprite name=\"Bag\"> bags, <sprite name=\"Item\"> items and <sprite name=\"Weapon\"> weapons to increase your power.";

			public static string Explanation3 = "This is your <sprite name=\"Coin\"> coin amount, used to buy items. You can find more <sprite name=\"Coin\"> coins by killing enemies!";

			public static string Explanation4 = "If you do not find anything you like in the shop, you can reroll it by using this button. Be aware though, the cost increases per reroll!";

			public static string Explanation5 = "This is your storage. Here you can store <sprite name=\"Bag\"> bags, <sprite name=\"Item\"> items or <sprite name=\"Weapon\"> weapons that you are not currently using.";

			public static string Explanation6 = "Selling unneeded <sprite name=\"Item\"> items or <sprite name=\"Weapon\"> weapons can be done here. Simply drop them on the table and the vendor will display how much he will buy it for.";

			public static string Explanation7 = "This panel shows all your hero\"s offensive, defensive and utility statistics. It will also show relics that you will earn during your adventures!";

			public static string Explanation8 = "Once you are satisfied with your <sprite name=\"Bag\"> bags, <sprite name=\"Item\"> items and <sprite name=\"Weapon\"> weapons, you can use this button to continue your adventure!";
		}

		public class TitanicSouls
		{
			public static string Explanation0 = "Welcome to the Altar of Change - here you can unlock all kinds of upgrades for your hero and fortress!";

			public static string Explanation1 = "On the left side, you find all available upgrades!";

			public static string Explanation2 = "Once you pick an upgrade. you can find its details here.";

			public static string Explanation3 = "If you then choose to buy it, press this button to spend your Titan Souls!";
		}
	}

	public class Backpack
	{
		public const int MaxItemWidth = 10;

		public const int MaxItemHeight = 10;

		public const int MaxBackpackWidth = 12;

		public const int MaxBackpackHeight = 12;

		public const int StorageGridWith = 12;

		public const int StorageGridHeight = 6;

		public const float AlphaTresholdToPreventDraggingOnTransparency = 0.001f;

		public const float HoverScaleSize = 1.2f;

		public const float HoverScaleSpeed = 0.15f;

		public const float DropFailedReturnSpeed = 0.2f;

		public const float CellSideSize = 48f;
	}

	public class Analytics
	{
		public class CustomEvents
		{
			public const string BackpackTutorialSkippedEventName = "backpackTutorialSkipped";

			public const string TownTutorialSkippedEventName = "townTutorialSkipped";

			public const string FeedbackEventName = "feedback";

			public const string UnlockableUnlockedEventName = "unlockableUnlocked";
		}

		public class CustomParameters
		{
			public const string SkippedTutorialAfterTimeName = "skippedTutorialAfterTime";

			public const string ActiveTutorialWhenSkipped = "activeTutorialWhenSkipped";

			public const string FeedbackText = "feedbackText";

			public const string UnlockableName = "UnlockableName";

			public const string UnlockablePointsInvested = "UnlockablePointsInvested";
		}

		public class Environments
		{
			public const string Development = "development";

			public const string DemoProduction = "demo-production";

			public const string AnalyticsTest = "analytics-test";

			public const string Production = "production";
		}
	}

	public class Layers
	{
		public const string Enemies = "ENEMIES";

		public const string FlyingEnemies = "FLYING_ENEMIES";

		public const string PhasingEnemies = "PHASING_ENEMIES";

		public const string SpecialMovementEnemies = "SPECIALMOVEMENT_ENEMIES";
	}

	public class EditorHeaderNames
	{
		public const string VFX = "VFX";

		public const string Core = "Core";

		public const string Visuals = "Visuals";

		public const string Base = "Base";

		public const string Prefabs = "Prefabs";

		public const string Audio = "Audio";

		public const string Video = "Video";

		public const string Gameplay = "Gameplay";

		public const string Global = "Global";

		public const string Music = "Music";

		public const string Buffs = "Buffs";

		public const string Debuff = "Debuff";

		public const string Ambiance = "Music";

		public const string SFX = "SFX";

		public const string Overrides = "Overrides";

		public const string Conditions = "Conditions";

		public const string CustomHandler = "Custom Handler";

		public const string Default = "Default";

		public const string Statistics = "Statistics";

		public const string Weapons = "Weapons";

		public const string Experience = "Experience";

		public const string Leveling = "Leveling";

		public const string Adventure = "Adventure";

		public const string Relics = "Relics";

		public const string Stats = "Stats";

		public const string Tags = "Tags";

		public const string Title = "Title";

		public const string Weapon = "Weapon";

		public const string CompletionReward = "Completion Reward";

		public const string CompletionRewardLocked = "Completion Reward Locked";

		public const string FirstTimeCompletion = "FirstTime Completion";

		public const string LevelVisuals = "Level Visuals";

		public const string AdventureEffects = "Adventure Effects";

		public const string Hellfire = "Hellfire";

		public const string DamageNumbers = "Damage numbers";

		public const string Healthbar = "Healthbar";

		public const string Assets = "Assets";

		public const string Animator = "Animator";

		public const string Materials = "Materials";

		public const string Textures = "Textures";

		public const string Follower = "Follower";

		public const string Hovering = "Hovering";

		public const string Price = "Price";

		public const string Flavor = "Flavor";

		public const string Backdrop = "Backdrop";

		public const string Border = "Border";

		public const string Reserving = "Reserving";

		public const string Banishing = "Banishing";

		public const string Discount = "Discount";

		public const string Reroll = "Reroll";

		public const string Icon = "Icon";

		public const string Starred = "Starred";

		public const string Size = "Size";

		public const string Complex = "Complex";

		public const string ComplexDetails = "Complex/Details";

		public const string Collection = "Collection";

		public const string Special = "Special";

		public const string Movement = "Movement";

		public const string Limits = "Limits";

		public const string TargetWavingMovement = "Waving Towards Target Movement";

		public const string CircleMovement = "Circling Transform Movement";

		public const string BoomerangMovement = "Boomerang Movement";

		public const string ThrownMovement = "Thrown Movement";

		public const string Rotation = "Rotation";

		public const string StartPosition = "Start Position";

		public const string Delay = "Delay";

		public const string Elite = "Elite";

		public const string Drops = "Drops";

		public const string Death = "Death";

		public const string Lines = "Lines";

		public const string Toast = "Toast";

		public const string Player = "Player";

		public const string Scenes = "Scenes";

		public const string UI = "UI";

		public const string Progression = "Progression";

		public const string Aiming = "Aiming";

		public const string Parent = "Parent";

		public const string Spawning = "Spawning";

		public const string Exit = "Exit";

		public const string Structure = "Structure";

		public const string Center = "Center";

		public const string Canvas = "Canvas";

		public const string Searching = "Searching";

		public const string Regular = "Regular";

		public const string Keynode = "Keynode";

		public const string Tutorial = "Tutorial";

		public const string Overlay = "Overlay";

		public const string Position = "Position";

		public const string Sprites = "Sprites";

		public const string Effects = "Effects";

		public const string Formula = "Formula";

		public const string Currency = "Currency";

		public const string Character = "Character";

		public const string Unlockables = "Unlockables";

		public const string GameMenuButtonIcons = "GameMenuButton Icons";

		public const string CoinValueIcons = "Coin Value Icons";

		public const string CustomIcons = "Custom Icons";

		public const string Sprites_GameMenuButtonIcons = "Sprites/GameMenuButton Icons";

		public const string Sprites_CoinValueIcons = "Sprites/Coin Value Icons";

		public const string Sprites_CustomIcons = "Sprites/Custom Icons";

		public const string BackpackPrefabs = "Backpack Prefabs";

		public const string CombatPrefabs = "Combat Prefabs";

		public const string Content = "Content";

		public const string Colors = "Colors";

		public const string BuildInfo = "BuildInfo";

		public const string SpawnTypeCircle = "SpawnType Circle";

		public const string SpawnTypeLine = "SpawnType Line";

		public const string SpawnTypeCircleLine = "SpawnType Circle/Line";

		public const string SpawnLocation = "SpawnLocation";

		public const string Debug = "DEBUG";
	}

	public const float DefaultPlayerMoveSpeed = 6f;

	public const float MaximumCooldownReductionPercentage = 0.25f;

	public const float DefaultPickupableMoveSpeedToPlayer = 0.5f;

	public const string DefaultDateTimeStringFormat = "dddd, dd MMMM yyyy";

	public const int DefaultCanvasWidth = 1920;

	public const int DefaultCanvasHeight = 1080;

	public const float TimePerLetter = 0.05f;
}
