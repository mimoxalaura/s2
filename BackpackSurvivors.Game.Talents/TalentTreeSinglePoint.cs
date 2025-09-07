using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentTreeSinglePoint : TalentTreeComponent
{
	internal override List<TalentTreePoint> UpdateTalentPoints(bool debug = true)
	{
		List<TalentTreePoint> list = base.UpdateTalentPoints();
		TalentTreePoint talentTreePoint = Object.Instantiate(base.TalentPointPrefab, base.TalentPointContainer);
		talentTreePoint.transform.localPosition = new Vector3(0f, 0f, 0f);
		talentTreePoint.transform.localScale = new Vector3(1f, 1f, 1f);
		talentTreePoint.SetTalent(base.Talents[0]);
		talentTreePoint.Init(debug);
		list.Add(talentTreePoint);
		return list;
	}
}
