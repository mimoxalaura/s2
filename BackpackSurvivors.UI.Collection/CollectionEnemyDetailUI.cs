using BackpackSurvivors.ScriptableObjects.Classes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection;

public class CollectionEnemyDetailUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	public void ShowDetails(EnemySO enemy)
	{
		_image.sprite = enemy.Icon;
		_image.preserveAspect = true;
		_title.SetText(enemy.Name);
		_description.SetText(enemy.Description);
	}
}
