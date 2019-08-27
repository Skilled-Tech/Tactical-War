using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using Assets.HeroEditor4D.Common.ExampleScripts;
using Assets.HeroEditor4D.Common.CharacterScripts;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class UnitBody : Unit.Module
	{
		public Animator Animator { get; protected set; }

        public AnimationEvents AnimationEvents { get; protected set; }

        public Character4D Character { get; protected set; }

        public CharacterAnimation CharacterAnimation { get; protected set; }

        public override void Configure(Unit data)
        {
            base.Configure(data);

            Animator = Dependancy.Get<Animator>(gameObject);

            AnimationEvents = Dependancy.Get<AnimationEvents>(gameObject);

            Character = Dependancy.Get<Character4D>(gameObject);

            CharacterAnimation = Dependancy.Get<CharacterAnimation>(gameObject);
        }

        public override void Init()
        {
            base.Init();

            Animator.SetFloat("Cycle Offset", Random.Range(0f, 1f));
        }
    }
}