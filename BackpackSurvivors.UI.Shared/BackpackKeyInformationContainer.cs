using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Shared;

public class BackpackKeyInformationContainer : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private BackpackKeyInformation[] _backpackKeyInformations;

	public void OnPointerEnter(PointerEventData eventData)
	{
		BackpackKeyInformation[] backpackKeyInformations = _backpackKeyInformations;
		for (int i = 0; i < backpackKeyInformations.Length; i++)
		{
			backpackKeyInformations[i].FadeInformationElements(fadeIn: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		BackpackKeyInformation[] backpackKeyInformations = _backpackKeyInformations;
		for (int i = 0; i < backpackKeyInformations.Length; i++)
		{
			backpackKeyInformations[i].FadeInformationElements(fadeIn: false);
		}
	}
}
