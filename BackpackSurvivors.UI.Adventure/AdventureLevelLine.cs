using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureLevelLine : MonoBehaviour
{
	[SerializeField]
	private Image _lineImage;

	[SerializeField]
	private Material _nextLevelCompletedMaterial;

	[SerializeField]
	private Material _nextLevelIncompletedMaterial;

	public void Init(bool completed)
	{
		_lineImage.material = (completed ? _nextLevelCompletedMaterial : _nextLevelIncompletedMaterial);
	}
}
