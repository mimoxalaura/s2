using System;
using BackpackSurvivors.Combat.ScriptableObjects;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[CreateAssetMenu(fileName = "Weapon", menuName = "Game/Items/Weapon", order = 3)]
public class WeaponSO : BaseItemSO
{
	public string _WeaponType;

	[SerializeField]
	public Enums.WeaponType WeaponType;

	[SerializeField]
	public Enums.WeaponAttackTypeDurationAndPiercing WeaponAttackTypeDurationAndPiercing;

	[SerializeField]
	public Enums.AttackTargetingType AttackType;

	[SerializeField]
	public Enums.PlaceableWeaponSubtype WeaponSubtype;

	[SerializeField]
	public WeaponAttack WeaponAttackPrefab;

	[SerializeField]
	public bool ShouldTriggerAudioOnDamage = true;

	[SerializeField]
	public bool IsPlayerWeapon;

	[SerializeField]
	public float DelayBeforePlayerAttackAnimation;

	[SerializeField]
	public GameObject[] ParticleEffectsPrefabs;

	[SerializeField]
	public GameObject[] LineEffectsPrefabs;

	[SerializeField]
	public Enums.WeaponAnimationSize WeaponAnimationSize;

	[SerializeField]
	public Enums.WeaponAnimationType WeaponAnimationType;

	[SerializeField]
	public bool DualWield;

	[SerializeField]
	public Sprite IngameOffhandImage;

	[SerializeField]
	public IngameWeaponObject IngameOffhandGameObject;

	[SerializeField]
	public WeaponStatsSO Stats;

	[SerializeField]
	public ItemStatsSO StarStats;

	[SerializeField]
	public DamageSO Damage;

	[SerializeField]
	public Enums.ProjectileChaining ProjectileChaining;

	[SerializeField]
	public Enums.ProjectileSpreading ProjectileSpreading;

	[SerializeField]
	public float SpreadOffsetDegrees = 10f;

	[SerializeField]
	public float FlightAffectTickTime = 0.5f;

	[SerializeField]
	public Enums.CardinalDirection[] CardinalDirections;

	[SerializeField]
	public Enums.CharacterType CharacterTypeForTriggerOnTouch = Enums.CharacterType.Enemy;

	[SerializeField]
	public bool AllowMultipleHitsOnSameCharacter;

	[SerializeField]
	public float KnockbackPower;

	[SerializeField]
	public bool IsPermanentEffect;

	[SerializeField]
	public WeaponAttackEffect[] WeaponAttackEffects;

	[SerializeField]
	public DebuffSO[] DebuffAttackEffects;

	[SerializeField]
	public Enums.TargetingTooltipType TargetingTooltipType;

	[SerializeField]
	public string CustomTargetingTooltipString;

	[SerializeField]
	public bool LogDamage = true;

	internal bool HasStarStats()
	{
		return StarStats != null;
	}

	public override bool Equals(object other)
	{
		if (other is WeaponSO weaponSO)
		{
			return Name.Equals(weaponSO.Name);
		}
		return base.Equals(other);
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		hashCode.Add(base.GetHashCode());
		hashCode.Add(Id);
		hashCode.Add(Name);
		hashCode.Add(Description);
		hashCode.Add(BuyingPrice);
		hashCode.Add(SellingPrice);
		hashCode.Add(ItemRarity);
		hashCode.Add(ItemType);
		hashCode.Add(ItemSize);
		hashCode.Add(StarringEffectIsPositive);
		hashCode.Add(AvailableInShop);
		hashCode.Add(Icon);
		hashCode.Add(IngameImage);
		hashCode.Add(BackpackImage);
		hashCode.Add(BeginDragAudio);
		hashCode.Add(EndDropSuccesAudio);
		hashCode.Add(EndDropFailedAudio);
		hashCode.Add(AttackType);
		hashCode.Add(WeaponSubtype);
		hashCode.Add(ParticleEffectsPrefabs);
		hashCode.Add(LineEffectsPrefabs);
		hashCode.Add(WeaponAnimationSize);
		hashCode.Add(WeaponAnimationType);
		hashCode.Add(DualWield);
		hashCode.Add(WeaponAttackPrefab);
		hashCode.Add(WeaponType);
		hashCode.Add(WeaponAttackTypeDurationAndPiercing);
		hashCode.Add(IngameOffhandImage);
		hashCode.Add(Stats);
		hashCode.Add(StarStats);
		hashCode.Add(WeaponAttackEffects);
		hashCode.Add(DebuffAttackEffects);
		hashCode.Add(Damage);
		hashCode.Add(ShouldTriggerAudioOnDamage);
		hashCode.Add(ProjectileChaining);
		hashCode.Add(ProjectileSpreading);
		hashCode.Add(FlightAffectTickTime);
		hashCode.Add(CharacterTypeForTriggerOnTouch);
		hashCode.Add(AllowMultipleHitsOnSameCharacter);
		hashCode.Add(KnockbackPower);
		hashCode.Add(CardinalDirections);
		hashCode.Add(TargetingTooltipType);
		hashCode.Add(CustomTargetingTooltipString);
		hashCode.Add(LogDamage);
		return hashCode.ToHashCode();
	}

	private void CreateData()
	{
		if (Icon != null)
		{
			_PreviewIcon = Icon.texture;
		}
		_Id = Id.ToString();
		_Title = Name;
		_Rarity = ItemRarity.ToString();
		_WeaponType = WeaponType.ToString();
	}
}
