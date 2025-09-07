using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Shared;

public class CursorTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private Enums.CursorState _cursorState;

	public void ChangeCursorState(Enums.CursorState cursorState)
	{
		_cursorState = cursorState;
	}

	public void RefreshCursor()
	{
		if (SingletonController<CursorController>.Instance == null)
		{
			Debug.LogWarning("No CursorController on the scene!");
		}
		else
		{
			SingletonController<CursorController>.Instance.TriggerCursorState(_cursorState);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (SingletonController<CursorController>.Instance == null)
		{
			Debug.LogWarning("No CursorController on the scene!");
		}
		else
		{
			SingletonController<CursorController>.Instance.TriggerCursorState(_cursorState);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (SingletonController<CursorController>.Instance == null)
		{
			Debug.LogWarning("No CursorController on the scene!");
		}
		else
		{
			SingletonController<CursorController>.Instance.TriggerDefaultCursorState();
		}
	}

	public void OMouseEnter()
	{
		if (SingletonController<CursorController>.Instance == null)
		{
			Debug.LogWarning("No CursorController on the scene!");
		}
		else
		{
			SingletonController<CursorController>.Instance.TriggerCursorState(_cursorState);
		}
	}

	public void OnMouseExit()
	{
		if (SingletonController<CursorController>.Instance == null)
		{
			Debug.LogWarning("No CursorController on the scene!");
		}
		else
		{
			SingletonController<CursorController>.Instance.TriggerDefaultCursorState();
		}
	}
}
