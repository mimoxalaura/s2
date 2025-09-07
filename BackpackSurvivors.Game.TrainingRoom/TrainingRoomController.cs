using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.TrainingRoom;

internal class TrainingRoomController : MonoBehaviour
{
	[SerializeField]
	private TrainingRoomItemSelectionController _trainingRoomItemSelectionController;

	[SerializeField]
	private TrainingRoomDamageAndDpsController _trainingRoomDamageAndDpsController;

	[SerializeField]
	private Enemy[] _trainingDummies;

	private void Start()
	{
		SingletonController<StatisticsController>.Instance.ClearAdventureStatistics();
		Enemy[] trainingDummies = _trainingDummies;
		foreach (Enemy enemy in trainingDummies)
		{
			SingletonController<EnemyController>.Instance.AddEnemy(enemy);
		}
		SingletonController<WeaponDamageAndDPSController>.Instance.Clear();
		SingletonController<WeaponDamageAndDPSController>.Instance.AddDamageExposer(_trainingRoomDamageAndDpsController);
		SingletonController<GameController>.Instance.Player.Spawn(teleportIn: false);
	}

	public void OpenBackpack()
	{
		_trainingRoomItemSelectionController.OpenUI();
	}

	public void CloseBackpack()
	{
		_trainingRoomItemSelectionController.CloseUI();
		Object.FindObjectOfType<WeaponController>().ReloadAndStart();
	}

	public void ResetEnemies()
	{
		Enemy[] trainingDummies = _trainingDummies;
		for (int i = 0; i < trainingDummies.Length; i++)
		{
			trainingDummies[i].Reset();
		}
	}

	public void ResetMeters()
	{
		_trainingRoomDamageAndDpsController.Reset();
	}

	internal void ReceiveDraggable(BaseDraggable baseDraggable)
	{
		baseDraggable.ChangeOwner(Enums.Backpack.DraggableOwner.Player);
		bool isBag = baseDraggable is DraggableBag;
		Transform newParentTransform = GetNewParentTransform(isBag);
		baseDraggable.transform.SetParent(newParentTransform);
		baseDraggable.Enabled = true;
		baseDraggable.ApplySize();
		_trainingRoomItemSelectionController.RegenerateAll();
	}

	private Transform GetNewParentTransform(bool isBag)
	{
		if (isBag)
		{
			return SingletonController<BackpackController>.Instance.BoughtBagsParentTransform;
		}
		return SingletonController<BackpackController>.Instance.BoughtItemsAndWeaponsParentTransform;
	}
}
