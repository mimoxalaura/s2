using BackpackSurvivors.Assets.UI.Tooltips;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.UI.Tooltip.Complex;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class CleanBagTooltip : BaseTooltip
{
	[Header("Base")]
	[SerializeField]
	private float _baseSpacingHeight = 16f;

	[SerializeField]
	private Vector2 _baseBagSize;

	[SerializeField]
	private bool _showCollectedFlag;

	[Header("Title")]
	[SerializeField]
	private TextMeshProUGUI _titleText;

	[SerializeField]
	private Transform _titleContainer;

	[Header("Size")]
	[SerializeField]
	private TooltipSizeContainer _tooltipSizeContainer;

	[SerializeField]
	private Transform _tooltipSizeRootContainer;

	[Header("Price")]
	[SerializeField]
	private TextMeshProUGUI _priceText;

	[Header("Flavor")]
	[SerializeField]
	private TextMeshProUGUI _flavorText;

	[Header("Border")]
	[SerializeField]
	private Image[] _imagesToSetRarityMaterialOn;

	[SerializeField]
	private Image[] _imagesToSetFadeOn;

	[SerializeField]
	private Image _mainBorder;

	[Header("Complex")]
	[SerializeField]
	private bool _allowShowComplexTooltips = true;

	[SerializeField]
	private GameObject _altHotkeyContainer;

	[SerializeField]
	private GameObject _altHotkeyKeyboard;

	[SerializeField]
	private GameObject _altHotkeyController;

	[SerializeField]
	private TextMeshProUGUI _altHotkeyText;

	[SerializeField]
	private Transform _tipContainer;

	[SerializeField]
	private ItemTooltipComplexTip _itemTooltipComplexTipPrefab;

	[SerializeField]
	private ItemTooltipComplexFormulaTip _itemTooltipComplexFormulaTipPrefab;

	[SerializeField]
	private GameObject _collectedIcon;

	private bool _hasComplexTooltipToShow;

	private bool _shouldShowComplex;

	public void SetBag(BagInstance bag, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		SetTitle(bag.BaseItemSO.Name, bag.BaseItemSO.ItemRarity);
		SetSizeImage(bag.BagSO);
		SetRarity(bag.BagSO);
		SetPrice(bag, owner, overriddenPrice);
		SetupComplexTooltips(bag.BagSO);
		SetupNewCollected(bag.BagSO);
		SetupDebug(bag.BagSO);
		SetupFlavor(bag.BagSO);
		SetTooltipSize(bag.BagSO);
	}

	public void SetBag(BagSO bag, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		SetTitle(bag.Name, bag.ItemRarity);
		SetSizeImage(bag);
		SetRarity(bag);
		SetPrice(bag, owner, overriddenPrice);
		SetupComplexTooltips(bag);
		SetupNewCollected(bag);
		SetupDebug(bag);
		SetupFlavor(bag);
		SetTooltipSize(bag);
	}

	private Vector2 SetTooltipSize(BagSO item)
	{
		float num = 0f;
		_ = 0f + ((RectTransform)_titleContainer).sizeDelta.y;
		float num2 = CalculateContentHeight();
		float num3 = CalculateContentWidth();
		num += num3;
		Vector2 baseBagSize = _baseBagSize;
		((RectTransform)base.transform).LeanSize(baseBagSize, 0f).setIgnoreTimeScale(useUnScaledTime: true);
		return baseBagSize;
	}

	private float CalculateContentWidth()
	{
		return GetSizeContainerSizeVector().y + _baseSpacingHeight + _baseSpacingHeight + 10f + _baseSpacingHeight;
	}

	private Vector2 GetSizeContainerSizeVector()
	{
		return ((RectTransform)_tooltipSizeRootContainer.transform).sizeDelta;
	}

	private float CalculateContentHeight()
	{
		float y = GetSizeContainerSizeVector().y;
		float num = 0f;
		num += _baseSpacingHeight * 4f;
		return Mathf.Max(y + 16f, num);
	}

	private void SetupNewCollected(BagSO item)
	{
		if (_showCollectedFlag)
		{
			_collectedIcon.SetActive(!SingletonController<CollectionController>.Instance.IsItemUnlocked(item.Id));
		}
		else
		{
			_collectedIcon.SetActive(value: false);
		}
	}

	internal void SetTitle(string name, Enums.PlaceableRarity rarity)
	{
		string sourceText = StringHelper.BagSprite + " <color=" + ColorHelper.GetColorHexcodeForRarity(rarity) + ">" + name + "</color>";
		_titleText.SetText(sourceText);
	}

	private void SetRarity(BagSO BagSO)
	{
		_mainBorder.material = MaterialHelper.GetTooltipBorderRarityMaterial(BagSO.ItemRarity);
		Image[] imagesToSetRarityMaterialOn = _imagesToSetRarityMaterialOn;
		for (int i = 0; i < imagesToSetRarityMaterialOn.Length; i++)
		{
			imagesToSetRarityMaterialOn[i].material = MaterialHelper.GetInternalTooltipBorderRarityMaterial(BagSO.ItemRarity);
		}
		imagesToSetRarityMaterialOn = _imagesToSetFadeOn;
		foreach (Image imageToSetRarityMaterialOn in imagesToSetRarityMaterialOn)
		{
			SetupColoringOnBackdrop(BagSO, imageToSetRarityMaterialOn);
		}
	}

	private static void SetupColoringOnBackdrop(BagSO BagSO, Image imageToSetRarityMaterialOn)
	{
		if (BagSO.ItemRarity == Enums.PlaceableRarity.Common)
		{
			imageToSetRarityMaterialOn.color = new Color(255f, 255f, 255f, 0.8f);
		}
		else
		{
			imageToSetRarityMaterialOn.color = new Color(255f, 255f, 255f, 0.5f);
		}
	}

	private void SetPrice(BagSO BagSO, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		string arg = "<size=16>" + StringHelper.CoinSprite + "</size>";
		if (owner == Enums.Backpack.DraggableOwner.Shop)
		{
			if (overriddenPrice > -1)
			{
				_priceText.SetText($"{overriddenPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, overriddenPrice))
				{
					_priceText.color = Color.green;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
			else
			{
				_priceText.SetText($"{BagSO.BuyingPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, BagSO.BuyingPrice))
				{
					_priceText.color = Color.white;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
		}
		else
		{
			_priceText.SetText($"{BagSO.SellingPrice}  {arg}");
			_priceText.color = Color.white;
		}
	}

	private void SetPrice(BagInstance BagInstance, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		string arg = "<size=16>" + StringHelper.CoinSprite + "</size>";
		if (owner == Enums.Backpack.DraggableOwner.Shop)
		{
			if (overriddenPrice > -1)
			{
				_priceText.SetText($"{overriddenPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, overriddenPrice))
				{
					_priceText.color = Color.green;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
			else
			{
				_priceText.SetText($"{BagInstance.BuyingPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, BagInstance.BuyingPrice))
				{
					_priceText.color = Color.white;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
		}
		else
		{
			_priceText.SetText($"{BagInstance.SellingPrice}  {arg}");
			_priceText.color = Color.white;
		}
	}

	private void SetSizeImage(BagSO BagSO)
	{
		_tooltipSizeContainer.SetSize(BagSO.ItemSize, BagSO.StarringEffectIsPositive);
	}

	private void SetupComplexTooltips(BagSO bag)
	{
		_shouldShowComplex = SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.AlwaysVisible || (SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.VisibleOnAlt && SingletonController<InputController>.Instance.AltIsDown);
		_hasComplexTooltipToShow = false;
		ToggleAltTooltip(_shouldShowComplex);
		SetHotkey();
	}

	internal void SetHotkey()
	{
		if (SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.VisibleOnAlt)
		{
			bool currentControlschemeIsKeyboard = SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard;
			_altHotkeyContainer.SetActive(_hasComplexTooltipToShow);
			_altHotkeyKeyboard.SetActive(_hasComplexTooltipToShow && currentControlschemeIsKeyboard);
			_altHotkeyController.SetActive(_hasComplexTooltipToShow && !currentControlschemeIsKeyboard);
		}
		else
		{
			_altHotkeyContainer.SetActive(value: false);
		}
	}

	private void SetupFlavor(BagSO bagSO)
	{
		_flavorText.SetText(bagSO.Description);
	}

	private void SetupDebug(BagSO bag)
	{
		if (GameDatabaseHelper.AllowDebugging)
		{
			SetTitle($"{bag.Name} ({bag.Id})", bag.ItemRarity);
		}
	}
}
