using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BackpackSurvivors.UI.Unlockables;

public class UnlocksUI : ModalUI
{
	[SerializeField]
	private UnlocksItemUI _unlocksItemUIPrefab;

	[SerializeField]
	private Transform _unlocksContainer;

	[SerializeField]
	private TextMeshProUGUI _detailTitleText;

	[SerializeField]
	private Image _detailIcon;

	[SerializeField]
	private TextMeshProUGUI _pointsText;

	[SerializeField]
	private TextMeshProUGUI _detailExplanationText;

	[SerializeField]
	private TextMeshProUGUI _priceText;

	[SerializeField]
	private Button _addPointsButton;

	[SerializeField]
	private AudioClip _upgradedOrSpendAudio;

	[SerializeField]
	private GameObject _initialPanel;

	[SerializeField]
	private Scrollbar _scrollbar;

	[SerializeField]
	private VideoPlayer _videoPlayer;

	[SerializeField]
	private GameObject _zoomedVideoContainer;

	[SerializeField]
	private GameObject _zoomedVideoContainerBackdrop;

	[SerializeField]
	private GameObject _zoomedButton;

	[SerializeField]
	private TextMeshProUGUI _zoomedTitleText;

	[SerializeField]
	private RawImage _zoomedRawImage;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Canvas _gainedTextCanvas;

	[SerializeField]
	private TextMeshProUGUI _gainedText;

	[SerializeField]
	private Transform _gainedTextStart;

	[SerializeField]
	private Transform _gainedTextEnd;

	[SerializeField]
	private GameObject _particles;

	[SerializeField]
	private AudioClip _titanParticleDrop;

	[SerializeField]
	private ParticleSystem _titanParticleSystem;

	[SerializeField]
	private GameObject _chaliceOverlay;

	private List<UnlocksItemUI> _unlockItems;

	private UnlocksItemUI _selectedUnlockableItemUI;

	private bool _isZoomed;

	private bool _isOpen;

	private float _animationZoomDelay = 0.5f;

	private Coroutine _vfxCoroutine;

	private void Start()
	{
		_unlockItems = new List<UnlocksItemUI>();
	}

	public void Init()
	{
		RefreshListItems();
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
		_isOpen = true;
		SelectFirstUnlock();
		_particles.gameObject.SetActive(value: true);
		SingletonController<InputController>.Instance.CanCancel = false;
		SingletonController<InputController>.Instance.OnAcceptHandler += InputController_OnAcceptHandler;
	}

	private void InputController_OnSpecial1Handler(object sender, EventArgs e)
	{
		if (_selectedUnlockableItemUI.UnlockableItem.TutorialVideo != null)
		{
			ZoomVideoPlayer();
		}
	}

	private void InputController_OnAcceptHandler(object sender, EventArgs e)
	{
		BuyUpgrade();
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		if (_vfxCoroutine != null)
		{
			StopCoroutine(_vfxCoroutine);
		}
		_titanParticleSystem.gameObject.SetActive(value: false);
		_particles.gameObject.SetActive(value: false);
		_isOpen = false;
		if (_isZoomed)
		{
			UnzoomVideoPlayer();
		}
		_chaliceOverlay.SetActive(value: false);
		SingletonController<InputController>.Instance.OnAcceptHandler -= InputController_OnAcceptHandler;
		base.CloseUI(openDirection);
	}

	public override void AfterOpenUI()
	{
		base.AfterOpenUI();
		SingletonController<InputController>.Instance.CanCancel = true;
		_chaliceOverlay.SetActive(value: true);
		_scrollbar.value = 1f;
		_gainedTextCanvas.gameObject.SetActive(value: true);
	}

	private void SelectFirstUnlock()
	{
		if (_selectedUnlockableItemUI != null)
		{
			FillDetail(_selectedUnlockableItemUI);
			return;
		}
		UnlocksItemUI unlocksItemUI = _unlockItems.OrderBy((UnlocksItemUI u) => !u.UnlockableItem.CanUpgrade()).FirstOrDefault();
		if (!(unlocksItemUI == null))
		{
			FillDetail(unlocksItemUI);
		}
	}

	public override void AfterCloseUI()
	{
		_gainedTextCanvas.gameObject.SetActive(value: false);
		_chaliceOverlay.SetActive(value: false);
		base.AfterCloseUI();
	}

	private void RefreshListItems()
	{
		ClearItemList();
		foreach (UnlockableItem item in from x in (from x in SingletonController<UnlocksController>.Instance.GetUnlockableItems()
				where x.BaseUnlockable.ShouldShowInShop
				select x).ToList()
			orderby !x.CanUpgrade()
			select x)
		{
			UnlocksItemUI unlocksItemUI = UnityEngine.Object.Instantiate(_unlocksItemUIPrefab, _unlocksContainer);
			_unlockItems.Add(unlocksItemUI);
			unlocksItemUI.OnClick += UnlocksItemUI_OnClick;
			unlocksItemUI.Init(item);
		}
	}

	private void ClearItemList()
	{
		foreach (UnlocksItemUI unlockItem in _unlockItems)
		{
			unlockItem.OnClick -= UnlocksItemUI_OnClick;
		}
		_unlockItems.Clear();
		foreach (Transform item in _unlocksContainer)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		_addPointsButton.interactable = false;
	}

	private void UnlocksItemUI_OnClick(object sender, UnlockItemSelectedEventArgs e)
	{
		FillDetail((UnlocksItemUI)sender);
	}

	private void FillDetail(UnlocksItemUI unlockableItem)
	{
		if (_selectedUnlockableItemUI != null)
		{
			_selectedUnlockableItemUI.UnSelect();
		}
		_initialPanel.SetActive(value: false);
		_selectedUnlockableItemUI = unlockableItem;
		_selectedUnlockableItemUI.Select();
		_detailTitleText.SetText(unlockableItem.UnlockableItem.BaseUnlockable.Name);
		_zoomedTitleText.SetText(unlockableItem.UnlockableItem.BaseUnlockable.Name);
		_detailIcon.sprite = unlockableItem.UnlockableItem.BaseUnlockable.Icon;
		string text = $"{unlockableItem.UnlockableItem.PointsInvested}/{unlockableItem.UnlockableItem.BaseUnlockable.UnlockAvailableAmount}";
		if (unlockableItem.UnlockableItem.Completed)
		{
			text = TextMeshProStringHelper.AddColor(Constants.Colors.HexStrings.Green, text);
		}
		_pointsText.SetText(text);
		string text2 = TextMeshProStringHelper.HighlightKeywords(unlockableItem.UnlockableItem.BaseUnlockable.FullDescription, Constants.Colors.HexStrings.HighlightKeyword);
		int num = unlockableItem.UnlockableItem.PointsInvested * unlockableItem.UnlockableItem.BaseUnlockable.FullDescriptionValueForCalculation;
		text2 = text2.Replace("[CALCULATEDVALUE]", $"<color={Constants.Colors.HexStrings.HighlightKeyword}>{num}</color>");
		text2 = text2.Replace("[NEWLINE]", Environment.NewLine);
		_detailExplanationText.SetText(text2);
		if (unlockableItem.UnlockableItem.FeatureUnlocked())
		{
			_priceText.SetText($"BUY FOR {unlockableItem.UnlockableItem.GetCostForNextPoint()}");
		}
		else
		{
			_priceText.SetText("Disabled in demo");
		}
		_addPointsButton.interactable = unlockableItem.UnlockableItem.CanUpgradeOrSpend();
		_addPointsButton.gameObject.SetActive(unlockableItem.UnlockableItem.CanUpgrade());
	}

	public void ZoomVideoPlayer()
	{
		if (!_isZoomed)
		{
			_isZoomed = true;
			_zoomedVideoContainer.SetActive(value: true);
			_zoomedVideoContainerBackdrop.SetActive(value: true);
			_zoomedButton.SetActive(value: true);
			_zoomedRawImage.gameObject.SetActive(value: false);
			LeanTween.scale(_zoomedVideoContainer, Vector3.one, _animationZoomDelay).setEaseInElastic().setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(_zoomedButton, Vector3.one, _animationZoomDelay).setEaseInElastic().setIgnoreTimeScale(useUnScaledTime: true);
			StartCoroutine(PlayZoomedVideo(_animationZoomDelay));
		}
	}

	public void UnzoomVideoPlayer()
	{
		StopCoroutine(PlayZoomedVideo(_animationZoomDelay));
		_isZoomed = false;
		_zoomedVideoContainer.SetActive(value: false);
		_zoomedVideoContainerBackdrop.SetActive(value: false);
		_zoomedButton.SetActive(value: false);
		_zoomedRawImage.gameObject.SetActive(value: false);
		LeanTween.scale(_zoomedVideoContainer, Vector3.zero, 0.5f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_zoomedButton, Vector3.zero, 0.5f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private IEnumerator PlayZoomedVideo(float delay)
	{
		_videoPlayer.Stop();
		yield return new WaitForSecondsRealtime(delay);
		_zoomedRawImage.gameObject.SetActive(value: true);
		_videoPlayer.Play();
	}

	public void BuyUpgrade()
	{
		if (!SingletonController<UnlocksController>.Instance.TryUpgradeOrSpend(_selectedUnlockableItemUI.UnlockableItem))
		{
			return;
		}
		UnlocksItemUI unlocksItemUI = _unlockItems.FirstOrDefault((UnlocksItemUI x) => x.UnlockableItem == _selectedUnlockableItemUI.UnlockableItem);
		if (!_selectedUnlockableItemUI.UnlockableItem.BaseUnlockable.ShowEffect)
		{
			ResetVFXState();
			if (_vfxCoroutine != null)
			{
				StopCoroutine(_vfxCoroutine);
			}
			_vfxCoroutine = StartCoroutine(ShowBuyUpgradeVFXAsync(unlocksItemUI));
		}
		else
		{
			unlocksItemUI.Init(unlocksItemUI.UnlockableItem);
			FillDetail(_selectedUnlockableItemUI);
		}
	}

	private void ResetVFXState()
	{
		_chaliceOverlay.SetActive(value: true);
		_titanParticleSystem.gameObject.SetActive(value: false);
	}

	private IEnumerator ShowBuyUpgradeVFXAsync(UnlocksItemUI unlockItem)
	{
		if (!_isOpen)
		{
			yield return null;
		}
		SingletonController<InputController>.Instance.CanCancel = false;
		_addPointsButton.interactable = unlockItem.UnlockableItem.CanUpgradeOrSpend();
		_addPointsButton.GetComponent<MainMenuButton>().OnExitHover();
		_titanParticleSystem.gameObject.SetActive(value: true);
		SetTitanSoulBurstCount(unlockItem.UnlockableItem.BaseUnlockable.UnlockPrice);
		float timePerAudioTick = 1f / (float)unlockItem.UnlockableItem.BaseUnlockable.UnlockPrice;
		for (int i = 0; i < unlockItem.UnlockableItem.BaseUnlockable.UnlockPrice; i++)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_titanParticleDrop, 1f, 0f, AudioController.GetPitchVariation());
			if (!_isOpen)
			{
				yield return null;
			}
			yield return new WaitForSecondsRealtime(timePerAudioTick);
		}
		if (!_isOpen)
		{
			yield return null;
		}
		yield return new WaitForSecondsRealtime(1f);
		if (!_isOpen)
		{
			yield return null;
		}
		_chaliceOverlay.SetActive(value: false);
		_titanParticleSystem.gameObject.SetActive(value: false);
		SingletonController<AudioController>.Instance.PlaySFXClip(_upgradedOrSpendAudio, 1f);
		if (_selectedUnlockableItemUI.UnlockableItem.Completed)
		{
			unlockItem.ShowFullyCompletedVFX();
		}
		else
		{
			unlockItem.ShowVFX();
		}
		ShowGainedTextAnimation(unlockItem);
		if (!_isOpen)
		{
			yield return null;
		}
		yield return new WaitForSecondsRealtime(1f);
		_chaliceOverlay.SetActive(value: true);
		unlockItem.Init(unlockItem.UnlockableItem);
		FillDetail(_selectedUnlockableItemUI);
		SingletonController<InputController>.Instance.CanCancel = true;
	}

	private void SetTitanSoulBurstCount(int v)
	{
		ParticleSystem.EmissionModule emission = _titanParticleSystem.emission;
		ParticleSystem.Burst[] array = new ParticleSystem.Burst[emission.burstCount];
		emission.GetBursts(array);
		_ = _titanParticleSystem.main;
		ref ParticleSystem.Burst reference = ref array[0];
		short minCount = (array[0].maxCount = (short)v);
		reference.minCount = minCount;
		emission.SetBursts(array);
	}

	private void ShowGainedTextAnimation(UnlocksItemUI unlockItem)
	{
		_animator.SetTrigger("Picked");
		LeanTween.cancel(_gainedText.gameObject);
		LeanTween.moveLocalY(_gainedText.gameObject, _gainedTextStart.localPosition.y, 0f).setIgnoreTimeScale(useUnScaledTime: true);
		_gainedText.SetText(unlockItem.UnlockableItem.BaseUnlockable.Name);
		LeanTween.value(_gainedText.gameObject, delegate(float val)
		{
			_gainedText.color = new Color(255f, 255f, 255f, val);
		}, 0f, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.moveLocalY(_gainedText.gameObject, _gainedTextEnd.localPosition.y, 1.5f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_gainedText.gameObject, delegate(float val)
		{
			_gainedText.color = new Color(255f, 255f, 255f, val);
		}, 1f, 0f, 0.2f).setDelay(1.5f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	internal void SelectUpgrade(Enums.Unlockable unlockable)
	{
		UnlocksItemUI unlocksItemUI = _unlockItems.FirstOrDefault((UnlocksItemUI x) => x.Unlockable == unlockable);
		if (unlocksItemUI != null)
		{
			FillDetail(unlocksItemUI);
		}
	}
}
