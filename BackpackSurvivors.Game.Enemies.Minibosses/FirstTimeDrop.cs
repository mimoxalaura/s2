using BackpackSurvivors.Game.Level;
using BackpackSurvivors.ScriptableObjects.Adventures;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Minibosses;

[RequireComponent(typeof(Enemy))]
public class FirstTimeDrop : MonoBehaviour
{
	[SerializeField]
	private RewardSO[] _rewardsToDropOnce;

	[SerializeField]
	private bool _itemsShouldBeAddedToFutureAdventures;

	private void Start()
	{
		InitFirstTimeDropHelper();
	}

	private void InitFirstTimeDropHelper()
	{
		FirstTimeDropHelper controllerByType = SingletonCacheController.Instance.GetControllerByType<FirstTimeDropHelper>();
		if (!(controllerByType == null))
		{
			Enemy component = GetComponent<Enemy>();
			string rewardDescription = "First time kill: <u>" + component.BaseEnemy.Name + "</u>";
			controllerByType.Init(_rewardsToDropOnce, _itemsShouldBeAddedToFutureAdventures, rewardDescription);
		}
	}
}
