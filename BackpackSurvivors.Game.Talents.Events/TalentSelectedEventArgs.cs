using System;
using BackpackSurvivors.ScriptableObjects.Talents;

namespace BackpackSurvivors.Game.Talents.Events;

public class TalentSelectedEventArgs : EventArgs
{
	public TalentSO Talent { get; set; }

	public bool WasActive { get; }

	public TalentSelectedEventArgs(TalentSO talent, bool wasActive)
	{
		Talent = talent;
		WasActive = wasActive;
	}
}
