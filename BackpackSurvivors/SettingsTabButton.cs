using UnityEngine;

namespace BackpackSurvivors;

public class SettingsTabButton : MainButton
{
	[SerializeField]
	private GameObject _selectedContainer;

	[SerializeField]
	private GameObject _activeContainer;

	[SerializeField]
	private float _defaultWidth = 192f;

	[SerializeField]
	private float _selectedWidth = 224f;

	[SerializeField]
	private SettingsTabButton[] _otherButtons;

	public override void OnHover()
	{
		base.OnHover();
		_selectedContainer.SetActive(value: true);
	}

	public override void OnExitHover()
	{
		base.OnHover();
		_selectedContainer.SetActive(value: false);
	}

	public override void OnClick()
	{
		base.OnClick();
		_activeContainer.SetActive(value: true);
		LeanTween.scaleX(_activeContainer.gameObject, _selectedWidth, 0.2f);
		SettingsTabButton[] otherButtons = _otherButtons;
		for (int i = 0; i < otherButtons.Length; i++)
		{
			otherButtons[i].DeactiveSelection();
		}
	}

	public void DeactiveSelection()
	{
		LeanTween.scaleX(_activeContainer.gameObject, _defaultWidth, 0.2f);
		_activeContainer.SetActive(value: false);
	}
}
