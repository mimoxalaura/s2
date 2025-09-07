using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

public class IngameWeaponObject : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Animator _animator;

	public void ChangeMaterial(Material material)
	{
		spriteRenderer.material = material;
	}

	public void AnimateSucces()
	{
	}

	public void AnimateDeath()
	{
	}

	public void AnimateAttack()
	{
		if (_animator != null)
		{
			_animator.SetTrigger("Attack");
		}
	}
}
