using BackpackSurvivors.Game.Shared.Interfaces;
using UnityEngine;

namespace BackpackSurvivors.System;

public abstract class SingletonController<T> : MonoBehaviour, IClearable, IInitializable where T : class
{
	public static T Instance { get; private set; }

	public bool IsInitialized { get; protected set; }

	private void Awake()
	{
		if (SetupSingleton())
		{
			AfterBaseAwake();
		}
	}

	private bool SetupSingleton()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
			return false;
		}
		Instance = this as T;
		Object.DontDestroyOnLoad(base.gameObject);
		return true;
	}

	public virtual void Clear()
	{
		Debug.LogWarning(string.Format("{0} is not implemented in {1}", "Clear", GetType()));
	}

	public virtual void ClearAdventure()
	{
		Debug.LogWarning(string.Format("{0} is not implemented in {1}", "ClearAdventure", GetType()));
	}

	public virtual void AfterBaseAwake()
	{
	}
}
