using UnityEngine;

namespace BackpackSurvivors.UI;

internal class WishlistButton : MonoBehaviour
{
	public void OnButtonClick()
	{
		Application.OpenURL("steam://openurl/https://store.steampowered.com/app/2294780/Backpack_Survivors#game_area_purchase");
	}
}
