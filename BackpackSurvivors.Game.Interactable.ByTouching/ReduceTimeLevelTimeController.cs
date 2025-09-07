using System;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Interactable.ByTouching;

internal class ReduceTimeLevelTimeController : MonoBehaviour
{
	[SerializeField]
	private int _timeToAddOnInteraction = 60;

	[SerializeField]
	private InteractableLever[] _interactableLevers;

	private void Start()
	{
		InteractableLever[] interactableLevers = _interactableLevers;
		for (int i = 0; i < interactableLevers.Length; i++)
		{
			interactableLevers[i].OnLeverPulled += InteractableLevers_OnLeverPulled;
		}
	}

	private void InteractableLevers_OnLeverPulled(object sender, EventArgs e)
	{
		TimeBasedLevelController controllerByType = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
		if (!(controllerByType == null) && !controllerByType.BossSpawned)
		{
			controllerByType.AddLevelSpendTime(_timeToAddOnInteraction);
			SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup("Reduced Time!", Constants.Colors.PositiveEffectColor, 3f);
		}
	}

	private void OnDestroy()
	{
		InteractableLever[] interactableLevers = _interactableLevers;
		for (int i = 0; i < interactableLevers.Length; i++)
		{
			interactableLevers[i].OnLeverPulled -= InteractableLevers_OnLeverPulled;
		}
	}
}
