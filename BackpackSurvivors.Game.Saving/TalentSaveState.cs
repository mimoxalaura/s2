using System;
using System.Collections.Generic;
using System.Linq;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class TalentSaveState : BaseSaveState
{
	public Dictionary<int, bool> TalentUnloackedStates;

	public int AvailableTalentPoints;

	public TalentSaveState()
	{
		Init();
	}

	public void Init()
	{
		TalentUnloackedStates = new Dictionary<int, bool>();
		AvailableTalentPoints = 0;
	}

	public void SetState(Dictionary<int, bool> unlockedTalents, int availableTalentPoints)
	{
		TalentUnloackedStates = unlockedTalents;
		AvailableTalentPoints = availableTalentPoints;
	}

	public override bool HasData()
	{
		if (TalentUnloackedStates == null || !TalentUnloackedStates.Any())
		{
			return AvailableTalentPoints > 0;
		}
		return true;
	}
}
