using System;
using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentTreeWheel : TalentTreeComponent
{
	internal override List<TalentTreePoint> UpdateTalentPoints(bool debug = true)
	{
		List<TalentTreePoint> list = base.UpdateTalentPoints();
		float num = ((RectTransform)base.TalentPointContainer).rect.width / 2f;
		float num2 = ((RectTransform)base.TalentPointContainer).rect.width / ((RectTransform)base.TalentPointContainer).rect.height;
		float num3 = ((RectTransform)base.TalentPointContainer).rect.height / ((RectTransform)base.TalentPointContainer).rect.width;
		for (int i = 0; i < base.Talents.Length; i++)
		{
			float x = MathF.PI * 2f / (float)base.Talents.Length * (float)i;
			float num4 = MathF.Sin(x);
			float num5 = MathF.Cos(x);
			if (num2 > 1f)
			{
				num4 *= num3;
			}
			if (num3 > 1f)
			{
				num5 *= num2;
			}
			Vector3 vector = new Vector3(num5, num4, 0f);
			Vector3 vector2 = base.TalentPointContainer.transform.localPosition + vector * num;
			base.CreatedPositions[i] = vector2;
			base.LineRenderer.SetPosition(i, vector2);
			TalentTreePoint talentTreePoint = UnityEngine.Object.Instantiate(base.TalentPointPrefab, base.TalentPointContainer);
			talentTreePoint.transform.localPosition = vector2;
			talentTreePoint.transform.localScale = new Vector3(1f, 1f, 1f);
			talentTreePoint.SetTalent(base.Talents[i]);
			talentTreePoint.Init(debug);
			list.Add(talentTreePoint);
		}
		for (int j = 0; j < list.Count; j++)
		{
			CreateNeighbors(j, list);
		}
		base.LineRenderer.SetPosition(base.Talents.Length, base.CreatedPositions[0]);
		return list;
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
