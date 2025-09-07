using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Debuffs;

public class DebuffVisualContainer : MonoBehaviour
{
	[SerializeField]
	private DebuffVisualItem _debuffVisualItemPrefab;

	[SerializeField]
	private Transform _debuffVisualContainer;

	[SerializeField]
	private Transform _debuffVisualEffectContainer;

	[SerializeField]
	private bool _isPlayerVisualContainer;

	[SerializeField]
	private DebuffContainer _debuffContainer;

	private List<DebuffVisualItem> DebuffVisualItems;

	internal void SetDebuffContainer(DebuffContainer debuffContainer)
	{
		_debuffContainer = debuffContainer;
		if (_debuffContainer != null)
		{
			_debuffContainer.OnDebuffApplied += Player_OnDebuffApplied;
			_debuffContainer.OnDebuffRemoved += Player_OnDebuffRemoved;
		}
	}

	private void Start()
	{
		DebuffVisualItems = new List<DebuffVisualItem>();
		if (_isPlayerVisualContainer)
		{
			SetDebuffContainer(SingletonController<GameController>.Instance.Player.GetPlayerDebuffContainer());
		}
		else if (_debuffContainer == null)
		{
			SetDebuffContainer(GetComponent<DebuffContainer>());
		}
	}

	private void Player_OnDebuffApplied(object sender, DebuffAppliedEventArgs e)
	{
		if (!e.Character.IsDead)
		{
			AddDebuff(e.Debuff);
			if (e.Character.IsPlayer)
			{
				e.Character.DamageVisualizer.ShowTextPopup(StringHelper.GetCleanString(e.Debuff.DebuffType), e.Debuff.DebuffColor);
			}
		}
	}

	private void Player_OnDebuffRemoved(object sender, DebuffRemovedEventArgs e)
	{
		RemoveDebuff(e.Debuff);
	}

	private void AddDebuff(DebuffSO debuffSO)
	{
		DebuffVisualItem debuffVisualItem = DebuffVisualItems.FirstOrDefault((DebuffVisualItem x) => x.DebuffId == debuffSO.Id);
		if (debuffVisualItem == null)
		{
			DebuffVisualItem debuffVisualItem2 = Object.Instantiate(_debuffVisualItemPrefab, _debuffVisualContainer);
			debuffVisualItem2.SetDebuff(debuffSO);
			if (_debuffVisualEffectContainer != null)
			{
				debuffVisualItem2.CreateDebuffEffect(_debuffVisualEffectContainer);
			}
			DebuffVisualItems.Add(debuffVisualItem2);
			debuffVisualItem2.UpdateStackCountStack(1);
		}
		else if (debuffSO.MaxStacks < debuffVisualItem.StackCount)
		{
			debuffVisualItem.UpdateStackCountStack(debuffVisualItem.StackCount + 1);
		}
	}

	private void RemoveDebuff(DebuffSO debuffSO)
	{
		DebuffVisualItem debuffVisualItem = DebuffVisualItems.FirstOrDefault((DebuffVisualItem x) => x.DebuffId == debuffSO.Id);
		if (!(debuffVisualItem == null))
		{
			debuffVisualItem.RemovebuffEffect();
			Object.Destroy(debuffVisualItem.gameObject);
			DebuffVisualItems.Remove(debuffVisualItem);
		}
	}

	internal void Reset()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		foreach (DebuffVisualItem debuffVisualItem in DebuffVisualItems)
		{
			debuffVisualItem.RemovebuffEffect();
			Object.Destroy(debuffVisualItem.gameObject);
		}
		if (_debuffVisualContainer != null)
		{
			for (int num = _debuffVisualContainer.childCount - 1; num >= 0; num--)
			{
				Object.Destroy(_debuffVisualContainer.GetChild(num).gameObject);
			}
		}
		DebuffVisualItems.Clear();
	}
}
