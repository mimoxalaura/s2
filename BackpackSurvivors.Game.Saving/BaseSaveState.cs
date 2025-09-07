using System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public abstract class BaseSaveState
{
	public abstract bool HasData();
}
