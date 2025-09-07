using System;
using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups;

public abstract class Lootdrop : MonoBehaviour
{
	[Header("Drops")]
	[SerializeField]
	private float _splashRange = 1f;

	[SerializeField]
	private bool _blockRetrievalOnLevelCompletion;

	[SerializeField]
	private Enums.LootType _lootType;

	private Transform _objectTransform;

	private float _delay;

	private float _pastTime;

	private float _when = 0.3f;

	private Vector3 _offset;

	private bool _done;

	protected bool _collected;

	private float _speedMod = 1f;

	private float _speedModMultiplier = 0.1f;

	public Enums.LootType LootType => _lootType;

	private void Awake()
	{
		_objectTransform = GetComponent<Transform>();
		_offset = new Vector3(UnityEngine.Random.Range(0f - Math.Abs(_splashRange), Math.Abs(_splashRange)), _offset.y, _offset.z);
		_offset = new Vector3(_offset.x, UnityEngine.Random.Range(0f - Math.Abs(_splashRange), Math.Abs(_splashRange)), _offset.z);
	}

	private void Start()
	{
		AfterStart();
	}

	internal virtual void AfterStart()
	{
	}

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	private void Update()
	{
		DoUpdate();
	}

	internal void MoveLootdropToPlayer(bool fromCompletion)
	{
		if (!fromCompletion || !_blockRetrievalOnLevelCompletion)
		{
			RemoveVisualAssetsOnMoving();
			StartCoroutine(AnimateLootdropToPlayer());
		}
	}

	internal virtual void RemoveVisualAssetsOnMoving()
	{
	}

	protected IEnumerator AnimateLootdropToPlayer()
	{
		float movementSpeed = 0.5f;
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		while (!_collected)
		{
			yield return null;
			movementSpeed *= 1.01f;
			Vector2 vector = Vector2.MoveTowards(base.transform.position, player.transform.position, movementSpeed);
			base.transform.position = vector;
			if (Vector2.Distance(vector, player.transform.position) < float.Epsilon)
			{
				Collect();
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PickupRadius component = collision.gameObject.GetComponent<PickupRadius>();
		if (!(component == null) && CanCollect())
		{
			switch (component.Type)
			{
			case PickupRadius.PickupRadiusType.MoveToPlayer:
				MoveLootdropToPlayer(fromCompletion: false);
				break;
			case PickupRadius.PickupRadiusType.Collect:
				Collect();
				break;
			}
		}
	}

	protected abstract void Collect();

	protected virtual bool CanCollect()
	{
		return true;
	}

	internal virtual void DoUpdate()
	{
		if (!_done)
		{
			if (_when >= _delay)
			{
				_pastTime = Time.deltaTime;
				float num = _speedMod + _speedModMultiplier * _pastTime;
				_objectTransform.position += _offset * Time.deltaTime * num;
				_delay += _pastTime;
			}
			else
			{
				_done = true;
			}
		}
	}

	internal virtual void SetLayer(int layerId)
	{
	}
}
