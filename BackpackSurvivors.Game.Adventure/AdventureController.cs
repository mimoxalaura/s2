using System.Collections;
using BackpackSurvivors.Assets.Game.Revive;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Steam;
using BackpackSurvivors.UI.Adventure;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using Cinemachine;
using UnityEngine;

namespace BackpackSurvivors.Game.Adventure;

public class AdventureController : BaseSingletonModalUIController<AdventureController>
{
	public delegate void OnAdventureStartedHandler(object sender, AdventureStartedEventArgs e);

	[SerializeField]
	private AdventureUINew _adventureUI;

	[SerializeField]
	private Camera _adventureUICamera;

	[SerializeField]
	private AudioClip _adventureStartedPortalAudioClip;

	[SerializeField]
	private AudioClip _adventureOpeningPortalAudioClip;

	private ShopController _shopController;

	private AdventureSO _activeAdventure;

	private CinemachineVirtualCamera _playerCamera;

	public AdventureSO ActiveAdventure => _activeAdventure;

	public event OnAdventureStartedHandler OnAdventureStarted;

	private void Start()
	{
		_adventureUI.OnAdventureSelected += AdventureUI_OnAdventureSelected;
		_adventureUI.OnAfterCloseUI += _adventureUI_OnAfterCloseUI;
		_adventureUI.OnAfterOpenUI += _adventureUI_OnAfterOpenUI;
		base.IsInitialized = true;
	}

	private void _adventureUI_OnAfterOpenUI()
	{
		SingletonController<InputController>.Instance.CanCancel = true;
	}

	private void _adventureUI_OnAfterCloseUI()
	{
		_adventureUICamera.enabled = false;
		SetCamerasEnabled(enabled: false);
	}

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	private void AdventureUI_OnAdventureSelected(object sender, AdventureSelectedEventArgs e)
	{
		_activeAdventure = e.SelectedAdventure;
		StartAdventure();
	}

	public override void CloseUI()
	{
		_adventureUI.CloseUI();
	}

	public override void OpenUI()
	{
		base.OpenUI();
		SingletonController<InputController>.Instance.CanCancel = false;
		SingletonController<MergeController>.Instance.ClearCompleteMergableLines();
		SingletonController<MergeController>.Instance.ClearIncompleteMergableLines();
		SingletonController<MergeController>.Instance.ClearPotentialLines();
		_adventureUICamera.enabled = true;
		_adventureUI.OpenUI();
		SteamController.SetRichPresence(Enums.RichPresenceOptions.Status_StartingNewAdventure);
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Adventure;
	}

	public void StartAdventure()
	{
		_adventureUICamera.enabled = false;
		_adventureUI.CloseUI();
		SingletonController<SaveGameController>.Instance.SaveProgression();
		SingletonController<StartingEquipmentController>.Instance.Reset();
		SingletonController<ReviveController>.Instance.ClearAdventure();
		_shopController = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		_shopController.OnShopClosed += ShopController_OnShopClosed;
		SingletonController<InLevelTransitionController>.Instance.Transition(OpenShopDuringTransition);
	}

	internal void OpenShopDuringTransition()
	{
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.Shop);
		SingletonController<CharactersController>.Instance.StoreCurrentExperience();
	}

	private void ShopController_OnShopClosed(object sender, ShopClosedEventArgs e)
	{
		_shopController.OnShopClosed -= ShopController_OnShopClosed;
		if (e.ContinueToAdventure)
		{
			StartCoroutine(PlayEnterAdventureAnimation());
		}
		else
		{
			StartCoroutine(ExitAdventureStart());
		}
	}

	private IEnumerator ExitAdventureStart()
	{
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.Adventure);
		yield return new WaitForSecondsRealtime(0.5f);
		Time.timeScale = 1f;
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: true);
		SingletonController<GameController>.Instance.Player.ResetBuffs();
		SingletonController<GameController>.Instance.Player.ResetDebuffs();
		SingletonController<GameController>.Instance.Player.ResetVisuals();
		SingletonController<GameController>.Instance.Player.RefreshVisuals();
		SingletonController<GameController>.Instance.ClearControllersOfAdventureState();
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: true);
	}

	private IEnumerator PlayEnterAdventureAnimation()
	{
		SingletonController<InputController>.Instance.SwitchToIngameActionMap();
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: false);
		SingletonController<GameController>.Instance.CanOpenMenu = false;
		SingletonController<GameController>.Instance.CanPauseGame = false;
		yield return new WaitForSecondsRealtime(1f);
		SingletonController<AudioController>.Instance.PlayMusicClip(null, 1f, loop: false);
		SingletonController<ReservedShopOfferController>.Instance.SaveReservedDraggables();
		GameObject gameObject = GameObject.Find("PortalInteractionZone");
		if (gameObject != null)
		{
			gameObject.SetActive(value: false);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		SingletonController<GameController>.Instance.Player.Despawn();
		yield return new WaitForSecondsRealtime(2.9f);
		GameObject gameObject2 = GameObject.Find("AnimationContainer");
		if (gameObject2 != null)
		{
			gameObject2.GetComponent<Animator>().runtimeAnimatorController = _activeAdventure.PortalAnimationController;
			gameObject2.GetComponent<Animator>().SetBool("IsOpen", value: true);
			gameObject2.GetComponent<Animator>().SetTrigger("Open");
			SingletonController<AudioController>.Instance.PlaySFXClip(_adventureOpeningPortalAudioClip, 1f);
		}
		Object.FindObjectOfType<CinemachineSwitcher>().SwitchTo(Enums.Unlockable.AdventurePortal);
		yield return new WaitForSecondsRealtime(0.8f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_adventureStartedPortalAudioClip, 1f);
		yield return new WaitForSecondsRealtime(0.1f);
		_playerCamera = GameObject.Find("PortalCamera").GetComponent<CinemachineVirtualCamera>();
		LeanTween.value(_playerCamera.gameObject, ZoomInPlayerCamera, 8f, 0.1f, 2f);
		yield return new WaitForSecondsRealtime(1f);
		SingletonController<GameController>.Instance.StartAdventure(_activeAdventure);
		this.OnAdventureStarted?.Invoke(this, new AdventureStartedEventArgs(_activeAdventure));
	}

	private void ZoomInPlayerCamera(float val)
	{
		_playerCamera.m_Lens.OrthographicSize = val;
	}

	public override void Clear()
	{
		_adventureUI.Reset();
	}

	public override void ClearAdventure()
	{
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}

	private void OnDestroy()
	{
		_adventureUI.OnAdventureSelected -= AdventureUI_OnAdventureSelected;
	}
}
