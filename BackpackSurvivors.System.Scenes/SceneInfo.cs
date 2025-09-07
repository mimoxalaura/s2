using UnityEngine;

namespace BackpackSurvivors.System.Scenes;

internal class SceneInfo : MonoBehaviour
{
	[SerializeField]
	private Enums.SceneType _sceneType;

	[SerializeField]
	private bool _shouldPauseOnSwitchToUIInputMap;

	internal Enums.SceneType SceneType => _sceneType;

	internal bool ShouldPauseOnSwitchToUIInputMap => _shouldPauseOnSwitchToUIInputMap;
}
