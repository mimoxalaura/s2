using UnityEngine;

namespace BackpackSurvivors.Game.Characters;

public class TargetArrowCornerEnforcer : MonoBehaviour
{
	public Transform _camera;

	public float _cameraSizeX;

	public float _cameraSizeY;

	public bool _active;

	public void EnableClamping(bool enabled)
	{
		_active = enabled;
	}
}
