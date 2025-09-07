using System;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Input.Events;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.Assets.Game.Revive;

internal class ReviveUI : ModalUI
{
	public delegate void ContinueButtonPressedHandler(object sender, EventArgs e);

	public delegate void GiveUpButtonPressedHandler(object sender, EventArgs e);

	[SerializeField]
	private Animator _uiAnimator;

	[SerializeField]
	private TextMeshProUGUI _killedByText;

	[SerializeField]
	private TextMeshProUGUI _timeAliveText;

	public event ContinueButtonPressedHandler OnContinueButtonPressed;

	public event GiveUpButtonPressedHandler OnGiveUpButtonPressed;

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
		ShowReviveUI();
	}

	internal void SetInformationValues(Character killedBy)
	{
		string text = "???";
		if (killedBy != null)
		{
			text = ((Enemy)killedBy).BaseEnemy.Name;
		}
		string text2 = TimeSpan.FromSeconds(SingletonController<StatisticsController>.Instance.GetAdventureDuration(useCurrentTime: true)).ToString("hh':'mm':'ss");
		_killedByText.SetText("by <b><color=#FF3E3E>" + text + "</b></color>");
		_timeAliveText.SetText("Time alive: <b>" + text2 + "</b>");
	}

	public void ShowReviveUI()
	{
		SingletonController<InputController>.Instance.OnSpecial1Handler += Instance_OnSpecial1Handler;
		SingletonController<InputController>.Instance.OnRotateHandler += Instance_OnRotateHandler;
		_uiAnimator.SetBool("Shown", value: true);
		_uiAnimator.SetTrigger("Show");
	}

	private void Instance_OnRotateHandler(object sender, RotationEventArgs e)
	{
		GiveUpButtonPressed();
	}

	private void Instance_OnSpecial1Handler(object sender, EventArgs e)
	{
		ContinueButtonPressed();
	}

	public void ContinueButtonPressed()
	{
		_uiAnimator.SetBool("Shown", value: false);
		this.OnContinueButtonPressed?.Invoke(this, new EventArgs());
	}

	public void GiveUpButtonPressed()
	{
		_uiAnimator.SetBool("Shown", value: false);
		this.OnGiveUpButtonPressed?.Invoke(this, new EventArgs());
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnSpecial1Handler -= Instance_OnSpecial1Handler;
		SingletonController<InputController>.Instance.OnRotateHandler -= Instance_OnRotateHandler;
	}
}
