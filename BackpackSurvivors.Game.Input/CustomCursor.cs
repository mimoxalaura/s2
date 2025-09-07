using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Input;

internal class CustomCursor : MonoBehaviour
{
	[SerializeField]
	private Image _cursorImage;

	private RectTransform _cursorRectTransform;

	internal Vector2 CursorImageSize => _cursorRectTransform.sizeDelta;

	private void Awake()
	{
		_cursorRectTransform = GetComponent<RectTransform>();
	}

	internal void ScaleSize(float scale)
	{
		_cursorImage.transform.LeanScale(new Vector3(scale, scale, 1f), 0f);
	}

	internal void SetCursorVisible(bool visible)
	{
		_cursorImage.gameObject.SetActive(visible);
	}
}
