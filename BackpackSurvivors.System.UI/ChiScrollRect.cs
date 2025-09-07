using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BackpackSurvivors.System.UI;

public class ChiScrollRect : ScrollRect, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private static string mouseScrollWheelAxis = "Mouse ScrollWheel";

	private bool swallowMouseWheelScrolls = true;

	private bool isMouseOver;

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
	}

	private void Update()
	{
		if (isMouseOver && IsMouseWheelRolling())
		{
			float axis = Input.GetAxis(mouseScrollWheelAxis);
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.scrollDelta = new Vector2(0f, axis);
			swallowMouseWheelScrolls = false;
			OnScroll(pointerEventData);
			swallowMouseWheelScrolls = true;
		}
	}

	public override void OnScroll(PointerEventData data)
	{
		if (!IsMouseWheelRolling() || !swallowMouseWheelScrolls)
		{
			if (data.scrollDelta.y < 0f - Mathf.Epsilon)
			{
				data.scrollDelta = new Vector2(0f, 0f - base.scrollSensitivity);
			}
			else if (data.scrollDelta.y > Mathf.Epsilon)
			{
				data.scrollDelta = new Vector2(0f, base.scrollSensitivity);
			}
			base.OnScroll(data);
		}
	}

	private static bool IsMouseWheelRolling()
	{
		return Input.GetAxis(mouseScrollWheelAxis) != 0f;
	}
}
