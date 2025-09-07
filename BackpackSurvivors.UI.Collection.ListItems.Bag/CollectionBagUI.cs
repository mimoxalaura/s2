using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection.ListItems.Bag;

public class CollectionBagUI : CollectionListItemUI
{
	public delegate void OnClickHandler(object sender, BagCollectionSelectedEventArgs e);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private GameObject _lockedOverlay;

	[SerializeField]
	private BagTooltipTrigger _tooltip;

	private BagSO _Bag;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(BagSO Bag, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		_Bag = Bag;
		_image.sprite = Bag.Icon;
		_lockedOverlay.SetActive(!unlocked);
		if (_tooltip.enabled && _unlocked)
		{
			_tooltip.SetBag(Bag, _unlocked, Enums.Backpack.DraggableOwner.Player);
			_tooltip.ToggleEnabled(unlocked);
		}
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new BagCollectionSelectedEventArgs(_Bag, this));
		}
	}
}
