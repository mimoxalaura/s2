using System.Collections.Generic;
using BackpackSurvivors.ScriptableObjects.Talents;
using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentTreeChain : TalentTreeComponent
{
	[SerializeField]
	private SerializableDictionaryBase<TalentSO, Vector2> _positions;

	[SerializeField]
	private Quaternion _rotation;

	internal override List<TalentTreePoint> UpdateTalentPoints(bool debug = true)
	{
		List<TalentTreePoint> list = base.UpdateTalentPoints();
		base.LineRenderer.positionCount = base.Talents.Length;
		_ = base.TalentPointContainer.transform.localPosition;
		_ = ((RectTransform)base.TalentPointContainer).rect.width / 2f;
		_ = ((RectTransform)base.TalentPointContainer).rect.height / 2f;
		for (int i = 0; i < base.Talents.Length; i++)
		{
			TalentTreePoint talentTreePoint = SpawnSingleTalentPoint(i, new Vector2(0f, 0f), debug);
			if (_positions.ContainsKey(base.Talents[i]))
			{
				Vector2 vector = _positions[base.Talents[i]];
				talentTreePoint.transform.localPosition = vector;
				base.CreatedPositions[i] = vector;
				base.LineRenderer.SetPosition(i, vector);
			}
			list.Add(talentTreePoint);
		}
		for (int j = 0; j < base.Talents.Length; j++)
		{
			CreateNeighbors(j, list);
		}
		return list;
	}

	private void StorePositions()
	{
		_positions.Clear();
		for (int i = 0; i < base.TalentPointContainer.childCount; i++)
		{
			Transform child = base.TalentPointContainer.GetChild(i);
			_positions.Add(base.Talents[i], child.transform.localPosition);
		}
	}

	private TalentTreePoint SpawnSingleTalentPoint(int index, Vector2 spawnPos, bool debug = true)
	{
		base.CreatedPositions[index] = spawnPos;
		base.LineRenderer.SetPosition(index, spawnPos);
		TalentTreePoint talentTreePoint = Object.Instantiate(base.TalentPointPrefab, base.TalentPointContainer);
		talentTreePoint.transform.localPosition = spawnPos;
		talentTreePoint.transform.localScale = new Vector3(1f, 1f, 1f);
		talentTreePoint.transform.rotation = Quaternion.identity * _rotation;
		talentTreePoint.SetTalent(base.Talents[index]);
		talentTreePoint.Init(debug);
		return talentTreePoint;
	}

	private void CreateNeighbors(int index, List<TalentTreePoint> result)
	{
		AddPreviousTalentPoint(index, result);
		AddNextTalentPoint(index, result);
	}

	private void AddPreviousTalentPoint(int index, List<TalentTreePoint> result)
	{
		if (index > 0)
		{
			TalentTreePoint neighbor = result[index - 1];
			result[index].AddNeighbor(neighbor);
		}
	}

	private void AddNextTalentPoint(int index, List<TalentTreePoint> result)
	{
		if (index + 1 < result.Count)
		{
			TalentTreePoint neighbor = result[index + 1];
			result[index].AddNeighbor(neighbor);
		}
	}
}
