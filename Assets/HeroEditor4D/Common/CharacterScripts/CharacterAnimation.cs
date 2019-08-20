using System;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.CharacterScripts
{
	/// <summary>
	/// Used to play animations.
	/// </summary>
	public class CharacterAnimation : MonoBehaviour
	{
		public Character4D Character;
		public Animator Animator;

		public void SetState(CharacterState state)
		{
			Animator.SetInteger("State", (int) state);
		}

		public void Slash()
		{
			Animator.SetTrigger("Slash");
		}

		public void Shot()
		{
			Animator.SetTrigger("Shot");
		}

		public void Attack()
		{
			switch (Character.WeaponType)
			{
				case WeaponType.Melee1H:
				case WeaponType.Melee2H:
					Slash();
					break;
				case WeaponType.Bow:
					Shot();
					break;
				default:
					throw new NotSupportedException();
			}
		}
	}
}