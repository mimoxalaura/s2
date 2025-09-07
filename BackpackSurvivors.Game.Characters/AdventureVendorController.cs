using System.Collections;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Characters;

public class AdventureVendorController : MonoBehaviour
{
	[Header("Base")]
	[SerializeField]
	private int _numberOfKillsNeededForVendorSpawn = 50;

	[SerializeField]
	private int _numberOfKillsNeededForFurtherVendorSpawn = 10;

	[SerializeField]
	private int _enemiesKilled;

	[Header("Special")]
	[SerializeField]
	private bool _allowSpawning = true;

	[SerializeField]
	private int _spawnCount = 999;

	[SerializeField]
	private Transform _forcedSpawnLocation;

	[SerializeField]
	private bool _spawnOnStartLevel;

	[Header("Visuals")]
	[SerializeField]
	private AdventureVendor _adventureVendor;

	[SerializeField]
	private Sprite _icon;

	[Header("Icon")]
	[SerializeField]
	private Image _vendorIcon;

	[SerializeField]
	[FormerlySerializedAs("_text")]
	private TextMeshProUGUI _percentageOfKillsDoneForVendorSpawn;

	[SerializeField]
	private Material _defaultMaterial;

	[SerializeField]
	private Material _vendorReadyMaterial;

	private AdventureVendor _activeVendor;

	private bool _vendorSpawned;

	private int _numberOfVendorsSpawned;

	private bool _vendorDisabledBecauseOfBossLevel;

	private TimeBasedLevelController _levelController;

	private float _scaleEffect = 1.3f;

	private float _scaleUpDuration = 0.1f;

	private float _scaleDownDuration = 0.2f;

	private int _activeSpawnCount;

	private bool _firstSpawnCompleted;

	private void Start()
	{
		InitTargetKillText();
		_levelController = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
		if (_levelController != null)
		{
			SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(_levelController, RegisterLevelController);
		}
		if (_spawnOnStartLevel && !_firstSpawnCompleted)
		{
			SpawnVendor();
			_firstSpawnCompleted = true;
		}
	}

	private void SetVendorEnabled(bool enabled)
	{
		_vendorIcon.gameObject.SetActive(enabled);
	}

	private void RegisterLevelController()
	{
		SetVendorEnabledStatus();
		_levelController.OnBossSpawned += LevelController_OnBossSpawned;
	}

	private void LevelController_OnBossSpawned()
	{
		SetVendorEnabled(enabled: false);
	}

	private void SetVendorEnabledStatus()
	{
		if (_levelController.IsBossLevel)
		{
			SetVendorEnabled(enabled: false);
			_vendorDisabledBecauseOfBossLevel = true;
		}
		else
		{
			SetVendorEnabled(enabled: true);
		}
	}

	private void InitTargetKillText()
	{
		CalculateNumberOfEnemiesToKillForNextVendor();
		UpdatePercentageOfKillsDoneText();
	}

	private void UpdatePercentageOfKillsDoneText()
	{
		int num = _enemiesKilled * 100 / _numberOfKillsNeededForVendorSpawn;
		_percentageOfKillsDoneForVendorSpawn.SetText($"{num}%");
	}

	private void CalculateNumberOfEnemiesToKillForNextVendor()
	{
		_numberOfKillsNeededForVendorSpawn += _numberOfKillsNeededForFurtherVendorSpawn * _numberOfVendorsSpawned;
	}

	[Command("vendor.spawn", Platform.AllPlatforms, MonoTargetType.Single)]
	public void SpawnVendor()
	{
		if (_allowSpawning && !VendorIsActive() && _activeSpawnCount <= _spawnCount && !_levelController.BossSpawned)
		{
			_activeSpawnCount++;
			_allowSpawning = _activeSpawnCount <= _spawnCount;
			DespawnVendor();
			Vector2 zero = Vector2.zero;
			AdventureVendor adventureVendor = Object.Instantiate(position: (!(_forcedSpawnLocation == null)) ? ((Vector2)_forcedSpawnLocation.position) : SingletonController<EnemyController>.Instance.GetRandomSpawnPosition(), original: _adventureVendor, rotation: base.transform.rotation);
			adventureVendor.Spawn();
			_vendorIcon.material = _vendorReadyMaterial;
			StartCoroutine(FinishSpawning(adventureVendor));
		}
	}

	private IEnumerator FinishSpawning(AdventureVendor adventureVendor)
	{
		yield return new WaitForSeconds(1f);
		_activeVendor = adventureVendor;
		TargetArrow arrow = SingletonCacheController.Instance.GetControllerByType<TargetArrowController>().SpawnArrow(adventureVendor.gameObject, _icon, SingletonController<GameDatabase>.Instance.GameDatabaseSO.DefaultArrowIcon);
		adventureVendor.SetArrow(arrow);
	}

	public void DespawnVendor()
	{
		if (_activeVendor != null)
		{
			_activeVendor.Despawn();
			_vendorSpawned = false;
			_vendorIcon.material = _defaultMaterial;
			InitTargetKillText();
		}
	}

	public void ToggleVendorMarker(bool active)
	{
		_vendorIcon.gameObject.SetActive(active);
	}

	public bool VendorIsActive()
	{
		return _activeVendor != null;
	}

	internal void AddEnemyKilled()
	{
		if (!_vendorDisabledBecauseOfBossLevel)
		{
			if (!_vendorSpawned)
			{
				_enemiesKilled++;
				UpdatePercentageOfKillsDoneText();
				AnimatePercentageOfKillsDoneText();
			}
			if (_enemiesKilled >= _numberOfKillsNeededForVendorSpawn)
			{
				_percentageOfKillsDoneForVendorSpawn.SetText("Spawned!");
				_vendorSpawned = true;
				_enemiesKilled = 0;
				_numberOfVendorsSpawned++;
				CalculateNumberOfEnemiesToKillForNextVendor();
				SpawnVendor();
			}
		}
	}

	private void AnimatePercentageOfKillsDoneText()
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.scale(_percentageOfKillsDoneForVendorSpawn.gameObject, new Vector3(_scaleEffect, _scaleEffect, _scaleEffect), _scaleUpDuration);
		LeanTween.scale(_percentageOfKillsDoneForVendorSpawn.gameObject, new Vector3(1f, 1f, 1f), _scaleDownDuration).setDelay(_scaleUpDuration);
	}

	private void OnDestroy()
	{
		_levelController.OnBossSpawned -= LevelController_OnBossSpawned;
	}
}
