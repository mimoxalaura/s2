using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using UnityEngine;

namespace BackpackSurvivors.UI.Stats;

public class ModalUiController : MonoBehaviour
{
	[SerializeField]
	private Enums.ModalUITypes _uiOnEscape;

	[SerializeField]
	private AudioClip _UIOpenAudioClip;

	[SerializeField]
	private AudioClip _UICloseAudioClip;

	private bool _isInitialized;

	private List<IModelUIController> _modalUIControllers = new List<IModelUIController>();

	private List<IModelUIController> _activeModalUIControllers = new List<IModelUIController>();

	private Stack<IModelUIController> _modalUIStack = new Stack<IModelUIController>();

	private void Start()
	{
		SingletonController<InputController>.Instance.OnCancelHandler += InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnPauseHandler += InputController_OnPauseHandler;
	}

	private void InitModalUIControllers()
	{
		foreach (IModelUIController item in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: false).OfType<IModelUIController>())
		{
			if (((MonoBehaviour)item).isActiveAndEnabled)
			{
				item.OnCloseButtonClicked += ModelUIController_OnCloseButtonClicked;
				_modalUIControllers.Add(item);
			}
		}
		_isInitialized = true;
	}

	private void OnDestroy()
	{
		foreach (IModelUIController modalUIController in _modalUIControllers)
		{
			modalUIController.OnCloseButtonClicked -= ModelUIController_OnCloseButtonClicked;
		}
		SingletonController<InputController>.Instance.OnCancelHandler -= InputController_OnCancelHandler;
		SingletonController<InputController>.Instance.OnPauseHandler -= InputController_OnPauseHandler;
	}

	private void ModelUIController_OnCloseButtonClicked(object sender, EventArgs e)
	{
		CloseModalUI(((IModelUIController)sender).GetModalUIType());
	}

	private void InputController_OnCancelHandler(object sender, EventArgs e)
	{
		CancelOrPause();
	}

	private void InputController_OnPauseHandler(object sender, EventArgs e)
	{
		CancelOrPause();
	}

	private void CancelOrPause()
	{
		if (SingletonController<TutorialController>.Instance.IsRunningTutorial)
		{
			return;
		}
		DragController dragController = UnityEngine.Object.FindObjectOfType<DragController>();
		if (dragController != null && dragController.IsDragging)
		{
			return;
		}
		if (_modalUIStack.Count > 0)
		{
			if (_modalUIStack.Peek().CloseOnCancelInput())
			{
				CloseTop(forceClose: false);
			}
		}
		else if (_uiOnEscape != Enums.ModalUITypes.None)
		{
			OpenModalUI(_uiOnEscape);
		}
	}

	public void OpenModalUI(Enums.ModalUITypes modalUIType)
	{
		if (!_isInitialized)
		{
			InitModalUIControllers();
		}
		if (modalUIType == Enums.ModalUITypes.GameMenu && _modalUIStack.Count > 0)
		{
			return;
		}
		IModelUIController modelUIController = _modalUIControllers.FirstOrDefault((IModelUIController x) => x != null && ((MonoBehaviour)x).isActiveAndEnabled && x.GetModalUIType() == modalUIType);
		if (modelUIController != null && (_modalUIStack.Count <= 0 || _modalUIStack.Peek().GetModalUIType() != modalUIType))
		{
			SingletonController<InputController>.Instance.SwitchToUIActionMap();
			if (modelUIController.AudioOnOpen())
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_UIOpenAudioClip, 1f);
			}
			_modalUIStack.Push(modelUIController);
			_activeModalUIControllers.Add(modelUIController);
			modelUIController.OpenUI();
		}
	}

	public void CloseAll()
	{
		for (int i = 0; i <= _modalUIStack.Count; i++)
		{
			CloseTop(forceClose: true);
		}
	}

	public void CloseTop(bool forceClose)
	{
		if (_modalUIStack.Any())
		{
			IModelUIController modelUIController = _modalUIStack.Peek();
			_activeModalUIControllers.Remove(modelUIController);
			if (modelUIController.AudioOnClose())
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_UICloseAudioClip, 1f);
			}
			if (forceClose || modelUIController.CloseOnCancelInput())
			{
				modelUIController.CloseUI();
			}
			_modalUIStack.Pop();
			SingletonController<InputController>.Instance.RevertToPreviousActionMap();
		}
	}

	public void CloseModalUI(Enums.ModalUITypes modalUIType, bool revertInput = true, bool unpause = true)
	{
		IModelUIController modelUIController = _modalUIControllers.FirstOrDefault((IModelUIController x) => x.GetModalUIType() == modalUIType);
		modelUIController.CloseUI();
		for (int num = 0; num < _modalUIStack.Count; num++)
		{
			if (_modalUIStack.Peek().GetModalUIType() != modalUIType)
			{
				_modalUIStack.Pop();
			}
			if (_modalUIStack.Count > 0)
			{
				_modalUIStack.Pop();
			}
		}
		_activeModalUIControllers.Remove(modelUIController);
		if (revertInput)
		{
			SingletonController<InputController>.Instance.RevertToPreviousActionMap(unpause);
		}
	}
}
