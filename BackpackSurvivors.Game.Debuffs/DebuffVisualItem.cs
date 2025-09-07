using BackpackSurvivors.ScriptableObjects.Debuffs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Debuffs;

public class DebuffVisualItem : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _text;

	private DebuffSO _debuffSO;

	private DebuffVisualVFX _debuffVisualEffect;

	private int _stackCount;

	public int DebuffId => _debuffSO.Id;

	public int StackCount => _stackCount;

	public void SetDebuff(DebuffSO debuffSO)
	{
		_debuffSO = debuffSO;
		_image.sprite = debuffSO.Icon;
	}

	public void CreateDebuffEffect(Transform transform)
	{
		if (_debuffSO != null && _debuffSO.DebuffVisualPrefab != null)
		{
			_debuffVisualEffect = Object.Instantiate(_debuffSO.DebuffVisualPrefab, transform);
			_debuffVisualEffect.ScaleVFX(transform.localScale.x);
		}
	}

	public void RemovebuffEffect()
	{
		if (_debuffVisualEffect != null)
		{
			Object.Destroy(_debuffVisualEffect.gameObject);
		}
	}

	public void UpdateStackCountStack(int stackCount)
	{
		_stackCount = stackCount;
		if (stackCount <= 1)
		{
			_text.SetText(string.Empty);
		}
		else
		{
			_text.SetText(stackCount.ToString());
		}
	}
}
