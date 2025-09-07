using System;
using EasyAspectRatio;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Input;

internal class InputCanvasResolutionSetter : MonoBehaviour
{
	[SerializeField]
	private CustomCursor _customCursor;

	private CanvasScaler scaler;

	private void Awake()
	{
		scaler = GetComponent<CanvasScaler>();
		SetReferenceResolution();
	}

	private void Start()
	{
		UnityEngine.Object.FindObjectOfType<AspectRatioControl>().OnAspectRatioUpdated += InputCanvasResolutionSetter_OnAspectRatioUpdated;
	}

	private void InputCanvasResolutionSetter_OnAspectRatioUpdated(object sender, EventArgs e)
	{
		SetReferenceResolution();
	}

	private void SetReferenceResolution()
	{
		scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
		float scale = (float)Screen.width / 1920f;
		_customCursor.ScaleSize(scale);
	}
}
