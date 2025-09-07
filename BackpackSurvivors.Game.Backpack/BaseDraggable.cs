using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.Game.TrainingRoom;
using BackpackSurvivors.ScriptableObjects.Backpack;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shop;
using QFSW.QC.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class BaseDraggable : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerMoveHandler
{
	internal delegate void DraggableSizeCalculatedHandler(object sender, DraggableSizeCalculatedEventArgs e);

	internal delegate void DraggableCannotAffordHandler(object sender, EventArgs e);

	internal delegate void DraggableBoughtHandler(object sender, EventArgs e);

	internal delegate void DraggableOwnedChangedHandler(object sender, EventArgs e);

	internal delegate void DraggableHoveredHandler(object sender, DraggableHoveredEventArgs e);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Image _vfxImage;

	[SerializeField]
	private Material _highlightMaterial;

	[SerializeField]
	private Material _negativeHighlightMaterial;

	[SerializeField]
	private AudioClip _audioClipOnHover;

	[SerializeField]
	private AudioClip _audioClipOnRotate;

	[SerializeField]
	protected Image _mergingLockedImage;

	internal bool Enabled;

	internal EventHandler OnRotationChanged;

	internal Vector3 StartDragPosition;

	internal Vector3 ActualPosition;

	internal bool CanInteract = true;

	private protected const float CellWidth = 48f;

	private RectTransform _rectTransform;

	private CanvasGroup _canvasGroup;

	private ShopController _shopController;

	private TrainingRoomController _trainingRoomController;

	private const float AlphaTresholdToPreventDraggingOnTransparency = 0.001f;

	private ShopOfferSlot _parentSlot;

	private bool _isScaleIncreased;

	private int _scalingTweenId;

	private Transform _originalParentTransform;

	private bool _isDiscounted;

	private int _discountedPrice;

	private int _rotationTweenId;

	private int _lockRotationTweenId;

	private Material _mergeLockMaterial;

	private int _fadePropertyID;

	private bool _mergeLockAnimationState = true;

	private bool _alreadyHighlighted;

	internal Image Image => _image;

	internal Image VFXImage => _vfxImage;

	internal BaseItemSO BaseItemSO { get; private protected set; }

	internal BaseItemInstance BaseItemInstance { get; private protected set; }

	internal ItemSizeSO ItemSizeSO => BaseItemSO.ItemSize;

	internal List<int> StartItemSlotids { get; private protected set; }

	internal Enums.Backpack.ItemRotation PreviousRotation { get; private protected set; }

	internal Enums.Backpack.ItemRotation CurrentRotation { get; private protected set; }

	internal Enums.Backpack.DraggableOwner Owner { get; private protected set; }

	private protected PlaceableInfo _placeableInfo { get; private set; }

	internal PlaceableInfo PlaceableInfo => _placeableInfo;

	internal bool IsCurrentlyHovered { get; private set; }

	internal bool CurrentlyMovingBackToPosition { get; private set; }

	internal Enums.Backpack.GridType StoredInGridType { get; private protected set; }

	internal Enums.Backpack.DraggableType DraggableType { get; private set; }

	internal int BuyingPrice => GetBuyingPrice();

	internal event DraggableSizeCalculatedHandler OnSizeCalculated;

	internal event DraggableCannotAffordHandler OnCannotAfford;

	internal event DraggableBoughtHandler OnBought;

	internal event DraggableOwnedChangedHandler OnOwnerChanged;

	internal event DraggableHoveredHandler OnDraggableHovered;

	public override string ToString()
	{
		if (BaseItemSO != null)
		{
			return $"{BaseItemInstance.Guid}. {BaseItemSO.Name} - used in recipe: {BaseItemInstance.CurrentMergeRecipeSet.Recipe.Title}";
		}
		return base.ToString();
	}

	private protected virtual void Awake()
	{
		RegisterComponents();
	}

	private protected virtual void Start()
	{
	}

	private void ResetScale()
	{
		LeanTween.cancel(base.gameObject, _scalingTweenId);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		_isScaleIncreased = false;
	}

	internal virtual void Dissolve()
	{
		CanInteract = false;
	}

	internal void ResetHighlight()
	{
		_image.material = BaseItemSO.BackpackMaterial;
		_alreadyHighlighted = false;
	}

	internal void Highlight(bool effectIsPositive)
	{
		_image.material = (effectIsPositive ? _highlightMaterial : _negativeHighlightMaterial);
		_alreadyHighlighted = true;
	}

	internal void SetHighlight(bool highlight, bool effectIsPositive)
	{
		if (highlight && !_alreadyHighlighted)
		{
			Highlight(effectIsPositive);
		}
		if (!highlight)
		{
			ResetHighlight();
		}
	}

	private int GetBuyingPrice()
	{
		if (_isDiscounted)
		{
			return _discountedPrice;
		}
		return BaseItemInstance.BuyingPrice;
	}

	internal void SetDiscountedPrice(int discountedPrice)
	{
		_isDiscounted = true;
		_discountedPrice = discountedPrice;
		OverridePriceInTooltip(discountedPrice);
	}

	internal virtual void OverridePriceInTooltip(int discountedPrice)
	{
	}

	internal virtual void SetStoreInGridType(Enums.Backpack.GridType gridType, bool updateLines = true)
	{
		StoredInGridType = gridType;
		if (StoredInGridType == Enums.Backpack.GridType.Storage)
		{
			SetMergingAllow(allowed: true);
			AnimateMergeLock(allowed: true);
			if (updateLines)
			{
				SingletonController<MergeController>.Instance.DrawCompleteMergableLines();
				SingletonController<MergeController>.Instance.DrawIncompleteMergableLines();
			}
		}
	}

	internal int ToggleMergingAllowed()
	{
		if (Owner == Enums.Backpack.DraggableOwner.Shop)
		{
			return -1;
		}
		if (StoredInGridType == Enums.Backpack.GridType.Storage)
		{
			return -1;
		}
		BaseItemInstance.ToggleMergingAllowed();
		AnimateMergeLock(BaseItemInstance.MergingAllowed);
		if (!BaseItemInstance.MergingAllowed)
		{
			return 0;
		}
		return 1;
	}

	internal void SetMergingAllow(bool allowed)
	{
		BaseItemInstance.SetMergingAllowed(allowed);
	}

	private void AnimateMergeLock(bool allowed)
	{
		if (!(_mergingLockedImage == null) && allowed != _mergeLockAnimationState)
		{
			if (_mergeLockMaterial == null)
			{
				_mergeLockMaterial = UnityEngine.Object.Instantiate(_mergingLockedImage.material);
				_mergingLockedImage.material = _mergeLockMaterial;
			}
			_fadePropertyID = Shader.PropertyToID("_FullGlowDissolveFade");
			if (!allowed)
			{
				LeanTween.value(_image.gameObject, UpdateBaseDraggableColor, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
			}
			else
			{
				LeanTween.value(_image.gameObject, UpdateBaseDraggableColor, 1f, 0f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
			}
			_mergeLockAnimationState = allowed;
		}
	}

	private void UpdateBaseDraggableColor(float val)
	{
		_mergeLockMaterial.SetFloat(_fadePropertyID, val);
	}

	internal void SetRotation(Enums.Backpack.ItemRotation rotation)
	{
		CurrentRotation = rotation;
		CalculatePlaceable();
	}

	private void MoveToDuringDragTransform()
	{
		_originalParentTransform = base.transform.parent;
		base.transform.SetParent(SingletonController<BackpackController>.Instance.DuringDragParentTransform, worldPositionStays: false);
	}

	internal void MoveToOriginalParentTransform()
	{
		base.transform.SetParent(_originalParentTransform, worldPositionStays: false);
		base.transform.localScale = Vector3.one;
	}

	internal void HideVfxImage()
	{
		if (_vfxImage.isActiveAndEnabled)
		{
			_vfxImage.gameObject.SetActive(value: false);
		}
	}

	private protected void Init(BaseItemSO baseItemSO, Enums.Backpack.DraggableType draggableType, ShopOfferSlot parentSlot, bool applySize = true)
	{
		Owner = Enums.Backpack.DraggableOwner.Shop;
		BaseItemSO = baseItemSO;
		DraggableType = draggableType;
		_parentSlot = parentSlot;
		CalculatePlaceable();
		if (applySize)
		{
			ApplySize();
		}
		else
		{
			_image.preserveAspect = true;
			_vfxImage.preserveAspect = true;
		}
		_image.material = BaseItemSO.BackpackMaterial;
	}

	internal void SetReservedMaterial(Material reservedMaterial)
	{
		_image.material = reservedMaterial;
	}

	internal void SetUnreservedMaterial(Material reservedMaterial)
	{
		_image.material = reservedMaterial;
	}

	internal void SetBaseItemInstance(BaseItemInstance baseItemInstance)
	{
		BaseItemInstance = baseItemInstance;
		baseItemInstance.SetDraggable(this);
	}

	internal void ApplySize()
	{
		PlaceableInfo.CalculateWidthAndHeight(ItemSizeSO.SizeInfo.ToList(), out var itemWidth, out var itemHeight, out var _, out var _);
		_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)itemWidth * 48f);
		_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)itemHeight * 48f);
		this.OnSizeCalculated?.Invoke(this, new DraggableSizeCalculatedEventArgs(itemWidth, itemHeight));
	}

	internal void SetImage(Sprite sprite)
	{
		_image.sprite = sprite;
		_vfxImage.sprite = sprite;
		DecreaseAlphaHitTresholdOnImage();
	}

	internal void SetImageAlpha(float alpha)
	{
		_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
	}

	internal void SetPlayerOwned()
	{
		ChangeOwner(Enums.Backpack.DraggableOwner.Player);
	}

	internal void SetStartItemSlotids(List<int> startItemSlotIds)
	{
		StartItemSlotids = startItemSlotIds;
	}

	internal void Rotate(bool clockwise = true)
	{
		if (clockwise && CurrentRotation == Enums.Backpack.ItemRotation.Rotation270)
		{
			CurrentRotation = Enums.Backpack.ItemRotation.Rotation0;
		}
		else if (!clockwise && CurrentRotation == Enums.Backpack.ItemRotation.Rotation0)
		{
			CurrentRotation = Enums.Backpack.ItemRotation.Rotation270;
		}
		else
		{
			int num = (clockwise ? 1 : (-1));
			CurrentRotation += num;
		}
		ApplyRotation(animated: true);
		OnRotationChanged?.Invoke(this, EventArgs.Empty);
	}

	private protected void CalculatePlaceable()
	{
		_placeableInfo = new PlaceableInfo(GetItemSizeByCurrentRotation());
	}

	internal void CenterDraggableOnItemSlots(List<int> itemSlotIds, BackpackVisualGrid backpackVisualGrid = null)
	{
		Vector2 centerOfCells = ((backpackVisualGrid == null) ? SingletonController<BackpackController>.Instance.GetHoveredVisualGrid() : backpackVisualGrid).GetCenterOfCells(itemSlotIds);
		base.transform.position = centerOfCells;
	}

	internal void ResetScaleAndZIndex()
	{
		StartCoroutine(ResetScaleAndZIndexAsync());
	}

	private IEnumerator ResetScaleAndZIndexAsync()
	{
		yield return new WaitFrame();
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
		base.transform.localScale = Vector3.one;
	}

	private protected bool CanPlaceEntireItemInCells(List<int> itemSlotIds)
	{
		return itemSlotIds.Count == GetNumberOfItemSlots();
	}

	internal void ApplyRotation(bool animated)
	{
		float num = 0f;
		float z = 0f;
		num = CurrentRotation switch
		{
			Enums.Backpack.ItemRotation.Rotation0 => 0f, 
			Enums.Backpack.ItemRotation.Rotation90 => -90f, 
			Enums.Backpack.ItemRotation.Rotation180 => -180f, 
			Enums.Backpack.ItemRotation.Rotation270 => -270f, 
			_ => 0f, 
		};
		float time = 0.15f;
		if (animated)
		{
			CancelExistingRotations();
			LTDescr lTDescr = LeanTween.rotate(base.gameObject, new Vector3(0f, 0f, num), time).setIgnoreTimeScale(useUnScaledTime: true);
			_rotationTweenId = lTDescr.uniqueId;
			if (_mergingLockedImage != null)
			{
				LTDescr lTDescr2 = LeanTween.rotate(_mergingLockedImage.gameObject, new Vector3(0f, 0f, z), 0.15f).setIgnoreTimeScale(useUnScaledTime: true);
				_lockRotationTweenId = lTDescr2.uniqueId;
			}
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, num);
			if (_mergingLockedImage != null)
			{
				_mergingLockedImage.transform.rotation = Quaternion.Euler(0f, 0f, z);
			}
		}
		SingletonController<AudioController>.Instance.PlaySFXClip(_audioClipOnRotate, 1f);
	}

	private void CancelExistingRotations()
	{
		if (_rotationTweenId != 0)
		{
			LeanTween.cancel(_rotationTweenId);
			if (_mergingLockedImage != null && _lockRotationTweenId != 0)
			{
				LeanTween.cancel(_lockRotationTweenId);
			}
		}
	}

	internal void ResetRotation()
	{
		CurrentRotation = Enums.Backpack.ItemRotation.Rotation0;
		ApplyRotation(animated: true);
		OnRotationChanged?.Invoke(this, EventArgs.Empty);
	}

	internal void RotateBackToPreviousRotation()
	{
		CurrentRotation = PreviousRotation;
		ApplyRotation(animated: true);
		OnRotationChanged?.Invoke(this, EventArgs.Empty);
	}

	internal List<Enums.Backpack.ItemSizeCellType> GetItemSizeByCurrentRotation()
	{
		return ItemSizeSO.GetItemSizeByRotation(CurrentRotation).ToList();
	}

	internal void ChangeOwner(Enums.Backpack.DraggableOwner newOwner)
	{
		Owner = newOwner;
		this.OnOwnerChanged?.Invoke(this, new EventArgs());
	}

	internal ShopController GetShopController()
	{
		if (_shopController == null)
		{
			_shopController = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		}
		return _shopController;
	}

	internal bool TryBuyIfNeeded()
	{
		if (Owner == Enums.Backpack.DraggableOwner.Shop && !GetShopController().TryBuyDraggable(this))
		{
			return false;
		}
		this.OnBought?.Invoke(this, new EventArgs());
		_trainingRoomController = UnityEngine.Object.FindObjectOfType<TrainingRoomController>();
		if (_trainingRoomController != null)
		{
			_trainingRoomController.ReceiveDraggable(this);
		}
		return true;
	}

	private void RegisterComponents()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_rectTransform = GetComponent<RectTransform>();
		StartItemSlotids = new List<int>();
	}

	internal virtual void Drag()
	{
		Debug.LogError("Drag() should be overridden in the child class of BaseDraggable");
	}

	internal virtual bool Drop()
	{
		Debug.LogError("Drop() should be overridden in the child class of BaseDraggable");
		return false;
	}

	internal virtual void RevertDrop()
	{
		Debug.LogError("RevertDrop() should be overridden in the child class of BaseDraggable");
	}

	internal virtual bool CanDrag(bool showCannotAffordUI = false)
	{
		bool num = IsOwned();
		bool showCannotAffordUI2 = !num && showCannotAffordUI;
		bool flag = CanAfford(showCannotAffordUI2);
		if (num || flag)
		{
			return !CurrentlyMovingBackToPosition;
		}
		return false;
	}

	private bool IsOwned()
	{
		return Owner == Enums.Backpack.DraggableOwner.Player;
	}

	private bool CanAfford(bool showCannotAffordUI)
	{
		bool num = SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, BuyingPrice, showCannotAffordUI);
		if (!num && showCannotAffordUI)
		{
			AnimateCannotAfford();
			DraggableCannotAffordHandler draggableCannotAffordHandler = this.OnCannotAfford;
			if (draggableCannotAffordHandler == null)
			{
				return num;
			}
			draggableCannotAffordHandler(this, new EventArgs());
		}
		return num;
	}

	private void AnimateCannotAfford()
	{
		float num = 0.1f;
		float num2 = -10f;
		_ = _image.transform.position;
		_ = _image.transform.position;
		float num3 = 1f;
		float num4 = 1.5f;
		Color white = Color.white;
		Color red = Color.red;
		LeanTween.value(_image.gameObject, UpdateBaseDraggableColor, white, red, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_image.gameObject, UpdateBaseDraggableScale, num3, num4, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_image.gameObject, UpdateBaseDraggableColor, red, white, num).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_image.gameObject, UpdateBaseDraggableScale, num4, num3, num).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void UpdateBaseDraggableColor(Color val)
	{
		_image.color = val;
	}

	private void UpdateBaseDraggableScale(float val)
	{
		_image.transform.localScale = new Vector3(val, val, val);
	}

	internal virtual void BeginDrag()
	{
		PreviousRotation = CurrentRotation;
		base.transform.SetAsLastSibling();
		MoveToDuringDragTransform();
		_canvasGroup.blocksRaycasts = false;
		SingletonController<BackpackController>.Instance.ClearAllVisualGridHighlights();
		if (_parentSlot != null && Owner == Enums.Backpack.DraggableOwner.Shop)
		{
			_parentSlot.SetReservation(reserved: false);
		}
	}

	internal virtual void EndDrag(bool dropWasSuccess)
	{
		_canvasGroup.blocksRaycasts = true;
		if (dropWasSuccess)
		{
			ActualPosition = base.transform.position;
		}
	}

	internal virtual void MoveToStorage()
	{
	}

	internal void SetCanvasGroupRaycastBlocking(bool shouldBlock)
	{
		_canvasGroup.blocksRaycasts = shouldBlock;
	}

	internal void ReturnToStartOfDrag()
	{
		MoveToOriginalParentTransform();
		CurrentlyMovingBackToPosition = true;
		float num = 0.2f;
		base.transform.position = SingletonController<InputController>.Instance.CursorPosition;
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0f);
		ActualPosition = StartDragPosition;
		if (Owner == Enums.Backpack.DraggableOwner.Shop)
		{
			base.transform.position = StartDragPosition;
		}
		else
		{
			LeanTween.move(base.gameObject, StartDragPosition, num).setIgnoreTimeScale(useUnScaledTime: true);
		}
		StartCoroutine(ResetCurrentlyMovingBackToPosition(num));
		CurrentlyMovingBackToPosition = false;
	}

	private IEnumerator ResetCurrentlyMovingBackToPosition(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		CurrentlyMovingBackToPosition = false;
	}

	internal int GetNumberOfItemSlots()
	{
		return ItemSizeSO.SizeInfo.Count((Enums.Backpack.ItemSizeCellType s) => s == Enums.Backpack.ItemSizeCellType.CellContainsPlacable);
	}

	private void DecreaseAlphaHitTresholdOnImage()
	{
		_image.alphaHitTestMinimumThreshold = 0.001f;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		PointerEnter();
	}

	internal virtual void PointerEnter()
	{
		if (CanInteract)
		{
			IsCurrentlyHovered = true;
			this.OnDraggableHovered?.Invoke(this, new DraggableHoveredEventArgs(isHovered: true));
			SingletonController<BackpackController>.Instance.UpdateCurrentlyHoveredDraggable(this);
			if (CanDrag())
			{
				LTDescr lTDescr = LeanTween.scale(_image.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.15f).setIgnoreTimeScale(useUnScaledTime: true);
				_scalingTweenId = lTDescr.id;
				_isScaleIncreased = true;
				SingletonController<AudioController>.Instance.PlaySFXClip(_audioClipOnHover, 0.5f);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		PointerExit();
	}

	internal virtual void PointerExit()
	{
		if (CanInteract)
		{
			IsCurrentlyHovered = false;
			this.OnDraggableHovered?.Invoke(this, new DraggableHoveredEventArgs(isHovered: false));
			SingletonController<BackpackController>.Instance.UpdateCurrentlyHoveredDraggable(null);
			if (_isScaleIncreased)
			{
				ResetScale();
			}
		}
	}

	internal void AnimateForEffect()
	{
		LeanTween.scale(_image.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.15f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_image.gameObject, new Vector3(1f, 1f, 1f), 0.15f).setDelay(0.15f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		PointerMove();
	}

	internal virtual void PointerMove()
	{
	}

	internal void DestroyDraggable(float delay = 0f)
	{
		StopAllCoroutines();
		base.gameObject.SetActive(value: false);
		DestroyDraggableAsync(delay);
	}

	private IEnumerator DestroyDraggableAsync(float delay = 0f)
	{
		yield return new WaitForSecondsRealtime(delay);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
