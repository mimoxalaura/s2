using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Combat.Weapons.ExternalStats;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Enemies.ExternalStats;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Scenes;
using BackpackSurvivors.System.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace BackpackSurvivors.Game.Input;

public class InputController : SingletonController<InputController>
{
	internal delegate void PlayerMovementHandler(object sender, PlayerMovementEventArgs e);

	internal delegate void PlayerDashHandler(object sender, EventArgs e);

	internal delegate void PlayerAimHandler(object sender, PlayerAimEventArgs e);

	internal delegate void PlayerMouseAimHandler(object sender, MousePositionEventArgs e);

	internal delegate void CursorMovementHandler(object sender, CursorPositionEventArgs e);

	internal delegate void SubmitHandler(object sender, SubmitEventArgs e);

	internal delegate void CancelHandler(object sender, EventArgs e);

	internal delegate void RotateHandler(object sender, RotationEventArgs e);

	internal delegate void UseHandler(object sender, EventArgs e);

	internal delegate void AltHandler(object sender, AltEventArgs e);

	internal delegate void ShiftHandler(object sender, ShiftEventArgs e);

	internal delegate void NextHandler(object sender, EventArgs e);

	internal delegate void RightClickHandler(object sender, RightClickEventArgs e);

	internal delegate void PreviousHandler(object sender, EventArgs e);

	internal delegate void UpHandler(object sender, EventArgs e);

	internal delegate void DownHandler(object sender, EventArgs e);

	internal delegate void AcceptHandler(object sender, EventArgs e);

	internal delegate void PauseHandler(object sender, EventArgs e);

	internal delegate void Special1Handler(object sender, EventArgs e);

	internal delegate void ToggleMergingAllowedHandler(object sender, EventArgs e);

	internal delegate void ControlSchemeChangedHandler(object sender, ControlSchemeChangedEventArgs e);

	internal delegate void InputMapSwitchedChangedHandler(object sender, InputMapSwitchedEventArgs e);

	[SerializeField]
	private CustomCursor _customCursor;

	[SerializeField]
	private EventSystem _eventSystem;

	private const string InGameMapName = "Player";

	private const string UiMapName = "UI";

	private const float DefaultCooldown = 0.2f;

	public bool AltIsDown;

	public bool ShiftIsDown;

	public Vector2 ControllerAimVector;

	internal bool CanCancel = true;

	internal bool CanSpecial1 = true;

	private PlayerInput _playerInput;

	private Vector3 _cursorMovement;

	private Vector3 _mouseMovement;

	private bool _cursorMoved;

	private bool _cursorEnabled = true;

	private float _currentRotationCooldown;

	private float _currentCancelCooldown;

	private Coroutine _rotationCooldownCoroutine;

	private Coroutine _cancelCooldownCoroutine;

	private Stack<Enums.InputType> _inputMapTypeStack = new Stack<Enums.InputType>();

	private string _currentControlScheme = "Keyboard&Mouse";

	private VirtualMouseInput _virtualMouseInput;

	private InputActionProperty _leftMouseButtonInputAction;

	internal Vector2 CursorPosition => _customCursor.transform.position;

	internal Vector2 CursorImageSize => _customCursor.CursorImageSize;

	internal bool CurrentControlschemeIsKeyboard => _currentControlScheme.Equals("Keyboard&Mouse");

	internal bool IsInPlayerInputMap => GetPlayerInput().currentActionMap.name == "Player";

	internal bool CursorEnabled => _cursorEnabled;

	internal event PlayerMovementHandler OnPlayerMovementHandler;

	internal event PlayerDashHandler OnPlayerDashHandler;

	internal event PlayerAimHandler OnPlayerAimHandler;

	internal event PlayerMouseAimHandler OnPlayerMouseAimHandler;

	internal event CursorMovementHandler OnCursorMovementHandler;

	internal event SubmitHandler OnSubmitHandler;

	internal event RightClickHandler OnRightClickHandler;

	internal event CancelHandler OnCancelHandler;

	internal event RotateHandler OnRotateHandler;

	internal event UseHandler OnUseHandler;

	internal event AltHandler OnAltHandler;

	internal event ShiftHandler OnShiftHandler;

	internal event NextHandler OnNextHandler;

	internal event PreviousHandler OnPreviousHandler;

	internal event UpHandler OnUpHandler;

	internal event DownHandler OnDownHandler;

	internal event AcceptHandler OnAcceptHandler;

	internal event PauseHandler OnPauseHandler;

	internal event Special1Handler OnSpecial1Handler;

	internal event ToggleMergingAllowedHandler OnToggleMergingAllowedHandler;

	internal event InputMapSwitchedChangedHandler OnInputModeSwitched;

	internal event ControlSchemeChangedHandler OnControlSchemeChanged;

	private void Start()
	{
		RegisterComponents();
		HideActualMouseCursor();
		base.IsInitialized = true;
		StartCoroutine(HandleInputControlschemeChanged());
	}

	private void Update()
	{
		UpdateRotationCooldown();
		HandleMouseMovement();
		HandleCursorMovementByStick();
		BroadcastNewCursorPosition();
	}

	private IEnumerator HandleInputControlschemeChanged()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (!_playerInput.currentControlScheme.Equals(_currentControlScheme))
			{
				_currentControlScheme = _playerInput.currentControlScheme;
				this.OnControlSchemeChanged?.Invoke(this, new ControlSchemeChangedEventArgs(_currentControlScheme));
			}
		}
	}

	public void SwitchToIngameActionMap(bool storeCurrentInputMap = true, bool unpause = true)
	{
		if (storeCurrentInputMap)
		{
			StoreCurrentInputMap();
		}
		GetPlayerInput().SwitchCurrentActionMap("Player");
		GetPlayerInput().defaultActionMap = "Player";
		_customCursor.SetCursorVisible(visible: false);
		if (unpause)
		{
			SingletonController<GameController>.Instance.SetGamePaused(gamePaused: false);
		}
		this.OnInputModeSwitched?.Invoke(this, new InputMapSwitchedEventArgs(isInPlayerInputMap: true));
	}

	internal void SwitchToUIActionMap(bool storeCurrentInputMap = true, bool shouldPause = true)
	{
		if (storeCurrentInputMap)
		{
			StoreCurrentInputMap();
		}
		GetPlayerInput().SwitchCurrentActionMap("UI");
		GetPlayerInput().defaultActionMap = "UI";
		if (IsInPlayerInputMap)
		{
			SetCursorToCenterOfScreen();
		}
		_customCursor.SetCursorVisible(visible: true);
		bool sceneShouldPauseFromSceneInfo = GetSceneShouldPauseFromSceneInfo();
		if (shouldPause && sceneShouldPauseFromSceneInfo)
		{
			SingletonController<GameController>.Instance.SetGamePaused(gamePaused: true);
		}
		this.OnInputModeSwitched?.Invoke(this, new InputMapSwitchedEventArgs(isInPlayerInputMap: false));
	}

	internal void SetCursorEnabled(bool enabled)
	{
		_customCursor.SetCursorVisible(enabled);
		_cursorEnabled = enabled;
		if (enabled)
		{
			SetCursorToCenterOfScreen();
		}
	}

	private void SetCursorToCenterOfScreen()
	{
		int num = Screen.width / 2;
		int num2 = Screen.height / 2;
		Vector2 vector = new Vector2(num, num2);
		_customCursor.transform.position = vector;
		_virtualMouseInput.cursorTransform.position = vector;
	}

	private bool GetSceneShouldPauseFromSceneInfo()
	{
		SceneInfo controllerByType = SingletonCacheController.Instance.GetControllerByType<SceneInfo>();
		if (controllerByType == null)
		{
			return false;
		}
		return controllerByType.ShouldPauseOnSwitchToUIInputMap;
	}

	internal void RevertToPreviousActionMap(bool unpause = true)
	{
		if (_inputMapTypeStack.Count == 0)
		{
			Debug.LogWarning("Trying to revert to previous action map without a previous action map stored");
			SceneInfo controllerByType = SingletonCacheController.Instance.GetControllerByType<SceneInfo>();
			if (!(controllerByType == null) && (controllerByType.SceneType == Enums.SceneType.Town || controllerByType.SceneType == Enums.SceneType.FinalShop))
			{
				SwitchToIngameActionMap();
			}
			return;
		}
		Enums.InputType inputType = _inputMapTypeStack.Pop();
		switch (inputType)
		{
		case Enums.InputType.Ingame:
			SwitchToIngameActionMap(storeCurrentInputMap: false, unpause);
			break;
		case Enums.InputType.UI:
			SwitchToUIActionMap(storeCurrentInputMap: false);
			break;
		default:
			Debug.LogWarning(string.Format("InputMapType {0} is not handled in {1}.{2}", inputType, "InputController", "RevertToPreviousActionMap"));
			break;
		}
		if (_inputMapTypeStack.Count == 0)
		{
			_inputMapTypeStack.Push(inputType);
		}
	}

	public override void Clear()
	{
		_inputMapTypeStack.Clear();
	}

	public override void ClearAdventure()
	{
		_inputMapTypeStack.Clear();
	}

	internal void SetInputEnabled(bool enabled)
	{
		if (enabled)
		{
			GetPlayerInput().ActivateInput();
		}
		else
		{
			GetPlayerInput().DeactivateInput();
		}
	}

	internal void SetCanvasGroupRaycastBlocking(bool shouldBlock)
	{
		BaseDraggable[] array = UnityEngine.Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetCanvasGroupRaycastBlocking(shouldBlock);
		}
	}

	public override void AfterBaseAwake()
	{
		ActivateEventSystem();
	}

	private void StoreCurrentInputMap()
	{
		string text = GetPlayerInput().currentActionMap.name;
		if (!(text == "Player"))
		{
			if (text == "UI")
			{
				_inputMapTypeStack.Push(Enums.InputType.UI);
			}
			else
			{
				Debug.LogWarning("Unexpected action map name for input: " + text);
			}
		}
		else
		{
			_inputMapTypeStack.Push(Enums.InputType.Ingame);
		}
	}

	private void ActivateEventSystem()
	{
		_eventSystem.gameObject.SetActive(value: true);
	}

	private void UpdateRotationCooldown()
	{
		if (!(_currentRotationCooldown <= 0f))
		{
			_currentRotationCooldown -= 0.1f;
		}
	}

	private void HandleMouseMovement()
	{
		if (!(_mouseMovement == Vector3.zero) && _cursorEnabled)
		{
			_customCursor.transform.position = UnityEngine.Input.mousePosition;
			_cursorMoved = true;
			_mouseMovement = Vector3.zero;
		}
	}

	private void HandleCursorMovementByStick()
	{
		if (!(_cursorMovement == Vector3.zero) && _cursorEnabled)
		{
			_cursorMoved = true;
		}
	}

	private void BroadcastNewCursorPosition()
	{
		if (_cursorMoved)
		{
			CursorPositionEventArgs e = new CursorPositionEventArgs(_customCursor.transform.position);
			this.OnCursorMovementHandler?.Invoke(this, e);
			_cursorMoved = false;
		}
	}

	private void RegisterComponents()
	{
		_playerInput = GetComponent<PlayerInput>();
		_virtualMouseInput = GetComponentInChildren<VirtualMouseInput>();
	}

	private PlayerInput GetPlayerInput()
	{
		if (_playerInput == null)
		{
			_playerInput = GetComponent<PlayerInput>();
		}
		return _playerInput;
	}

	private void HideActualMouseCursor()
	{
		Cursor.visible = false;
	}

	private void OnMove(InputValue input)
	{
		Vector2 playerMovement = input.Get<Vector2>();
		this.OnPlayerMovementHandler?.Invoke(this, new PlayerMovementEventArgs(playerMovement));
	}

	private void OnDash(InputValue input)
	{
		this.OnPlayerDashHandler?.Invoke(this, null);
	}

	private void OnAim(InputValue input)
	{
		Vector2 playerAim = input.Get<Vector2>();
		if (!(playerAim.magnitude < 0.1f))
		{
			ControllerAimVector = playerAim.normalized;
			this.OnPlayerAimHandler?.Invoke(this, new PlayerAimEventArgs(playerAim));
		}
	}

	private void OnMouseAim(InputValue input)
	{
		Vector2 mousePosition = input.Get<Vector2>();
		this.OnPlayerMouseAimHandler?.Invoke(this, new MousePositionEventArgs(mousePosition));
	}

	private void OnUse(InputValue input)
	{
		this.OnUseHandler?.Invoke(this, new EventArgs());
	}

	private void OnAlt(InputValue input)
	{
		ShiftIsDown = input.isPressed;
		this.OnAltHandler?.Invoke(this, new AltEventArgs(input.isPressed));
	}

	private void OnShift(InputValue input)
	{
		ShiftIsDown = input.isPressed;
		this.OnShiftHandler?.Invoke(this, new ShiftEventArgs(input.isPressed));
	}

	private void OnNext(InputValue input)
	{
		this.OnNextHandler?.Invoke(this, new EventArgs());
	}

	private void OnPrevious(InputValue input)
	{
		this.OnPreviousHandler?.Invoke(this, new EventArgs());
	}

	private void OnUp(InputValue input)
	{
		this.OnUpHandler?.Invoke(this, new EventArgs());
	}

	private void OnDown(InputValue input)
	{
		this.OnDownHandler?.Invoke(this, new EventArgs());
	}

	private void OnAccept(InputValue input)
	{
		this.OnAcceptHandler?.Invoke(this, null);
	}

	private void OnPause(InputValue input)
	{
		this.OnPauseHandler?.Invoke(this, null);
	}

	private void OnCHEAT_KillOneEnemy()
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			SingletonCacheController.Instance.GetControllerByType<EnemyController>().CHEAT_KillOneEnemy();
		}
	}

	private void OnCHEAT_KillAllEnemies()
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			SingletonCacheController.Instance.GetControllerByType<EnemyController>().CHEAT_KillAllEnemies();
		}
	}

	private void OnCHEAT_FinishRemainingWaves()
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			OnCHEAT_KillAllEnemies();
		}
	}

	private void OnCHEAT_FinishLevel()
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>().CHEAT_FinishLevel();
			OnCHEAT_KillAllEnemies();
		}
	}

	private void OnCHEAT_MuteMusic()
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			SingletonController<SettingsController>.Instance.AudioSettingsController.UpdateMusicVolume(0f);
		}
	}

	private void OnCHEAT_ReloadExternalStats()
	{
		if (SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			ExternalWeaponStats.ReloadStats();
			ExternalEnemyStats.ReloadStats();
		}
	}

	private void OnClick(InputValue input)
	{
		if (_cursorEnabled)
		{
			this.OnSubmitHandler?.Invoke(this, new SubmitEventArgs(input.isPressed));
		}
	}

	private void OnRightClick(InputValue input)
	{
		if (_cursorEnabled)
		{
			this.OnRightClickHandler?.Invoke(this, new RightClickEventArgs(input.isPressed));
		}
	}

	private void OnScrollWheel(InputValue input)
	{
		if (_cursorEnabled && !(_currentRotationCooldown > 0f))
		{
			_currentRotationCooldown = 0.2f;
			if (_rotationCooldownCoroutine != null)
			{
				StopCoroutine(_rotationCooldownCoroutine);
			}
			_rotationCooldownCoroutine = StartCoroutine(StartRotationCooldown());
			Vector2 vector = (Vector2)input.Get();
			this.OnRotateHandler?.Invoke(this, new RotationEventArgs(vector.y > 0f));
		}
	}

	private IEnumerator StartRotationCooldown()
	{
		yield return new WaitForSecondsRealtime(_currentRotationCooldown);
		_currentRotationCooldown = 0f;
	}

	private void OnRotate(InputValue input)
	{
		if (_cursorEnabled)
		{
			this.OnRotateHandler?.Invoke(this, new RotationEventArgs(clockwise: true));
		}
	}

	private void OnRotateCounterclockwise(InputValue input)
	{
		if (_cursorEnabled)
		{
			this.OnRotateHandler?.Invoke(this, new RotationEventArgs(clockwise: false));
		}
	}

	private void OnFakeMouseMovement(InputValue input)
	{
		if (_cursorEnabled)
		{
			Vector2 vector = input.Get<Vector2>();
			_cursorMovement = vector;
		}
	}

	private void OnMouseMovement(InputValue input)
	{
		if (_cursorEnabled)
		{
			Vector2 vector = input.Get<Vector2>();
			_mouseMovement = vector;
		}
	}

	private void OnCancel(InputValue input)
	{
		if (_cursorEnabled && CanCancel)
		{
			this.OnCancelHandler?.Invoke(this, null);
		}
	}

	private void OnToggleMergingAllowed(InputValue input)
	{
		if (_cursorEnabled)
		{
			this.OnToggleMergingAllowedHandler?.Invoke(this, null);
		}
	}

	private void OnSpecial1(InputValue input)
	{
		if (_cursorEnabled && CanSpecial1)
		{
			this.OnSpecial1Handler?.Invoke(this, null);
		}
	}

	private void PlayerInput_OnControlsChanged(PlayerInput playerInput)
	{
		this.OnControlSchemeChanged?.Invoke(this, new ControlSchemeChangedEventArgs(playerInput.currentControlScheme));
	}

	private void OnSwitchInputMap(InputValue input)
	{
		string text = GetPlayerInput().currentActionMap.name;
		if (!(text == "Player"))
		{
			if (text == "UI")
			{
				SwitchToIngameActionMap(storeCurrentInputMap: false);
			}
		}
		else
		{
			SwitchToUIActionMap(storeCurrentInputMap: false, shouldPause: false);
		}
	}
}
