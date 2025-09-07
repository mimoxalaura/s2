using System;
using BackpackSurvivors.ScriptableObjects.Talents;

namespace BackpackSurvivors.Game.Talents;

[Serializable]
public struct TalentConnector
{
	public TalentSO TalentOne;

	public TalentSO TalentTwo;
}
