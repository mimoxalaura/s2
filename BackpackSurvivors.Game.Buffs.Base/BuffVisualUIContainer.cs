using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs.Base;

public class BuffVisualUIContainer : MonoBehaviour
{
	private BuffController _buffController;

	[Header("Visuals")]
	[SerializeField]
	private BuffVisualUIItem _buffVisualItemPrefab;

	private List<BuffVisualUIItem> _buffVisualItems;

	private void Start()
	{
		_buffVisualItems = new List<BuffVisualUIItem>();
	}

	private void _buffController_OnBuffAdded(object sender, BuffAddedEventArgs e)
	{
		AddBuff(e.BuffHandler);
	}

	private void _buffController_OnBuffRemoved(object sender, BuffRemovedEventArgs e)
	{
		RemoveBuff(e.BuffHandler);
	}

	public void AddBuff(BuffHandler buffHandler)
	{
		BuffVisualUIItem buffVisualUIItem = _buffVisualItems.FirstOrDefault((BuffVisualUIItem x) => x.BuffId == buffHandler.BuffSO.Id);
		if (buffVisualUIItem == null)
		{
			BuffVisualUIItem buffVisualUIItem2 = Object.Instantiate(_buffVisualItemPrefab, base.transform);
			buffVisualUIItem2.SetBuff(buffHandler);
			_buffVisualItems.Add(buffVisualUIItem2);
		}
		else
		{
			buffVisualUIItem.AddStack();
		}
	}

	public void RemoveBuff(BuffHandler buffHandler)
	{
		BuffVisualUIItem buffVisualUIItem = _buffVisualItems.FirstOrDefault((BuffVisualUIItem x) => x.BuffId == buffHandler.BuffSO.Id);
		if (!(buffVisualUIItem == null))
		{
			buffVisualUIItem.RemoveStack();
			if (buffVisualUIItem.Stacks <= 0)
			{
				_buffVisualItems.Remove(buffVisualUIItem);
				Object.Destroy(buffVisualUIItem.gameObject);
			}
		}
	}

	internal void SetBuffController(BuffController buffController)
	{
		_buffController = buffController;
		_buffController.OnBuffAdded += _buffController_OnBuffAdded;
		_buffController.OnBuffRemoved += _buffController_OnBuffRemoved;
		_buffVisualItems = new List<BuffVisualUIItem>();
	}

	private void OnDestroy()
	{
		_buffVisualItems.Clear();
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.transform.GetChild(num).gameObject);
		}
		if (_buffController != null)
		{
			_buffController.OnBuffAdded -= _buffController_OnBuffAdded;
			_buffController.OnBuffRemoved -= _buffController_OnBuffRemoved;
		}
	}
}
