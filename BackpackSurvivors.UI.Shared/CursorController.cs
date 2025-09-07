using System;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

public class CursorController : SingletonController<CursorController>
{
	[Header("Base")]
	[SerializeField]
	private Enums.CursorState _cursorDefaultState;

	[Header("Textures")]
	[SerializeField]
	private Texture2D _cursorDefault;

	[SerializeField]
	private Texture2D _cursorGrabbing;

	[SerializeField]
	private Texture2D _cursorCannotDoAction;

	[SerializeField]
	private Texture2D _cursorCanActivate;

	[SerializeField]
	private Texture2D _cursorCanDeactivate;

	[Header("Follower")]
	[SerializeField]
	private bool _enableFollower;

	[SerializeField]
	private CursorFollower _cursorFollowerPrefab;

	[SerializeField]
	private bool _enableParticles;

	[SerializeField]
	private bool _enableLight;

	private CursorFollower _cursorFollower;

	private Enums.CursorState _activeCursorState;

	private void Start()
	{
		EnableCursorFollower();
		TriggerCursorState(_cursorDefaultState);
		base.IsInitialized = true;
	}

	private void EnableCursorFollower()
	{
		if (_enableFollower)
		{
			_cursorFollower = UnityEngine.Object.FindAnyObjectByType<CursorFollower>();
			if (_cursorFollower == null)
			{
				_cursorFollower = UnityEngine.Object.Instantiate(_cursorFollowerPrefab, new Vector3(0f, 0f, 0f), default(Quaternion));
			}
			_cursorFollower.ToggleParticles(_enableParticles);
			_cursorFollower.ToggleLight(_enableLight);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (_activeCursorState == Enums.CursorState.Default)
			{
				TriggerCursorState(Enums.CursorState.Grabbing);
			}
		}
		else if (_activeCursorState == Enums.CursorState.Grabbing)
		{
			TriggerCursorState(Enums.CursorState.Default);
		}
	}

	public void TriggerDefaultCursorState()
	{
		TriggerCursorState(_cursorDefaultState);
	}

	public void TriggerCursorState(Enums.CursorState cursorState)
	{
		_activeCursorState = cursorState;
		switch (cursorState)
		{
		case Enums.CursorState.Default:
			SetCursor(_cursorDefault);
			break;
		case Enums.CursorState.Grabbing:
			SetCursor(_cursorGrabbing);
			break;
		case Enums.CursorState.CannotDoAction:
			SetCursor(_cursorCannotDoAction);
			break;
		case Enums.CursorState.CanActivate:
			SetCursor(_cursorCanActivate);
			break;
		case Enums.CursorState.CanDeactivate:
			SetCursor(_cursorCanDeactivate);
			break;
		default:
			throw new NotImplementedException("TriggerCursorState is not known - expand enum switch!");
		}
	}

	public void ToggleParticles(bool showParticles)
	{
		_cursorFollower.ToggleParticles(_enableParticles);
	}

	internal void ToggleLight(bool enableLight)
	{
		_cursorFollower.ToggleLight(_enableLight);
	}

	private void SetCursor(Texture2D texture)
	{
		Cursor.SetCursor(texture, new Vector2(0f, 0f), CursorMode.Auto);
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
