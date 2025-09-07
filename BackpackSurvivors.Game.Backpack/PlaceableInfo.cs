using System.Collections.Generic;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Backpack;

public class PlaceableInfo
{
	private int _itemWidth;

	private int _itemHeight;

	private int _totalWidth;

	private int _totalHeight;

	public int TotalWidth => _totalWidth;

	public int TotalHeight => _totalHeight;

	public int ItemWidth => _itemWidth;

	public int ItemHeight => _itemHeight;

	public List<Enums.Backpack.ItemSizeCellType> ItemSizeInfo { get; private set; }

	public PlaceableInfo(List<Enums.Backpack.ItemSizeCellType> itemSizeInfo)
	{
		ItemSizeInfo = itemSizeInfo;
		CalculateWidthAndHeight(itemSizeInfo, out _itemWidth, out _itemHeight, out _totalWidth, out _totalHeight);
	}

	public static void CalculateWidthAndHeight(List<Enums.Backpack.ItemSizeCellType> itemSizeInfo, out int itemWidth, out int itemHeight, out int totalWidth, out int totalHeight)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		HashSet<int> hashSet3 = new HashSet<int>();
		HashSet<int> hashSet4 = new HashSet<int>();
		for (int i = 0; i < itemSizeInfo.Count; i++)
		{
			Enums.Backpack.ItemSizeCellType itemSizeCellType = itemSizeInfo[i];
			if (itemSizeCellType != Enums.Backpack.ItemSizeCellType.None)
			{
				int item = i / 10;
				int item2 = i % 10;
				hashSet3.Add(item);
				hashSet4.Add(item2);
				if (itemSizeCellType == Enums.Backpack.ItemSizeCellType.CellContainsPlacable)
				{
					hashSet.Add(item);
					hashSet2.Add(item2);
				}
			}
		}
		itemWidth = hashSet2.Count;
		itemHeight = hashSet.Count;
		totalWidth = hashSet4.Count;
		totalHeight = hashSet3.Count;
	}
}
