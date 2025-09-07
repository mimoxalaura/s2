using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Shop;

public class SellArea : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private TextMeshProUGUI _sellForText;

	[SerializeField]
	private GameObject _sellForGameObject;

	[SerializeField]
	private GameObject _sellAreaHighlight;

	[SerializeField]
	private GameObject _sellInformationGameObject;

	[SerializeField]
	private Animator _vendorAnimator;

	public bool IsCurrentlyHovered { get; private set; }

	internal void ResetCurrentlyHovered()
	{
		IsCurrentlyHovered = false;
	}

	private void Awake()
	{
		_sellForGameObject.SetActive(value: false);
	}

	public void ShowSellText(int sellForPrice)
	{
		_sellForText.text = $"<color={Constants.Colors.HexStrings.SellForColor}>{sellForPrice}</color>";
		_sellForGameObject.SetActive(value: true);
		_sellInformationGameObject.SetActive(value: false);
		_vendorAnimator.SetBool("VendorSelling", value: true);
	}

	public void HideSellText()
	{
		_sellForGameObject.SetActive(value: false);
		_vendorAnimator.SetBool("VendorSelling", value: false);
	}

	public void HighlightSellArea(bool show)
	{
		_sellAreaHighlight.SetActive(show);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (SingletonCacheController.Instance.GetControllerByType<DragController>().IsDragging)
		{
			IsCurrentlyHovered = true;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		IsCurrentlyHovered = false;
	}
}
