using System;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Shared;

public class GenericPopup : ModalUI
{
	internal delegate void PopupButtonYesClickedHandler(object sender, EventArgs e);

	internal delegate void PopupButtonNoClickedHandler(object sender, EventArgs e);

	internal delegate void PopupButtonCancelClickedHandler(object sender, EventArgs e);

	internal delegate void PopupButtonOkClickedHandler(object sender, EventArgs e);

	[SerializeField]
	private TextMeshProUGUI _headerText;

	[SerializeField]
	private TextMeshProUGUI _bodyText;

	[SerializeField]
	private Button _yesButton;

	[SerializeField]
	private Button _noButton;

	[SerializeField]
	private Button _cancelButton;

	[SerializeField]
	private Button _okButton;

	[SerializeField]
	private Image _image;

	internal event PopupButtonYesClickedHandler OnPopupButtonYesClicked;

	internal event PopupButtonNoClickedHandler OnPopupButtonNoClicked;

	internal event PopupButtonCancelClickedHandler OnPopupButtonCancelClicked;

	internal event PopupButtonOkClickedHandler OnPopupButtonOkClicked;

	public void Init(Enums.GenericPopupLocation genericPopupLocation, string headerText, string bodyText, Enums.GenericPopupButtons buttons, Sprite image)
	{
		_headerText.SetText(headerText);
		_bodyText.SetText(bodyText);
		_yesButton.gameObject.SetActive(buttons.HasFlag(Enums.GenericPopupButtons.Yes));
		_noButton.gameObject.SetActive(buttons.HasFlag(Enums.GenericPopupButtons.No));
		_cancelButton.gameObject.SetActive(buttons.HasFlag(Enums.GenericPopupButtons.Cancel));
		_okButton.gameObject.SetActive(buttons.HasFlag(Enums.GenericPopupButtons.Ok));
		if (_image != null)
		{
			_image.sprite = image;
		}
		RepositionPopup(genericPopupLocation);
		_yesButton.onClick.AddListener(OnYesCLick);
		_noButton.onClick.AddListener(OnNoCLick);
		_cancelButton.onClick.AddListener(OnCancelCLick);
		_okButton.onClick.AddListener(OnOkCLick);
	}

	private void OnYesCLick()
	{
		this.OnPopupButtonYesClicked?.Invoke(this, new EventArgs());
	}

	private void OnNoCLick()
	{
		this.OnPopupButtonNoClicked?.Invoke(this, new EventArgs());
	}

	private void OnCancelCLick()
	{
		this.OnPopupButtonCancelClicked?.Invoke(this, new EventArgs());
	}

	private void OnOkCLick()
	{
		this.OnPopupButtonOkClicked?.Invoke(this, new EventArgs());
	}

	private void RepositionPopup(Enums.GenericPopupLocation genericPopupLocation)
	{
		RectTransform component = GetComponent<RectTransform>();
		float num = (float)Screen.width / 4f;
		float num2 = (float)Screen.height / 4f;
		switch (genericPopupLocation)
		{
		case Enums.GenericPopupLocation.MiddleOfScreen:
			component.anchoredPosition = new Vector2(0f, 0f);
			break;
		case Enums.GenericPopupLocation.BottomOfScreen:
			component.anchoredPosition = new Vector2(0f, 0f - num2);
			break;
		case Enums.GenericPopupLocation.TopOfScreen:
			component.anchoredPosition = new Vector2(0f, num2);
			break;
		case Enums.GenericPopupLocation.LeftOfScreen:
			component.anchoredPosition = new Vector2(0f - num, 0f);
			break;
		case Enums.GenericPopupLocation.RightOfScreen:
			component.anchoredPosition = new Vector2(num, 0f);
			break;
		}
	}

	internal void ClosePopup()
	{
		base.CloseUI();
	}

	internal void ShowPopup()
	{
		base.OpenUI();
	}

	public override void AfterCloseUI()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		_yesButton.onClick.RemoveAllListeners();
		_noButton.onClick.RemoveAllListeners();
		_cancelButton.onClick.RemoveAllListeners();
		_okButton.onClick.RemoveAllListeners();
	}
}
