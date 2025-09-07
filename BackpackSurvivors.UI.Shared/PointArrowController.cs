using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

public class PointArrowController : MonoBehaviour
{
	[SerializeField]
	private PointArrowTarget _target;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private float _rangeToTrigger;

	private bool _onTarget;

	private bool _isReady;

	private PointArrowSource _source;

	private void Start()
	{
		_isReady = true;
		_source = Object.FindObjectOfType<PointArrowSource>();
	}

	private void FixedUpdate()
	{
		if (!_isReady)
		{
			return;
		}
		if (Vector3.Distance(_target.transform.position, _source.transform.position) > _rangeToTrigger)
		{
			if (_onTarget)
			{
				_spriteRenderer.enabled = true;
				_target.ToggleInRange(inRange: false);
				_onTarget = false;
			}
			Vector3 normalized = (_target.transform.position - _source.transform.position).normalized;
			Vector3 up = base.transform.up;
			normalized = Quaternion.AngleAxis(Vector3.SignedAngle(up, normalized, Vector3.forward) + 90f, Vector3.forward) * up;
			base.transform.rotation = Quaternion.LookRotation(Vector3.forward, normalized);
			base.transform.position = _source.transform.position;
		}
		else if (!_onTarget)
		{
			_spriteRenderer.enabled = false;
			_target.ToggleInRange(inRange: true);
			_onTarget = true;
		}
	}
}
