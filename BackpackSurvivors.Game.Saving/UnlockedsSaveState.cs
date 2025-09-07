using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class UnlockedsSaveState : BaseSaveState
{
	public Dictionary<Enums.Unlockable, int> UnlockedUpgrades;

	public UnlockedsSaveState()
	{
		Init();
	}

	public void Init()
	{
		UnlockedUpgrades = new Dictionary<Enums.Unlockable, int>();
	}

	public void SetState(Dictionary<Enums.Unlockable, int> unlockedUpgrades)
	{
		UnlockedUpgrades = unlockedUpgrades;
	}

	public override bool HasData()
	{
		if (UnlockedUpgrades != null)
		{
			return UnlockedUpgrades.Any();
		}
		return false;
	}
}
