using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

public class BackpackCell : MonoBehaviour
{
	public enum BackpackCellAlpha
	{
		Full,
		Partially,
		None
	}

	[SerializeField]
	private Image _gridCellImage;

	[SerializeField]
	private Image _statusColorImage;

	[SerializeField]
	private Image _starImage;

	[SerializeField]
	private Image _starOutlineImage;

	[SerializeField]
	private Image _cellOutlineImage;

	[SerializeField]
	private Enums.Backpack.GridType _hoveredCellGridType;

	[SerializeField]
	private bool _canShowStars;

	[SerializeField]
	private GameObject _lockedImage;

	[SerializeField]
	private Sprite _starImageSpritePositive;

	[SerializeField]
	private Sprite _starImageSpriteNegative;

	public int SlotId { get; private set; }

	public void Init(int slotId)
	{
		SlotId = slotId;
		InitQuadrants();
	}

	public void FadeAlpha(BackpackCellAlpha backpackCellAlpha)
	{
		float a = 0f;
		switch (backpackCellAlpha)
		{
		case BackpackCellAlpha.Full:
			a = 1f;
			break;
		case BackpackCellAlpha.Partially:
			a = 0.3f;
			break;
		case BackpackCellAlpha.None:
			a = 0f;
			break;
		}
		_gridCellImage.color = new Color(_gridCellImage.color.r, _gridCellImage.color.g, _gridCellImage.color.b, a);
	}

	public void ShowStatusColorImage(Color color)
	{
		_statusColorImage.enabled = true;
		_statusColorImage.color = color;
	}

	public void ToggleLockedState(bool locked)
	{
		_lockedImage.SetActive(locked);
	}

	public void HideStatusColorImage()
	{
		_statusColorImage.enabled = false;
	}

	public void SetStarImageVisibility(bool visibility, bool showStarredFilled = false, bool starEffectIsPositive = true)
	{
		HideStarImages();
		Image obj = (showStarredFilled ? _starImage : _starOutlineImage);
		obj.enabled = visibility && _canShowStars;
		obj.sprite = (starEffectIsPositive ? _starImageSpritePositive : _starImageSpriteNegative);
	}

	public void SetCellOutlineImageVisibility(bool visibility)
	{
		_cellOutlineImage.enabled = visibility;
	}

	private void HideStarImages()
	{
		_starImage.enabled = false;
		_starOutlineImage.enabled = false;
	}

	private void InitQuadrants()
	{
		BackpackCellQuadrant[] componentsInChildren = GetComponentsInChildren<BackpackCellQuadrant>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Init(SlotId, _hoveredCellGridType);
		}
	}
}
