using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Shared.Interfaces;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Shared;

internal class InitializationController : SingletonController<InitializationController>
{
	private void Start()
	{
		base.IsInitialized = true;
	}

	public void ExecuteCallbackWhenInitialized(IInitializable initializable, Action callback)
	{
		StartCoroutine(ExecuteCallbackWhenInitializedCoroutine(initializable, callback));
	}

	public void ExecuteCallbackWhenInitialized(IInitializable initializable, Action<bool> callback, bool parameter)
	{
		StartCoroutine(ExecuteCallbackWhenInitializedCoroutine(initializable, callback, parameter));
	}

	private IEnumerator ExecuteCallbackWhenInitializedCoroutine(IInitializable initializable, Action callback)
	{
		while (!initializable.IsInitialized)
		{
			yield return null;
		}
		callback();
	}

	private IEnumerator ExecuteCallbackWhenInitializedCoroutine(IInitializable initializable, Action<bool> callback, bool parameter)
	{
		while (!initializable.IsInitialized)
		{
			yield return null;
		}
		callback(parameter);
	}

	public void ExecuteCallbackWhenInitialized(IEnumerable<IInitializable> initializables, Action callback)
	{
		StartCoroutine(ExecuteCallbackWhenInitializedCoroutine(initializables, callback));
	}

	private IEnumerator ExecuteCallbackWhenInitializedCoroutine(IEnumerable<IInitializable> initializables, Action callback)
	{
		while (initializables.Any((IInitializable i) => !i.IsInitialized))
		{
			yield return null;
		}
		callback();
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
