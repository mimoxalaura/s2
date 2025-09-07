using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.UI.Collection.ListItems.Item;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureRewardItem : AdventureReward
{
	public delegate void OnClickHandler(object sender, ItemCollectionSelectedEventArgs e);

	[SerializeField]
	private ItemTooltipTrigger _tooltip;

	private ItemSO _item;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(ItemSO item, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		SetImage(item.Icon);
		SetText(item.Name);
		_item = item;
		_tooltip.SetItemContent(item);
		_tooltip.ToggleEnabled(unlocked);
		LeanTween.scale(base.Image.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEaseInOutBounce().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(base.Image.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new ItemCollectionSelectedEventArgs(_item, null));
		}
	}
}
