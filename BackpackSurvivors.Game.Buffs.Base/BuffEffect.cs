using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs.Base;

public abstract class BuffEffect : MonoBehaviour
{
	private BuffSO _buffSO;

	public virtual void Init(BuffSO buffSO)
	{
		_buffSO = buffSO;
	}

	public virtual void Trigger(Character buffedCharacter)
	{
		if (_buffSO.AudioOnTrigger != null)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_buffSO.AudioOnTrigger, 1f);
		}
	}

	public virtual void OnFallOff(Character buffedCharacter)
	{
		if (_buffSO.AudioOnFallOff != null)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_buffSO.AudioOnFallOff, 1f);
		}
	}
}
