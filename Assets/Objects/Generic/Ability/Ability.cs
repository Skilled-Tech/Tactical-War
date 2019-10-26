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
	public class Ability : MonoBehaviour
	{
        public Proponent User { get; protected set; }
        public AbilityTemplate Template { get; protected set; }
        public virtual void Configure(Proponent user, AbilityTemplate template)
        {
            this.User = user;
            this.Template = template;
        }

        public float Cooldown => Template.Usage.Cooldown;

        public virtual void Init()
        {

        }

        public event Action OnEnd;
        protected virtual void End()
        {
            if (OnEnd != null) OnEnd();
        }
    }
}