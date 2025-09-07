using System.Collections.Generic;
using BackpackSurvivors.ScriptableObjects.Talents;
using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentTreeComponent : MonoBehaviour
{
	[SerializeField]
	private TalentTreePoint _talentPointPrefab;

	[SerializeField]
	private Transform _talentPointContainer;

	[SerializeField]
	private TalentSO[] _talents;

	[SerializeField]
	private LineRenderer _lineRenderer;

	private Vector3[] _createdPositions;

	protected TalentTreePoint TalentPointPrefab => _talentPointPrefab;

	protected Transform TalentPointContainer => _talentPointContainer;

	protected TalentSO[] Talents => _talents;

	protected LineRenderer LineRenderer => _lineRenderer;

	protected Vector3[] CreatedPositions => _createdPositions;

	internal virtual List<TalentTreePoint> UpdateTalentPoints(bool debug = true)
	{
		for (int num = TalentPointContainer.childCount - 1; num >= 0; num--)
		{
			Object.DestroyImmediate(TalentPointContainer.GetChild(num).gameObject);
		}
		LineRenderer.positionCount = 0;
		LineRenderer.positionCount = Talents.Length + 1;
		_createdPositions = new Vector3[Talents.Length];
		return new List<TalentTreePoint>();
	}
}
