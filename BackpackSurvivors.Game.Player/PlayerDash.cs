using System;
using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Player.Events;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.Game.World;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Scenes;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
	public delegate void DashCooldownUpdatedHandler(object sender, DashCooldownEventArgs e);

	public delegate void DashedHandler(object sender, DashCooldownEventArgs e);

	public delegate void DashCountChangedHandler(object sender, DashCooldownEventArgs e);

	public delegate void DashReadyHandler(object sender, DashCooldownEventArgs e);

	[SerializeField]
	private float _dashDistance;

	[SerializeField]
	private float _dashDuration;

	[SerializeField]
	private float _dashCooldown;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, PlayerAnimatorController> _playerAnimatorController;

	[SerializeField]
	private AudioClip[] _dashAudio;

	[Header("VFX")]
	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, TrailRenderer> _dashTrail;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, Transform> _dashVfxSpawnPoint;

	[SerializeField]
	private SerializableDictionaryBase<Enums.CharacterClass, PlayerDashVisualPrefab> _dashVfxPrefab;

	private Rigidbody2D _rigidbody2D;

	private Player _player;

	private int _totalDashes;

	private int _currentDashes;

	private Vector2 _dashDirection;

	private float _currentCooldown;

	private const float _endDashWaitTime = 0.3f;

	private bool _canDash = true;

	private bool _shouldCheckForInteraction;

	public int TotalDashes => _totalDashes;

	public int CurrentDashes => _currentDashes;

	public bool IsDashing { get; internal set; }

	public event DashCooldownUpdatedHandler OnDashCooldownUpdated;

	public event DashedHandler OnDashed;

	public event DashReadyHandler OnDashReady;

	public event DashCountChangedHandler OnDashCountChanged;

	public event EventHandler OnDashesCountSet;

	private void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_player = GetComponent<Player>();
		_totalDashes = 1;
		_currentDashes = _totalDashes;
		_currentCooldown = _dashCooldown;
	}

	private void Start()
	{
		UpdateTotalDashCount();
		SetShouldCheckForInteraction();
	}

	private void SetShouldCheckForInteraction()
	{
		SceneInfo controllerByType = SingletonCacheController.Instance.GetControllerByType<SceneInfo>();
		if (controllerByType == null)
		{
			_shouldCheckForInteraction = false;
		}
		else
		{
			_shouldCheckForInteraction = controllerByType.SceneType == Enums.SceneType.Town;
		}
	}

	private void Update()
	{
		DecreaseCooldown();
	}

	internal void ResetCooldown()
	{
		_currentCooldown = 0f;
	}

	internal void SetCanDash(bool canDash)
	{
		_canDash = canDash;
	}

	internal void UpdateTotalDashCount()
	{
		int totalDashes = _totalDashes;
		_totalDashes = (int)SingletonController<GameController>.Instance.Player.CalculatedStats.TryGet(Enums.ItemStatType.ExtraDash, 0f);
		_currentDashes = _totalDashes;
		if (totalDashes != _totalDashes)
		{
			this.OnDashCountChanged?.Invoke(this, new DashCooldownEventArgs(_currentCooldown, _dashCooldown, _currentDashes, _totalDashes));
		}
		this.OnDashesCountSet?.Invoke(this, new EventArgs());
	}

	private void DecreaseCooldown()
	{
		if (_currentDashes < _totalDashes)
		{
			if (_currentCooldown <= 0f)
			{
				_currentDashes++;
				this.OnDashReady?.Invoke(this, new DashCooldownEventArgs(_currentCooldown, _dashCooldown, _currentDashes, _totalDashes));
				_currentCooldown = _dashCooldown;
			}
			else
			{
				_currentCooldown -= Time.deltaTime;
				DashCooldownEventArgs e = new DashCooldownEventArgs(_currentCooldown, _dashCooldown, _currentDashes, _totalDashes);
				this.OnDashCooldownUpdated?.Invoke(this, e);
			}
		}
	}

	public void Dash(Vector2 dashDirection)
	{
		if (_currentDashes > 0 && _player.CanAct && _canDash && !ShouldInteractInsteadOfDashing())
		{
			_currentDashes--;
			SingletonController<GameController>.Instance.Player.SetImmunityForDuration(Enums.ImmunitySource.Dash, 0.3f);
			_dashDirection = dashDirection;
			_player.IsAttackingWithVisualWeapon = false;
			_playerAnimatorController[SingletonController<CharactersController>.Instance.ActiveCharacter.Character].ShowDashAnimation();
			StartCoroutine(SpawnDashCopies());
			IsDashing = true;
			_rigidbody2D.AddForce(_dashDirection * _dashDistance, ForceMode2D.Impulse);
			int num = UnityEngine.Random.Range(0, _dashAudio.Length);
			SingletonController<AudioController>.Instance.PlaySFXClip(_dashAudio[num], 0.5f);
			this.OnDashed?.Invoke(this, new DashCooldownEventArgs(_currentCooldown, _dashCooldown, _currentDashes, _totalDashes));
			StartCoroutine(HideDash(0.3f));
		}
	}

	private bool ShouldInteractInsteadOfDashing()
	{
		if (!_shouldCheckForInteraction)
		{
			return false;
		}
		return UnityEngine.Object.FindObjectsOfType<Interaction>().Any((Interaction i) => i.IsInRange);
	}

	private IEnumerator SpawnDashCopies()
	{
		for (int i = 0; i < 4; i++)
		{
			PlayerDashVisualPrefab playerDashVisualPrefab = UnityEngine.Object.Instantiate(_dashVfxPrefab[SingletonController<CharactersController>.Instance.ActiveCharacter.Character], _dashVfxSpawnPoint[SingletonController<CharactersController>.Instance.ActiveCharacter.Character]);
			playerDashVisualPrefab.SetMaterial(_player.GetSpriteRenderer().material);
			playerDashVisualPrefab.transform.SetParent(null);
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator HideDash(float waitDuration)
	{
		yield return new WaitForSeconds(waitDuration);
		_rigidbody2D.velocity = Vector2.zero;
		IsDashing = false;
	}
}
