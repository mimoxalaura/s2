using UnityEngine;

namespace BackpackSurvivors.Game.Effects.AttackLogic.WeaponSpecific;

[CreateAssetMenu(fileName = "AttackLogicStep", menuName = "Game/Items/Weapon/AttackLogicStep", order = 3)]
public class AttackLogicStepSO : ScriptableObject
{
	[SerializeField]
	private float _hitDelay;

	[SerializeField]
	private bool _rotate;

	[SerializeField]
	private Quaternion _preferredColliderRotation;

	[SerializeField]
	private bool _scale;

	[SerializeField]
	private Vector3 _preferredColliderScale = Vector3.one;

	internal float HitDelay => _hitDelay;

	internal Quaternion PreferredColliderRotation => _preferredColliderRotation;

	internal Vector3 PreferredColliderScale => _preferredColliderScale;

	internal bool Rotate => _rotate;

	internal bool Scale => _scale;

	private void RenameFile()
	{
	}
}
