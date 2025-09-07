using System;
using System.Collections;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private float _aimTargetLineLength;

	private Player _player;

	private float _moveSpeed;

	private Vector2 _movementInput;

	private Vector2 _aimInput;

	private PlayerDash _playerDash;

	private Rigidbody2D _rigidBody;

	private bool _isAutoRunning;

	internal bool IsRotatedToRight { get; private set; }

	public float MovementSpeed => _moveSpeed;

	[SerializeField]
	public bool IsMoving => _movementInput != Vector2.zero;

	public bool IsBeingKnockedback { get; internal set; }

	public event Action<Vector2> OnPlayerAimUpdated;

	public event Action<float> OnPlayerRotationUpdated;

	private void FixedUpdate()
	{
		if (_player.CanAct)
		{
			ApplyMovement();
			HandleRotation();
		}
	}

	private void Start()
	{
		Init();
		RegisterComponents();
		RegisterEvents();
	}

	private void Init()
	{
		IsRotatedToRight = true;
	}

	private void RegisterEvents()
	{
		SingletonController<InputController>.Instance.OnPlayerAimHandler += InputController_OnPlayerAimHandler;
		SingletonController<InputController>.Instance.OnPlayerDashHandler += InputController_OnPlayerDashHandler;
		SingletonController<InputController>.Instance.OnPlayerMovementHandler += InputController_OnPlayerMovementHandler;
		SingletonController<InputController>.Instance.OnPlayerMouseAimHandler += InputController_OnPlayerMouseAimHandler;
	}

	private void InputController_OnPlayerMovementHandler(object sender, PlayerMovementEventArgs e)
	{
		if (!SingletonController<TutorialController>.Instance.IsRunningTutorial)
		{
			_movementInput = e.PlayerMovement;
		}
	}

	private void InputController_OnPlayerDashHandler(object sender, EventArgs e)
	{
		if (!SingletonController<TutorialController>.Instance.IsRunningTutorial)
		{
			_playerDash.Dash(_movementInput);
		}
	}

	private void InputController_OnPlayerMouseAimHandler(object sender, MousePositionEventArgs e)
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(e.MousePosition) - base.transform.position;
		UpdatePlayerAim(vector);
	}

	private void InputController_OnPlayerAimHandler(object sender, PlayerAimEventArgs e)
	{
		if (!(e.PlayerAim.normalized - e.PlayerAim != Vector2.zero))
		{
			UpdatePlayerAim(e.PlayerAim);
		}
	}

	private void UpdatePlayerAim(Vector2 aimInput)
	{
		_aimInput = aimInput.normalized;
		this.OnPlayerAimUpdated?.Invoke(_aimInput);
	}

	private void RegisterComponents()
	{
		_player = GetComponent<Player>();
		_playerDash = GetComponent<PlayerDash>();
		_rigidBody = GetComponent<Rigidbody2D>();
	}

	private void ApplyMovement()
	{
		if (!_playerDash.IsDashing && !IsBeingKnockedback && !SingletonController<TutorialController>.Instance.IsRunningTutorial)
		{
			Vector2 velocity = _movementInput * _moveSpeed * Time.deltaTime;
			_rigidBody.velocity = velocity;
		}
	}

	private void HandleRotation()
	{
		if (_isAutoRunning || _movementInput.x != 0f)
		{
			int num = ((_movementInput.x > 0f) ? 1 : (-1));
			int num2 = (IsRotatedToRight ? 1 : (-1));
			if (num != num2)
			{
				this.OnPlayerRotationUpdated?.Invoke(num);
				IsRotatedToRight = (float)num > 0f;
			}
		}
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnPlayerAimHandler -= InputController_OnPlayerAimHandler;
		SingletonController<InputController>.Instance.OnPlayerDashHandler -= InputController_OnPlayerDashHandler;
		SingletonController<InputController>.Instance.OnPlayerMovementHandler -= InputController_OnPlayerMovementHandler;
		SingletonController<InputController>.Instance.OnPlayerMouseAimHandler -= InputController_OnPlayerMouseAimHandler;
	}

	internal void SetMovementSpeed(float speed)
	{
		_moveSpeed = Mathf.Clamp(speed, 0f, 10000f);
	}

	internal void StopAllMovement()
	{
		_isAutoRunning = false;
		_movementInput = Vector2.zero;
		_rigidBody.simulated = false;
	}

	internal void ContinueMovement()
	{
		_rigidBody.simulated = true;
	}

	internal void StopMoveToPosition()
	{
		_isAutoRunning = false;
		_movementInput = Vector2.zero;
		StopAllCoroutines();
	}

	internal void LockPosition()
	{
		_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	internal void UnlockPosition()
	{
		_rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	internal void MoveToPosition(Transform targetPosition)
	{
		StartCoroutine(StartPlayerMovementToTransitionPoint(targetPosition));
	}

	internal void MoveToPositionInstant(Transform targetPosition)
	{
		_movementInput = Vector2.zero;
		_rigidBody.MovePosition(targetPosition.position);
	}

	private IEnumerator StartPlayerMovementToTransitionPoint(Transform targetPosition)
	{
		_isAutoRunning = true;
		if (targetPosition.position.x > _player.transform.position.x)
		{
			_movementInput = new Vector2(1f, 1f);
		}
		else
		{
			_movementInput = new Vector2(-1f, 1f);
		}
		while (_isAutoRunning && _player.transform.position.x != targetPosition.position.x)
		{
			Vector2 position = Vector2.MoveTowards(_player.transform.position, targetPosition.position, _moveSpeed / 30f * Time.deltaTime);
			_rigidBody.MovePosition(position);
			yield return null;
		}
		while (_isAutoRunning && _player.transform.position.y != targetPosition.position.y)
		{
			Vector2 position2 = Vector2.MoveTowards(_player.transform.position, targetPosition.position, _moveSpeed / 30f * Time.deltaTime);
			_rigidBody.MovePosition(position2);
			yield return null;
		}
		_isAutoRunning = false;
		_movementInput = Vector2.zero;
	}
}
