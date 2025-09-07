using System;
using BackpackSurvivors.System;

namespace BackpackSurvivors.UI.Stats;

public class OnMenuButtonClickedHandlerEventArgs : EventArgs
{
	public Enums.GameMenuButtonType GameMenuButtonType { get; }

	public OnMenuButtonClickedHandlerEventArgs(Enums.GameMenuButtonType gameMenuButtonType)
	{
		GameMenuButtonType = gameMenuButtonType;
	}
}
