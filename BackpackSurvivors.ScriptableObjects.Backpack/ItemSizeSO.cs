using System.Linq;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Backpack;

[CreateAssetMenu(fileName = "new ItemSize", menuName = "Game/Backpack/ItemSize", order = 1)]
public class ItemSizeSO : ScriptableObject
{
	[SerializeField]
	public string SizeName;

	[SerializeField]
	public Enums.Backpack.BagSizes BagSize;

	[SerializeField]
	public Enums.Backpack.ItemSizeCellType[] SizeInfo = new Enums.Backpack.ItemSizeCellType[100];

	private int _maxItemWidth = 10;

	private int _maxItemHeight = 10;

	public int GetItemSlotCount()
	{
		int num = 0;
		Enums.Backpack.ItemSizeCellType[] sizeInfo = SizeInfo;
		for (int i = 0; i < sizeInfo.Length; i++)
		{
			if (sizeInfo[i] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable)
			{
				num++;
			}
		}
		return num;
	}

	public int GetItemStarCount()
	{
		int num = 0;
		Enums.Backpack.ItemSizeCellType[] sizeInfo = SizeInfo;
		for (int i = 0; i < sizeInfo.Length; i++)
		{
			if (sizeInfo[i] == Enums.Backpack.ItemSizeCellType.CellContainsStar)
			{
				num++;
			}
		}
		return num;
	}

	public int GetItemWidth()
	{
		int num = -1;
		for (int i = 0; i < _maxItemHeight; i++)
		{
			for (int j = 0; j < _maxItemWidth; j++)
			{
				if ((SizeInfo[i * _maxItemWidth + j] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable || SizeInfo[i * _maxItemWidth + j] == Enums.Backpack.ItemSizeCellType.CellContainsStar) && j + 1 > num)
				{
					num = j + 1;
				}
			}
		}
		return num;
	}

	public int GetItemHeight()
	{
		int result = -1;
		for (int i = 0; i < _maxItemHeight; i++)
		{
			if (SizeInfo[i * _maxItemWidth] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable || SizeInfo[i * _maxItemWidth] == Enums.Backpack.ItemSizeCellType.CellContainsStar)
			{
				result = i + 1;
			}
		}
		return result;
	}

	public int GetItemWidthWithoutStars()
	{
		int num = 0;
		for (int i = 0; i < _maxItemWidth; i++)
		{
			if (SizeInfo[i] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable)
			{
				num++;
			}
		}
		return num;
	}

	public Vector2 GetItemSizeWithoutStars()
	{
		int num = 1;
		int num2 = 1;
		for (int i = 0; i < _maxItemHeight; i++)
		{
			for (int j = 0; j < _maxItemWidth; j++)
			{
				int num3 = j + i * _maxItemHeight;
				if (SizeInfo[num3] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable)
				{
					if (i > num)
					{
						num = i;
					}
					if (j > num2)
					{
						num2 = j;
					}
				}
			}
		}
		return new Vector2(num2, num + 1);
	}

	public Enums.Backpack.ItemSizeCellType[] GetItemSizeByRotation(Enums.Backpack.ItemRotation rotation)
	{
		return rotation switch
		{
			Enums.Backpack.ItemRotation.Rotation0 => SizeInfo, 
			Enums.Backpack.ItemRotation.Rotation90 => GetItemSizeRotated90(), 
			Enums.Backpack.ItemRotation.Rotation180 => GetItemSizeRotated180(), 
			Enums.Backpack.ItemRotation.Rotation270 => GetItemSizeRotated270(), 
			_ => SizeInfo, 
		};
	}

	private Enums.Backpack.ItemSizeCellType[] GetItemSizeRotated270()
	{
		Enums.Backpack.ItemSizeCellType[] array = new Enums.Backpack.ItemSizeCellType[_maxItemWidth * _maxItemHeight];
		int maxItemWidth = _maxItemWidth;
		for (int i = 0; i < maxItemWidth; i++)
		{
			for (int j = 0; j < maxItemWidth; j++)
			{
				array[(maxItemWidth - j - 1) * maxItemWidth + i] = SizeInfo[i * maxItemWidth + j];
			}
		}
		return array;
	}

	private Enums.Backpack.ItemSizeCellType[] GetItemSizeRotated180()
	{
		return SizeInfo.ToArray().Reverse().ToArray();
	}

	public Enums.Backpack.ItemSizeCellType[] GetItemSizeRotated90()
	{
		Enums.Backpack.ItemSizeCellType[] array = new Enums.Backpack.ItemSizeCellType[_maxItemWidth * _maxItemHeight];
		int maxItemWidth = _maxItemWidth;
		for (int i = 0; i < maxItemWidth; i++)
		{
			for (int j = 0; j < maxItemWidth; j++)
			{
				array[i * maxItemWidth + j] = SizeInfo[(maxItemWidth - j - 1) * maxItemWidth + i];
			}
		}
		return array;
	}
}
