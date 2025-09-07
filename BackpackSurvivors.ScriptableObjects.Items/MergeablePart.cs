using System;
using BackpackSurvivors.Game.Core;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
public class MergeablePart
{
	public Texture Texture;

	public string ItemName;

	[SerializeField]
	private BaseItemSO _baseItemSO;

	private GameDatabaseSO gameDatabase;

	[HideInInspector]
	public BaseItemSO BaseItem => _baseItemSO;

	private void CreateData()
	{
		try
		{
			if (_baseItemSO != null)
			{
				if (_baseItemSO.Icon != null)
				{
					Texture = _baseItemSO.Icon.texture;
				}
				ItemName = _baseItemSO.Name;
			}
			else
			{
				Texture = null;
				ItemName = string.Empty;
			}
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"{arg}");
		}
	}

	private Color GetItemColor()
	{
		return Color.white;
	}
}
