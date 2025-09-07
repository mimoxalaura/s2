using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class BookController : ModalUI
{
	[SerializeField]
	private Camera _bookCamera;

	[SerializeField]
	private IndexBookPage _indexPage;

	[SerializeField]
	private IndexDetailPage _indexPageDetail;

	[SerializeField]
	private SerializableDictionaryBase<int, BookPage> _pages;

	[SerializeField]
	private SerializableDictionaryBase<int, DetailPage> _detailPages;

	[SerializeField]
	private Animator _bookAnimator;

	[SerializeField]
	private Animator _pageAnimator;

	[SerializeField]
	private Animator _tabAnimator;

	[SerializeField]
	private GameObject[] _tabSelectors;

	[SerializeField]
	private GameObject[] _tabSelectorsDuringPageTurn;

	[SerializeField]
	private Sprite[] _tabSprites;

	[SerializeField]
	private Image[] _tabIcons;

	[SerializeField]
	private Image[] _tabPageIcons;

	[SerializeField]
	private GameObject _blackPanel;

	[SerializeField]
	private GameObject _nextButton;

	[SerializeField]
	private GameObject _previousButton;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _openBookAudioClip;

	[SerializeField]
	private AudioClip _closeBookAudioClip;

	[SerializeField]
	private AudioClip _changePageLeftAudioClip;

	[SerializeField]
	private AudioClip _changePageRightAudioClip;

	[SerializeField]
	private AudioClip _buttonPressed;

	[SerializeField]
	private Button _closeButton;

	private int _currentTab = -1;

	private bool _animating;

	public void OpenBook()
	{
		if (!_animating)
		{
			_animating = true;
			_bookCamera.enabled = true;
			_blackPanel.SetActive(value: true);
			for (int i = 0; i < _tabSprites.Length; i++)
			{
				_tabIcons[i].sprite = _tabSprites[i];
				_tabPageIcons[i].sprite = _tabSprites[i];
			}
			StopAllCoroutines();
			_tabAnimator.SetBool("TabsVisible", value: false);
			_bookAnimator.SetBool("Open", value: false);
			_bookAnimator.StopPlayback();
			_tabAnimator.StopPlayback();
			_currentTab = -1;
			_previousButton.SetActive(value: false);
			_nextButton.SetActive(value: false);
			StartCoroutine(OpenBookAsync());
		}
	}

	private IEnumerator OpenBookAsync()
	{
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: false);
		SingletonController<AudioController>.Instance.PlaySFXClip(_openBookAudioClip, 1f);
		yield return new WaitForEndOfFrame();
		_indexPage.InitPage();
		_bookAnimator.SetBool("Open", value: true);
		yield return new WaitForSecondsRealtime(0.6f);
		_tabAnimator.SetBool("TabsVisible", value: true);
		yield return new WaitForSecondsRealtime(0.5f);
		_indexPage.gameObject.SetActive(value: true);
		_indexPageDetail.gameObject.SetActive(value: true);
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: true);
		_animating = false;
		_closeButton.gameObject.SetActive(value: true);
		_closeButton.interactable = true;
	}

	public void CloseBook()
	{
		if (!_animating)
		{
			_animating = true;
			_closeButton.gameObject.SetActive(value: false);
			_closeButton.interactable = false;
			_blackPanel.SetActive(value: false);
			StopAllCoroutines();
			StartCoroutine(CloseBookAsync());
		}
	}

	private IEnumerator CloseBookAsync()
	{
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: false);
		HideCurrentPage();
		GameObject[] tabSelectorsDuringPageTurn = _tabSelectorsDuringPageTurn;
		for (int i = 0; i < tabSelectorsDuringPageTurn.Length; i++)
		{
			tabSelectorsDuringPageTurn[i].SetActive(value: false);
		}
		tabSelectorsDuringPageTurn = _tabSelectors;
		for (int i = 0; i < tabSelectorsDuringPageTurn.Length; i++)
		{
			tabSelectorsDuringPageTurn[i].SetActive(value: false);
		}
		SingletonController<AudioController>.Instance.PlaySFXClip(_closeBookAudioClip, 1f);
		_tabAnimator.SetBool("TabsVisible", value: false);
		yield return new WaitForSecondsRealtime(0.5f);
		_bookAnimator.SetBool("Open", value: false);
		yield return new WaitForSecondsRealtime(0.5f);
		_bookCamera.enabled = false;
		SingletonController<InputController>.Instance.SetInputEnabled(enabled: true);
		_animating = false;
	}

	public void NextPage()
	{
		if (_currentTab != -1 && _currentTab < 5)
		{
			_tabSelectorsDuringPageTurn[_currentTab + 1].SetActive(value: true);
			SingletonController<AudioController>.Instance.PlaySFXClip(_buttonPressed, 1f);
			StartCoroutine(GoToPage(_currentTab + 1));
		}
	}

	public void PreviousPage()
	{
		if (_currentTab != -1 && _currentTab > 0)
		{
			_tabSelectorsDuringPageTurn[_currentTab].SetActive(value: true);
			SingletonController<AudioController>.Instance.PlaySFXClip(_buttonPressed, 1f);
			StartCoroutine(GoToPage(_currentTab - 1));
		}
	}

	public void ShowTab1()
	{
		StartCoroutine(GoToPage(0));
	}

	public void ShowTab2()
	{
		StartCoroutine(GoToPage(1));
	}

	public void ShowTab3()
	{
		StartCoroutine(GoToPage(2));
	}

	public void ShowTab4()
	{
		StartCoroutine(GoToPage(3));
	}

	public void ShowTab5()
	{
		StartCoroutine(GoToPage(4));
	}

	public void ShowTab6()
	{
		StartCoroutine(GoToPage(5));
	}

	private IEnumerator GoToPage(int page)
	{
		if (_currentTab != page)
		{
			HideCurrentPage();
			if (_currentTab > page)
			{
				_pageAnimator.SetTrigger("GoRight");
				SingletonController<AudioController>.Instance.PlaySFXClip(_changePageLeftAudioClip, 1f);
			}
			else
			{
				_pageAnimator.SetTrigger("GoLeft");
				SingletonController<AudioController>.Instance.PlaySFXClip(_changePageRightAudioClip, 1f);
			}
			_currentTab = page;
			yield return new WaitForSecondsRealtime(0.6f);
			GameObject[] tabSelectorsDuringPageTurn = _tabSelectorsDuringPageTurn;
			for (int i = 0; i < tabSelectorsDuringPageTurn.Length; i++)
			{
				tabSelectorsDuringPageTurn[i].SetActive(value: false);
			}
			tabSelectorsDuringPageTurn = _tabSelectors;
			for (int i = 0; i < tabSelectorsDuringPageTurn.Length; i++)
			{
				tabSelectorsDuringPageTurn[i].SetActive(value: false);
			}
			_tabSelectors[_currentTab].SetActive(value: true);
			_previousButton.SetActive(page != 0);
			_nextButton.SetActive(page != 5);
			ShowCurrentPage();
		}
	}

	private void ShowCurrentPage()
	{
		_indexPage.gameObject.SetActive(value: false);
		foreach (KeyValuePair<int, BookPage> page in _pages)
		{
			page.Value.gameObject.SetActive(page.Key == _currentTab);
			if (page.Key == _currentTab)
			{
				_pages[_currentTab].InitPage();
			}
		}
		foreach (KeyValuePair<int, DetailPage> detailPage in _detailPages)
		{
			detailPage.Value.gameObject.SetActive(detailPage.Key == _currentTab);
		}
	}

	private void HideCurrentPage()
	{
		_indexPage.gameObject.SetActive(value: false);
		foreach (KeyValuePair<int, BookPage> page in _pages)
		{
			page.Value.gameObject.SetActive(value: false);
		}
		foreach (KeyValuePair<int, DetailPage> detailPage in _detailPages)
		{
			detailPage.Value.gameObject.SetActive(value: false);
		}
	}
}
