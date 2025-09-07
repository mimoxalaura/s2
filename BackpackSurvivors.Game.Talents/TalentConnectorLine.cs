using UnityEngine;

namespace BackpackSurvivors.Game.Talents;

public class TalentConnectorLine : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private TalentNode _talentPoint1;

	[SerializeField]
	private TalentNode _talentPoint2;

	public void Init(TalentNode talentPoint1, TalentNode talentPoint2)
	{
		_lineRenderer.positionCount = 2;
		_talentPoint1 = talentPoint1;
		_talentPoint2 = talentPoint2;
		RefreshPosition();
	}

	public void RefreshPosition()
	{
		_lineRenderer.SetPosition(0, _talentPoint1.transform.position);
		_lineRenderer.SetPosition(1, _talentPoint2.transform.position);
	}
}
