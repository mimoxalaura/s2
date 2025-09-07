using BackpackSurvivors.ScriptableObjects.Unlockable;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.MainMenu;

public class SaveSlotUnlockedUIItem : MonoBehaviour
{
	[SerializeField]
	private Image _unlockableImage;

	[SerializeField]
	private TextMeshProUGUI _unlockableCount;

	[SerializeField]
	private DefaultTooltipTrigger _tooltipTrigger;

	public void Init(UnlockableSO unlockableSO, int count)
	{
		_unlockableImage.sprite = unlockableSO.Icon;
		string empty = string.Empty;
		if (unlockableSO.UnlockAvailableAmount > 1)
		{
			empty = $"{unlockableSO.Name} ({count}/{unlockableSO.UnlockAvailableAmount})";
			_unlockableCount.SetText(count.ToString());
		}
		else
		{
			empty = unlockableSO.Name ?? "";
			_unlockableCount.SetText("");
		}
		_tooltipTrigger.SetDefaultContent(empty, unlockableSO.Description, active: true);
	}
}
