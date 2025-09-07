using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors;

public class MainMenuButton : MainButton
{
	[SerializeField]
	private GameObject _selectedContainer;

	public bool Interactable
	{
		get
		{
			return GetComponent<Button>().interactable;
		}
		set
		{
			GetComponent<Button>().interactable = value;
		}
	}

	public override void OnHover()
	{
		base.OnHover();
		Button component = GetComponent<Button>();
		if (component != null && component.interactable)
		{
			_selectedContainer.SetActive(value: true);
		}
	}

	public override void OnExitHover()
	{
		base.OnHover();
		Button component = GetComponent<Button>();
		if (component != null && component.interactable)
		{
			_selectedContainer.SetActive(value: false);
		}
	}
}
