using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Waves;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects.Core;

internal class LingeringEffectsController : SingletonController<LingeringEffectsController>
{
	[SerializeField]
	private SerializableDictionaryBase<Enums.LingeringEffectType, int> _maxNumberOfLingeringEffectsPerType;

	private Dictionary<Enums.LingeringEffectType, List<LingeringEffect>> _lingeringEffectsByType = new Dictionary<Enums.LingeringEffectType, List<LingeringEffect>>();

	private const int _defaultMaxNumberOfLingeringEffectsAllowed = 50;

	internal void AddLingeringEffect(LingeringEffect lingeringEffect)
	{
		AddEntryIfNeeded(lingeringEffect);
		_lingeringEffectsByType[lingeringEffect.LingeringEffectType].Add(lingeringEffect);
	}

	internal void RemoveLingeringEffect(LingeringEffect lingeringEffect)
	{
		if (_lingeringEffectsByType.ContainsKey(lingeringEffect.LingeringEffectType))
		{
			_lingeringEffectsByType[lingeringEffect.LingeringEffectType].Remove(lingeringEffect);
		}
	}

	internal bool ShouldPreventSpawning(Enums.LingeringEffectType lingeringEffectType, Vector2 spawnPosition, float range)
	{
		if (MaxNumberOfLingeringEffectsReached(lingeringEffectType))
		{
			return true;
		}
		if (EffectOfTypeExistsWithinRange(lingeringEffectType, spawnPosition, range))
		{
			return true;
		}
		if (SpawnPositionIsOutsideOfMap(spawnPosition))
		{
			return true;
		}
		return false;
	}

	internal int GetNextSortingOrder()
	{
		if (_lingeringEffectsByType.Values.Count == 0)
		{
			return 0;
		}
		if (!_lingeringEffectsByType.Values.Any((List<LingeringEffect> ll) => ll.Any()))
		{
			return 0;
		}
		return _lingeringEffectsByType.Values.Cast<List<LingeringEffect>>().Max((List<LingeringEffect> ll) => ll.Max((LingeringEffect le) => le.GetSortingOrder())) + 1;
	}

	private bool SpawnPositionIsOutsideOfMap(Vector2 spawnPosition)
	{
		WorldSpawnPositionGenerator controllerByType = SingletonCacheController.Instance.GetControllerByType<WorldSpawnPositionGenerator>();
		float x = controllerByType.transform.position.x;
		float y = controllerByType.transform.position.y;
		if (spawnPosition.x < x + controllerByType.LeftBoundX)
		{
			return true;
		}
		if (spawnPosition.x > x + controllerByType.RightBoundX)
		{
			return true;
		}
		if (spawnPosition.y < y + controllerByType.BottomBoundY)
		{
			return true;
		}
		if (spawnPosition.y > y + controllerByType.TopBoundY)
		{
			return true;
		}
		return false;
	}

	private bool MaxNumberOfLingeringEffectsReached(Enums.LingeringEffectType lingeringEffectType)
	{
		int num = (_lingeringEffectsByType.ContainsKey(lingeringEffectType) ? _lingeringEffectsByType[lingeringEffectType].Count : 0);
		int num2 = (_maxNumberOfLingeringEffectsPerType.ContainsKey(lingeringEffectType) ? _maxNumberOfLingeringEffectsPerType[lingeringEffectType] : 50);
		if (num >= num2)
		{
			return true;
		}
		return false;
	}

	private bool EffectOfTypeExistsWithinRange(Enums.LingeringEffectType lingeringEffectType, Vector2 position, float rangeToCheck)
	{
		if (!_lingeringEffectsByType.ContainsKey(lingeringEffectType))
		{
			return false;
		}
		return _lingeringEffectsByType[lingeringEffectType].Any((LingeringEffect le) => Vector2.Distance(position, le.transform.position) <= rangeToCheck);
	}

	private void AddEntryIfNeeded(LingeringEffect lingeringEffect)
	{
		if (!_lingeringEffectsByType.ContainsKey(lingeringEffect.LingeringEffectType))
		{
			_lingeringEffectsByType.Add(lingeringEffect.LingeringEffectType, new List<LingeringEffect>());
		}
	}

	public override void Clear()
	{
		_lingeringEffectsByType.Clear();
	}

	public override void ClearAdventure()
	{
		_lingeringEffectsByType.Clear();
	}
}
