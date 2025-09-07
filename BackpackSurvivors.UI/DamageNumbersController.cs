using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.UI;

public class DamageNumbersController : MonoBehaviour
{
	[SerializeField]
	private NumberPopup _numberPopupPrefab;

	[SerializeField]
	private TextPopup _textPopupPrefab;

	[SerializeField]
	private float offset;

	public void InstantiateNumberPopup(Vector2 position, int number, Color color, bool attentionEffect)
	{
		if (!LeanTweenHelper.IsAtMaxCapacity())
		{
			float x = Random.Range(position.x - offset, position.x + offset);
			Object.Instantiate(_numberPopupPrefab, new Vector3(x, position.y), Quaternion.identity).Init(number, color, attentionEffect);
		}
	}

	public void InstantiateTextPopup(Vector2 position, string text, Color color, float fadeOutTime = 1f, float movementY = 0.5f)
	{
		if (!LeanTweenHelper.IsAtMaxCapacity())
		{
			float x = Random.Range(position.x - offset, position.x + offset);
			Object.Instantiate(_textPopupPrefab, new Vector3(x, position.y), Quaternion.identity).Init(text, color, fadeOutTime, movementY);
		}
	}
}
