using System;
using BackpackSurvivors.System;
using Cinemachine;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class CinemachineSwitcher : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private CinemachineStateDrivenCamera _cinemachineStateDrivenCamera;

	public void SwitchTo(Enums.Unlockable unlockable)
	{
		_animator.Play(unlockable.ToString());
	}

	public void SwitchToPlayer()
	{
		_animator.Play("Player");
	}

	internal float GetAndSetDelay(Transform origin, Transform target)
	{
		Vector3 vector = origin.position - target.position;
		float num = (Math.Abs(vector.x) + Math.Abs(vector.y)) * 0.05f;
		_cinemachineStateDrivenCamera.m_DefaultBlend.m_Time = num;
		return num;
	}
}
