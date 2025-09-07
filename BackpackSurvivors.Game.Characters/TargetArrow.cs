using BackpackSurvivors.Game.Player;
using UnityEngine;

namespace BackpackSurvivors.Game.Characters;

public class TargetArrow : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private SpriteRenderer _iconRenderer;

	[SerializeField]
	private TargetArrowCornerEnforcer _targetArrowCornerEnforcer;

	private GameObject _objectToTarget;

	private BackpackSurvivors.Game.Player.Player _player;

	private float _maximumDistanceToShow = 4f;

	private bool _toggled;

	public Vector3 DEBUG_Direction;

	public Quaternion DEBUG_Rotation;

	private bool isReady;

	public void Setup(BackpackSurvivors.Game.Player.Player player, GameObject objectToTarget, float maximumDistanceToShow, Sprite icon, Sprite arrowSprite)
	{
		_player = player;
		_objectToTarget = objectToTarget;
		_maximumDistanceToShow = maximumDistanceToShow;
		isReady = true;
		_iconRenderer.sprite = icon;
		_spriteRenderer.sprite = arrowSprite;
	}

	public void UpdateTarget(GameObject objectToTarget)
	{
		_objectToTarget = objectToTarget;
		isReady = _objectToTarget != null;
	}

	public void Toggle(bool toggled)
	{
		_toggled = toggled;
		_spriteRenderer.enabled = _toggled;
		_iconRenderer.enabled = _toggled;
	}

	public bool IsToggled()
	{
		return _toggled;
	}

	private void Start()
	{
		_spriteRenderer.enabled = false;
		_iconRenderer.enabled = false;
	}

	private void Update()
	{
		if (isReady && _toggled && _objectToTarget != null)
		{
			if (Vector3.Distance(_objectToTarget.transform.position, _player.gameObject.transform.position) > _maximumDistanceToShow)
			{
				_spriteRenderer.enabled = true;
				_iconRenderer.enabled = true;
				Vector3 normalized = (_objectToTarget.transform.position - _player.gameObject.transform.position).normalized;
				Vector3 up = base.transform.up;
				normalized = Quaternion.AngleAxis(Vector3.SignedAngle(up, normalized, Vector3.forward) + 90f, Vector3.forward) * up;
				_spriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, normalized);
				_spriteRenderer.transform.position = _player.gameObject.transform.position;
				_iconRenderer.transform.rotation = new Quaternion(0f - _spriteRenderer.transform.rotation.x, 0f - _spriteRenderer.transform.rotation.y, 0f - _spriteRenderer.transform.rotation.y, 0f - _spriteRenderer.transform.rotation.z);
				DEBUG_Direction = normalized;
				DEBUG_Rotation = _spriteRenderer.transform.rotation;
			}
			else
			{
				_spriteRenderer.enabled = false;
				_iconRenderer.enabled = false;
			}
		}
	}
}
