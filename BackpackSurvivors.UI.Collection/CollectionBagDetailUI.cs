using BackpackSurvivors.ScriptableObjects.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection;

public class CollectionBagDetailUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	public void ShowDetails(BagSO bag)
	{
		_image.sprite = bag.Icon;
		_title.SetText(bag.Name);
		_description.SetText(bag.Description);
	}
}
