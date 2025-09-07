using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.Game.Backpack;

public class BackpackCellQuadrant : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public delegate void BackpackCellQuadrantHoveredHandler(object sender, BackpackCellQuadrantHoveredEventArgs e);

	[SerializeField]
	private int _quadrantIdentifier;

	public int QuadrantIdentifier => _quadrantIdentifier;

	public int BackpackSlotId { get; private set; }

	public bool IsCurrentlyHovered { get; private set; }

	public Enums.Backpack.GridType HoveredCellGridType { get; private set; }

	public event BackpackCellQuadrantHoveredHandler OnBackpackCellQuadrantHoverEnter;

	public event BackpackCellQuadrantHoveredHandler OnBackpackCellQuadrantHoverExit;

	public void Init(int backPackSlotId, Enums.Backpack.GridType hoveredCellGridType)
	{
		BackpackSlotId = backPackSlotId;
		HoveredCellGridType = hoveredCellGridType;
		SingletonController<BackpackController>.Instance.HoveredSlotProvider.RegisterQuadrant(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		IsCurrentlyHovered = true;
		BackpackCellQuadrantHoveredEventArgs e = new BackpackCellQuadrantHoveredEventArgs(this);
		this.OnBackpackCellQuadrantHoverEnter?.Invoke(this, e);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		IsCurrentlyHovered = false;
		BackpackCellQuadrantHoveredEventArgs e = new BackpackCellQuadrantHoveredEventArgs(this);
		this.OnBackpackCellQuadrantHoverExit?.Invoke(this, e);
	}
}
