using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

internal class SpawnPosition : MonoBehaviour
{
	[SerializeField]
	public bool IsBossSpawnPoint;

	private bool _isActive = true;

	public bool IsActive => _isActive;

	public void ToggleSpriteRenderer()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		component.enabled = !component.enabled;
	}

	public void SetActiveState(bool isActive)
	{
		_isActive = isActive;
	}
}
