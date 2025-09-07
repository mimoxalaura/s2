using System;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EasyAspectRatio;

[AddComponentMenu("Easy Aspect Ratio/Aspect Ratio Control")]
public class AspectRatioControl : MonoBehaviour
{
	public Vector2 targetAspectRatio = new Vector2(16f, 9f);

	private Camera blackBarCamera;

	private float _lastScreenWidth;

	private float _lastScreenHeight;

	public event EventHandler OnAspectRatioUpdated;

	private void Start()
	{
		SetupLetterbox();
	}

	[Command("letterbox.info", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void DEBUG_Letterbox()
	{
		float num = targetAspectRatio.x / targetAspectRatio.y;
		Debug.Log("Letterbox:");
		float num2 = (float)Display.main.systemWidth / (float)Display.main.systemHeight;
		Debug.Log($"actual windowratio: {num2}");
		float num3 = (float)Screen.width / (float)Screen.height;
		Debug.Log($"windowratio: {num3}");
		float num4 = num3 / num;
		Debug.Log($"scaleheight: {num4}");
		Debug.Log($"rect: {GetComponent<Camera>().rect.size}");
		Debug.Log($"Screen.width: {Screen.width}");
		Debug.Log($"Screen.height: {Screen.height}");
		Debug.Log($"Actual Screen.width: {Screen.currentResolution.width}");
		Debug.Log($"Actual Screen.height: {Screen.currentResolution.height}");
	}

	[Command("letterbox.setup", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void SetupLetterbox()
	{
		float num = targetAspectRatio.x / targetAspectRatio.y;
		float num2 = (float)Screen.width / (float)Screen.height / num;
		if (num2 >= 0.99f && num2 <= 1.01f)
		{
			return;
		}
		Camera component = GetComponent<Camera>();
		Rect rect = component.rect;
		if (num2 < 1f)
		{
			rect = component.rect;
			rect.width = 1f;
			rect.height = num2;
			rect.x = 0f;
			rect.y = (1f - num2) / 2f;
			component.rect = rect;
		}
		else
		{
			float num3 = 1f / num2;
			rect = component.rect;
			rect.width = num3;
			rect.height = 1f;
			rect.x = (1f - num3) / 2f;
			rect.y = 0f;
			component.rect = rect;
		}
		if (blackBarCamera == null)
		{
			blackBarCamera = new GameObject("BlackBar Camera").AddComponent<Camera>();
			blackBarCamera.clearFlags = CameraClearFlags.Color;
			blackBarCamera.backgroundColor = Color.black;
			blackBarCamera.depth = Camera.main.depth - 1f;
			blackBarCamera.transform.SetParent(Camera.main.transform);
			blackBarCamera.transform.position = Vector3.zero;
		}
		foreach (Camera item in Camera.main.GetComponent<UniversalAdditionalCameraData>().cameraStack)
		{
			if (item != null)
			{
				item.rect = rect;
			}
		}
		this.OnAspectRatioUpdated?.Invoke(this, new EventArgs());
	}

	private void OnDestroy()
	{
		this.OnAspectRatioUpdated = null;
	}

	private void Update()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		if (_lastScreenWidth != num || _lastScreenHeight != num2)
		{
			_lastScreenWidth = num;
			_lastScreenHeight = num2;
			SetupLetterbox();
		}
	}
}
