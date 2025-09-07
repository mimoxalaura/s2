using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.ScriptableObjects.Buffs;
using UnityEngine;

namespace BackpackSurvivors.Game.Buffs.Base;

public class BuffVisualOnPlayerContainer : MonoBehaviour
{
	[Header("Core")]
	[SerializeField]
	private BuffController _buffController;

	[Header("Visuals")]
	[SerializeField]
	private SpriteRenderer[] _spriteRenderersForBuffs;

	[SerializeField]
	private Material _defaultMaterial;

	private List<BuffVisualEffectOnPlayer> _buffVisualEffectOnPlayers;

	private List<BuffHandler> _buffHandlersOnPlayer;

	private BuffHandler _activeVisualEffectOnPlayer;

	private void Start()
	{
		_buffVisualEffectOnPlayers = new List<BuffVisualEffectOnPlayer>();
		_buffHandlersOnPlayer = new List<BuffHandler>();
		_buffController.OnBuffAdded += _buffController_OnBuffAdded;
		_buffController.OnBuffRemoved += _buffController_OnBuffRemoved;
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
		BuffVisualEffectOnPlayer[] buffVisualOnPlayerPrefabs = buffHandler.BuffSO.BuffVisualOnPlayerPrefabs;
		for (int i = 0; i < buffVisualOnPlayerPrefabs.Length; i++)
		{
			BuffVisualEffectOnPlayer buffVisualEffectOnPlayer = Object.Instantiate(buffVisualOnPlayerPrefabs[i], base.transform);
			buffVisualEffectOnPlayer.Init(buffHandler);
			_buffVisualEffectOnPlayers.Add(buffVisualEffectOnPlayer);
		}
		_buffHandlersOnPlayer.Add(buffHandler);
		if (_activeVisualEffectOnPlayer == null || _activeVisualEffectOnPlayer.BuffSO.VisualPriority < buffHandler.BuffSO.VisualPriority)
		{
			SetMaterialAndColor(buffHandler);
		}
	}

	private void SetMaterialAndColor(BuffHandler buffHandler)
	{
		_activeVisualEffectOnPlayer = buffHandler;
		SpriteRenderer[] spriteRenderersForBuffs = _spriteRenderersForBuffs;
		foreach (SpriteRenderer spriteRenderer in spriteRenderersForBuffs)
		{
			spriteRenderer.color = buffHandler.BuffSO.BuffColor;
			if (buffHandler.BuffSO.BuffMaterial != null)
			{
				spriteRenderer.material = buffHandler.BuffSO.BuffMaterial;
			}
		}
	}

	public void RemoveBuff(BuffHandler buffHandler)
	{
		_buffHandlersOnPlayer.Remove(buffHandler);
		if (buffHandler.BuffSO.Id == _activeVisualEffectOnPlayer.BuffSO.Id)
		{
			SpriteRenderer[] spriteRenderersForBuffs = _spriteRenderersForBuffs;
			foreach (SpriteRenderer spriteRenderer in spriteRenderersForBuffs)
			{
				spriteRenderer.color = Color.white;
				if (buffHandler.BuffSO.BuffMaterial != null)
				{
					spriteRenderer.material = _defaultMaterial;
				}
			}
		}
		IEnumerable<BuffVisualEffectOnPlayer> buffVisualOnPlayerEffects = _buffVisualEffectOnPlayers.Where((BuffVisualEffectOnPlayer x) => x.BuffHandler == buffHandler);
		foreach (BuffVisualEffectOnPlayer item in buffVisualOnPlayerEffects)
		{
			if (item != null && item.isActiveAndEnabled)
			{
				Object.Destroy(item.gameObject);
			}
		}
		_buffVisualEffectOnPlayers.RemoveAll((BuffVisualEffectOnPlayer x) => buffVisualOnPlayerEffects.Contains(x));
		if (_buffHandlersOnPlayer.Any((BuffHandler x) => !x.CanBeDestroyed()))
		{
			BuffHandler materialAndColor = (from x in _buffHandlersOnPlayer
				where !x.CanBeDestroyed()
				orderby x.BuffSO.VisualPriority descending
				select x).First();
			SetMaterialAndColor(materialAndColor);
		}
		else
		{
			_activeVisualEffectOnPlayer = null;
		}
	}

	private void OnDestroy()
	{
		_buffController.OnBuffAdded -= _buffController_OnBuffAdded;
		_buffController.OnBuffRemoved -= _buffController_OnBuffRemoved;
	}
}
