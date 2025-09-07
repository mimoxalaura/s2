using System;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

internal class AdventureBossAnimationController : MonoBehaviour
{
	[SerializeField]
	private float _fullDuration;

	[SerializeField]
	private GameObject _bossNameImage;

	[SerializeField]
	private FinalBossCinemachineSwitcher _finalBossCinemachineSwitcher;

	[SerializeField]
	private GameObject _cancelTip;

	private Action _onCancelOrFinishAction;

	public GameObject BossNameImage => _bossNameImage;

	public virtual void Complete()
	{
		_cancelTip.SetActive(value: false);
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: true);
		_onCancelOrFinishAction();
	}

	public virtual void Animate(Action onCancelOrFinish, Enemy enemy)
	{
		_cancelTip.SetActive(value: true);
		_onCancelOrFinishAction = onCancelOrFinish;
	}

	public float GetAnimationDuration()
	{
		return _fullDuration;
	}

	public void FocusOnPlayer()
	{
		_finalBossCinemachineSwitcher.SwitchToPlayer();
	}

	public void FocusOnBoss()
	{
		_finalBossCinemachineSwitcher.SwitchToBoss();
	}
}
