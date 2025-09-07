using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection.ListItems.Item;

public class CollectionItemUI : CollectionListItemUI
{
	public delegate void OnClickHandler(object sender, ItemCollectionSelectedEventArgs e);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Image _backdropImage;

	[SerializeField]
	private GameObject _lockedOverlay;

	[SerializeField]
	private ItemTooltipTrigger _tooltip;

	private ItemSO _item;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(ItemSO item, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		_image.sprite = item.BackpackImage;
		_backdropImage.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(item.ItemRarity);
		_item = item;
		_lockedOverlay.SetActive(!unlocked);
		if (_tooltip.enabled && _unlocked)
		{
			_tooltip.SetItemContent(item);
			_tooltip.ToggleEnabled(unlocked);
		}
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new ItemCollectionSelectedEventArgs(_item, this));
		}
	}
}
