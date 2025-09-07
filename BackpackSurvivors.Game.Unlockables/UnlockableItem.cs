using System;
using BackpackSurvivors.Game.Analytics;
using BackpackSurvivors.Game.Analytics.Events;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Unlockable;
using BackpackSurvivors.System;
using UnityEngine.Video;

namespace BackpackSurvivors.Game.Unlockables;

public class UnlockableItem
{
	public delegate void UnlockableUnlockedHandler(object sender, UnlockableUnlockedEventArgs e);

	public Enums.Unlockable Unlockable => BaseUnlockable.Unlockable;

	public bool IsUnlocked => PointsInvested > 0;

	public int PointsInvested { get; set; }

	public UnlockableSO BaseUnlockable { get; private set; }

	public VideoClip TutorialVideo => BaseUnlockable.TutorialVideo;

	public bool Completed => PointsInvested == BaseUnlockable.UnlockAvailableAmount;

	public event UnlockableUnlockedHandler OnUnlockableUnlocked;

	public UnlockableItem(UnlockableSO unlockable, int pointsInvested = 0)
	{
		BaseUnlockable = unlockable;
		PointsInvested = pointsInvested;
	}

	public bool CanUpgradeOrSpend()
	{
		if (FeatureUnlocked() && CanUpgrade())
		{
			return GetCostForNextPoint() <= SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.TitanSouls);
		}
		return false;
	}

	public bool CanUpgrade()
	{
		return PointsInvested < BaseUnlockable.UnlockAvailableAmount;
	}

	public bool FeatureUnlocked()
	{
		return BaseUnlockable.FeatureUnlocked;
	}

	public int GetCostForNextPoint()
	{
		if (PointsInvested == 0)
		{
			return BaseUnlockable.UnlockPrice;
		}
		int unlockPrice = BaseUnlockable.UnlockPrice;
		return Convert.ToInt32((float)PointsInvested * BaseUnlockable.ScalingPriceMultiplier * (float)unlockPrice);
	}

	internal void Upgrade()
	{
		PointsInvested++;
		this.OnUnlockableUnlocked?.Invoke(this, new UnlockableUnlockedEventArgs(this, fromLoad: false, fromUI: true));
		LogUpgradeInAnalytics();
	}

	private void LogUpgradeInAnalytics()
	{
		UnlockableUnlockedEvent eventToRecord = new UnlockableUnlockedEvent
		{
			UnlockableName = BaseUnlockable.Name.ToString(),
			UnlockablePointsInvested = PointsInvested
		};
		SingletonController<AnalyticsController>.Instance.RecordEvent(eventToRecord);
	}

	public void ResetToLock()
	{
		PointsInvested = 0;
	}
}
