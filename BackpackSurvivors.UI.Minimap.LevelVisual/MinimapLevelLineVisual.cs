using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Minimap.LevelVisual;

internal class MinimapLevelLineVisual : MonoBehaviour
{
	[SerializeField]
	private Image _lineImage;

	public void SetFillState(float fillState)
	{
		_lineImage.fillAmount = fillState;
	}
}
