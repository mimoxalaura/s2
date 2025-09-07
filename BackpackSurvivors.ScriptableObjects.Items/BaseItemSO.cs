using BackpackSurvivors.ScriptableObjects.Backpack;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[CreateAssetMenu(fileName = "Base", menuName = "Game/Items/BASE", order = 2)]
public class BaseItemSO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Id;

	public string _Title;

	public string _Rarity;

	[SerializeField]
	public int Id;

	[SerializeField]
	public string Name;

	[SerializeField]
	public string Description;

	[SerializeField]
	public bool AvailableInShop;

	[SerializeField]
	public int BuyingPrice;

	[SerializeField]
	public int SellingPrice;

	[SerializeField]
	public Enums.PlaceableRarity ItemRarity;

	[SerializeField]
	public Enums.PlaceableType ItemType;

	[SerializeField]
	public ItemSizeSO ItemSize;

	[SerializeField]
	public bool StarringEffectIsPositive = true;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public Sprite IngameImage;

	[SerializeField]
	public Material IngameImageMaterial;

	[SerializeField]
	public IngameWeaponObject IngameObject;

	[SerializeField]
	public Sprite BackpackImage;

	[SerializeField]
	public Sprite[] BackpackAnimatedImages;

	[SerializeField]
	public Material BackpackMaterial;

	public AudioClip BeginDragAudio;

	public AudioClip EndDropSuccesAudio;

	public float EndDropSuccesAudioVolume = 1f;

	public float EndDropSuccesAudioOffset;

	public AudioClip EndDropFailedAudio;

	public float EndDropFailedAudioVolume = 1f;

	public float EndDropFailedAudioOffset;

	public AudioClip SpawnAudio;

	public bool IsTestItem;

	private void Open()
	{
	}

	private Color GetRarityColor()
	{
		return EditorHelper.GetRarityColor(ItemRarity);
	}
}
