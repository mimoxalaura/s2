using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.RuneEffects;

public abstract class RuneSpecialEffect : MonoBehaviour
{
	[SerializeField]
	private Enums.RuneSpecialEffectDestructionType _runeSpecialEffectDestructionType;

	[SerializeField]
	private int _runeSpecialEffectTriggerMaxCount = 1;

	[SerializeField]
	private string _description;

	[SerializeField]
	private string _triggerMessage;

	[SerializeField]
	private TextMeshProUGUI _triggerMessageText;

	[SerializeField]
	private Animator _dissolveAnimator;

	[SerializeField]
	private int _triggerPriority;

	private int _currentTriggerCount;

	private BaseDraggable _baseDraggable;

	internal BaseDraggable BaseDraggable => _baseDraggable;

	internal int TriggerPriority => _triggerPriority;

	internal int CurrentTriggerCount => _currentTriggerCount;

	internal int MaxTriggerCount => _runeSpecialEffectTriggerMaxCount;

	public Enums.RuneSpecialEffectDestructionType RuneSpecialEffectDestructionType => _runeSpecialEffectDestructionType;

	internal virtual void Init(BaseDraggable baseDraggable)
	{
		_baseDraggable = baseDraggable;
		_triggerMessageText.SetText(GetTriggerMessage());
	}

	internal virtual void Dissolve()
	{
		_baseDraggable.Dissolve();
	}

	internal virtual void TriggerVfx()
	{
		_dissolveAnimator.SetTrigger("Dissolve");
	}

	internal virtual void FailedToTrigger()
	{
	}

	public virtual bool ShouldTrigger()
	{
		bool flag = BaseDraggable.Owner == Enums.Backpack.DraggableOwner.Player && BaseDraggable.StoredInGridType == Enums.Backpack.GridType.Backpack;
		if (_runeSpecialEffectDestructionType == Enums.RuneSpecialEffectDestructionType.DestroyAfterX)
		{
			if (flag)
			{
				return _currentTriggerCount < _runeSpecialEffectTriggerMaxCount;
			}
			return false;
		}
		return flag;
	}

	public virtual bool Trigger()
	{
		_currentTriggerCount++;
		TriggerVfx();
		return true;
	}

	public virtual string GetDescription()
	{
		return _description;
	}

	public virtual string GetTriggerMessage()
	{
		return _triggerMessage;
	}
}
