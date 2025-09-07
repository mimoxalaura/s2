using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureEffectDetail : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private DefaultTooltipTrigger _defaultTooltipTrigger;

	public void Init(Sprite image, string name, string description)
	{
		_image.sprite = image;
		_text.SetText(name);
		_defaultTooltipTrigger.SetDefaultContent(name, description, active: true);
	}
}
