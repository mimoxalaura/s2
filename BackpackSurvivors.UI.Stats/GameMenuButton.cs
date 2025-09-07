using UnityEngine;

namespace BackpackSurvivors.UI.Stats;

public class GameMenuButton : MainButton
{
	[SerializeField]
	private GameObject _selectedContainer;

	[SerializeField]
	private Color _color;

	internal void SetIconColor()
	{
		SetIconColor(_color);
	}

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
}
