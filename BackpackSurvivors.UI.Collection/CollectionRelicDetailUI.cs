using BackpackSurvivors.ScriptableObjects.Relics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection;

public class CollectionRelicDetailUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private TextMeshProUGUI _gameplayDescription;

	public void ShowDetails(RelicSO relic)
	{
		_image.sprite = relic.Icon;
		_title.SetText(relic.Name ?? "");
		_description.SetText(relic.Description);
		_gameplayDescription.SetText("PLACEHOLDER - should be a detailed set of information similar to an expanded tooltip");
	}
}
