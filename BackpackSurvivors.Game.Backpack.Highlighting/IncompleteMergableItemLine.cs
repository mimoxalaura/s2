using UnityEngine;

namespace BackpackSurvivors.Game.Backpack.Highlighting;

public class IncompleteMergableItemLine : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private Material _material;

	public void Init(Vector2 startPos, Vector3 endPos)
	{
		_lineRenderer.material = _material;
		_lineRenderer.positionCount = 2;
		_lineRenderer.SetPosition(0, startPos);
		_lineRenderer.SetPosition(1, endPos);
	}
}
