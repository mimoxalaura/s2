using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers.TriggerConditions;

internal class BaseTriggerCondition : MonoBehaviour
{
	public virtual bool ShouldExecute()
	{
		return true;
	}
}
