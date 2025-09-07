using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback;

[RequireComponent(typeof(Canvas))]
public class EnvelopController : MonoBehaviour
{
	[SerializeField]
	private Image _topBlackBorder;

	[SerializeField]
	private Image _bottomBlackBorder;

	[SerializeField]
	private Image _topBlackBorderTarget;

	[SerializeField]
	private Image _bottomBlackBorderTarget;

	[SerializeField]
	private Image _topBlackBorderOrigin;

	[SerializeField]
	private Image _bottomBlackBorderOrigin;

	public void ShowEnvelop(float showDuration)
	{
		SetEnvelopVisible();
		SetEnvelopInvisible(showDuration);
	}

	public void HideEnvelop()
	{
		SetEnvelopInvisible();
	}

	public void SetEnvelopVisible()
	{
		_topBlackBorder.transform.position = _topBlackBorderOrigin.transform.position;
		_bottomBlackBorder.transform.position = _bottomBlackBorderOrigin.transform.position;
		LeanTween.value(_topBlackBorder.gameObject, delegate(float val)
		{
			_topBlackBorder.color = new Color(0f, 0f, 0f, val);
		}, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_bottomBlackBorder.gameObject, delegate(float val)
		{
			_bottomBlackBorder.color = new Color(0f, 0f, 0f, val);
		}, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.moveLocalY(_topBlackBorder.gameObject, _topBlackBorderTarget.transform.localPosition.y, 1f).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.moveLocalY(_bottomBlackBorder.gameObject, _bottomBlackBorderTarget.transform.localPosition.y, 1f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void SetEnvelopInvisible(float delay = 0f)
	{
		LeanTween.value(_topBlackBorder.gameObject, delegate(float val)
		{
			_topBlackBorder.color = new Color(0f, 0f, 0f, val);
		}, 1f, 0f, 1f).setIgnoreTimeScale(useUnScaledTime: true).setDelay(delay);
		LeanTween.value(_bottomBlackBorder.gameObject, delegate(float val)
		{
			_bottomBlackBorder.color = new Color(0f, 0f, 0f, val);
		}, 1f, 0f, 1f).setIgnoreTimeScale(useUnScaledTime: true).setDelay(delay);
		LeanTween.moveLocalY(_topBlackBorder.gameObject, _topBlackBorderOrigin.transform.localPosition.y, 1f).setIgnoreTimeScale(useUnScaledTime: true).setDelay(delay);
		LeanTween.moveLocalY(_bottomBlackBorder.gameObject, _bottomBlackBorderOrigin.transform.localPosition.y, 1f).setIgnoreTimeScale(useUnScaledTime: true).setDelay(delay);
	}

	private void OnDestroy()
	{
	}
}
