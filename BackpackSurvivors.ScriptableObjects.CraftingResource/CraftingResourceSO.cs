using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.CraftingResource;

[CreateAssetMenu(fileName = "CraftingResource", menuName = "Game/CraftingResource/CraftingResource", order = 1)]
public class CraftingResourceSO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Title;

	[SerializeField]
	public int Id;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public string Name;

	[SerializeField]
	public string Description;

	[SerializeField]
	public Enums.CraftingResource CraftingResource;

	[SerializeField]
	public AdventureSO AdventureSource;

	[SerializeField]
	public LevelSO LevelSource;

	private void Open()
	{
	}

	private void CreateData()
	{
		if (Icon != null)
		{
			_PreviewIcon = Icon.texture;
		}
		_Title = Name;
	}
}
