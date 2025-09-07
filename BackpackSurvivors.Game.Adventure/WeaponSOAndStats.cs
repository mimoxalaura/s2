using BackpackSurvivors.ScriptableObjects.Items;

namespace BackpackSurvivors.Game.Adventure;

public class WeaponSOAndStats
{
	public WeaponSO WeaponSO;

	public float Damage { get; private set; }

	public float TotalTimeWeaponWasActive { get; private set; }

	public float DPS => Damage / TotalTimeWeaponWasActive;

	public void AddDamage(float damage)
	{
		Damage += damage;
	}

	public void AddTimeActive(float timeActive)
	{
		TotalTimeWeaponWasActive += timeActive;
	}
}
