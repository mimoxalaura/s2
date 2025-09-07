using BackpackSurvivors.ScriptableObjects.Items;

namespace BackpackSurvivors.Game.Adventure.Interfaces;

internal interface IDPSLoggagable
{
	internal WeaponSO WeaponSO { get; }

	internal float ActiveSinceTime { get; set; }
}
