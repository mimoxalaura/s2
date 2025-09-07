using BackpackSurvivors.ScriptableObjects.Classes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class EnemyDetailPage : DetailPage
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private TextMeshProUGUI _gameplayDescription;

	[SerializeField]
	private Image _itemBackground;

	[SerializeField]
	private Image[] _uiAssets;

	internal void InitDetailPage(EnemySO enemy)
	{
		_image.sprite = enemy.Icon;
		_title.SetText(enemy.Name ?? "");
		_description.SetText(enemy.Description);
		_gameplayDescription.SetText(enemy.GameplayDescription);
		Image[] uiAssets = _uiAssets;
		for (int i = 0; i < uiAssets.Length; i++)
		{
			uiAssets[i].gameObject.SetActive(value: true);
		}
	}
}
