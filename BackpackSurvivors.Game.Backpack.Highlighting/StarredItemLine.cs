using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack.Highlighting;

public class StarredItemLine : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private Image _startImage;

	[SerializeField]
	private Image _endImage;

	[SerializeField]
	private Material _material;

	[SerializeField]
	private Material _negativeMaterial;

	[SerializeField]
	private Material _materialPoint;

	[SerializeField]
	private Material _negativeMaterialPoint;

	public void Init(Vector2 startPos, Vector3 endPos, bool effectIsPositive)
	{
		if (effectIsPositive)
		{
			_lineRenderer.material = _material;
			_startImage.material = _materialPoint;
			_endImage.material = _materialPoint;
		}
		else
		{
			_lineRenderer.material = _negativeMaterial;
			_startImage.material = _negativeMaterialPoint;
			_endImage.material = _negativeMaterialPoint;
		}
		_lineRenderer.positionCount = 2;
		_lineRenderer.SetPosition(0, startPos);
		_lineRenderer.SetPosition(1, endPos);
		_startImage.transform.localPosition = startPos;
		_endImage.transform.localPosition = endPos;
	}
}
