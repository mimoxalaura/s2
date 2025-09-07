using BackpackSurvivors.Combat.ScriptableObjects;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Relics;

[CreateAssetMenu(fileName = "Relic", menuName = "Game/Relics/Relic", order = 1)]
public class RelicSO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Id;

	public string _Title;

	[Multiline(3)]
	public string _Description;

	public string _Rarity;

	[SerializeField]
	public int Id;

	[SerializeField]
	public string Name;

	[SerializeField]
	public string Description;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public Enums.PlaceableRarity Rarity;

	[SerializeField]
	public Enums.RelicSource RelicSource;

	[Header("Global")]
	[SerializeField]
	public ItemStatsSO GlobalStats;

	[SerializeField]
	public ItemStatsSO[] ConditionalStats;

	[Header("Overrides")]
	[SerializeField]
	public WeaponStatTypeCalculationOverride[] WeaponStatTypeCalculationOverrides;

	[SerializeField]
	public WeaponDamageTypeValueOverride[] WeaponDamageTypeCalculationOverrides;

	[Header("Conditions")]
	[SerializeField]
	public ConditionSO[] Conditions;

	[Header("Custom Handler")]
	[SerializeField]
	public RelicHandler RelicHandler;

	private void CreateData()
	{
		if (Icon != null)
		{
			_PreviewIcon = Icon.texture;
		}
		_Title = Name;
		_Description = Description;
		_Id = Id.ToString();
		_Rarity = Rarity.ToString();
	}

	private Color GetRarityColor()
	{
		return EditorHelper.GetRarityColor(Rarity);
	}
}
