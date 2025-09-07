using BackpackSurvivors.Game.Player;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

internal class VisibleOnMinimapAfterInRange : MonoBehaviour
{
	[SerializeField]
	private MinimapPoint _minimapPoint;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.gameObject.GetComponent<Player>() == null))
		{
			_minimapPoint.ActivateMinimapClamp();
			base.gameObject.SetActive(value: false);
		}
	}
}
