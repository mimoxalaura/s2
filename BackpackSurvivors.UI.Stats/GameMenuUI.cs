using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Scenes;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Tooltip;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Stats;

public class GameMenuUI : ModalUI
{
	public delegate void OnMenuButtonClickedHandler(object sender, OnMenuButtonClickedHandlerEventArgs e);

	[SerializeField]
	private GameMenuButton _mainButtonPrefab;

	[SerializeField]
	private Transform _buttonsContainer;

	private Dictionary<Enums.GameMenuButtonType, MainButton> _buttonDictionary;

	private Dictionary<Enums.GameMenuButtonType, UnityAction> _buttonActionDictionary;

	public event OnMenuButtonClickedHandler OnMenuButtonClicked;

	private void Awake()
	{
		_buttonDictionary = new Dictionary<Enums.GameMenuButtonType, MainButton>();
		_buttonActionDictionary = new Dictionary<Enums.GameMenuButtonType, UnityAction>();
		_buttonActionDictionary.Add(Enums.GameMenuButtonType.Resume, ResumeClicked);
		_buttonActionDictionary.Add(Enums.GameMenuButtonType.Feedback, FeedbackClicked);
		_buttonActionDictionary.Add(Enums.GameMenuButtonType.Settings, SettingsClicked);
		_buttonActionDictionary.Add(Enums.GameMenuButtonType.BackToTown, BackToTownClicked);
		_buttonActionDictionary.Add(Enums.GameMenuButtonType.ExitGame, ExitGameClicked);
		_buttonActionDictionary.Add(Enums.GameMenuButtonType.QuitGame, QuitGameClicked);
	}

	public override void AfterOpenUI()
	{
		base.AfterOpenUI();
	}

	public override void AfterCloseUI()
	{
		base.AfterCloseUI();
	}

	public void CreateButton(Enums.GameMenuButtonType gameMenuButtonType)
	{
		if (_buttonActionDictionary.ContainsKey(gameMenuButtonType) && !_buttonDictionary.ContainsKey(gameMenuButtonType))
		{
			GameMenuButton gameMenuButton = Object.Instantiate(_mainButtonPrefab, _buttonsContainer);
			gameMenuButton.SetIconAndText(StringHelper.GetButtonText(gameMenuButtonType).ToUpper(), SpriteHelper.GetMenuIconSprite(gameMenuButtonType));
			gameMenuButton.ToggleHoverEffects(toggled: true);
			RectTransform component = gameMenuButton.transform.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, 48f);
			_buttonDictionary.Add(gameMenuButtonType, gameMenuButton);
			gameMenuButton.GetComponent<Button>().onClick.AddListener(_buttonActionDictionary[gameMenuButtonType]);
		}
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		SingletonController<StatController>.Instance.ToggleOpenWithSoloBackdrop(openWithSoloBackdrop: true);
		SingletonController<StatController>.Instance.ToggleOpenWithBuffsAndDebuffs(openWithBuffsAndDebuffs: true);
		SingletonController<StatController>.Instance.OpenUI();
		SceneInfo sceneInfo = Object.FindObjectOfType<SceneInfo>();
		if (sceneInfo != null)
		{
			bool flag = sceneInfo.SceneType != Enums.SceneType.Town;
			SingletonController<BackpackController>.Instance.PauseMenuCanvas.SetActive(flag);
			SingletonController<BackpackController>.Instance.SetCamerasEnabled(enabled: true);
			SingletonController<MergeController>.Instance.SetCamerasEnabled(flag);
			SingletonController<StarLineController>.Instance.SetCamerasEnabled(flag);
		}
		SingletonController<BackpackController>.Instance.OpenPauseMenuUI();
		SingletonController<AudioController>.Instance.SetMusicVolumeByModAndGetOriginal(0.5f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		HandleBlackPanel(showBlackPanel: true);
		SingletonController<AudioController>.Instance.PlayDefaultAudio(Enums.DefaultAudioType.UIOpen);
		SingletonController<TooltipController>.Instance.Hide(null);
		StartCoroutine(DoAfterOpenUI(0.3f));
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		SingletonController<AudioController>.Instance.ResetMusicVolume();
		SingletonController<StatController>.Instance.CloseUI();
		SingletonController<StatController>.Instance.ToggleOpenWithSoloBackdrop(openWithSoloBackdrop: false);
		SingletonController<StatController>.Instance.ToggleOpenWithBuffsAndDebuffs(openWithBuffsAndDebuffs: false);
		SingletonController<BackpackController>.Instance.ClosePauseMenuUI();
		LeanTween.scaleX(base.gameObject, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
		HandleBlackPanel(showBlackPanel: false);
		SingletonController<AudioController>.Instance.PlayDefaultAudio(Enums.DefaultAudioType.UIClose);
		SingletonController<TooltipController>.Instance.Hide(null);
		SingletonController<MergeController>.Instance.SetCamerasEnabled(enabled: false);
		SingletonController<StarLineController>.Instance.SetCamerasEnabled(enabled: false);
		StartCoroutine(DoAfterCloseUI(0.1f));
	}

	private void ResumeClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.Resume));
	}

	private void FeedbackClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.Feedback));
	}

	private void CharacterClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.Character));
	}

	private void SettingsClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.Settings));
	}

	private void BackToTownClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.BackToTown));
	}

	private void ExitGameClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.ExitGame));
	}

	private void QuitGameClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.QuitGame));
	}

	private void CollectionClicked()
	{
		this.OnMenuButtonClicked?.Invoke(this, new OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType.Collection));
	}
}
