using BackpackSurvivors.Game.Combat.EnemyAttacks;
using BackpackSurvivors.Game.Enemies.Bosses.VoidCorruption;
using BackpackSurvivors.Game.Enemies.Movement;
using BackpackSurvivors.Game.Enemies.Triggers;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

public class VoidCorruption : Enemy
{
	[SerializeField]
	private LeavingDarknessEffect _leavingDarknessEffect;

	[SerializeField]
	private SpawnPrefabAfterTime[] _spawningPrefabAfterTime;

	[SerializeField]
	private Material _defaultMaterial;

	[SerializeField]
	private FollowPlayerAndIncreaseSpeedMovement _followPlayerAndIncreaseSpeedMovement;

	private void Awake()
	{
		GetSpriteRenderer().transform.localScale = new Vector3(0f, 0f, 0f);
		GetSpriteRenderer().material = _defaultMaterial;
		GetSpriteRenderer().color = new Color(255f, 255f, 255f, 0f);
		SetCanAct(canAct: false);
		Initialize();
		_followPlayerAndIncreaseSpeedMovement.ResetSpeed();
	}

	public override void SetCanAct(bool canAct)
	{
		base.SetCanAct(canAct);
		_leavingDarknessEffect.enabled = canAct;
		EnemyWeaponInitializer[] enemyWeaponInitializers = base.EnemyWeaponInitializers;
		for (int i = 0; i < enemyWeaponInitializers.Length; i++)
		{
			enemyWeaponInitializers[i].enabled = canAct;
		}
		for (int j = 0; j < _spawningPrefabAfterTime.Length; j++)
		{
			_spawningPrefabAfterTime[j].Enabled = canAct;
		}
	}

	public override void ResetToDefaultVisualState()
	{
		GetSpriteRenderer().transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
		GetSpriteRenderer().material = _defaultMaterial;
		GetSpriteRenderer().color = new Color(255f, 255f, 255f, 1f);
	}
}
