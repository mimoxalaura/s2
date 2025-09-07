using System;
using System.Text.RegularExpressions;
using BackpackSurvivors.Combat.ScriptableObjects;
using BackpackSurvivors.UI.Tooltip;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Talents;

[CreateAssetMenu(fileName = "Talent", menuName = "Game/Talents/Talent", order = 1)]
public class TalentSO : ScriptableObject
{
	public Texture _PreviewIcon;

	public string _Id;

	[HelpBox("$_Description", null)]
	public string _Title;

	[HideInInspector]
	[TextArea]
	public string _Description;

	public bool _IsKeyStone;

	[SerializeField]
	public int Id;

	[SerializeField]
	public string Name;

	[SerializeField]
	public string Description;

	[SerializeField]
	public bool UseCustomDescription;

	[SerializeField]
	public Sprite Icon;

	[SerializeField]
	public bool IsKeystone;

	[SerializeField]
	public ItemStatsSO Stats;

	private void CreateData()
	{
		if (Icon != null)
		{
			_PreviewIcon = Icon.texture;
		}
		_Title = Name;
		_Id = Id.ToString();
		_IsKeyStone = IsKeystone;
		string description = TalentTooltip.CreateTooltipDescription(active: false, showActivatedPart: false, this);
		description = CleanDescription(description);
		_Description = description;
	}

	private string CleanDescription(string description)
	{
		string text = description;
		int num = 0;
		string text2 = "<sprite name=\"";
		string endTag = "\">";
		while (text.Contains(text2) && num < 10)
		{
			text = RemoveBetween(text, text2, endTag);
			text = text.Replace("<sprite name=\"\">", string.Empty);
			text = text.Replace(Environment.NewLine, string.Empty);
			num++;
		}
		return text;
	}

	private string RemoveBetween(string sourceString, string startTag, string endTag)
	{
		return new Regex($"{Regex.Escape(startTag)}(.*?){Regex.Escape(endTag)}", RegexOptions.RightToLeft).Replace(sourceString, startTag + endTag);
	}
}
