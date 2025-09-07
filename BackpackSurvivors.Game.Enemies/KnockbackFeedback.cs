using System;
using System.Collections;
using BackpackSurvivors.Game.Player;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

public class KnockbackFeedback : MonoBehaviour
{
	[SerializeField]
	private Rigidbody2D _rigidBody;

	[SerializeField]
	private BackpackSurvivors.Game.Player.Player _player;

	[SerializeField]
	private float _delayUntillMoving = 0.15f;

	private Enemy _enemy;

	public event EventHandler OnBegin;

	public event EventHandler OnDone;

	private void Awake()
	{
		_enemy = GetComponentInParent<Enemy>();
	}

	private IEnumerator Reset()
	{
		yield return new WaitForSeconds(_delayUntillMoving);
		_rigidBody.velocity = Vector3.zero;
		_player?.SetBeingKnockedBack(isBeingKnockedBack: false);
		this.OnDone?.Invoke(this, null);
	}

	public void PlayFeedback(Transform sourceForKnockback, float knockbackPower, float knockbackDefense)
	{
		if (sourceForKnockback == null)
		{
			return;
		}
		float num = Mathf.Abs(knockbackPower) - Mathf.Abs(knockbackDefense);
		if (!(num <= 0f))
		{
			_ = 0f;
			_player?.SetBeingKnockedBack(isBeingKnockedBack: true);
			StopAllCoroutines();
			this.OnBegin?.Invoke(this, null);
			Vector2 vector = (Vector2)((knockbackPower > 0f) ? (base.transform.position - sourceForKnockback.position).normalized : (sourceForKnockback.position - base.transform.position).normalized) * num;
			if (_player != null)
			{
				_rigidBody.AddForce(vector, ForceMode2D.Impulse);
				StartCoroutine(Reset());
			}
			else if (_enemy != null)
			{
				_enemy.EnemyMovement.SetKnockbackMovement(vector);
			}
			else
			{
				Debug.Log("Both enemy and player are null in KnockbackFeedback");
			}
		}
	}
}
