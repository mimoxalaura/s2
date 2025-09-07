using BackpackSurvivors.Combat.ScriptableObjects;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Classes;

[CreateAssetMenu(fileName = "Character", menuName = "Game/Characters/Character", order = 1)]
public class CharacterSO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Id;

	public string _Title;

	[Header("Base")]
	[SerializeField]
	public int Id;

	[SerializeField]
	public string Name;

	[SerializeField]
	public string Description;

	[SerializeField]
	public string Flavor;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public Sprite Face;

	[SerializeField]
	public Sprite SpeakingImage;

	[SerializeField]
	public Enums.Unlockable Unlockable;

	[SerializeField]
	public Enums.CharacterClass Character;

	[SerializeField]
	public AudioClip[] OnHitAudio;

	[SerializeField]
	public AudioClip[] OnWeakHitAudio;

	[SerializeField]
	public AudioClip[] OnHitDamageAudio;

	[SerializeField]
	public AudioClip[] OnDamagePreventedAudio;

	[SerializeField]
	public AudioClip OnDeathAudio;

	[SerializeField]
	public ItemStatsSO StartingStats;

	[SerializeField]
	public float BaseMovementSpeed;

	[Tooltip("The relic(s) this character starts with")]
	[SerializeField]
	public RelicSO[] StartingRelics;

	[Tooltip("The bag(s) this character starts with - added to storage on run start")]
	[SerializeField]
	public BagSO[] StartingBags;

	[Tooltip("The weapon(s) this character starts with - added to storage on run start")]
	[SerializeField]
	public WeaponSO[] StartingWeapons;

	[Tooltip("The item(s) this character starts with - added to storage on run start")]
	[SerializeField]
	public ItemSO[] StartingItems;

	[SerializeField]
	public int StartingCoins;

	[Tooltip("Adds more chance for placeables with this tag to drop in the shop")]
	[SerializeField]
	public Enums.WeaponType WeaponTypeAffinityTag;

	[SerializeField]
	public Enums.PlaceableWeaponSubtype WeaponSubtypeAffinityTag;

	[SerializeField]
	public Enums.PlaceableItemSubtype ItemSubtypeAffinityTag;

	[SerializeField]
	public Enums.DamageType DamageTypeAffinityTag;

	[SerializeField]
	public float ChanceForPlaceableWithAffinityTag;

	[SerializeField]
	public float BagChance;

	[SerializeField]
	public float ItemChance;

	[SerializeField]
	public float WeaponChance;

	[SerializeField]
	public SerializableDictionaryBase<Enums.PlaceableRarity, float> ShopOfferRarityChances;

	public Enums.PlaceableTag CombinedPlaceableTags => GetCombinedAffinityPlaceableTag();

	private Enums.PlaceableTag GetCombinedAffinityPlaceableTag()
	{
		Enums.PlaceableTag num = EnumHelper.WeaponTypesToPlaceableTags(WeaponTypeAffinityTag);
		Enums.PlaceableTag placeableTag = EnumHelper.WeaponSubtypeToPlaceableTag(WeaponSubtypeAffinityTag);
		Enums.PlaceableTag placeableTag2 = EnumHelper.ItemSubtypeToPlaceableTag(ItemSubtypeAffinityTag);
		Enums.PlaceableTag placeableTag3 = EnumHelper.DamageTypesToPlaceableTags(DamageTypeAffinityTag);
		return num | placeableTag | placeableTag2 | placeableTag3;
	}

	private void CreateData()
	{
		if (Icon != null)
		{
			_PreviewIcon = Face.texture;
		}
		_Title = Name;
		_Id = Id.ToString();
	}
}
