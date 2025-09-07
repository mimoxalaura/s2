using BackpackSurvivors;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.UI;

public class AdventureStartButton : MainButton
{
	[SerializeField]
	private Image _keyboardImage;

	[SerializeField]
	private Image _controllerImage;

	[SerializeField]
	private GameObject _selectedContainer;

	private new void Start()
	{
		SetupButtonIcons();
		SingletonController<InputController>.Instance.OnControlSchemeChanged += Instance_OnControlSchemeChanged;
	}

	private void SetupButtonIcons()
	{
		_keyboardImage.gameObject.SetActive(SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard);
		_controllerImage.gameObject.SetActive(!SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard);
	}

	private void Instance_OnControlSchemeChanged(object sender, ControlSchemeChangedEventArgs e)
	{
		SetupButtonIcons();
	}

	public override void OnHover()
	{
		base.OnHover();
		_selectedContainer.SetActive(value: true);
	}

	public override void OnExitHover()
	{
		base.OnHover();
		_selectedContainer.SetActive(value: false);
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnControlSchemeChanged -= Instance_OnControlSchemeChanged;
	}
}
