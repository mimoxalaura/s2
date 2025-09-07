using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

public class SnakeBoss : Enemy
{
	private GameObject[] parts;

	private Vector2 targetPosition;

	[Header("Sprites")]
	[SerializeField]
	private Sprite head;

	[SerializeField]
	private Sprite body;

	[SerializeField]
	private Sprite tail;

	[Header("Properties")]
	[SerializeField]
	private int length = 3;

	[SerializeField]
	private float gap;

	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private float speedFadeDistance = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float bodySpeedMultiplier = 1f;

	[Header("Rendering")]
	[SerializeField]
	private string wormLayer;

	[SerializeField]
	private bool reverseSorting;

	public int Length => length;

	public float MoveSpeed => base.BaseEnemy.Stats[Enums.ItemStatType.SpeedPercentage];

	public float Gap
	{
		get
		{
			return gap;
		}
		set
		{
			gap = value;
		}
	}

	public float RotationSpeed
	{
		get
		{
			return rotationSpeed;
		}
		set
		{
			rotationSpeed = value;
		}
	}

	public float BodySpeedMultiplier
	{
		get
		{
			return bodySpeedMultiplier;
		}
		set
		{
			bodySpeedMultiplier = Mathf.Clamp01(value);
		}
	}

	public Vector2 TargetPosition
	{
		get
		{
			return targetPosition;
		}
		set
		{
			targetPosition = value;
		}
	}

	public Vector2 HeadPosition => parts[0].transform.position;

	public GameObject[] Parts => parts;

	private void OnValidate()
	{
		length = Mathf.Clamp(length, 2, int.MaxValue);
	}

	private void Start()
	{
		parts = new GameObject[length];
		for (int i = 0; i < length; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sortingLayerName = wormLayer;
			spriteRenderer.sortingOrder = (reverseSorting ? (length - (i + 1)) : i);
			if (i == 0)
			{
				gameObject.name = "Head";
				spriteRenderer.sprite = head;
			}
			else if (i == length - 1)
			{
				gameObject.name = "Tail";
				spriteRenderer.sprite = tail;
			}
			else
			{
				gameObject.name = "Body";
				spriteRenderer.sprite = body;
			}
			parts[i] = gameObject;
		}
	}

	private void Update()
	{
		TargetPosition = SingletonController<GameController>.Instance.PlayerPosition;
		for (int i = 0; i < parts.Length; i++)
		{
			Transform transform = parts[i].transform;
			if (i == 0)
			{
				Vector2 vector = targetPosition - (Vector2)transform.position;
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, vector), rotationSpeed * 0.01f * Time.fixedDeltaTime);
				transform.transform.Translate(Vector2.up * MoveSpeed * Time.fixedDeltaTime);
				continue;
			}
			Vector2 vector2 = parts[i - 1].transform.position;
			Vector3 upwards = vector2 - (Vector2)transform.position;
			transform.rotation = Quaternion.LookRotation(Vector3.forward, upwards);
			if (Vector2.Distance(transform.position, vector2) > gap)
			{
				float num = Mathf.InverseLerp(gap, gap + speedFadeDistance, Vector2.Distance(transform.position, vector2));
				transform.transform.position = Vector2.Lerp(transform.position, vector2, MoveSpeed * Time.fixedDeltaTime * bodySpeedMultiplier * num);
			}
		}
	}
}
