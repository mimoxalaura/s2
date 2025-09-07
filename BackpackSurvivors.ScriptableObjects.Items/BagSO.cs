using System;
using BackpackSurvivors.Combat.ScriptableObjects;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[CreateAssetMenu(fileName = "Bag", menuName = "Game/Items/Bag", order = 1)]
public class BagSO : BaseItemSO
{
	[SerializeField]
	public ItemStatsSO Stats;

	[SerializeField]
	public Sprite BackpackImageWithoutSlots;

	public override bool Equals(object other)
	{
		if (other is BagSO bagSO)
		{
			return Name.Equals(bagSO.Name);
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
		hashCode.Add(Stats);
		hashCode.Add(BackpackImageWithoutSlots);
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
	}
}
