using BackpackSurvivors.Game.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

public class BackpackVisualLockArea : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private Image _interactionImage;

	[SerializeField]
	private Image _lockImage;

	[SerializeField]
	private Image _backdropImage;

	private bool _locked;

	private DragController _dragController;

	private void Start()
	{
		_dragController = SingletonCacheController.Instance.GetControllerByType<DragController>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!_dragController.IsDragging)
		{
			HighLight(locked: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!_dragController.IsDragging)
		{
			HighLight(locked: false);
		}
	}

	public void HighLight(bool locked)
	{
		if (_locked)
		{
			_lockImage.gameObject.SetActive(locked);
			_backdropImage.gameObject.SetActive(locked);
		}
	}

	public void ToggleLocked(bool locked)
	{
		_locked = locked;
		_interactionImage.raycastTarget = locked;
	}
}
