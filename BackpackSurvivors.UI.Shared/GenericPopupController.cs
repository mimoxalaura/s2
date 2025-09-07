using System;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Stats;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

public class GenericPopupController : BaseSingletonModalUIController<GenericPopupController>
{
	internal delegate void PopupOpenedHandler(object sender, EventArgs e);

	internal delegate void PopupButtonYesClickedHandler(object sender, EventArgs e);

	internal delegate void PopupButtonNoClickedHandler(object sender, EventArgs e);

	internal delegate void PopupButtonCancelClickedHandler(object sender, EventArgs e);

	internal delegate void PopupButtonOkClickedHandler(object sender, EventArgs e);

	[SerializeField]
	private GenericPopup _prefab;

	[SerializeField]
	private Canvas _popupCanvasParent;

	[SerializeField]
	private GameObject _blackBackdrop;

	private GenericPopup _activePopup;

	private Enums.GenericPopupButtons _buttonToClose;

	private Action _callbackYesClicked;

	private Action _callbackNoClicked;

	private Action _callbackOkClicked;

	private Action _callbackCancelClicked;

	private bool _closeOnCancelInput;

	internal event PopupButtonYesClickedHandler OnPopupButtonYesClicked;

	internal event PopupButtonNoClickedHandler OnPopupButtonNoClicked;

	internal event PopupButtonCancelClickedHandler OnPopupButtonCancelClicked;

	internal event PopupButtonOkClickedHandler OnPopupButtonOkClicked;

	[Command("GenericPopup.Create", Platform.AllPlatforms, MonoTargetType.Single)]
	public void DEBUG_POPUPCREATE()
	{
		CreatePopup(Enums.GenericPopupLocation.TopOfScreen, "TEST", "Description here", Enums.GenericPopupButtons.No, Enums.GenericPopupButtons.No, closeOnCancel: false, null);
	}

	[Command("GenericPopup.Open", Platform.AllPlatforms, MonoTargetType.Single)]
	public void DEBUG_OPENPOPUP()
	{
		ShowPopup();
	}

	private void _activePopup_OnPopupButtonYesClicked(object sender, EventArgs e)
	{
		Debug.Log("YES CLICKED");
	}

	public void DEBUG_Yes_Callback()
	{
		Debug.Log("YES CALLBACK");
	}

	public void DEBUG_No_Callback()
	{
		Debug.Log("NO CALLBACK");
	}

	public void DEBUG_Ok_Callback()
	{
		Debug.Log("OK CALLBACK");
	}

	public void DEBUG_Cancel_Callback()
	{
		Debug.Log("CANCEL CALLBACK");
	}

	private void Start()
	{
		base.IsInitialized = true;
	}

	public void CreatePopup(Enums.GenericPopupLocation genericPopupLocation, string headerText, string bodyText, Enums.GenericPopupButtons buttons, Enums.GenericPopupButtons onCancelAction, bool closeOnCancel, Sprite image)
	{
		if (_activePopup != null)
		{
			CloseUI();
		}
		_buttonToClose = onCancelAction;
		_closeOnCancelInput = closeOnCancel;
		GenericPopup genericPopup = UnityEngine.Object.Instantiate(_prefab, _popupCanvasParent.transform);
		genericPopup.Init(genericPopupLocation, headerText, bodyText, buttons, image);
		genericPopup.OnPopupButtonYesClicked += GenericPopup_OnPopupButtonYesClicked;
		genericPopup.OnPopupButtonNoClicked += GenericPopup_OnPopupButtonNoClicked;
		genericPopup.OnPopupButtonCancelClicked += GenericPopup_OnPopupButtonCancelClicked;
		genericPopup.OnPopupButtonOkClicked += GenericPopup_OnPopupButtonOkClicked;
		_activePopup = genericPopup;
	}

	public void CreatePopup(Enums.GenericPopupLocation genericPopupLocation, string headerText, string bodyText, Enums.GenericPopupButtons buttons, Enums.GenericPopupButtons buttonToClose, Action callbackYesClicked, Action callbackNoClicked, Action callbackOkClicked, Action callbackCancelClicked, bool closeOnCancel, Sprite image)
	{
		if (_activePopup != null)
		{
			CloseUI();
		}
		_buttonToClose = buttonToClose;
		_closeOnCancelInput = closeOnCancel;
		_callbackYesClicked = callbackYesClicked;
		_callbackNoClicked = callbackNoClicked;
		_callbackOkClicked = callbackOkClicked;
		_callbackCancelClicked = callbackCancelClicked;
		GenericPopup genericPopup = UnityEngine.Object.Instantiate(_prefab, _popupCanvasParent.transform);
		genericPopup.Init(genericPopupLocation, headerText, bodyText, buttons, image);
		genericPopup.OnPopupButtonYesClicked += GenericPopup_OnPopupButtonYesClicked;
		genericPopup.OnPopupButtonNoClicked += GenericPopup_OnPopupButtonNoClicked;
		genericPopup.OnPopupButtonCancelClicked += GenericPopup_OnPopupButtonCancelClicked;
		genericPopup.OnPopupButtonOkClicked += GenericPopup_OnPopupButtonOkClicked;
		_activePopup = genericPopup;
	}

	public void ShowPopup(bool blackBackdrop = false)
	{
		_blackBackdrop.SetActive(blackBackdrop);
		UnityEngine.Object.FindObjectOfType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.GenericPopup);
	}

	public void ClosePopup(bool revertInput = true)
	{
		_blackBackdrop.SetActive(value: false);
		UnityEngine.Object.FindObjectOfType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.GenericPopup, revertInput);
	}

	[Command("GenericPopup.Close", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void CloseUI()
	{
		if (!(_activePopup == null))
		{
			base.CloseUI();
			if (_buttonToClose == Enums.GenericPopupButtons.Cancel)
			{
				_callbackCancelClicked?.Invoke();
			}
			else if (_buttonToClose == Enums.GenericPopupButtons.No)
			{
				_callbackNoClicked?.Invoke();
			}
			else if (_buttonToClose == Enums.GenericPopupButtons.Yes)
			{
				_callbackYesClicked?.Invoke();
			}
			else if (_buttonToClose == Enums.GenericPopupButtons.Ok)
			{
				_callbackOkClicked?.Invoke();
			}
			_blackBackdrop.SetActive(value: false);
			_activePopup.ClosePopup();
			_activePopup = null;
		}
	}

	public override void OpenUI()
	{
		if (!(_activePopup == null))
		{
			base.OpenUI();
			_activePopup.ShowPopup();
		}
	}

	private void GenericPopup_OnPopupButtonOkClicked(object sender, EventArgs e)
	{
		this.OnPopupButtonOkClicked?.Invoke(this, new EventArgs());
		_callbackOkClicked?.Invoke();
		if (_buttonToClose == Enums.GenericPopupButtons.No)
		{
			CloseUI();
		}
	}

	private void GenericPopup_OnPopupButtonCancelClicked(object sender, EventArgs e)
	{
		this.OnPopupButtonCancelClicked?.Invoke(this, new EventArgs());
		_callbackCancelClicked?.Invoke();
		if (_buttonToClose == Enums.GenericPopupButtons.Cancel)
		{
			CloseUI();
		}
	}

	private void GenericPopup_OnPopupButtonNoClicked(object sender, EventArgs e)
	{
		this.OnPopupButtonNoClicked?.Invoke(this, new EventArgs());
		_callbackNoClicked?.Invoke();
		if (_buttonToClose == Enums.GenericPopupButtons.No)
		{
			CloseUI();
		}
	}

	private void GenericPopup_OnPopupButtonYesClicked(object sender, EventArgs e)
	{
		this.OnPopupButtonYesClicked?.Invoke(this, new EventArgs());
		_callbackYesClicked?.Invoke();
		if (_buttonToClose == Enums.GenericPopupButtons.Yes)
		{
			CloseUI();
		}
	}

	public override bool CloseOnCancelInput()
	{
		return _closeOnCancelInput;
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.GenericPopup;
	}

	public override void ClearAdventure()
	{
		CloseUI();
	}

	public override void Clear()
	{
	}

	public void ClearEvents()
	{
		if (this.OnPopupButtonYesClicked != null)
		{
			Delegate[] invocationList = this.OnPopupButtonYesClicked.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				OnPopupButtonYesClicked -= (PopupButtonYesClickedHandler)obj;
			}
		}
		if (this.OnPopupButtonNoClicked != null)
		{
			Delegate[] invocationList = this.OnPopupButtonNoClicked.GetInvocationList();
			foreach (Delegate obj2 in invocationList)
			{
				OnPopupButtonNoClicked -= (PopupButtonNoClickedHandler)obj2;
			}
		}
		if (this.OnPopupButtonOkClicked != null)
		{
			Delegate[] invocationList = this.OnPopupButtonOkClicked.GetInvocationList();
			foreach (Delegate obj3 in invocationList)
			{
				OnPopupButtonOkClicked -= (PopupButtonOkClickedHandler)obj3;
			}
		}
		if (this.OnPopupButtonCancelClicked != null)
		{
			Delegate[] invocationList = this.OnPopupButtonCancelClicked.GetInvocationList();
			foreach (Delegate obj4 in invocationList)
			{
				OnPopupButtonCancelClicked -= (PopupButtonCancelClickedHandler)obj4;
			}
		}
	}
}
