using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Game.Events;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.Shop;

internal class ShopUI : ModalUI
{
	internal delegate void ShopRerollClickedHandler(object sender, EventArgs e);

	[SerializeField]
	private ShopOfferSlot _shopOfferSlotPrefab;

	[SerializeField]
	private Transform _shopOfferSlotParent;

	[SerializeField]
	private SellArea _sellArea;

	[SerializeField]
	private GameObject _backdrop;

	[SerializeField]
	private GameObject _cancelButton;

	[Header("Reroll")]
	[SerializeField]
	private TextMeshProUGUI _rerollButtonText;

	[SerializeField]
	private TextMeshProUGUI _freeRerollText;

	[SerializeField]
	private GameObject _freeRerollGameObject;

	[SerializeField]
	private DefaultTooltipTrigger _freeRerollTooltipTrigger;

	[SerializeField]
	private MainMenuButton _rerollButton;

	[SerializeField]
	private Animator _rerollChestTarget;

	[SerializeField]
	private Transform _rerollChestTransform;

	[SerializeField]
	private Transform _rerollButtonHiddenPosition;

	[SerializeField]
	private Transform _rerollButtonVisiblePosition;

	[SerializeField]
	private Transform _rerollPriceContainer;

	[SerializeField]
	private Color _rerollTooHighColor;

	[SerializeField]
	private Color _rerollPossibleColor;

	[Header("Banishing")]
	[SerializeField]
	private GameObject _banishCountContainer;

	[SerializeField]
	private TextMeshProUGUI _banishCountText;

	[SerializeField]
	private DefaultTooltipTrigger _banishTooltipTrigger;

	private List<ShopOfferSlot> _shopOfferSlots = new List<ShopOfferSlot>();

	private const int _maxNumberOfShopOfferSlots = 9;

	private int _rerollCost = 1;

	private TimeBasedLevelController _timeBasedLevelController;

	internal List<ShopOfferSlot> ShopOfferSlots => _shopOfferSlots;

	internal int NumberOfActiveReservedSlots => _shopOfferSlots.Count((ShopOfferSlot x) => x.IsReserved);

	internal bool ShopOfferReservationPossible => NumberOfActiveReservedSlots < SingletonController<BackpackController>.Instance.MaximumShopReservations;

	internal bool ShopOfferBanishmentPossible => SingletonController<BackpackController>.Instance.UsedShopBanishes < SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopBanishes);

	internal bool IsInitialized { get; private set; }

	internal event ShopRerollClickedHandler OnShopRerollClicked;

	private void Start()
	{
		RegisterEvents();
		_timeBasedLevelController = UnityEngine.Object.FindObjectOfType<TimeBasedLevelController>();
		_cancelButton.SetActive(SingletonController<SceneChangeController>.Instance.CurrentSceneName.Contains("4. Town"));
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CurrencyController>.Instance, RegisterCurrencyEvents);
	}

	private void RegisterCurrencyEvents()
	{
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged += CurrencyController_OnCurrencyChanged;
	}

	private void CurrencyController_OnCurrencyChanged(object sender, CurrencyChangedEventArgs e)
	{
		if (e.CurrencyType == Enums.CurrencyType.Coins)
		{
			bool flag = _rerollCost <= e.NewAmount;
			UpdateRerollButtonTextColor(flag);
			_rerollButton.Interactable = flag;
		}
	}

	public List<ItemInstance> GetItemsFromShop()
	{
		List<ItemInstance> list = new List<ItemInstance>();
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			DraggableItem draggableItem = shopOfferSlot.DraggableForSale as DraggableItem;
			if (!(draggableItem == null) && draggableItem.Owner != Enums.Backpack.DraggableOwner.Player)
			{
				list.Add(draggableItem.ItemInstance);
			}
		}
		return list;
	}

	public List<WeaponInstance> GetWeaponsFromShop()
	{
		List<WeaponInstance> list = new List<WeaponInstance>();
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			DraggableWeapon draggableWeapon = shopOfferSlot.DraggableForSale as DraggableWeapon;
			if (!(draggableWeapon == null) && draggableWeapon.Owner != Enums.Backpack.DraggableOwner.Player)
			{
				list.Add(draggableWeapon.WeaponInstance);
			}
		}
		return list;
	}

	public List<BagInstance> GetBagsFromShop()
	{
		List<BagInstance> list = new List<BagInstance>();
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			DraggableBag draggableBag = shopOfferSlot.DraggableForSale as DraggableBag;
			if (!(draggableBag == null) && draggableBag.Owner != Enums.Backpack.DraggableOwner.Player)
			{
				list.Add(draggableBag.BagInstance);
			}
		}
		return list;
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
		SingletonController<InputController>.Instance.CanCancel = SingletonController<SceneChangeController>.Instance.CurrentSceneName.Contains("4. Town");
		SingletonController<GameController>.Instance.Player.RefreshVisuals(returnToIdle: true);
		_backdrop.SetActive(value: true);
		int usedShopBanishes = SingletonController<BackpackController>.Instance.UsedShopBanishes;
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopBanishes);
		UpdateBanishmentText(unlockedCount - usedShopBanishes);
		_banishCountContainer.SetActive(SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopBanishes) > 0);
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.CloseUI(openDirection);
		SingletonController<InputController>.Instance.CanCancel = true;
		_backdrop.SetActive(value: false);
	}

	internal Dictionary<int, BaseDraggable> GetReservedOffers()
	{
		Dictionary<int, BaseDraggable> dictionary = new Dictionary<int, BaseDraggable>();
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			if (shopOfferSlot.IsReserved)
			{
				dictionary.Add(shopOfferSlot.ShopOfferSlotId, shopOfferSlot.DraggableForSale);
			}
		}
		return dictionary;
	}

	internal void ResetReservedShopOffers()
	{
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			shopOfferSlot.SetReservation(reserved: false, playAudio: false);
		}
	}

	internal void InitShopOfferSlots(int activeShopSlots, ShopRandomizer shopRandomizer, ShopGenerator shopGenerator, float discount, bool isInitial)
	{
		for (int i = 0; i < 9; i++)
		{
			ShopOfferSlot shopOfferSlot = UnityEngine.Object.Instantiate(_shopOfferSlotPrefab, _shopOfferSlotParent);
			shopOfferSlot.Init(GameDatabaseHelper.GetDraggableBagPrefab(), GameDatabaseHelper.GetDraggableItemPrefab(), GameDatabaseHelper.GetDraggableWeaponPrefab(), shopRandomizer, shopGenerator, this, _timeBasedLevelController);
			shopOfferSlot.SetShopOfferSlotId(i);
			shopOfferSlot.RegenerateShopOffer(discount, _shopOfferSlots, isInitial);
			shopOfferSlot.gameObject.SetActive(i < activeShopSlots);
			shopOfferSlot.OnShopOfferReserveStateChanged += ShopOfferSlot_OnShopOfferReserveStateChanged;
			_shopOfferSlots.Add(shopOfferSlot);
		}
		SetReservationStatus(_shopOfferSlots);
		IsInitialized = true;
	}

	private void SetReservationStatus(List<ShopOfferSlot> shopOfferSlots)
	{
		foreach (ShopOfferSlot shopOfferSlot in shopOfferSlots)
		{
			shopOfferSlot.SetReservation(shopOfferSlot.IsReserved, playAudio: false);
		}
	}

	private void ShopOfferSlot_OnShopOfferReserveStateChanged(object sender, EventArgs e)
	{
		int num = 6;
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ExtraShopOffers);
		int numberOfActiveReservedSlots = NumberOfActiveReservedSlots;
		_rerollButton.Interactable = numberOfActiveReservedSlots < num + unlockedCount;
	}

	internal void UpdateNumberOfUnlockedShopOfferSlots(int numberOfUnlockedSlots)
	{
		int num = 0;
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			if (!(shopOfferSlot == null))
			{
				bool active = num < numberOfUnlockedSlots;
				shopOfferSlot.gameObject.SetActive(active);
				num++;
			}
		}
	}

	public void RerollShopButtonClicked()
	{
		this.OnShopRerollClicked?.Invoke(this, new EventArgs());
	}

	internal void RerollShop(int newCost, float discount)
	{
		StartCoroutine(HandleShopRerollAndAnimate(newCost, discount));
	}

	private IEnumerator HandleShopRerollAndAnimate(int newCost, float discount)
	{
		_rerollButton.OnExitHover();
		_rerollButton.Interactable = false;
		if (_shopOfferSlots.Any((ShopOfferSlot x) => x.DraggableForSale.Owner == Enums.Backpack.DraggableOwner.Shop))
		{
			_rerollChestTarget.SetBool("Closed", value: false);
		}
		foreach (ShopOfferSlot item in _shopOfferSlots.Where((ShopOfferSlot x) => x.DraggableForSale.Owner == Enums.Backpack.DraggableOwner.Shop))
		{
			item.RemoveShopOffer(new Vector2(_rerollChestTransform.gameObject.transform.position.x, _rerollChestTransform.gameObject.transform.position.y));
		}
		yield return new WaitForSecondsRealtime(0.5f);
		List<ShopOfferSlot> list = new List<ShopOfferSlot>();
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			shopOfferSlot.RegenerateShopOffer(discount, list, isInitial: false);
			list.Add(shopOfferSlot);
		}
		_rerollChestTarget.SetBool("Closed", value: true);
		_rerollButton.Interactable = newCost <= SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.Coins);
		_rerollCost = newCost;
		yield return new WaitForSecondsRealtime(0.2f);
		foreach (ShopOfferSlot shopOfferSlot2 in _shopOfferSlots)
		{
			shopOfferSlot2.DraggableForSale.Enabled = true;
		}
		SingletonController<MergeController>.Instance.UpdateMergePossibilities();
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: false);
	}

	internal void RerollShopInstantly()
	{
		foreach (ShopOfferSlot item in _shopOfferSlots.Where((ShopOfferSlot x) => x.DraggableForSale.Owner == Enums.Backpack.DraggableOwner.Shop))
		{
			item.RemoveShopOffer(new Vector2(_rerollChestTransform.gameObject.transform.position.x, _rerollChestTransform.gameObject.transform.position.y));
		}
		List<ShopOfferSlot> list = new List<ShopOfferSlot>();
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			shopOfferSlot.RegenerateShopOffer(0f, list, isInitial: false);
			list.Add(shopOfferSlot);
		}
		foreach (ShopOfferSlot shopOfferSlot2 in _shopOfferSlots)
		{
			shopOfferSlot2.DraggableForSale.Enabled = true;
		}
		SingletonController<MergeController>.Instance.UpdateMergePossibilities();
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: false);
	}

	internal void UpdateRerollButtonText(int newCost, bool initial)
	{
		string text = $"{newCost}";
		_rerollButtonText.text = text;
		LeanTween.scale(_rerollButtonText.gameObject, new Vector3(1.4f, 1.4f, 1.4f), 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_rerollButtonText.gameObject, Vector3.one, 0.2f).setDelay(0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		bool canReroll = initial || newCost <= SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.Coins);
		UpdateRerollButtonTextColor(canReroll);
	}

	private void UpdateRerollButtonTextColor(bool canReroll)
	{
		Color color = (canReroll ? _rerollPossibleColor : _rerollTooHighColor);
		_rerollButtonText.color = color;
	}

	internal void ShowOrHideRerollButtonText()
	{
		_freeRerollGameObject.SetActive(SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopRerolls) > 0);
	}

	internal void UpdateFreeRerollButtonText(int currentRerollsDone, int totalRerollsAvailable)
	{
		int num = totalRerollsAvailable - currentRerollsDone;
		string empty = string.Empty;
		empty = ((num <= 0) ? ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase) : ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase));
		string text = $"free: <color={empty}>{num}</color>/{totalRerollsAvailable}";
		_freeRerollText.text = text;
		if (num != 0)
		{
			LeanTween.scale(_freeRerollText.gameObject, new Vector3(1.1f, 1.1f, 1.1f), 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(_freeRerollText.gameObject, Vector3.one, 0.2f).setDelay(0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		}
		_freeRerollTooltipTrigger.SetDefaultContent($"Free rerolls: {num}", "Unlock more at the Altar of Change. These free rerolls are renewed at the start of an Adventure.", active: true);
	}

	internal void UpdateBanishmentText(int totalBanishmentsAvailable)
	{
		_banishCountText.text = $"{totalBanishmentsAvailable}";
		if (totalBanishmentsAvailable != 0)
		{
			LeanTween.scale(_banishCountText.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(_banishCountText.gameObject, Vector3.one, 0.2f).setDelay(0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		}
		_banishTooltipTrigger.SetDefaultContent($"Banishments: {totalBanishmentsAvailable}", "Unlock more at the Altar of Change. These are renewed at the start of an Adventure.", active: true);
	}

	public void ShowRerollPriceText()
	{
		LeanTween.scaleX(_rerollPriceContainer.gameObject, 1f, 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.moveX(_rerollPriceContainer.gameObject, _rerollButtonVisiblePosition.position.x, 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void HideRerollPriceText()
	{
		LeanTween.scaleX(_rerollPriceContainer.gameObject, 0f, 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.moveX(_rerollPriceContainer.gameObject, _rerollButtonHiddenPosition.position.x, 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	internal void SetSellPriceVisibility(BaseDraggable draggable)
	{
		if (draggable.Owner == Enums.Backpack.DraggableOwner.Player)
		{
			_sellArea.HighlightSellArea(show: true);
		}
		if (_sellArea.IsCurrentlyHovered && draggable.Owner == Enums.Backpack.DraggableOwner.Player)
		{
			ShowSellText(draggable);
		}
		else
		{
			HideSellText();
		}
	}

	internal void HideSellText()
	{
		_sellArea.HideSellText();
	}

	private void ShowSellText(BaseDraggable draggable)
	{
		_sellArea.ShowSellText(draggable.BaseItemSO.SellingPrice);
	}

	internal bool TrySellDraggable(BaseDraggable draggable)
	{
		if (!_sellArea.IsCurrentlyHovered)
		{
			return false;
		}
		SellDraggable(draggable);
		return true;
	}

	internal void ResetSellAreaCurrentlyHovered()
	{
		_sellArea.ResetCurrentlyHovered();
	}

	private void SellDraggable(BaseDraggable draggable)
	{
		int sellingPrice = draggable.BaseItemSO.SellingPrice;
		SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.Coins, sellingPrice, Enums.CurrencySource.Shop);
		HideSellText();
		_sellArea.HighlightSellArea(show: false);
		draggable.DestroyDraggable();
	}

	internal void SetGuaranteedItemToShopOfferSlot(Enums.PlaceableType itemType, ScriptableObject sellableSO)
	{
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			if (!shopOfferSlot.HasGuaranteedItemSet)
			{
				shopOfferSlot.SetGuaranteedItem(itemType, sellableSO, setIsReserved: false);
				return;
			}
		}
		Debug.LogWarning("All shop offer slots already have a guaranteed item set");
	}

	internal void SetShopReservationsEnabled()
	{
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			if (shopOfferSlot.DraggableForSale.Owner != Enums.Backpack.DraggableOwner.Player && !shopOfferSlot.IsReserved)
			{
				shopOfferSlot.SetShopReservationButtonEnabled(ShopOfferReservationPossible);
			}
		}
	}

	internal void SetShopBanishmentsEnabled()
	{
		foreach (ShopOfferSlot shopOfferSlot in _shopOfferSlots)
		{
			if (shopOfferSlot.DraggableForSale.Owner != Enums.Backpack.DraggableOwner.Player)
			{
				shopOfferSlot.SetShopBanishmentButtonEnabled(ShopOfferBanishmentPossible);
			}
		}
	}

	internal void SetSellAreaVisibility(bool visible)
	{
		_sellArea.HighlightSellArea(visible);
	}

	private void OnDestroy()
	{
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged -= CurrencyController_OnCurrencyChanged;
	}
}
