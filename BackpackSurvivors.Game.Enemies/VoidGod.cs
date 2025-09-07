using BackpackSurvivors.Game.Combat.EnemyAttacks;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

public class VoidGod : Enemy
{
	private void Awake()
	{
		GetSpriteRenderer().transform.localScale = new Vector3(0f, 0f, 0f);
		GetSpriteRenderer().color = new Color(255f, 255f, 255f, 0f);
		SetCanAct(canAct: false);
		Initialize();
	}

	public override void SetCanAct(bool canAct)
	{
		base.SetCanAct(canAct);
		EnemyWeaponInitializer[] enemyWeaponInitializers = base.EnemyWeaponInitializers;
		for (int i = 0; i < enemyWeaponInitializers.Length; i++)
		{
			enemyWeaponInitializers[i].enabled = canAct;
		}
	}

	public override void ResetToDefaultVisualState()
	{
		GetSpriteRenderer().transform.localScale = new Vector3(1f, 1f, 1f);
		GetSpriteRenderer().color = new Color(255f, 255f, 255f, 1f);
	}

	public void TeleportIn()
	{
		base.Animator.SetTrigger("TeleportIn");
	}

	public void TeleportOut()
	{
		base.Animator.SetTrigger("TeleportIn");
	}

	public void Attack1()
	{
		base.Animator.SetTrigger("OnAttack1");
	}

	public void Attack2()
	{
		base.Animator.SetTrigger("OnAttack2");
	}

	public void Attack3()
	{
		base.Animator.SetTrigger("OnAttack3");
	}

	public void Attack4()
	{
		base.Animator.SetTrigger("OnAttack4");
	}
}
