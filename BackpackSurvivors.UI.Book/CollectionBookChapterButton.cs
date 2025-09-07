using BackpackSurvivors.Assets.UI.Book;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.Book;

internal class CollectionBookChapterButton : MonoBehaviour
{
	[SerializeField]
	private GameObject _highLightObject;

	[SerializeField]
	private BookController _controller;

	[SerializeField]
	private TextMeshProUGUI _collectedText;

	internal void Init(int currentKnown, int total)
	{
		string colorStringForTooltip = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase);
		string colorStringForTooltip2 = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase);
		string colorStringForTooltip3 = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase);
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (currentKnown == total)
		{
			empty = colorStringForTooltip;
			empty2 = colorStringForTooltip;
		}
		else
		{
			empty = colorStringForTooltip2;
			empty2 = colorStringForTooltip3;
		}
		_collectedText.SetText($"<color={empty}>{currentKnown}</color>/<color={empty2}>{total}</color>");
	}

	public void OnHover()
	{
		_highLightObject.SetActive(value: true);
	}

	public void OnExitHover()
	{
		_highLightObject.SetActive(value: false);
	}

	public void OnClicked(int tab)
	{
		_highLightObject.SetActive(value: false);
		if (tab == 1)
		{
			_controller.ShowTab1();
		}
		if (tab == 2)
		{
			_controller.ShowTab2();
		}
		if (tab == 3)
		{
			_controller.ShowTab3();
		}
		if (tab == 4)
		{
			_controller.ShowTab4();
		}
		if (tab == 5)
		{
			_controller.ShowTab5();
		}
		if (tab == 6)
		{
			_controller.ShowTab6();
		}
	}
}
