using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Collection.ListItems.Bag;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureRewardBag : AdventureReward
{
	public delegate void OnClickHandler(object sender, BagCollectionSelectedEventArgs e);

	[SerializeField]
	private BagTooltipTrigger _tooltip;

	private BagSO _bag;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(BagSO bag, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		SetImage(bag.Icon);
		SetText(bag.Name);
		_bag = bag;
		_tooltip.SetBag(bag, active: true, Enums.Backpack.DraggableOwner.Player);
		_tooltip.ToggleEnabled(unlocked);
		LeanTween.scale(base.Image.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEaseInOutBounce().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(base.Image.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new BagCollectionSelectedEventArgs(_bag, null));
		}
	}
}
