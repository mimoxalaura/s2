using BackpackSurvivors.System;

namespace BackpackSurvivors.UI.Shared;

public interface IModelUIController
{
	event BaseModalUIController.CloseButtonClickedHandler OnCloseButtonClicked;

	void OpenUI();

	void CloseUI();

	bool CloseOnCancelInput();

	bool AudioOnOpen();

	bool AudioOnClose();

	Enums.ModalUITypes GetModalUIType();
}
