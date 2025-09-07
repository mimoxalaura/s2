using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

namespace BackpackSurvivors.Game.Input;

public class VirtualMouseUI : MonoBehaviour
{
	private VirtualMouseInput virtualMouseInput;

	private void Awake()
	{
		virtualMouseInput = GetComponent<VirtualMouseInput>();
	}

	private void LateUpdate()
	{
		Vector2 value = virtualMouseInput.virtualMouse.position.value;
		value.x = Mathf.Clamp(value.x, 0f, Screen.width);
		value.y = Mathf.Clamp(value.y, 0f, Screen.height);
		InputState.Change(virtualMouseInput.virtualMouse.position, value);
	}
}
