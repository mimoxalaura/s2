using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

internal class PlayerTargetLine : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _lineRenderer;

	[SerializeField]
	private Transform _sourceLine;

	private void Start()
	{
		SingletonController<InputController>.Instance.OnCursorMovementHandler += Instance_OnCursorMovementHandler;
		SingletonController<InputController>.Instance.OnPlayerMouseAimHandler += Instance_OnPlayerMouseAimHandler;
	}

	private void Instance_OnPlayerMouseAimHandler(object sender, MousePositionEventArgs e)
	{
	}

	private void Instance_OnCursorMovementHandler(object sender, CursorPositionEventArgs e)
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(e.CursorPosition);
		_lineRenderer.SetPosition(0, _sourceLine.position);
		_lineRenderer.SetPosition(1, new Vector3(vector.x, vector.y, 0f));
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnCursorMovementHandler -= Instance_OnCursorMovementHandler;
	}
}
