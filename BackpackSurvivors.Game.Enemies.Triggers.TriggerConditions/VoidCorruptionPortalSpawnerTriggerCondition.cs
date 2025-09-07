using System.Linq;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Triggers.TriggerConditions;

internal class VoidCorruptionPortalSpawnerTriggerCondition : BaseTriggerCondition
{
	[SerializeField]
	private int _maxEnemyCount;

	public override bool ShouldExecute()
	{
		return Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None).Count() <= _maxEnemyCount;
	}
}
