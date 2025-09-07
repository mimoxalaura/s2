using System;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.Tooltip;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(InlineTextTooltipTrigger))]
public class InlineTooltipHelper : MonoBehaviour
{
	[SerializeField]
	private Camera uiCamera;

	private TextMeshProUGUI _textElement;

	private InlineTextTooltipTrigger _inlineTextTooltipTrigger;

	private string linkId;

	private void Start()
	{
		_textElement = GetComponent<TextMeshProUGUI>();
		_inlineTextTooltipTrigger = GetComponent<InlineTextTooltipTrigger>();
	}

	private void Update()
	{
		Vector3 mousePosition = Input.mousePosition;
		int num = TMP_TextUtilities.FindIntersectingLink(_textElement, mousePosition, uiCamera);
		if (num != -1)
		{
			TMP_LinkInfo tMP_LinkInfo = _textElement.textInfo.linkInfo[num];
			string linkID = tMP_LinkInfo.GetLinkID();
			if (!(linkId != linkID))
			{
				return;
			}
			linkId = linkID;
			{
				foreach (Enums.Debuff.DebuffType value in Enum.GetValues(typeof(Enums.Debuff.DebuffType)))
				{
					if (StringHelper.GetCleanString(value) == linkId)
					{
						TipFeedbackElement tipFeedbackElement = new TipFeedbackElement();
						tipFeedbackElement.Name = StringHelper.GetColoredName(value);
						tipFeedbackElement.Description = StringHelper.GetDescription(value);
						tipFeedbackElement.Icon = SpriteHelper.GetDebuffIcon(value);
						SingletonController<TooltipController>.Instance.ShowInlineTooltip(tipFeedbackElement, _inlineTextTooltipTrigger);
						Debug.Log("showing inline");
					}
				}
				return;
			}
		}
		Debug.Log("Hiding inline");
		linkId = string.Empty;
		SingletonController<TooltipController>.Instance.HideInlineTooltip();
	}
}
