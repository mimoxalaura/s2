using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Unlockables;

public class UnlocksItemUI : MonoBehaviour
{
	public delegate void OnClickHandler(object sender, UnlockItemSelectedEventArgs e);

	[Header("Visuals")]
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TextMeshProUGUI _titleText;

	[SerializeField]
	private TextMeshProUGUI _pointsSpendText;

	[SerializeField]
	private TextMeshProUGUI _descriptionText;

	[SerializeField]
	private GameObject _selectedImage;

	[SerializeField]
	private GameObject _hoveredImage;

	[SerializeField]
	private GameObject _fullyUnlocked;

	[SerializeField]
	private Animator _animator;

	public UnlockableItem UnlockableItem;

	public string UnlockName => UnlockableItem.BaseUnlockable.Name;

	public Enums.Unlockable Unlockable => UnlockableItem.BaseUnlockable.Unlockable;

	public event OnClickHandler OnClick;

	public void Init(UnlockableItem unlockableItem)
	{
		UnlockableItem = unlockableItem;
		_icon.sprite = unlockableItem.BaseUnlockable.Icon;
		_titleText.SetText(unlockableItem.BaseUnlockable.Name);
		_descriptionText.SetText(unlockableItem.BaseUnlockable.Description);
		string text = $"{unlockableItem.PointsInvested}/{unlockableItem.BaseUnlockable.UnlockAvailableAmount}";
		if (unlockableItem.Completed)
		{
			text = TextMeshProStringHelper.AddColor(Constants.Colors.HexStrings.Green, text);
			_fullyUnlocked.SetActive(value: true);
		}
		_pointsSpendText.SetText(text);
	}

	public void ShowVFX()
	{
		_animator.SetTrigger("Picked");
	}

	public void ShowFullyCompletedVFX()
	{
		_animator.SetTrigger("PickedCompleted");
	}

	public void Select()
	{
		_selectedImage.SetActive(value: true);
	}

	public void UnSelect()
	{
		_selectedImage.SetActive(value: false);
	}

	public void Clicked()
	{
		this.OnClick?.Invoke(this, new UnlockItemSelectedEventArgs(UnlockableItem));
	}

	public void Hovered()
	{
		_hoveredImage.SetActive(value: true);
	}

	public void ExitHovered()
	{
		_hoveredImage.SetActive(value: false);
	}
}
