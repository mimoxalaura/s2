using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using UnityEngine;

namespace BackpackSurvivors.Game.Quests;

public class QuestController : BaseSingletonModalUIController<QuestController>
{
	[SerializeField]
	private QuestUI _questUI;

	private void Start()
	{
		base.IsInitialized = true;
	}

	public override void OpenUI()
	{
		base.OpenUI();
		_questUI.gameObject.SetActive(value: true);
		_questUI.OpenUI();
		_questUI.Init();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		_questUI.CloseUI();
	}

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Quests;
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}
}
