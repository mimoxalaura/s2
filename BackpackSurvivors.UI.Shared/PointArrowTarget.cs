using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

public class PointArrowTarget : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _onTargetSprite;

	public void ToggleInRange(bool inRange)
	{
		_onTargetSprite.enabled = inRange;
	}
}
