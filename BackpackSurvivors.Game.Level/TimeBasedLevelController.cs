using System;
using System.Collections;
using System.Linq;
using BackpackSurvivors.Assets.Game.Revive;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Enemies.Minibosses;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level.Events;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shared.Interfaces;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.Game.Waves.Events;
using BackpackSurvivors.Game.World;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.GameplayFeedback;
using BackpackSurvivors.UI.Tooltip;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.Game.Level;

internal class TimeBasedLevelController : MonoBehaviour, IInitializable
{
	internal delegate void LevelCompletedHandler(object sender, EventArgs e);

	internal delegate void LevelStartedHandler(object sender, EventArgs e);

	internal delegate void TimeRemainingUpdatedHandler(object sender, TimeRemainingEventArgs e);

	[SerializeField]
	private int _levelId;

	[SerializeField]
	private Light2D _globalLight;

	[SerializeField]
	private BossHealthFeedback _bossHealthFeedback;

	[SerializeField]
	private AudioClip _levelCompletedMusic;

	private LevelSO _levelSO;

	private TimeBasedWaveController _timeBasedWaveController;

	private WeaponController _weaponController;

	private ShopController _shopController;

	private SpawningPortal _spawningPortal;

	private BackpackSurvivors.Game.Player.Player _player;

	private int _timeSpentInLevel;

	private bool _waveControllerStarted;

	private bool _isPaused;

	private bool _bossSpawned;

	internal int TimeSpentInLevel => _timeSpentInLevel;

	internal bool IsLevelFinished { get; private set; }

	internal bool IsBossLevel => _levelSO.BossLevel;

	internal float LevelExperienceModifier => GetlevelExperienceModifier();

	internal LevelSO CurrentLevel => _levelSO;

	public bool IsInitialized { get; private set; }

	public bool BossSpawned => _bossSpawned;

	internal event LevelCompletedHandler OnLevelCompleted;

	internal event LevelStartedHandler OnLevelStarted;

	internal event TimeRemainingUpdatedHandler OnTimeRemainingInLevelUpdated;

	internal event Action OnBossSpawned;

	private float GetlevelExperienceModifier()
	{
		if (_levelSO == null)
		{
			LoadLevelSO();
		}
		return _levelSO.ExperienceModifier;
	}

	private void Start()
	{
		SingletonController<EnemyController>.Instance.ResetActiveEnemyCount();
		SingletonController<EnemyController>.Instance.RegisterTimeBasedLevelController(this);
		LoadLevelSO();
		PreloadBossMusic();
		RegisterGameObjects();
		RegisterEvents();
		InitFeedback();
		UnlockRarities();
		SetupLevelEffects();
		AfterStart();
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: false);
		StartCoroutine(RunLevelStartupAnimations());
		IsInitialized = true;
	}

	protected virtual void AfterStart()
	{
	}

	private void SetupLevelEffects()
	{
		int activeDifficulty = SingletonController<DifficultyController>.Instance.ActiveDifficulty;
		foreach (AdventureEffectSO adventureEffect in _levelSO.Adventure.AdventureEffects)
		{
			if (adventureEffect.ActiveFromHellfireLevel <= activeDifficulty)
			{
				adventureEffect.AdventureEffectController.InitializeEffect(_timeBasedWaveController, _levelSO);
			}
		}
	}

	private void UnlockRarities()
	{
		foreach (Enums.PlaceableRarity item in _levelSO.ItemQualitiesToUnlock)
		{
			SingletonController<GameController>.Instance.GameState.UnlockItemRarity(item);
		}
	}

	private IEnumerator RunLevelStartupAnimations()
	{
		SingletonController<TooltipController>.Instance.Hide(null);
		if (_spawningPortal != null)
		{
			_spawningPortal.SetInfernoLevel(SingletonController<DifficultyController>.Instance.ActiveDifficulty);
			float animationDuration = _spawningPortal.GetAnimationDuration(SingletonController<DifficultyController>.Instance.ActiveDifficulty);
			yield return new WaitForSeconds(animationDuration);
		}
		yield return new WaitForSeconds(0.5f);
		_player.Spawn();
		yield return new WaitForSeconds(1.8f);
		SingletonController<InputController>.Instance.SwitchToIngameActionMap();
		SingletonController<TooltipController>.Instance.Hide(null);
		if (_timeBasedWaveController != null)
		{
			_timeBasedWaveController.InitLevel(_levelSO);
		}
		StartSpawning();
		_weaponController.ReloadAndStart();
		SingletonCacheController.Instance.GetControllerByType<LevelMusicPlayer>().PlayLevelMusic();
		SingletonController<GameController>.Instance.CanOpenMenu = true;
		SingletonController<GameController>.Instance.CanPauseGame = true;
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: false);
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: true);
		if (_spawningPortal != null)
		{
			_spawningPortal.StopShaking();
		}
		SingletonCacheController.Instance.GetControllerByType<ReviveController>().UpdateReviveCountUI();
		SingletonController<StatController>.Instance.GetAndRecalculateStats(healHealth: true);
		this.OnLevelStarted?.Invoke(this, new EventArgs());
	}

	internal void StopCountdown()
	{
		StopAllCoroutines();
	}

	private IEnumerator CountdownLevel()
	{
		while (_timeSpentInLevel < _levelSO.LevelDuration)
		{
			while (_isPaused)
			{
				yield return null;
			}
			yield return new WaitForSeconds(1f);
			_timeSpentInLevel++;
			int timeRemaining = _levelSO.LevelDuration - _timeSpentInLevel;
			this.OnTimeRemainingInLevelUpdated?.Invoke(this, new TimeRemainingEventArgs(timeRemaining, _levelSO.LevelDuration));
		}
		SpawnLevelBoss();
	}

	internal void AddLevelSpendTime(int timeToAdd)
	{
		_timeSpentInLevel += timeToAdd;
	}

	[Command("level.complete", Platform.AllPlatforms, MonoTargetType.Single)]
	private void FinishLevel(bool levelWon = true)
	{
		StartCoroutine(FinishLevelAsync(levelWon));
	}

	private IEnumerator FinishLevelAsync(bool levelWon = true)
	{
		_timeBasedWaveController.StopSpawning();
		SingletonController<ReservedShopOfferController>.Instance.SaveReservedDraggables();
		IsLevelFinished = true;
		SingletonController<GameController>.Instance.DestroyAllLevelAssets(levelWon);
		StopLevelActivity(levelWon);
		yield return new WaitForSeconds(1f);
		HandleFirstTimeKillRewards();
		while (SingletonController<GameController>.Instance.IsShowingOneTimeRewards)
		{
			yield return new WaitForSeconds(0.5f);
		}
		SingletonController<GameController>.Instance.Player.SwitchToSuccesAnimation();
		StartLevelFinishCountdown(levelWon);
	}

	private void HandleFirstTimeKillRewards()
	{
		if (SingletonController<SaveGameController>.Instance.ActiveSaveGame.StatisticsState.GetEnemyKilledCount(_levelSO.LevelBoss.Id) == 1)
		{
			SingletonCacheController.Instance.GetControllerByType<FirstTimeDropHelper>().HandleDrops();
		}
	}

	private void PreloadBossMusic()
	{
		SingletonController<AudioController>.Instance.PreloadAudioClip(_levelSO.BossCombatMusic);
	}

	private void SpawnLevelBoss()
	{
		if (_levelSO.LevelBoss == null)
		{
			FinishLevel();
			return;
		}
		float enemyHealthMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetEnemyHealthMultiplierFromHellfire(_levelSO);
		float enemyDamageMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetEnemyDamageMultiplierFromHellfire(_levelSO);
		float rewardMultiplierFromHellfire = SingletonController<DifficultyController>.Instance.GetRewardMultiplierFromHellfire(_levelSO);
		PlayBossAudio();
		Enemy enemy = UnityEngine.Object.Instantiate(_levelSO.LevelBoss.EnemyPrefab, base.transform, worldPositionStays: true);
		WaveSO waveSO = _levelSO.Waves.OrderBy((WaveSO x) => x.HealthScaleFactor).Last();
		enemy.ScaleHealth(waveSO.HealthScaleFactor, enemyHealthMultiplierFromHellfire, 1f);
		enemy.ScaleDamage(enemyDamageMultiplierFromHellfire);
		enemy.ScaleMovementSpeed(waveSO.MovementspeedScaleFactor);
		enemy.ScaleLoot(waveSO.LootScaleFactor * rewardMultiplierFromHellfire);
		Vector2 randomSpawnPosition = SingletonController<EnemyController>.Instance.GetRandomSpawnPosition(selectOnlyBossSpawnLocations: true);
		enemy.transform.position = randomSpawnPosition;
		SingletonController<EnemyController>.Instance.AddEnemy(enemy);
		SingletonController<EnemyController>.Instance.SetMaxEnemiesToDuringBossFight();
		_timeBasedWaveController.BossSpawned = true;
		SetupBoss(enemy, showHealthBar: false);
		PlayerMessageController controllerByType = SingletonCacheController.Instance.GetControllerByType<PlayerMessageController>();
		if (controllerByType != null)
		{
			float showDuration = 3f;
			float delay = 1.5f;
			controllerByType.ShowMessage("LEVEL BOSS APPEARED!", SpriteHelper.GetSkullSprite(), null, Enums.PlayerMessageType.Bad, showDuration);
			StartCoroutine(SetupBossHealthBarAfterDelay(enemy, delay));
		}
		else
		{
			Debug.LogWarning("No PlayerMessageController found on scene!");
			SetupBossHealthBar(enemy);
		}
		TargetArrow arrow = SingletonCacheController.Instance.GetControllerByType<TargetArrowController>().SpawnArrow(enemy.gameObject, enemy.BaseEnemy.ArrowSprite, SingletonController<GameDatabase>.Instance.GameDatabaseSO.SpecialBossArrowIcon, 15f);
		enemy.SetArrow(arrow);
		_bossSpawned = true;
		this.OnBossSpawned?.Invoke();
	}

	private IEnumerator SetupBossHealthBarAfterDelay(Enemy enemy, float delay)
	{
		yield return new WaitForSeconds(delay);
		SetupBossHealthBar(enemy);
	}

	internal void SetupBoss(Enemy enemy, bool showHealthBar)
	{
		enemy.OnKilled += Boss_OnCharacterKilled;
		enemy.CanDropLoot = true;
		if (showHealthBar)
		{
			SetupBossHealthBar(enemy);
		}
	}

	internal void SetupBossHealthBar(Enemy enemy)
	{
		_bossHealthFeedback.gameObject.SetActive(value: true);
		_bossHealthFeedback.SetupBossHealth(enemy, _levelSO);
		_bossHealthFeedback.ShowHealthbar();
	}

	private void Boss_OnCharacterKilled(object sender, EventArgs e)
	{
		SingletonController<GameController>.Instance.Player.PreventDamageAndDying();
		FinishLevel();
	}

	private void StartLevelFinishCountdown(bool levelWon)
	{
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: false);
		if (levelWon)
		{
			StartCoroutine(MovePickupsToPlayerAfterDelay());
		}
	}

	private IEnumerator MovePickupsToPlayerAfterDelay()
	{
		yield return new WaitForSecondsRealtime(0.1f);
		SingletonController<GameController>.Instance.MovePickupsToPlayer();
		this.OnLevelCompleted?.Invoke(this, null);
	}

	private void _levelFinishCountdown_OnCountdownCompleted(object sender, EventArgs e)
	{
		this.OnLevelCompleted?.Invoke(this, null);
	}

	internal void StopLevelActivity(bool levelWon)
	{
		SingletonController<BackpackController>.Instance.LogDPSLoggableActiveTime();
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: true, setTimeScale: false);
		SingletonController<GameController>.Instance.Player.StopAllMovement();
		if (levelWon)
		{
			SingletonController<AudioController>.Instance.PlayMusicClip(_levelCompletedMusic);
			SingletonController<EnemyController>.Instance.SetCountEnemiesKilled(countEnemiesKilled: false);
			SingletonController<EnemyController>.Instance.StopAllEnemies();
			SingletonController<EnemyController>.Instance.KillAllEnemies();
			SingletonController<EnemyController>.Instance.SetCountEnemiesKilled(countEnemiesKilled: true);
		}
	}

	private void RegisterEvents()
	{
		_shopController.OnShopClosed += ShopController_OnShopClosed;
		if (_timeBasedWaveController != null)
		{
			_timeBasedWaveController.OnWaveStarted += TimeBasedWaveController_OnWaveStarted;
		}
		SingletonController<GameController>.Instance.OnPauseUpdated += GameController_OnPauseUpdated;
		SingletonController<EnemyController>.Instance.OnEnemyKilled += EnemyController_OnEnemyKilled;
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharactersControllerLoadedEvent);
		RegisterBossEvents();
	}

	private void ShopController_OnShopClosed(object sender, ShopClosedEventArgs e)
	{
		SingletonController<GameController>.Instance.Player.SetImmunityForDuration(Enums.ImmunitySource.LeavingShop, 3f);
		SingletonCacheController.Instance.GetControllerByType<AdventureVendorController>().DespawnVendor();
		_weaponController.ReloadAndStart();
		StartCoroutine(DelayAfterShopClose(1f));
	}

	internal void RegisterCharactersControllerLoadedEvent()
	{
		SingletonController<GameController>.Instance.Player.LoadCharacter(SingletonController<CharactersController>.Instance.ActiveCharacter);
	}

	private void RegisterBossEvents()
	{
		_ = _levelSO.BossLevel;
	}

	private void EnemyController_OnEnemyKilled(object sender, EnemyKilledEventArgs e)
	{
		AdventureVendorController controllerByType = SingletonCacheController.Instance.GetControllerByType<AdventureVendorController>();
		if (controllerByType != null)
		{
			controllerByType.AddEnemyKilled();
		}
	}

	private void InitFeedback()
	{
		if (_levelSO.BossLevel)
		{
			UnityEngine.Object.FindObjectOfType<BossHealthFeedback>(includeInactive: true).Init();
		}
		else
		{
			UnityEngine.Object.FindObjectOfType<TimedLevelProgressFeedback>(includeInactive: true).Init(_levelSO.LevelDuration);
		}
	}

	private void GameController_OnPauseUpdated(bool isPaused)
	{
		_isPaused = isPaused;
	}

	private void TimeBasedWaveController_OnWaveStarted(object sender, WaveStartedEventArgs e)
	{
		StartCoroutine(CountdownLevel());
	}

	private IEnumerator DelayAfterShopClose(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: false, setTimeScale: false);
	}

	private void StartSpawning()
	{
		if (!_waveControllerStarted)
		{
			_waveControllerStarted = true;
			if (_timeBasedWaveController != null)
			{
				_timeBasedWaveController.StartSpawning();
			}
		}
	}

	private void RegisterGameObjects()
	{
		_timeBasedWaveController = SingletonCacheController.Instance.GetControllerByType<TimeBasedWaveController>();
		_weaponController = SingletonCacheController.Instance.GetControllerByType<WeaponController>();
		_shopController = SingletonCacheController.Instance.GetControllerByType<ShopController>();
		_spawningPortal = SingletonCacheController.Instance.GetControllerByType<SpawningPortal>();
		_player = SingletonCacheController.Instance.GetControllerByType<BackpackSurvivors.Game.Player.Player>();
	}

	private void LoadLevelSO()
	{
		_levelSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.Levels.First((LevelSO l) => l.LevelId == _levelId);
	}

	private void OnDestroy()
	{
		_shopController.OnShopClosed -= ShopController_OnShopClosed;
		if (_timeBasedWaveController != null)
		{
			_timeBasedWaveController.OnWaveStarted -= TimeBasedWaveController_OnWaveStarted;
		}
		SingletonController<EnemyController>.Instance.OnEnemyKilled -= EnemyController_OnEnemyKilled;
		SingletonController<GameController>.Instance.OnPauseUpdated -= GameController_OnPauseUpdated;
		SingletonController<EnemyController>.Instance.UnregisterTimeBasedLevelController();
	}

	internal void CHEAT_FinishLevel()
	{
		_timeSpentInLevel = _levelSO.LevelDuration - 1;
	}

	internal void PlayBossAudio(float time = 0f)
	{
		SingletonController<AudioController>.Instance.PlayMusicClip(_levelSO.BossCombatMusic, 1f, loop: true, time);
	}
}
