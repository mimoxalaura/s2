using BackpackSurvivors.UI.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors;

public class LeanTweenTester : MonoBehaviour
{
	[SerializeField]
	private Image _keystoneActivatedEffectImage;

	[SerializeField]
	private ImageAnimation _animation;

	public void Activate()
	{
		_animation.enabled = false;
		_animation.enabled = true;
		_animation.ResetToBeginning();
		Color white = Color.white;
		Color to = new Color(0f, 0f, 0f, 0f);
		_keystoneActivatedEffectImage.color = white;
		LeanTween.value(_keystoneActivatedEffectImage.gameObject, setColorCallback, white, to, 0.5f).setDelay(0.3f);
	}

	private void setColorCallback(Color c)
	{
		_keystoneActivatedEffectImage.color = c;
		Color color = _keystoneActivatedEffectImage.color;
		_keystoneActivatedEffectImage.color = color;
	}
}
