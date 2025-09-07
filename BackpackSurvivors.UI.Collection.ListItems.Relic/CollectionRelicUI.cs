using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection.ListItems.Relic;

public class CollectionRelicUI : CollectionListItemUI
{
	public delegate void OnClickHandler(object sender, RelicCollectionSelectedEventArgs e);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private GameObject _lockedOverlay;

	[SerializeField]
	private Image _backdropImage;

	[SerializeField]
	private RelicTooltipTrigger _tooltip;

	private RelicSO _relic;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(RelicSO relic, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		_relic = relic;
		_backdropImage.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(relic.Rarity);
		_image.sprite = relic.Icon;
		_lockedOverlay.SetActive(!unlocked);
		if (_tooltip.enabled)
		{
			_tooltip.SetRelic(relic, _unlocked);
			_tooltip.ToggleEnabled(unlocked);
		}
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new RelicCollectionSelectedEventArgs(_relic, this));
		}
	}
}
