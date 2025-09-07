using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentTreeLine : TalentTreeComponent
{
	internal override List<TalentTreePoint> UpdateTalentPoints(bool debug = true)
	{
		List<TalentTreePoint> list = base.UpdateTalentPoints();
		base.LineRenderer.positionCount = base.Talents.Length;
		Vector3 localPosition = base.TalentPointContainer.transform.localPosition;
		float num = ((RectTransform)base.TalentPointContainer).rect.width / 2f;
		_ = ((RectTransform)base.TalentPointContainer).rect.height / 2f;
		Vector2 vector = new Vector2(localPosition.x - num, localPosition.y);
		Vector2 vector2 = (new Vector2(localPosition.x + num, localPosition.y) - vector) / (base.Talents.Length - 1);
		for (int i = 0; i < base.Talents.Length; i++)
		{
			Vector2 spawnPos = vector + vector2 * i;
			list.Add(SpawnSingleTalentPoint(i, spawnPos, debug));
		}
		for (int j = 0; j < base.Talents.Length; j++)
		{
			CreateNeighbors(j, list);
		}
		return list;
	}

	private TalentTreePoint SpawnSingleTalentPoint(int index, Vector2 spawnPos, bool debug = true)
	{
		base.CreatedPositions[index] = spawnPos;
		base.LineRenderer.SetPosition(index, spawnPos);
		TalentTreePoint talentTreePoint = Object.Instantiate(base.TalentPointPrefab, base.TalentPointContainer);
		talentTreePoint.transform.localPosition = spawnPos;
		talentTreePoint.transform.localScale = new Vector3(1f, 1f, 1f);
		talentTreePoint.transform.rotation = Quaternion.identity;
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
