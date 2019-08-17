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

namespace Game
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimationEventsRewind))]
    public class UnitBody : Unit.Module
	{
		public Animator Animator { get; protected set; }

        public AnimationEventsRewind AnimationEvent { get; protected set; }

        public override void Configure(Unit data)
        {
            base.Configure(data);

            Animator = Dependancy.Get<Animator>(gameObject);

            AnimationEvent = Dependancy.Get<AnimationEventsRewind>(gameObject);
        }

        public override void Init()
        {
            base.Init();

            Animator.SetFloat("Cycle Offset", Random.Range(0f, 1f));
        }
    }
}