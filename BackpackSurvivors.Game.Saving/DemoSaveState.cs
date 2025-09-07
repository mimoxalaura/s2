using System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
internal class DemoSaveState : BaseSaveState
{
	public bool HasShownDemoPopup;

	public DemoSaveState(bool hasShownDemoPopup)
	{
		HasShownDemoPopup = hasShownDemoPopup;
	}

	public override bool HasData()
	{
		return false;
	}
}
