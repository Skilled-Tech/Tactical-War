using System;
using System.Collections.Generic;
using HeroEditor.Common;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.CharacterScripts
{
	/// <summary>
	/// Controls 4 characters (for each direction).
	/// </summary>
	public class Character4D : Character4DBase
	{
		public void OnValidate()
		{
			Parts = new List<CharacterBase> { Front, Back, Left, Right };
			Parts.ForEach(i => i.BodyRenderers.ForEach(j => j.color = BodyColor));
			Parts.ForEach(i => i.EarRenderers.ForEach(j => j.color = BodyColor));
		}

		public override void Initialize()
		{
			Parts.ForEach(i => i.Initialize());
		}

		public override void CopyFrom(Character4DBase character)
		{
			for (var i = 0; i < Parts.Count; i++)
			{
				Parts[i].CopyFrom(character.Parts[i]);
			}
		}

		public override string ToJson()
		{
			throw new System.NotImplementedException();
		}

		public override void LoadFromJson(string json)
		{
			throw new System.NotImplementedException();
		}

		public void SetDirection(Vector2 direction)
		{
			Parts.ForEach(i => i.transform.localPosition = Vector3.zero);

			if (direction == Vector2.left)
			{
				Parts.ForEach(i => i.gameObject.SetActive(i == Left));
			}
			else if (direction == Vector2.right)
			{
				Parts.ForEach(i => i.gameObject.SetActive(i == Right));
			}
			else if (direction == Vector2.up)
			{
				Parts.ForEach(i => i.gameObject.SetActive(i == Back));
			}
			else if (direction == Vector2.down)
			{
				Parts.ForEach(i => i.gameObject.SetActive(i == Front));
			}
			else
			{
				throw new NotSupportedException();
			}
		}
	}
}