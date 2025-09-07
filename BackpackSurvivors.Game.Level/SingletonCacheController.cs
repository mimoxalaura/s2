using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

internal class SingletonCacheController : MonoBehaviour
{
	internal static SingletonCacheController Instance;

	private List<Object> _storedSingletons = new List<Object>();

	internal T GetControllerByType<T>() where T : Object
	{
		Object obj = _storedSingletons.FirstOrDefault((Object o) => o is T);
		if (obj != null)
		{
			return obj as T;
		}
		T val = Object.FindAnyObjectByType<T>();
		_storedSingletons.Add(val);
		return val;
	}

	private void Awake()
	{
		SetupSingleton();
	}

	private void SetupSingleton()
	{
		if (Instance != null)
		{
			Debug.LogWarning("SingletonCacheController already exists. Only one is supposed to exist per scene");
		}
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
