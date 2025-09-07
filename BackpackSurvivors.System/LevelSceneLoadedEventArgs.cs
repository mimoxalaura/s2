using System;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.System;

public class LevelSceneLoadedEventArgs : EventArgs
{
	public Scene NewScene { get; }

	public LevelSceneLoadedEventArgs(Scene newScene)
	{
		NewScene = newScene;
	}
}
