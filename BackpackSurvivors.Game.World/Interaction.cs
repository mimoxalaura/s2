using System;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Stats;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public abstract class Interaction : MonoBehaviour
{
	public delegate void OnInteractionZoneEnteredHandler(object sender, EventArgs e);

	public delegate void OnInteractionZoneExitedHandler(object sender, EventArgs e);

	[SerializeField]
	private Material _interactableMaterial;

	[SerializeField]
	private SpriteRenderer[] _interactableSpriteRenderers;

	[SerializeField]
	private bool _changeMaterialOnRange;

	[SerializeField]
	private AudioClip _audioClipOnInRange;

	[SerializeField]
	private GameObject _keyboardInteractionText;

	[SerializeField]
	private GameObject _controllerInteractionText;

	[SerializeField]
	private GameObject _interactionTextObject;

	[SerializeField]
	private bool _showInteractionText;

	public bool IsInRange;

	public bool CanInteract = true;

	private TextMeshProUGUI _interactionText;

	private Material _originalMaterial;

	private ModalUiController _modalUiController;

	public event OnInteractionZoneEnteredHandler OnInteractionZoneEntered;

	public event OnInteractionZoneExitedHandler OnInteractionZoneExited;

	private void Start()
	{
		DoStart();
	}

	public virtual void DoStart()
	{
		SetInteractionText(inrange: false);
		_modalUiController = UnityEngine.Object.FindAnyObjectByType<ModalUiController>();
	}

	public ModalUiController GetModalUiController()
	{
		if (_modalUiController == null)
		{
			_modalUiController = UnityEngine.Object.FindAnyObjectByType<ModalUiController>();
		}
		return _modalUiController;
	}

	private void SwitchMaterial(bool inRange)
	{
		if (!CanInteract || !(_interactableMaterial != null) || !_interactableSpriteRenderers.Any())
		{
			return;
		}
		if (inRange)
		{
			_originalMaterial = _interactableSpriteRenderers[0].material;
			SpriteRenderer[] interactableSpriteRenderers = _interactableSpriteRenderers;
			for (int i = 0; i < interactableSpriteRenderers.Length; i++)
			{
				interactableSpriteRenderers[i].material = _interactableMaterial;
			}
		}
		else if (!(_originalMaterial == null))
		{
			SpriteRenderer[] interactableSpriteRenderers = _interactableSpriteRenderers;
			for (int i = 0; i < interactableSpriteRenderers.Length; i++)
			{
				interactableSpriteRenderers[i].material = _originalMaterial;
			}
		}
	}

	public void ResetToOriginalMaterial()
	{
		if (!(_originalMaterial == null))
		{
			SpriteRenderer[] interactableSpriteRenderers = _interactableSpriteRenderers;
			for (int i = 0; i < interactableSpriteRenderers.Length; i++)
			{
				interactableSpriteRenderers[i].material = _originalMaterial;
			}
		}
	}

	public virtual void DoInRange()
	{
		IsInRange = true;
		SetInteractionText(inrange: true);
		SingletonController<InputController>.Instance.OnUseHandler += InputController_OnUseHandler;
		if (_changeMaterialOnRange)
		{
			SwitchMaterial(inRange: true);
		}
		SingletonController<AudioController>.Instance.PlaySFXClip(_audioClipOnInRange, 1f);
		this.OnInteractionZoneEntered?.Invoke(this, new EventArgs());
	}

	private void SetInteractionText(bool inrange)
	{
		if (_interactionTextObject == null || _keyboardInteractionText == null || _controllerInteractionText == null)
		{
			return;
		}
		_interactionTextObject.SetActive(_showInteractionText && inrange);
		if (inrange)
		{
			if (SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard)
			{
				_keyboardInteractionText.transform.localScale = new Vector3(0f, 0f, 0f);
				_keyboardInteractionText.SetActive(value: true);
				_controllerInteractionText.SetActive(value: false);
				LeanTween.scale(_keyboardInteractionText.gameObject, new Vector3(1f, 1f, 1f), 0.3f).setEaseInBounce().setEaseOutBounce();
			}
			else
			{
				_controllerInteractionText.transform.localScale = new Vector3(0f, 0f, 0f);
				_controllerInteractionText.SetActive(value: true);
				_keyboardInteractionText.SetActive(value: false);
				LeanTween.scale(_controllerInteractionText.gameObject, new Vector3(1f, 1f, 1f), 0.3f).setEaseInBounce().setEaseOutBounce();
			}
		}
		else
		{
			_keyboardInteractionText.SetActive(value: false);
			_controllerInteractionText.SetActive(value: false);
		}
	}

	public virtual void DoOutOfRange()
	{
		IsInRange = false;
		SetInteractionText(inrange: false);
		SingletonController<InputController>.Instance.OnUseHandler -= InputController_OnUseHandler;
		if (_changeMaterialOnRange)
		{
			SwitchMaterial(inRange: false);
		}
		EnableDash();
		this.OnInteractionZoneExited?.Invoke(this, new EventArgs());
	}

	private void EnableDash()
	{
		SingletonController<GameController>.Instance.Player.SetCanDash(canDash: true);
	}

	private void InputController_OnUseHandler(object sender, EventArgs e)
	{
		DoInteract();
	}

	public virtual void DoInteract()
	{
		Debug.LogError($"Not overriding the Interaction class in {GetType()}");
	}

	public void ChangeInteractionText(string interactionText)
	{
		string format = SingletonController<LocalizationController>.Instance.Translate(base.gameObject);
		_interactionText.text = string.Format(format, interactionText);
	}
}
