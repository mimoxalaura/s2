using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection.ListItems.Enemy;

public class CollectionEnemyUI : CollectionListItemUI
{
	public delegate void OnClickHandler(object sender, EnemyCollectionSelectedEventArgs e);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Image _border;

	[SerializeField]
	private GameObject _lockedOverlay;

	[SerializeField]
	private TooltipTrigger _tooltip;

	private EnemySO _enemy;

	private bool _unlocked;

	[SerializeField]
	private Sprite _regularEnemyBorder;

	[SerializeField]
	private Sprite _minibossEnemyBorder;

	[SerializeField]
	private Sprite _bossEnemyBorder;

	public event OnClickHandler OnClick;

	public void Init(EnemySO enemy, bool unlocked, bool interactable)
	{
		Init(interactable);
		switch (enemy.EnemyType)
		{
		case Enums.Enemies.EnemyType.Miniboss:
			_border.sprite = _minibossEnemyBorder;
			break;
		case Enums.Enemies.EnemyType.Boss:
			_border.sprite = _bossEnemyBorder;
			break;
		default:
			_border.sprite = _regularEnemyBorder;
			break;
		}
		_unlocked = unlocked;
		_enemy = enemy;
		_image.sprite = enemy.Icon;
		_lockedOverlay.SetActive(!unlocked);
		if (_tooltip.enabled && _unlocked)
		{
			_tooltip.SetContent(enemy.Name, enemy.Description);
			_tooltip.ToggleEnabled(unlocked);
		}
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new EnemyCollectionSelectedEventArgs(_enemy, this));
		}
	}
}
