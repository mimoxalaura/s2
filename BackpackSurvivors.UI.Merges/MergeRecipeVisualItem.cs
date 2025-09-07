using System.Linq;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection.ListItems.Recipe;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Merges;

internal class MergeRecipeVisualItem : MonoBehaviour
{
	public delegate void OnClickHandler(object sender, RecipeCollectionSelectedEventArgs e);

	[SerializeField]
	private GameObject _selected;

	[SerializeField]
	private GameObject _locked;

	[SerializeField]
	private Image _input1;

	[SerializeField]
	private Image _input1Backdrop;

	[SerializeField]
	private ItemTooltipTrigger _input1TooltipItem;

	[SerializeField]
	private WeaponTooltipTrigger _input1TooltipWeapon;

	[SerializeField]
	private Image _input2;

	[SerializeField]
	private Image _input2Backdrop;

	[SerializeField]
	private ItemTooltipTrigger _input2TooltipItem;

	[SerializeField]
	private WeaponTooltipTrigger _input2TooltipWeapon;

	[SerializeField]
	private TextMeshProUGUI _input2Amount;

	[SerializeField]
	private Image _output;

	[SerializeField]
	private Image _outputBackdrop;

	[SerializeField]
	private ItemTooltipTrigger _outputTooltipItem;

	[SerializeField]
	private WeaponTooltipTrigger _outputTooltipWeapon;

	private MergableSO _mergableSO;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	internal void Init(MergableSO mergableSO, bool unlocked)
	{
		_mergableSO = mergableSO;
		_unlocked = unlocked;
		MergeableIngredient mergeableIngredient = _mergableSO.Input.FirstOrDefault((MergeableIngredient x) => x.IsPrimary);
		MergeableIngredient mergeableIngredient2 = _mergableSO.Input.FirstOrDefault((MergeableIngredient x) => !x.IsPrimary);
		_input1.sprite = mergeableIngredient.BaseItem.BackpackImage;
		_input1.material = MaterialHelper.GetItemBorderRarityMaterial(mergeableIngredient.BaseItem.ItemRarity);
		((RectTransform)_input1.transform).LeanSize(new Vector2(150f, mergeableIngredient.BaseItem.BackpackImage.pivot.y * 2f), 0f);
		Color colorForRarity = ColorHelper.GetColorForRarity(mergeableIngredient.BaseItem.ItemRarity);
		_input1Backdrop.color = new Color(colorForRarity.r, colorForRarity.g, colorForRarity.b, 0.1f);
		SetTooltip(mergeableIngredient, _input1TooltipItem, _input1TooltipWeapon);
		_input2.sprite = mergeableIngredient2.BaseItem.BackpackImage;
		_input2.material = MaterialHelper.GetItemBorderRarityMaterial(mergeableIngredient2.BaseItem.ItemRarity);
		((RectTransform)_input2.transform).LeanSize(new Vector2(150f, mergeableIngredient2.BaseItem.BackpackImage.pivot.y * 2f), 0f);
		_input2Amount.SetText(mergeableIngredient2.Amount.ToString());
		Color colorForRarity2 = ColorHelper.GetColorForRarity(mergeableIngredient2.BaseItem.ItemRarity);
		_input2Backdrop.color = new Color(colorForRarity2.r, colorForRarity2.g, colorForRarity2.b, 0.1f);
		SetTooltip(mergeableIngredient2, _input2TooltipItem, _input2TooltipWeapon);
		_output.sprite = _mergableSO.Output.BaseItem.BackpackImage;
		_output.material = MaterialHelper.GetItemBorderRarityMaterial(_mergableSO.Output.BaseItem.ItemRarity);
		((RectTransform)_output.transform).LeanSize(new Vector2(150f, mergableSO.Output.BaseItem.BackpackImage.pivot.y * 2f), 0f);
		Color colorForRarity3 = ColorHelper.GetColorForRarity(_mergableSO.Output.BaseItem.ItemRarity);
		_outputBackdrop.color = new Color(colorForRarity3.r, colorForRarity3.g, colorForRarity3.b, 0.1f);
		SetTooltip(_mergableSO.Output, _outputTooltipItem, _outputTooltipWeapon);
		if (!_unlocked)
		{
			_input1.color = Color.black;
			_input2.color = Color.black;
			_output.color = Color.black;
			_input1Backdrop.color = new Color(0f, 0f, 0f, 0f);
			_input2Backdrop.color = new Color(0f, 0f, 0f, 0f);
			_outputBackdrop.color = new Color(0f, 0f, 0f, 0f);
			_input2Amount.SetText("?");
		}
	}

	private void SetTooltip(MergeableIngredient ingredient, ItemTooltipTrigger itemTooltipTrigger, WeaponTooltipTrigger weaponTooltipTrigger)
	{
		itemTooltipTrigger.enabled = false;
		weaponTooltipTrigger.enabled = false;
	}

	private void SetTooltip(MergeableResult result, ItemTooltipTrigger itemTooltipTrigger, WeaponTooltipTrigger weaponTooltipTrigger)
	{
		itemTooltipTrigger.enabled = false;
		weaponTooltipTrigger.enabled = false;
	}

	public void ToggleSelect(bool selected)
	{
		_selected.SetActive(selected);
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new RecipeCollectionSelectedEventArgs(_mergableSO, this));
		}
	}
}
