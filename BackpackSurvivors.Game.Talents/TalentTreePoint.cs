using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Talents.Events;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Talents;

public class TalentTreePoint : TalentNode
{
	public delegate void OnTalentSelectedHandler(object sender, TalentSelectedEventArgs e);

	[Header("Base")]
	[SerializeField]
	private TalentTooltipTrigger _tooltipTrigger;

	[SerializeField]
	private TalentSO _talentSO;

	[SerializeField]
	private TextMeshProUGUI _DEBUGTEXT;

	[SerializeField]
	private TextMeshProUGUI _DEBUGKEYSTONETEXT;

	[Header("Regular")]
	[SerializeField]
	private GameObject _normalContainer;

	[SerializeField]
	private Image _normalImage;

	[SerializeField]
	private Image _inactiveImage;

	[SerializeField]
	private Image _activeBorderImage;

	[Header("Keynode")]
	[SerializeField]
	private GameObject _keynodeContainer;

	[SerializeField]
	private Image _normalKeynodeImage;

	[SerializeField]
	private Image _inactiveKeynodeImage;

	[Header("Animator")]
	[SerializeField]
	private Image _activeHighlight;

	[SerializeField]
	private Image _keystoneActivatedEffectImage;

	[SerializeField]
	private ImageAnimation _keystoneActivatedAnimationImage;

	[SerializeField]
	private Image _regularPointActivatedEffectImage;

	[SerializeField]
	private ImageAnimation _regularPointActivatedAnimationImage;

	[Header("Border")]
	[SerializeField]
	private Image _regularNodeFoundBorder;

	[SerializeField]
	private Image _keystoneFoundBorder;

	private bool _activated;

	public List<TalentTreePoint> Neighbors { get; set; }

	public event OnTalentSelectedHandler OnTalentSelected;

	public override int GetId()
	{
		return _talentSO.Id;
	}

	public override bool IsKeystone()
	{
		return _talentSO.IsKeystone;
	}

	public override bool IsActive()
	{
		return _activated;
	}

	public void SetTalent(TalentSO talentSO)
	{
		_talentSO = talentSO;
		if (talentSO.IsKeystone)
		{
			_normalContainer.SetActive(value: false);
			_keynodeContainer.SetActive(value: true);
			_normalKeynodeImage.sprite = talentSO.Icon;
			_inactiveKeynodeImage.sprite = talentSO.Icon;
		}
		else
		{
			_normalContainer.SetActive(value: true);
			_keynodeContainer.SetActive(value: false);
			_normalImage.sprite = talentSO.Icon;
			_inactiveImage.sprite = talentSO.Icon;
		}
	}

	public void TalentSelected()
	{
		this.OnTalentSelected?.Invoke(this, new TalentSelectedEventArgs(_talentSO, _activated));
	}

	public override void Activate(bool animate = true)
	{
		_activated = true;
		UpdateTooltip();
		if (IsKeystone())
		{
			_normalKeynodeImage.gameObject.SetActive(value: true);
			_inactiveKeynodeImage.gameObject.SetActive(value: false);
			KeystoneAnimation(animate);
		}
		else
		{
			_normalImage.gameObject.SetActive(value: true);
			_activeBorderImage.gameObject.SetActive(value: true);
			_inactiveImage.gameObject.SetActive(value: false);
			RegularPointAnimation(animate);
		}
	}

	public override void Deactivate()
	{
		_activated = false;
		UpdateTooltip();
		if (IsKeystone())
		{
			_normalKeynodeImage.gameObject.SetActive(value: false);
			_inactiveKeynodeImage.gameObject.SetActive(value: true);
		}
		else
		{
			_normalImage.gameObject.SetActive(value: false);
			_activeBorderImage.gameObject.SetActive(value: false);
			_inactiveImage.gameObject.SetActive(value: true);
		}
	}

	public override void SetHighlightInSearch(bool highlight)
	{
		if (IsKeystone())
		{
			_keystoneFoundBorder.gameObject.SetActive(highlight);
		}
		else
		{
			_regularNodeFoundBorder.gameObject.SetActive(highlight);
		}
	}

	public override void Init(bool debug = true)
	{
		if (debug)
		{
			_DEBUGKEYSTONETEXT.SetText(_talentSO.Id.ToString());
			_DEBUGTEXT.SetText(_talentSO.Id.ToString());
		}
		else
		{
			_DEBUGKEYSTONETEXT.SetText(string.Empty);
			_DEBUGTEXT.SetText(string.Empty);
		}
		Neighbors = new List<TalentTreePoint>();
		_tooltipTrigger.Init(this);
		_tooltipTrigger.SetTooltipContent(_talentSO, active: false);
	}

	public void AddNeighbor(TalentTreePoint neighbor)
	{
		if (!Neighbors.Contains(neighbor))
		{
			Neighbors.Add(neighbor);
			neighbor.AddNeighbor(this);
		}
	}

	private void KeystoneAnimation(bool animate)
	{
		_keystoneActivatedEffectImage.gameObject.SetActive(value: true);
		_keystoneActivatedAnimationImage.enabled = false;
		_keystoneActivatedAnimationImage.enabled = true;
		_keystoneActivatedAnimationImage.ResetToBeginning();
		Color white = Color.white;
		Color color = new Color(0f, 0f, 0f, 0f);
		_keystoneActivatedEffectImage.color = white;
		if (animate)
		{
			LeanTween.value(_keystoneActivatedEffectImage.gameObject, SetKeystoneColorCallback, white, color, 0.5f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		}
		else
		{
			_keystoneActivatedEffectImage.color = color;
		}
	}

	private void SetKeystoneColorCallback(Color c)
	{
		_keystoneActivatedEffectImage.color = c;
		Color color = _keystoneActivatedEffectImage.color;
		_keystoneActivatedEffectImage.color = color;
	}

	private void RegularPointAnimation(bool animate)
	{
		_regularPointActivatedEffectImage.gameObject.SetActive(value: true);
		_regularPointActivatedAnimationImage.enabled = false;
		_regularPointActivatedAnimationImage.enabled = true;
		_regularPointActivatedAnimationImage.ResetToBeginning();
		Color white = Color.white;
		Color color = new Color(0f, 0f, 0f, 0f);
		_regularPointActivatedEffectImage.color = white;
		if (animate)
		{
			LeanTween.value(_regularPointActivatedEffectImage.gameObject, SetRegularPointColorCallback, white, color, 0.5f).setDelay(0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		}
		else
		{
			_regularPointActivatedEffectImage.color = color;
		}
	}

	private void SetRegularPointColorCallback(Color c)
	{
		_regularPointActivatedEffectImage.color = c;
		Color color = _regularPointActivatedEffectImage.color;
		_regularPointActivatedEffectImage.color = color;
	}

	private void UpdateTooltip()
	{
		if (_tooltipTrigger != null && _talentSO != null)
		{
			_tooltipTrigger.SetTooltipContent(_talentSO, _activated);
			_tooltipTrigger.UpdateContentVisual();
		}
	}

	public bool ShouldShowActivatedPart()
	{
		bool flag = SingletonController<NewTalentController>.Instance.HasAnyActiveNeighbors(_talentSO.Id);
		return _activated || flag;
	}

	internal override bool FitsSearch(string searchTerm)
	{
		if (_talentSO == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(searchTerm))
		{
			return false;
		}
		string cleanedSearchTerm = searchTerm.ToLower();
		if (_talentSO.Name.ToLower().Contains(cleanedSearchTerm))
		{
			return true;
		}
		if (_talentSO.UseCustomDescription && _talentSO.Description.ToLower().Contains(cleanedSearchTerm))
		{
			return true;
		}
		if (_talentSO.Stats != null && _talentSO.Stats.DamageTypeValues.Any((KeyValuePair<Enums.DamageType, float> x) => x.Key.ToString().ToLower().Contains(cleanedSearchTerm)))
		{
			return true;
		}
		if (_talentSO.Stats != null && _talentSO.Stats.StatValues.Any((KeyValuePair<Enums.ItemStatType, float> x) => x.Key.ToString().ToLower().Contains(cleanedSearchTerm)))
		{
			return true;
		}
		if (_talentSO.Stats != null && _talentSO.Stats.Conditions != null)
		{
			ConditionSO[] conditions = _talentSO.Stats.Conditions;
			foreach (ConditionSO conditionSO in conditions)
			{
				if (conditionSO.ItemStatType.ToString().ToLower().Contains(cleanedSearchTerm))
				{
					return true;
				}
				if (conditionSO.DamageType.ToString().ToLower().Contains(cleanedSearchTerm))
				{
					return true;
				}
				if (conditionSO.PlaceableType.ToString().ToLower().Contains(cleanedSearchTerm))
				{
					return true;
				}
				if (conditionSO.PlaceableRarity.ToString().ToLower().Contains(cleanedSearchTerm))
				{
					return true;
				}
				if (conditionSO.ItemSubtypeTag.ToString().ToLower().Contains(cleanedSearchTerm))
				{
					return true;
				}
				if (conditionSO.WeaponType.ToString().ToLower().Contains(cleanedSearchTerm))
				{
					return true;
				}
				if (conditionSO.WeaponSubtypeTag.ToString().ToLower().Contains(cleanedSearchTerm))
				{
					return true;
				}
			}
		}
		return false;
	}
}
