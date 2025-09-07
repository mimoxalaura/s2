using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Shared;

[RequireComponent(typeof(TMP_Text))]
public class LinkHandlerForTMPTextWithURLClickability : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private TMP_Text _tmpTextBox;

	private Canvas _canvasToCheck;

	private Camera _cameraToUse;

	public static event Action<string> ClickedOnLink;

	private void Awake()
	{
		_tmpTextBox = GetComponent<TMP_Text>();
		_canvasToCheck = GetComponentInParent<Canvas>();
		if (_canvasToCheck.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			_cameraToUse = null;
		}
		else
		{
			_cameraToUse = _canvasToCheck.worldCamera;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		int num = TMP_TextUtilities.FindIntersectingLink(position: new Vector3(eventData.position.x, eventData.position.y, 0f), text: _tmpTextBox, camera: _cameraToUse);
		if (num != -1)
		{
			TMP_LinkInfo tMP_LinkInfo = _tmpTextBox.textInfo.linkInfo[num];
			string linkID = tMP_LinkInfo.GetLinkID();
			if (linkID.StartsWith("http://") || linkID.StartsWith("https://"))
			{
				Application.OpenURL(linkID);
			}
			else
			{
				LinkHandlerForTMPTextWithURLClickability.ClickedOnLink?.Invoke(tMP_LinkInfo.GetLinkText());
			}
		}
	}
}
