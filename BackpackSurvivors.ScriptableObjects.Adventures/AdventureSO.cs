using System.Collections.Generic;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Adventures;

[CreateAssetMenu(fileName = "Adventure", menuName = "Game/Adventures/Adventure", order = 1)]
public class AdventureSO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Id;

	public string _Title;

	[SerializeField]
	public int AdventureId;

	[SerializeField]
	public string AdventureName;

	[SerializeField]
	public string Description;

	[SerializeField]
	public List<LevelSO> Levels;

	[SerializeField]
	public bool AllowHellfire = true;

	[SerializeField]
	public List<RewardSO> CompletionRewards;

	[SerializeField]
	public List<AdventureEffectSO> AdventureEffects;

	[SerializeField]
	public float TitanicSoulMultiplier = 0.5f;

	[SerializeField]
	public float TitanicSoulBaseValueOnInfinite = 10f;

	[SerializeField]
	public float HellfireCoinMultiplier = 0.3f;

	[SerializeField]
	public float HellfireEnemyCountMultiplier = 0.3f;

	[SerializeField]
	public float HellfireEnemyHealthMultiplier = 0.3f;

	[SerializeField]
	public float HellfireEnemyDamageMultiplier = 0.3f;

	[SerializeField]
	public float HellfireExperienceMultiplier = 0.3f;

	[SerializeField]
	public float MaxHellfireCoinMultiplier = 0.5f;

	[SerializeField]
	public float MaxHellfireEnemyCountMultiplier = 0.5f;

	[SerializeField]
	public float MaxHellfireEnemyHealthMultiplier = 0.5f;

	[SerializeField]
	public float MaxHellfireEnemyDamageMultiplier = 0.5f;

	[SerializeField]
	public float MaxHellfireExperienceMultiplier = 0.5f;

	[SerializeField]
	public Sprite BackgroundImage;

	[SerializeField]
	public RuntimeAnimatorController PortalAnimationController;

	public bool Available
	{
		get
		{
			if (!SingletonController<GameDatabase>.Instance.GameDatabaseSO.AdventureAvailability.ContainsKey(this))
			{
				return false;
			}
			return SingletonController<GameDatabase>.Instance.GameDatabaseSO.AdventureAvailability[this];
		}
	}

	private void CreateData()
	{
		if (BackgroundImage != null)
		{
			_PreviewIcon = BackgroundImage.texture;
		}
		_Title = AdventureName;
		_Id = AdventureId.ToString();
	}
}
