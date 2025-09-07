using System;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.System.Saving.SavefileEditorComponents;

internal class UnlockableSavefileEditor : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _unlockableNameLabel;

	[SerializeField]
	private TMP_InputField _unlockablePointsInvestedInputField;

	internal Enums.Unlockable Unlockable => Enum.Parse<Enums.Unlockable>(_unlockableNameLabel.text);

	internal int PointsInvested => int.Parse(_unlockablePointsInvestedInputField.text);

	internal void LoadUnlockable(Enums.Unlockable unlockable, int pointsInvested)
	{
		_unlockableNameLabel.text = unlockable.ToString();
		_unlockablePointsInvestedInputField.text = pointsInvested.ToString();
	}
}
