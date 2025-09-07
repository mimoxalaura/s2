using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Game.Events;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Tooltip;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Shop;

internal class ShopOfferSlot : MonoBehaviour
{
	public delegate void OnShopOfferReserveStateChangedHandler(object sender, EventArgs e);

	[SerializeField]
	private Transform _draggableContainer;

	[SerializeField]
	private TextMeshProUGUI _cannotAffordText;

	[SerializeField]
	private AudioClip _reserveAudio;

	[SerializeField]
	private AudioClip _unreserveAudio;

	[Header("Price")]
	[SerializeField]
	private GameObject _priceTag;

	[SerializeField]
	private TextMeshProUGUI _priceTagText;

	[SerializeField]
	private Sprite _priceSprite;

	[SerializeField]
	private Sprite _priceSpriteLocked;

	[SerializeField]
	private Image _priceImage;

	[Header("Backdrop")]
	[SerializeField]
	private Image _borderSpriteRenderer;

	[SerializeField]
	private Sprite _itemBackdrop;

	[SerializeField]
	private Sprite _weaponBackdrop;

	[SerializeField]
	private Sprite _bagBackdrop;

	[SerializeField]
	private Sprite _boughtBackdrop;

	[Header("Border")]
	[SerializeField]
	private Image _rarityBorderSpriteRenderer;

	[Header("Reserving")]
	[SerializeField]
	private Material _reserveMaterial;

	[SerializeField]
	private Material _unreserveMaterial;

	[SerializeField]
	private Animator _reservationAnimator;

	[SerializeField]
	private Button _reserveButton;

	[SerializeField]
	private GameObject _reserveButtonBackdrop;

	[SerializeField]
	private Image _lockSpriteRenderer;

	[SerializeField]
	private Sprite _locked;

	[SerializeField]
	private Sprite _unlocked;

	[SerializeField]
	private Sprite _lockedItemIcon;

	[SerializeField]
	private Sprite _unlockedItemIcon;

	[SerializeField]
	private Sprite _lockedWeaponIcon;

	[SerializeField]
	private Sprite _unlockedWeaponIcon;

	[SerializeField]
	private Sprite _lockedBagIcon;

	[SerializeField]
	private Sprite _unlockedBagIcon;

	[SerializeField]
	private Sprite _lockedCoin;

	[SerializeField]
	private Sprite _unlockedCoin;

	[SerializeField]
	private Image _draggableTypeBagIcon;

	[SerializeField]
	private Image _draggableTypeWeaponIcon;

	[SerializeField]
	private Image _draggableTypeItemIcon;

	[SerializeField]
	private Image _priceTagImage;

	[SerializeField]
	private Image _CoinImage;

	[SerializeField]
	private DefaultTooltipTrigger _tooltip;

	[Header("Banishing")]
	[SerializeField]
	private Button _banishButton;

	[SerializeField]
	private AudioClip _banishAudio;

	[SerializeField]
	private Image _banishVfx;

	[SerializeField]
	private Animator _banishAnimator;

	[SerializeField]
	private TextMeshProUGUI _banishedText;

	[Header("Discount")]
	[SerializeField]
	private GameObject _discountContainer;

	[SerializeField]
	private Image _discountMaterialHolder;

	[SerializeField]
	private TextMeshProUGUI _discountText;

	[SerializeField]
	private GameObject _discountParticles;

	[SerializeField]
	private AudioClip _itemDiscountedAudioClip;

	[Header("Icon")]
	[SerializeField]
	private GameObject _weaponIcon;

	[SerializeField]
	private GameObject _itemIcon;

	[SerializeField]
	private GameObject _bagIcon;

	[Header("VFX")]
	[SerializeField]
	private ParticleSystem _spawningVfx;

	private int _shopOfferSlotId;

	private bool _isReserved;

	private bool _isBanished;

	private DraggableBag _draggableBagPrefab;

	private DraggableItem _draggableItemPrefab;

	private DraggableWeapon _draggableWeaponPrefab;

	private ShopRandomizer _shopRandomizer;

	private ShopGenerator _shopGenerator;

	private float _discount;

	private Enums.PlaceableType _guaranteedItemType;

	private ScriptableObject _guaranteedItemSO;

	private ShopUI _shopUI;

	private SerializableDictionaryBase<Enums.PlaceableRarity, float> _levelPlaceableRarities = new SerializableDictionaryBase<Enums.PlaceableRarity, float>();

	private bool _isInitial = true;

	private int _actualPrice;

	private TimeBasedLevelController _timeBasedLevelController;

	internal BaseDraggable DraggableForSale { get; private set; }

	internal bool HasGuaranteedItemSet => _guaranteedItemSO != null;

	internal bool IsReserved => _isReserved;

	internal bool IsBanished => _isBanished;

	internal int ShopOfferSlotId => _shopOfferSlotId;

	public event OnShopOfferReserveStateChangedHandler OnShopOfferReserveStateChanged;

	private void Awake()
	{
		RegisterEvents();
	}

	private void RegisterEvents()
	{
		_reserveButton.onClick.AddListener(ToggleReservation);
		_banishButton.onClick.AddListener(BanishItem);
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged += Instance_OnCurrencyChanged;
	}

	private void Instance_OnCurrencyChanged(object sender, CurrencyChangedEventArgs e)
	{
		SetPriceColor(_actualPrice);
	}

	internal void SetShopOfferSlotId(int shopOfferSlotId)
	{
		_shopOfferSlotId = shopOfferSlotId;
	}

	internal void RegenerateShopOffer(float discount, List<ShopOfferSlot> rolledShopOfferSlots, bool isInitial)
	{
		_isInitial = isInitial;
		if (_isReserved)
		{
			return;
		}
		if (DraggableForSale != null && DraggableForSale.Owner == Enums.Backpack.DraggableOwner.Shop)
		{
			RemoveCurrentDraggable();
		}
		_discount = discount;
		if (SingletonController<ReservedShopOfferController>.Instance.HasReservationForSlotId(_shopOfferSlotId))
		{
			BaseDraggable reservationForSlotId = SingletonController<ReservedShopOfferController>.Instance.GetReservationForSlotId(_shopOfferSlotId);
			Enums.PlaceableType itemType = EnumHelper.DraggableTypeToPlaceableType(reservationForSlotId.DraggableType);
			BaseItemSO baseItemSO = reservationForSlotId.BaseItemSO;
			SetGuaranteedItem(itemType, baseItemSO);
			SingletonController<ReservedShopOfferController>.Instance.RemoveReservationForShopOfferSlotId(_shopOfferSlotId);
		}
		if (_guaranteedItemSO != null)
		{
			InstantiateShopOffer(_guaranteedItemType, _guaranteedItemSO, IsDiscounted());
			ResetGuaranteedItem();
		}
		else
		{
			int num = 0;
			Enums.PlaceableType randomPlaceableType;
			BaseItemSO sellableSO;
			do
			{
				Player player = SingletonController<GameController>.Instance.Player;
				Dictionary<Enums.PlaceableRarity, float> rarityChances = GetRarityChances(player);
				float calculatedStat = player.GetCalculatedStat(Enums.ItemStatType.LuckPercentage);
				Enums.PlaceableRarity randomRarity = _shopRandomizer.GetRandomRarity(rarityChances, calculatedStat);
				float bagChance = player.BaseCharacter.BagChance;
				float weaponChance = player.BaseCharacter.WeaponChance;
				float itemChance = player.BaseCharacter.ItemChance;
				randomPlaceableType = _shopRandomizer.GetRandomPlaceableType(bagChance, weaponChance, itemChance);
				sellableSO = GetSellableSO(randomRarity, randomPlaceableType);
				num++;
			}
			while (!IsValidUniqueOffer(sellableSO, rolledShopOfferSlots) && num < 10);
			InstantiateShopOffer(randomPlaceableType, sellableSO, IsDiscounted());
		}
		_tooltip.SetContent("Unlocked", "This <u>will</u> be rerolled when rerolling the shop");
	}

	private bool IsDiscounted()
	{
		float num = 0.01f;
		float num2 = 0.01f;
		float num3 = (float)SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.DiscountChance) * num;
		float num4 = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.LuckPercentage) * num2;
		float num5 = 0f;
		return RandomHelper.GetRollSuccess(Mathf.Clamp(0.01f + num3 + num5 + num4, 0f, 1f));
	}

	private int SetDiscount(bool isDiscounted, BaseDraggable draggableGameObject)
	{
		_discountContainer.SetActive(isDiscounted);
		float num = 0.5f;
		if (isDiscounted)
		{
			_discountParticles.SetActive(value: false);
			LeanTween.scale(_discountContainer.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(_discountContainer.gameObject, Vector3.one, 0.3f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.value(_discountMaterialHolder.gameObject, delegate(float val)
			{
				_discountMaterialHolder.color = new Color(255f, 255f, 255f, val);
			}, 0f, 1f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.value(_discountMaterialHolder.gameObject, delegate(float val)
			{
				_discountMaterialHolder.color = new Color(255f, 255f, 255f, val);
			}, 1f, 0f, 0.3f).setDelay(0.1f).setIgnoreTimeScale(useUnScaledTime: true);
			if (!_isInitial)
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_itemDiscountedAudioClip, 1f);
			}
			_discountParticles.SetActive(value: true);
			int num2 = (int)((float)draggableGameObject.BuyingPrice * num);
			draggableGameObject.SetDiscountedPrice(num2);
			_discountText.SetText($"{num2}");
			return num2;
		}
		return draggableGameObject.BuyingPrice;
	}

	private void SetRarityBorder(Enums.PlaceableRarity rarity)
	{
		_rarityBorderSpriteRenderer.material = MaterialHelper.GetShopOfferBorderRarityMaterial(rarity);
	}

	private Dictionary<Enums.PlaceableRarity, float> GetRarityChances(Player player)
	{
		Dictionary<Enums.PlaceableRarity, float> dictionary = new Dictionary<Enums.PlaceableRarity, float>();
		foreach (KeyValuePair<Enums.PlaceableRarity, float> levelPlaceableRarity in _levelPlaceableRarities)
		{
			if (!dictionary.ContainsKey(levelPlaceableRarity.Key))
			{
				dictionary.Add(levelPlaceableRarity.Key, levelPlaceableRarity.Value);
			}
		}
		foreach (KeyValuePair<Enums.PlaceableRarity, float> shopOfferRarityChance in player.ShopOfferRarityChances)
		{
			if (!dictionary.ContainsKey(shopOfferRarityChance.Key))
			{
				dictionary.Add(shopOfferRarityChance.Key, 0f);
			}
			dictionary[shopOfferRarityChance.Key] += shopOfferRarityChance.Value;
		}
		return dictionary;
	}

	private bool IsValidUniqueOffer(object sellableSo, List<ShopOfferSlot> rolledShopOfferSlots)
	{
		BaseItemSO baseItemSO = sellableSo as BaseItemSO;
		if (baseItemSO == null)
		{
			return true;
		}
		foreach (ShopOfferSlot rolledShopOfferSlot in rolledShopOfferSlots)
		{
			if (rolledShopOfferSlot.DraggableForSale.BaseItemSO.ItemType == baseItemSO.ItemType && !(rolledShopOfferSlot.DraggableForSale.BaseItemSO.Name != baseItemSO.Name))
			{
				return false;
			}
		}
		return true;
	}

	private List<Enums.PlaceableRarity> GetUnlockedRarities()
	{
		List<Enums.PlaceableRarity> list = new List<Enums.PlaceableRarity>();
		if (SingletonController<GameController>.Instance.GameState != null)
		{
			list = SingletonController<GameController>.Instance.GameState.UnlockedItemRarities;
		}
		else
		{
			list.Add(Enums.PlaceableRarity.Common);
		}
		return list;
	}

	internal void SetGuaranteedItem(Enums.PlaceableType itemType, ScriptableObject sellableSO, bool setIsReserved = true)
	{
		_guaranteedItemType = itemType;
		_guaranteedItemSO = sellableSO;
		if (setIsReserved)
		{
			_isReserved = true;
		}
	}

	private void ResetGuaranteedItem()
	{
		_guaranteedItemSO = null;
	}

	private void InstantiateShopOffer(Enums.PlaceableType itemType, ScriptableObject sellableSO, bool isDiscounted = false)
	{
		BaseDraggable baseDraggable = UnityEngine.Object.Instantiate(GetPrefabByItemType(itemType));
		baseDraggable.transform.SetParent(_draggableContainer, worldPositionStays: false);
		InitDraggable(baseDraggable, sellableSO, itemType, isDiscounted);
		DraggableForSale.Enabled = false;
		LeanTween.scale(DraggableForSale.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(DraggableForSale.gameObject, new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.2f).setIgnoreTimeScale(useUnScaledTime: true);
		_reserveButton.gameObject.SetActive(_shopUI.ShopOfferReservationPossible);
		_reserveButtonBackdrop.gameObject.SetActive(_shopUI.ShopOfferReservationPossible);
		_priceTag.gameObject.SetActive(!_isInitial || _guaranteedItemSO != null);
	}

	internal void RemoveShopOffer(Vector2 target)
	{
		if (!_isReserved)
		{
			DraggableForSale.Enabled = false;
			LeanTween.move(DraggableForSale.gameObject, target, 0.4f).setEase(LeanTweenType.easeInOutQuad).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(DraggableForSale.gameObject, new Vector3(0f, 0f, 0f), 0.1f).setDelay(0.4f).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	private void InitDraggable(BaseDraggable draggableGameObject, object sellableSO, Enums.PlaceableType itemType, bool isDiscounted)
	{
		draggableGameObject.OnCannotAfford += DraggableGameObject_OnCannotAfford;
		draggableGameObject.OnBought += DraggableGameObject_OnBought;
		_isBanished = false;
		_reserveButton.gameObject.SetActive(value: true);
		_reserveButtonBackdrop.gameObject.SetActive(value: true);
		_banishButton.gameObject.SetActive(!CannotBanish());
		switch (itemType)
		{
		case Enums.PlaceableType.Bag:
			InitBag(draggableGameObject, sellableSO as BagSO, isDiscounted);
			break;
		case Enums.PlaceableType.Weapon:
			InitWeapon(draggableGameObject, sellableSO as WeaponSO, isDiscounted);
			break;
		case Enums.PlaceableType.Item:
			InitItem(draggableGameObject, sellableSO as ItemSO, isDiscounted);
			break;
		default:
			throw new Exception(string.Format("ItemType {0} is not handled in {1}.{2}()", itemType, "ShopOfferSlot", "InitDraggable"));
		}
	}

	private void DraggableGameObject_OnBought(object sender, EventArgs e)
	{
		DisableBuyingShopOffer();
	}

	private void DisableBuyingShopOffer()
	{
		DraggableForSale.OnCannotAfford -= DraggableGameObject_OnCannotAfford;
		DraggableForSale.OnBought -= DraggableGameObject_OnBought;
		_borderSpriteRenderer.material = null;
		_rarityBorderSpriteRenderer.material = null;
		_reserveButton.gameObject.SetActive(value: false);
		_reserveButtonBackdrop.gameObject.SetActive(value: false);
		_banishButton.gameObject.SetActive(value: false);
		_weaponIcon.gameObject.SetActive(value: false);
		_itemIcon.gameObject.SetActive(value: false);
		_bagIcon.gameObject.SetActive(value: false);
		_priceTag.gameObject.SetActive(value: false);
	}

	private void DraggableGameObject_OnCannotAfford(object sender, EventArgs e)
	{
		AnimateCannotAfford();
	}

	internal void AnimateCannotAfford()
	{
		LeanTween.cancel(_cannotAffordText.gameObject);
		float num = 0.3f;
		float num2 = 1f;
		float num3 = 0f;
		float num4 = 1f;
		LeanTween.value(_cannotAffordText.gameObject, UpdateCannotAffordTextAlpha, num3, num4, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_cannotAffordText.gameObject, UpdateCannotAffordTextAlpha, num4, num3, num).setDelay(num + num2).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void UpdateCannotAffordTextAlpha(float alpha)
	{
		_cannotAffordText.color = new Color(_cannotAffordText.color.r, _cannotAffordText.color.g, _cannotAffordText.color.b, alpha);
	}

	private IEnumerator RemoveSpawningVfx()
	{
		yield return new WaitForSecondsRealtime(0.8f);
		_spawningVfx.gameObject.SetActive(value: false);
	}

	private void InitBag(BaseDraggable draggableGameObject, BagSO bagSO, bool isDiscounted)
	{
		DraggableBag component = draggableGameObject.GetComponent<DraggableBag>();
		component.Init(bagSO, this);
		draggableGameObject.BaseItemInstance.SetBuyingPriceDiscount(_discount);
		_priceTagText.SetText(draggableGameObject.BaseItemInstance.BuyingPrice.ToString());
		DraggableForSale = component;
		_borderSpriteRenderer.sprite = _bagBackdrop;
		_weaponIcon.SetActive(value: false);
		_itemIcon.SetActive(value: false);
		_bagIcon.SetActive(value: true);
		_priceTag.gameObject.SetActive(value: true);
		SetRarityBorder(bagSO.ItemRarity);
		_actualPrice = SetDiscount(isDiscounted, draggableGameObject);
		SetPriceColor(_actualPrice);
		SingletonController<AudioController>.Instance.PlaySFXClip(component.BaseItemSO.SpawnAudio, 0.5f);
		ShowSpawningVfx(component.BaseItemSO.ItemRarity, component);
	}

	private void InitWeapon(BaseDraggable draggableGameObject, WeaponSO weaponSO, bool isDiscounted)
	{
		DraggableWeapon component = draggableGameObject.GetComponent<DraggableWeapon>();
		component.Init(weaponSO, this);
		draggableGameObject.BaseItemInstance.SetBuyingPriceDiscount(_discount);
		_priceTagText.SetText(draggableGameObject.BaseItemInstance.BuyingPrice.ToString());
		DraggableForSale = component;
		_borderSpriteRenderer.sprite = _weaponBackdrop;
		_weaponIcon.SetActive(value: true);
		_itemIcon.SetActive(value: false);
		_bagIcon.SetActive(value: false);
		_priceTag.gameObject.SetActive(value: true);
		SetRarityBorder(weaponSO.ItemRarity);
		_actualPrice = SetDiscount(isDiscounted, draggableGameObject);
		SetPriceColor(_actualPrice);
		SingletonController<AudioController>.Instance.PlaySFXClip(component.BaseItemSO.SpawnAudio, 0.5f);
		ShowSpawningVfx(component.BaseItemSO.ItemRarity, component);
	}

	private void InitItem(BaseDraggable draggableGameObject, ItemSO itemSO, bool isDiscounted)
	{
		DraggableItem component = draggableGameObject.GetComponent<DraggableItem>();
		component.Init(itemSO, this);
		draggableGameObject.BaseItemInstance.SetBuyingPriceDiscount(_discount);
		_priceTagText.SetText(draggableGameObject.BaseItemInstance.BuyingPrice.ToString());
		DraggableForSale = component;
		_borderSpriteRenderer.sprite = _itemBackdrop;
		_weaponIcon.SetActive(value: false);
		_itemIcon.SetActive(value: true);
		_bagIcon.SetActive(value: false);
		_priceTag.gameObject.SetActive(value: true);
		SetRarityBorder(itemSO.ItemRarity);
		_actualPrice = SetDiscount(isDiscounted, draggableGameObject);
		SetPriceColor(_actualPrice);
		SingletonController<AudioController>.Instance.PlaySFXClip(component.BaseItemSO.SpawnAudio, 0.5f);
		ShowSpawningVfx(component.BaseItemSO.ItemRarity, component);
	}

	private void SetPriceColor(int price)
	{
		if (price > SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.Coins))
		{
			_priceTagText.color = Color.red;
		}
		else
		{
			_priceTagText.color = new Color(255f, 249f, 186f);
		}
	}

	private void ShowSpawningVfx(Enums.PlaceableRarity placeableRarity, BaseDraggable baseDraggable)
	{
		if (placeableRarity == Enums.PlaceableRarity.Unique)
		{
			LeanTween.scale(baseDraggable.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.2f).setEaseInOutBounce().setLoopPingPong(1)
				.setIgnoreTimeScale(useUnScaledTime: true);
			baseDraggable.Image.material = UnityEngine.Object.Instantiate(baseDraggable.Image.material);
			Material material = baseDraggable.Image.materialForRendering;
			LeanTween.value(base.gameObject, delegate(float val)
			{
				material.SetFloat("_Brightness", val);
			}, 1f, 10f, 0.4f).setLoopPingPong(1).setIgnoreTimeScale(useUnScaledTime: true);
			_spawningVfx.gameObject.SetActive(value: true);
			StartCoroutine(RemoveSpawningVfx());
		}
	}

	private BaseItemSO GetSellableSO(Enums.PlaceableRarity rarity, Enums.PlaceableType itemType)
	{
		return itemType switch
		{
			Enums.PlaceableType.Bag => _shopGenerator.GetRandomBagByRarity(rarity), 
			Enums.PlaceableType.Weapon => _shopGenerator.GetRandomWeaponByRarity(rarity), 
			Enums.PlaceableType.Item => _shopGenerator.GetRandomItemByRarity(rarity), 
			_ => throw new Exception(string.Format("ItemType {0} is not handled in {1}.{2}()", itemType, "ShopOfferSlot", "GetSellableSO")), 
		};
	}

	private BaseDraggable GetPrefabByItemType(Enums.PlaceableType itemType)
	{
		return itemType switch
		{
			Enums.PlaceableType.Bag => _draggableBagPrefab, 
			Enums.PlaceableType.Weapon => _draggableWeaponPrefab, 
			Enums.PlaceableType.Item => _draggableItemPrefab, 
			_ => throw new Exception(string.Format("ItemType {0} is not handled in {1}.{2}()", itemType, "ShopOfferSlot", "GetPrefabByItemType")), 
		};
	}

	private void RemoveCurrentDraggable()
	{
		UnityEngine.Object.Destroy(DraggableForSale.gameObject);
	}

	private void OnEnable()
	{
		_reservationAnimator.SetBool("Reserved", _isReserved);
	}

	internal void ToggleReservation()
	{
		if (DraggableForSale.Owner != Enums.Backpack.DraggableOwner.Player && !CannotReserve())
		{
			_isReserved = !_isReserved;
			SetReservation(_isReserved);
			_shopUI.SetShopReservationsEnabled();
		}
	}

	private void RemoveReservation()
	{
		_isReserved = false;
		SetReservation(_isReserved, playAudio: false);
	}

	internal void BanishItem()
	{
		if (DraggableForSale.Owner != Enums.Backpack.DraggableOwner.Player && !CannotBanish())
		{
			if (_isReserved)
			{
				RemoveReservation();
			}
			SetBanished();
			_shopUI.SetShopBanishmentsEnabled();
		}
	}

	private void SetBanished()
	{
		_banishButton.GetComponent<FadingInOutButton>().FadeInformationElements(fadeIn: false);
		_isBanished = true;
		SingletonController<BackpackController>.Instance.UsedShopBanishes++;
		DisableBuyingShopOffer();
		BanishItemAnimation();
		SingletonController<TooltipController>.Instance.Hide(null);
		SingletonController<AudioController>.Instance.PlaySFXClip(_banishAudio, 1f, 0.3f);
		int usedShopBanishes = SingletonController<BackpackController>.Instance.UsedShopBanishes;
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopBanishes);
		_shopUI.UpdateBanishmentText(unlockedCount - usedShopBanishes);
		switch (DraggableForSale.DraggableType)
		{
		case Enums.Backpack.DraggableType.Bag:
			SingletonController<BanishedShopOfferController>.Instance.BanishBag((BagSO)DraggableForSale.BaseItemSO);
			break;
		case Enums.Backpack.DraggableType.Item:
			SingletonController<BanishedShopOfferController>.Instance.BanishItem((ItemSO)DraggableForSale.BaseItemSO);
			break;
		case Enums.Backpack.DraggableType.Weapon:
			SingletonController<BanishedShopOfferController>.Instance.BanishWeapon((WeaponSO)DraggableForSale.BaseItemSO);
			break;
		}
	}

	private void BanishItemAnimation()
	{
		_banishAnimator.SetTrigger("Banish");
		float time = 0.5f;
		LeanTween.scale(DraggableForSale.gameObject, Vector3.zero, time).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_banishedText.gameObject, UpdateBanishedTextAlpha, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_banishedText.gameObject, UpdateBanishedTextAlpha, 1f, 0f, 1f).setDelay(1f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void UpdateBanishedTextAlpha(float alpha)
	{
		_banishedText.color = new Color(_banishedText.color.r, _banishedText.color.g, _banishedText.color.b, alpha);
	}

	private bool CannotReserve()
	{
		if (!_shopUI.ShopOfferReservationPossible && !_isReserved)
		{
			return !_isBanished;
		}
		return false;
	}

	private bool CannotBanish()
	{
		int usedShopBanishes = SingletonController<BackpackController>.Instance.UsedShopBanishes;
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.ShopBanishes);
		if (!_isBanished)
		{
			return usedShopBanishes >= unlockedCount;
		}
		return true;
	}

	internal void SetReservation(bool reserved, bool playAudio = true)
	{
		_isReserved = reserved;
		if (reserved)
		{
			if (playAudio)
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_reserveAudio, 0.5f, 0.2f, 2f);
			}
			DraggableForSale.SetReservedMaterial(_reserveMaterial);
			_lockSpriteRenderer.sprite = _locked;
			_tooltip.SetContent("Locked", "This <u>will not</u> be rerolled when rerolling the shop");
			_tooltip.UpdateContentVisual();
			_priceImage.sprite = _priceSpriteLocked;
			_draggableTypeBagIcon.sprite = _lockedBagIcon;
			_draggableTypeItemIcon.sprite = _lockedItemIcon;
			_draggableTypeWeaponIcon.sprite = _lockedWeaponIcon;
			_CoinImage.sprite = _lockedCoin;
		}
		else
		{
			if (playAudio)
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_unreserveAudio, 1f);
			}
			DraggableForSale.SetUnreservedMaterial(DraggableForSale.BaseItemSO.BackpackMaterial);
			_lockSpriteRenderer.sprite = _unlocked;
			_tooltip.SetContent("Unlocked", "This <u>will</u> be rerolled when rerolling the shop");
			_tooltip.UpdateContentVisual();
			_priceImage.sprite = _priceSprite;
			_draggableTypeBagIcon.sprite = _unlockedBagIcon;
			_draggableTypeItemIcon.sprite = _unlockedItemIcon;
			_draggableTypeWeaponIcon.sprite = _unlockedWeaponIcon;
			_CoinImage.sprite = _unlockedCoin;
		}
		_reservationAnimator.SetBool("Reserved", _isReserved);
		this.OnShopOfferReserveStateChanged?.Invoke(this, new EventArgs());
	}

	internal void Init(DraggableBag draggableBagPrefab, DraggableItem draggableItemPrefab, DraggableWeapon draggableWeaponPrefab, ShopRandomizer shopRandomizer, ShopGenerator shopGenerator, ShopUI shopUI, TimeBasedLevelController timeBasedLevelController)
	{
		_draggableBagPrefab = draggableBagPrefab;
		_draggableItemPrefab = draggableItemPrefab;
		_draggableWeaponPrefab = draggableWeaponPrefab;
		_shopRandomizer = shopRandomizer;
		_shopGenerator = shopGenerator;
		_shopUI = shopUI;
		_timeBasedLevelController = UnityEngine.Object.FindObjectOfType<TimeBasedLevelController>();
		InitLevelPlaceableRarities();
	}

	private void InitLevelPlaceableRarities()
	{
		_levelPlaceableRarities = GetDefaultRarityChances();
		if (_timeBasedLevelController != null)
		{
			SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(_timeBasedLevelController, delegate
			{
				_levelPlaceableRarities = _timeBasedLevelController.CurrentLevel.ShopOfferRarityChances;
			});
			return;
		}
		ShopLevelController shopLevelController = UnityEngine.Object.FindObjectOfType<ShopLevelController>();
		if (!(shopLevelController == null))
		{
			SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(shopLevelController, delegate
			{
				_levelPlaceableRarities = shopLevelController.LevelSO.ShopOfferRarityChances;
			});
		}
	}

	private SerializableDictionaryBase<Enums.PlaceableRarity, float> GetDefaultRarityChances()
	{
		return new SerializableDictionaryBase<Enums.PlaceableRarity, float>
		{
			{
				Enums.PlaceableRarity.Common,
				1f
			},
			{
				Enums.PlaceableRarity.Uncommon,
				0.05f
			},
			{
				Enums.PlaceableRarity.Rare,
				0.001f
			},
			{
				Enums.PlaceableRarity.Epic,
				0f
			},
			{
				Enums.PlaceableRarity.Legendary,
				0f
			},
			{
				Enums.PlaceableRarity.Mythic,
				0f
			},
			{
				Enums.PlaceableRarity.Unique,
				0f
			}
		};
	}

	internal void SetShopReservationButtonEnabled(bool canUnlock)
	{
		_reserveButton.gameObject.SetActive(canUnlock && !_isBanished);
		_reserveButtonBackdrop.gameObject.SetActive(canUnlock && !_isBanished);
	}

	internal void SetShopBanishmentButtonEnabled(bool canBanish)
	{
		if (!_isBanished)
		{
			_banishButton.gameObject.SetActive(canBanish);
		}
		else
		{
			_banishButton.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		_reserveButton.onClick.RemoveListener(ToggleReservation);
		_banishButton.onClick.RemoveListener(BanishItem);
		SingletonController<CurrencyController>.Instance.OnCurrencyChanged -= Instance_OnCurrencyChanged;
	}
}
