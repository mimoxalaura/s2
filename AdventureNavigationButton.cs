using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.UI;

public class AdventureNavigationButton : MonoBehaviour
{
	[SerializeField]
	private Image _directionImage;

	[SerializeField]
	private Color _interactableDirectionImageColor;

	[SerializeField]
	private Color _notInteractableDirectionImageColor;

	[SerializeField]
	private Image _keyboardImage;

	[SerializeField]
	private Image _controllerImage;

	[SerializeField]
	private Color _interactableKeyboardHotkeyTextColor;

	[SerializeField]
	private Color _notInteractableKeyboardHotkeyTextColor;

	[SerializeField]
	private Button _interactionButton;

	private bool _interactable;

	public void SetInteractable(bool interactable)
	{
		_interactable = interactable;
		_interactionButton.interactable = interactable;
		if (_directionImage != null)
		{
			_directionImage.color = (interactable ? _interactableDirectionImageColor : _notInteractableDirectionImageColor);
		}
		_keyboardImage.color = (interactable ? _interactableDirectionImageColor : _notInteractableDirectionImageColor);
		_controllerImage.color = (interactable ? _interactableDirectionImageColor : _notInteractableDirectionImageColor);
	}

	private void Start()
	{
		SetupButtonIcons();
		SingletonController<InputController>.Instance.OnControlSchemeChanged += Instance_OnControlSchemeChanged;
	}

	private void SetupButtonIcons()
	{
		if (_keyboardImage != null)
		{
			_keyboardImage.gameObject.SetActive(SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard);
		}
		if (_controllerImage != null)
		{
			_controllerImage.gameObject.SetActive(!SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard);
		}
	}

	private void Instance_OnControlSchemeChanged(object sender, ControlSchemeChangedEventArgs e)
	{
		SetupButtonIcons();
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnControlSchemeChanged -= Instance_OnControlSchemeChanged;
	}
}
