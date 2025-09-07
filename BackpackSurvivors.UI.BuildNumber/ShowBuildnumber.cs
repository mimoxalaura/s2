using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.BuildNumber;

public class ShowBuildnumber : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _buildnumberText;

	[SerializeField]
	private string _buildnumberPrefix = "Build:";

	[SerializeField]
	private BuildnumberSO _buildnumberSO;

	[SerializeField]
	private string _buildnumberSuffix;

	private void Start()
	{
		_buildnumberText.text = _buildnumberPrefix + _buildnumberSO.Buildnumber + _buildnumberSuffix;
		SingletonController<SaveGameController>.Instance.BuildNumber = int.Parse(_buildnumberSO.Buildnumber);
	}
}
