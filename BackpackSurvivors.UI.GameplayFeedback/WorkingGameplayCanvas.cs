using System.Collections;
using BackpackSurvivors.Game.Shop;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback;

internal class WorkingGameplayCanvas : MonoBehaviour
{
	[SerializeField]
	private GameObject _currencyFeedback;

	private ShopController _shopController;

	private bool _boundShopController;

	private void Start()
	{
		StartCoroutine(BindShopController());
	}

	private IEnumerator BindShopController()
	{
		while (!_boundShopController)
		{
			yield return new WaitForSeconds(0.5f);
			_shopController = Object.FindObjectOfType<ShopController>();
			if (_shopController != null)
			{
				_shopController.OnShopClosed += _shopController_OnShopClosed;
				_shopController.OnShopOpened += _shopController_OnShopOpened;
				_boundShopController = true;
			}
		}
	}

	private void _shopController_OnShopOpened()
	{
		_currencyFeedback.SetActive(value: false);
	}

	private void _shopController_OnShopClosed(object sender, ShopClosedEventArgs e)
	{
		_currencyFeedback.SetActive(value: true);
	}
}
