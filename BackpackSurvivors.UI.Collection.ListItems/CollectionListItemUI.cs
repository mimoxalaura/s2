using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection.ListItems;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(MainButton))]
public class CollectionListItemUI : MonoBehaviour
{
	[SerializeField]
	private GameObject _selectedImage;

	public void ToggleSelect(bool selected)
	{
		_selectedImage.SetActive(selected);
	}

	public void Init(bool interactable)
	{
		GetComponent<MainButton>().ToggleHoverEffects(interactable);
		GetComponent<Button>().enabled = interactable;
	}
}
