using System.Collections.Generic;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Characters;

public class TargetArrowController : MonoBehaviour
{
	[SerializeField]
	private TargetArrow _targetArrowPrefab;

	private List<TargetArrow> _activeArrows;

	private void Start()
	{
		_activeArrows = new List<TargetArrow>();
	}

	public TargetArrow SpawnArrow(GameObject target, Sprite icon, Sprite arrowIcon, float distanceToShow = 10f)
	{
		TargetArrow targetArrow = Object.Instantiate(_targetArrowPrefab);
		targetArrow.Setup(SingletonController<GameController>.Instance.Player, target, distanceToShow, icon, arrowIcon);
		_activeArrows.Add(targetArrow);
		targetArrow.Toggle(toggled: true);
		return targetArrow;
	}

	public void RemoveArrow(TargetArrow targetArrow)
	{
		if (targetArrow != null && targetArrow.isActiveAndEnabled)
		{
			_activeArrows.Remove(targetArrow);
			Object.Destroy(targetArrow.gameObject);
		}
	}

	public void RemoveAllArrows()
	{
		for (int i = 0; i < _activeArrows.Count; i++)
		{
			RemoveArrow(_activeArrows[i]);
		}
	}
}
