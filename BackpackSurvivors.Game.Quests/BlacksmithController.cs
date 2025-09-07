using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using UnityEngine;

namespace BackpackSurvivors.Game.Quests;

public class BlacksmithController : BaseSingletonModalUIController<QuestController>
{
	[SerializeField]
	private BlacksmithUI _blacksmithUI;

	private void Start()
	{
		base.IsInitialized = true;
	}

	public override void OpenUI()
	{
		base.OpenUI();
		_blacksmithUI.gameObject.SetActive(value: true);
		_blacksmithUI.OpenUI();
		_blacksmithUI.Init();
	}

	public override void CloseUI()
	{
		base.CloseUI();
		_blacksmithUI.CloseUI();
	}

	public override bool CloseOnCancelInput()
	{
		return true;
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Blacksmith;
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
