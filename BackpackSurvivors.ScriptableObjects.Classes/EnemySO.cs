using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Loot;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Classes;

[CreateAssetMenu(fileName = "Enemy", menuName = "Game/Enemies/Enemy", order = 1)]
public class EnemySO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Id;

	public string _Title;

	[SerializeField]
	public int Id;

	[SerializeField]
	public string Name;

	[SerializeField]
	public string Description;

	[SerializeField]
	public string GameplayDescription;

	[SerializeField]
	public bool KillOnLevelCompleted = true;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public Enums.Enemies.EnemyType EnemyType;

	[SerializeField]
	public Enemy EnemyPrefab;

	[SerializeField]
	public SerializableDictionaryBase<Enums.ItemStatType, float> Stats;

	[SerializeField]
	public SerializableDictionaryBase<Enums.DamageType, float> DamageTypeValues;

	[SerializeField]
	public DamageSO DamageSO;

	[SerializeField]
	public float KnockbackDefense;

	[SerializeField]
	public bool CanBeKnockedBack;

	[SerializeField]
	public Enums.Debuff.DebuffType[] DebuffImmunities;

	[SerializeField]
	public bool IsFlying;

	[SerializeField]
	public bool IsDummyTarget;

	[SerializeField]
	public int BaseExperience = 1;

	[SerializeField]
	public LootBagSO LootBagSO;

	[SerializeField]
	public float DeathAnimationDuration = 1f;

	[SerializeField]
	public Sprite ArrowSprite;

	[SerializeField]
	public Enums.Enemies.EnemyOnHitType EnemyOnHitType;

	[SerializeField]
	public AudioClip SpawnAudioClip;

	[SerializeField]
	public AudioClip EnemyOnDeathAudio;

	private void CreateData()
	{
		if (Icon != null)
		{
			_PreviewIcon = Icon.texture;
		}
		_Title = Name;
		_Id = Id.ToString();
	}
}
