using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

public class KeyInformation : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _keyboardInformationElements;

	[SerializeField]
	private GameObject[] _controllerInformationElements;

	private void Start()
	{
		SetupButtonIcons();
		SingletonController<InputController>.Instance.OnControlSchemeChanged += InputController_OnControlSchemeChanged;
	}

	internal void SetupButtonIcons()
	{
		GameObject[] keyboardInformationElements = _keyboardInformationElements;
		for (int i = 0; i < keyboardInformationElements.Length; i++)
		{
			keyboardInformationElements[i].gameObject.SetActive(SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard);
		}
		keyboardInformationElements = _controllerInformationElements;
		for (int i = 0; i < keyboardInformationElements.Length; i++)
		{
			keyboardInformationElements[i].gameObject.SetActive(!SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard);
		}
	}

	private void InputController_OnControlSchemeChanged(object sender, ControlSchemeChangedEventArgs e)
	{
		SetupButtonIcons();
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnControlSchemeChanged -= InputController_OnControlSchemeChanged;
	}
}
