using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection;

public class CollectionWeaponDetailUI : MonoBehaviour
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

	public void ShowDetails(WeaponSO weapon)
	{
		_image.sprite = weapon.BackpackImage;
		_title.SetText("<color=#" + ColorHelper.GetColorHexcodeForRarity(weapon.ItemRarity) + ">" + weapon.Name + "</color>");
		_description.SetText(weapon.Description);
		_gameplayDescription.SetText("PLACEHOLDER - should be a detailed set of information similar to an expanded tooltip");
		_itemBackground.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(weapon.ItemRarity);
	}
}
