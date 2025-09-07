using UnityEngine;

namespace BackpackSurvivors.Game.Backpack.Highlighting;

public class PotentialMergableItemLine : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private Material _material;

	private void Start()
	{
		LeanTween.value(base.gameObject, delegate(float val)
		{
			_lineRenderer.startWidth = val;
			_lineRenderer.endWidth = val;
		}, 0f, 0.3f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void Init(Vector2 startPos, Vector3 endPos)
	{
		_lineRenderer.material = _material;
		_lineRenderer.positionCount = 2;
		_lineRenderer.SetPosition(0, startPos);
		_lineRenderer.SetPosition(1, endPos);
	}
}
