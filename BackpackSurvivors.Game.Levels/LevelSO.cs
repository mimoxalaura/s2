using System.Collections.Generic;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Waves;
using BackpackSurvivors.System;
using Tymski;
using UnityEngine;

namespace BackpackSurvivors.Game.Levels;

[CreateAssetMenu(fileName = "Level", menuName = "Game/Levels/Level", order = 1)]
public class LevelSO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Id;

	public string _Title;

	[SerializeField]
	public int LevelId;

	[SerializeField]
	public string LevelName;

	[SerializeField]
	public string Description;

	[SerializeField]
	public List<WaveSO> Waves;

	[SerializeField]
	public EnemySO LevelBoss;

	[SerializeField]
	public Sprite BossIconSkull;

	[SerializeField]
	public SceneReference LevelScene;

	[SerializeField]
	public List<Enums.PlaceableRarity> ItemQualitiesToUnlock;

	[SerializeField]
	public bool BossLevel;

	[SerializeField]
	public Sprite LocationUIIcon;

	[SerializeField]
	public int LevelDuration;

	[SerializeField]
	public float ExperienceModifier = 1f;

	[SerializeField]
	public AudioClip BossCombatMusic;

	[SerializeField]
	public AdventureSO Adventure;

	[SerializeField]
	public SerializableDictionaryBase<Enums.PlaceableRarity, float> ShopOfferRarityChances;

	[SerializeField]
	public bool AllowMultipleShops = true;

	[SerializeField]
	public bool ShouldShowInList = true;

	private void CreateData()
	{
		if (LocationUIIcon != null)
		{
			_PreviewIcon = LocationUIIcon.texture;
		}
		_Title = LevelName;
		_Id = LevelId.ToString();
	}
}
