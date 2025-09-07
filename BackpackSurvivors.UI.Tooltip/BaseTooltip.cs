using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class BaseTooltip : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _headerObject;

	[SerializeField]
	private TextMeshProUGUI _contentObject;

	[SerializeField]
	private LayoutElement _layoutElement;

	[SerializeField]
	private int _characterWrapLimit;

	[SerializeField]
	private Canvas _canvas;

	[SerializeField]
	private bool _moveWithCursor = true;

	private RectTransform _rect;

	private bool _followCursor = true;

	private int _tagPriorityRarity = 1;

	private int _tagPriorityMeleeRanged = 2;

	private int _tagPriorityItemType = 3;

	private int _tagPriorityDamageTypes = 4;

	private int _tagPriorityWeaponTypes = 5;

	private int _tagPriorityIngredient = 6;

	private int _tagPriorityUnique = 7;

	private RectTransform GetRectTransform()
	{
		if (_rect == null)
		{
			_rect = GetComponent<RectTransform>();
		}
		return _rect;
	}

	private void Update()
	{
		RepositionTooltip();
	}

	public void SetFollowCursor(bool follow)
	{
		_followCursor = follow;
	}

	public void RepositionTooltip()
	{
		if (_followCursor && _moveWithCursor)
		{
			SetPivotPoint();
		}
	}

	private void SetPivotPoint()
	{
		RectTransform rectTransform = GetRectTransform();
		Vector2 cursorPosition = SingletonController<InputController>.Instance.CursorPosition;
		Vector2 basePivotPoint = GetBasePivotPoint(cursorPosition);
		Vector2 pivotToKeepTooltipWithinScreenBounds = GetPivotToKeepTooltipWithinScreenBounds(cursorPosition, basePivotPoint, rectTransform.sizeDelta);
		pivotToKeepTooltipWithinScreenBounds = GetPivotWithCursorCorrection(cursorPosition, pivotToKeepTooltipWithinScreenBounds, rectTransform.sizeDelta.y);
		rectTransform.pivot = pivotToKeepTooltipWithinScreenBounds;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, cursorPosition, _canvas.worldCamera, out var localPoint);
		base.transform.position = _canvas.transform.TransformPoint(localPoint);
	}

	public void SetText(string content, string header = "", string price = "")
	{
		_headerObject.gameObject.SetActive(!string.IsNullOrEmpty(header));
		_contentObject.gameObject.SetActive(!string.IsNullOrEmpty(content));
		_headerObject.SetText(header);
		_contentObject.SetText(content);
		RepositionTooltip();
	}

	internal virtual void ToggleAltTooltip(bool show)
	{
	}

	internal IEnumerable<TipFeedbackElement> GetTipFeedbackElementsFromDebuffs(DebuffSO[] debuffSOs)
	{
		List<TipFeedbackElement> list = new List<TipFeedbackElement>();
		foreach (DebuffSO debuffSO in debuffSOs)
		{
			TipFeedbackElement tipFeedbackElement = new TipFeedbackElement();
			tipFeedbackElement.Name = StringHelper.GetColoredName(debuffSO.DebuffType);
			tipFeedbackElement.Description = StringHelper.GetDescription(debuffSO.DebuffType);
			tipFeedbackElement.Icon = SpriteHelper.GetDebuffIcon(debuffSO.DebuffType);
			list.Add(tipFeedbackElement);
		}
		return list;
	}

	private Vector2 GetPivotToKeepTooltipWithinScreenBounds(Vector3 cursorPosition, Vector2 pivotPoint, Vector2 tooltipSize)
	{
		float correctedXPivot = GetCorrectedXPivot(cursorPosition, pivotPoint.x, tooltipSize.x);
		float correctedYPivot = GetCorrectedYPivot(cursorPosition, pivotPoint.y, tooltipSize.y);
		return new Vector2(correctedXPivot, correctedYPivot);
	}

	private float GetCorrectedXPivot(Vector3 cursorPosition, float pivotX, float width)
	{
		if (IsCursorOnLeftHalfOfScreen(cursorPosition))
		{
			float num = cursorPosition.x + width - (float)Screen.width;
			if (num <= 0f)
			{
				return pivotX;
			}
			return num / width;
		}
		float num2 = cursorPosition.x - width;
		float num3 = 0f - num2;
		if (num3 <= 0f)
		{
			return pivotX;
		}
		float value = num3 / width;
		return 1f - Math.Abs(value);
	}

	private float GetCorrectedYPivot(Vector3 cursorPosition, float pivotY, float height)
	{
		if (IsCursorOnBottomHalfOfScreen(cursorPosition))
		{
			float num = cursorPosition.y + height - (float)Screen.height;
			if (num <= 0f)
			{
				return pivotY;
			}
			return num / height;
		}
		float num2 = cursorPosition.y - height;
		float num3 = 0f - num2;
		if (num3 <= 0f)
		{
			return pivotY;
		}
		float f = num3 / height;
		return 1f - Mathf.Abs(f);
	}

	private Vector2 GetPivotWithCursorCorrection(Vector2 cursorPosition, Vector2 correctedPivot, float height)
	{
		if (IsCursorOnBottomHalfOfScreen(cursorPosition))
		{
			return correctedPivot;
		}
		float num = SingletonController<InputController>.Instance.CursorImageSize.y / height;
		float y = correctedPivot.y + num;
		return new Vector2(correctedPivot.x, y);
	}

	private Vector2 GetBasePivotPoint(Vector2 cursorPosition)
	{
		bool flag = IsCursorOnLeftHalfOfScreen(cursorPosition);
		bool num = IsCursorOnBottomHalfOfScreen(cursorPosition);
		int num2 = ((!flag) ? 1 : 0);
		int num3 = ((!num) ? 1 : 0);
		return new Vector2(num2, num3);
	}

	private bool IsCursorOnLeftHalfOfScreen(Vector3 mousePointerPosition)
	{
		return mousePointerPosition.x <= (float)Screen.width / 2f;
	}

	private bool IsCursorOnBottomHalfOfScreen(Vector3 mousePointerPosition)
	{
		return mousePointerPosition.y <= (float)Screen.height / 2f;
	}

	internal void ClearContainer(Transform container)
	{
		for (int num = container.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(container.GetChild(num).gameObject);
		}
	}

	internal void ClearContainer(Transform container, Type typeToClear)
	{
		for (int num = container.childCount - 1; num >= 0; num--)
		{
			if (container.GetChild(num).GetComponent(typeToClear.ToString()) != null)
			{
				UnityEngine.Object.Destroy(container.GetChild(num).gameObject);
			}
		}
	}

	internal IOrderedEnumerable<Enums.PlaceableTag> OrderTags(IEnumerable<Enums.PlaceableTag> unsortedList)
	{
		return unsortedList.OrderBy((Enums.PlaceableTag x) => GetTagPriority(x));
	}

	private int GetTagPriority(Enums.PlaceableTag placeableTag)
	{
		if (placeableTag <= Enums.PlaceableTag.Javelin)
		{
			if (placeableTag <= Enums.PlaceableTag.Gloves)
			{
				if (placeableTag <= Enums.PlaceableTag.Holy)
				{
					if (placeableTag <= Enums.PlaceableTag.Void)
					{
						if ((ulong)placeableTag <= 8uL)
						{
							switch (placeableTag)
							{
							case Enums.PlaceableTag.Physical:
							case Enums.PlaceableTag.Fire:
							case Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Lightning:
								goto IL_032c;
							case Enums.PlaceableTag.None:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
							case Enums.PlaceableTag.Physical | Enums.PlaceableTag.Fire | Enums.PlaceableTag.Cold:
								goto IL_035d;
							}
						}
						if (placeableTag == Enums.PlaceableTag.Void)
						{
							goto IL_032c;
						}
					}
					else if (placeableTag == Enums.PlaceableTag.Poison || placeableTag == Enums.PlaceableTag.Energy || placeableTag == Enums.PlaceableTag.Holy)
					{
						goto IL_032c;
					}
				}
				else if (placeableTag <= Enums.PlaceableTag.BodyArmor)
				{
					if (placeableTag == Enums.PlaceableTag.Trinket || placeableTag == Enums.PlaceableTag.Shield || placeableTag == Enums.PlaceableTag.BodyArmor)
					{
						goto IL_0333;
					}
				}
				else if (placeableTag == Enums.PlaceableTag.LegArmor || placeableTag == Enums.PlaceableTag.Boots || placeableTag == Enums.PlaceableTag.Gloves)
				{
					goto IL_0333;
				}
			}
			else if (placeableTag <= Enums.PlaceableTag.Hammer)
			{
				if (placeableTag <= Enums.PlaceableTag.Ring)
				{
					if (placeableTag == Enums.PlaceableTag.Amulet || placeableTag == Enums.PlaceableTag.Ring)
					{
						goto IL_0333;
					}
				}
				else
				{
					if (placeableTag == Enums.PlaceableTag.Headwear)
					{
						goto IL_0333;
					}
					if (placeableTag == Enums.PlaceableTag.Sword || placeableTag == Enums.PlaceableTag.Hammer)
					{
						goto IL_033a;
					}
				}
			}
			else if (placeableTag <= Enums.PlaceableTag.Halberd)
			{
				if (placeableTag == Enums.PlaceableTag.Axe || placeableTag == Enums.PlaceableTag.FistWeapon || placeableTag == Enums.PlaceableTag.Halberd)
				{
					goto IL_033a;
				}
			}
			else if (placeableTag == Enums.PlaceableTag.Bow || placeableTag == Enums.PlaceableTag.Crossbow || placeableTag == Enums.PlaceableTag.Javelin)
			{
				goto IL_033a;
			}
		}
		else if (placeableTag <= Enums.PlaceableTag.Uncommon)
		{
			if (placeableTag <= Enums.PlaceableTag.Whip)
			{
				if (placeableTag <= Enums.PlaceableTag.Wand)
				{
					if (placeableTag == Enums.PlaceableTag.Throwing || placeableTag == Enums.PlaceableTag.Wand)
					{
						goto IL_033a;
					}
				}
				else if (placeableTag == Enums.PlaceableTag.Staff || placeableTag == Enums.PlaceableTag.Spellbook || placeableTag == Enums.PlaceableTag.Whip)
				{
					goto IL_033a;
				}
			}
			else if (placeableTag <= Enums.PlaceableTag.Ranged)
			{
				if (placeableTag == Enums.PlaceableTag.Exotic)
				{
					goto IL_033a;
				}
				if (placeableTag == Enums.PlaceableTag.Melee || placeableTag == Enums.PlaceableTag.Ranged)
				{
					return _tagPriorityMeleeRanged;
				}
			}
			else if (placeableTag != Enums.PlaceableTag.SpellDONOTUSE && (placeableTag == Enums.PlaceableTag.Common || placeableTag == Enums.PlaceableTag.Uncommon))
			{
				goto IL_0348;
			}
		}
		else if (placeableTag <= Enums.PlaceableTag.FireArm)
		{
			if (placeableTag <= Enums.PlaceableTag.Epic)
			{
				if (placeableTag == Enums.PlaceableTag.Rare || placeableTag == Enums.PlaceableTag.Epic)
				{
					goto IL_0348;
				}
			}
			else
			{
				if (placeableTag == Enums.PlaceableTag.Legendary || placeableTag == Enums.PlaceableTag.Mythic)
				{
					goto IL_0348;
				}
				if (placeableTag == Enums.PlaceableTag.FireArm)
				{
					goto IL_033a;
				}
			}
		}
		else if (placeableTag <= Enums.PlaceableTag.Blunt)
		{
			if (placeableTag == Enums.PlaceableTag.Special)
			{
				goto IL_0333;
			}
			if (placeableTag == Enums.PlaceableTag.Unique)
			{
				return _tagPriorityUnique;
			}
			if (placeableTag == Enums.PlaceableTag.Blunt)
			{
				goto IL_032c;
			}
		}
		else
		{
			if (placeableTag == Enums.PlaceableTag.Slashing || placeableTag == Enums.PlaceableTag.Piercing)
			{
				goto IL_032c;
			}
			if (placeableTag == Enums.PlaceableTag.MergeIngredient)
			{
				return _tagPriorityIngredient;
			}
		}
		goto IL_035d;
		IL_035d:
		return 0;
		IL_032c:
		return _tagPriorityDamageTypes;
		IL_0348:
		return _tagPriorityRarity;
		IL_033a:
		return _tagPriorityWeaponTypes;
		IL_0333:
		return _tagPriorityItemType;
	}
}
