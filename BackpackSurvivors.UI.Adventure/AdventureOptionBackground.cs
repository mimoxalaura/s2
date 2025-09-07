using BackpackSurvivors.ScriptableObjects.Adventures;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureOptionBackground : MonoBehaviour
{
	[SerializeField]
	private Image _adventureBackgroundImage;

	private AdventureSO _adventure;

	internal AdventureSO Adventure => _adventure;

	public void Init(AdventureSO adventure)
	{
		_adventure = adventure;
		_adventureBackgroundImage.sprite = adventure.BackgroundImage;
	}
}
