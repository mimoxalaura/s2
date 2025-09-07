using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.UI.Collection.ListItems.Relic;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureRewardRelic : AdventureReward
{
	public delegate void OnClickHandler(object sender, RelicCollectionSelectedEventArgs e);

	[SerializeField]
	private RelicTooltipTrigger _tooltip;

	private RelicSO _relic;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(RelicSO relic, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		SetImage(relic.Icon);
		SetText(relic.Name);
		_relic = relic;
		_tooltip.SetRelic(relic, unlocked);
		_tooltip.ToggleEnabled(unlocked);
		LeanTween.scale(base.Image.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.3f).setEaseInOutBounce().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(base.Image.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new RelicCollectionSelectedEventArgs(_relic, null));
		}
	}
}
