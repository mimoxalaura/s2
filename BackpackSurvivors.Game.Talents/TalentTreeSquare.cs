using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentTreeSquare : TalentTreeComponent
{
	internal override List<TalentTreePoint> UpdateTalentPoints(bool debug = false)
	{
		List<TalentTreePoint> list = base.UpdateTalentPoints();
		Vector3 localPosition = base.TalentPointContainer.transform.localPosition;
		float num = ((RectTransform)base.TalentPointContainer).rect.width / 2f;
		float num2 = ((RectTransform)base.TalentPointContainer).rect.height / 2f;
		list.Add(SpawnSingleTalentPoint(0, new Vector2(localPosition.x + num, localPosition.y + num2), debug));
		list.Add(SpawnSingleTalentPoint(1, new Vector2(localPosition.x - num, localPosition.y + num2), debug));
		list.Add(SpawnSingleTalentPoint(2, new Vector2(localPosition.x - num, localPosition.y - num2), debug));
		list.Add(SpawnSingleTalentPoint(3, new Vector2(localPosition.x + num, localPosition.y - num2), debug));
		base.LineRenderer.SetPosition(base.Talents.Length, base.CreatedPositions[0]);
		for (int i = 0; i < list.Count; i++)
		{
			CreateNeighbors(i, list);
		}
		return list;
	}

	private TalentTreePoint SpawnSingleTalentPoint(int index, Vector2 spawnPos, bool debug = false)
	{
		base.CreatedPositions[index] = spawnPos;
		base.LineRenderer.SetPosition(index, spawnPos);
		TalentTreePoint talentTreePoint = Object.Instantiate(base.TalentPointPrefab, base.TalentPointContainer);
		talentTreePoint.transform.localPosition = spawnPos;
		talentTreePoint.transform.localScale = new Vector3(1f, 1f, 1f);
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
		else if (index + 1 == result.Count)
		{
			TalentTreePoint neighbor2 = result[0];
			result[index].AddNeighbor(neighbor2);
		}
	}
}
