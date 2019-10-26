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
        [SerializeField]
        protected float cooldown = 20f;
        public float Cooldown { get { return cooldown; } }

        public Proponent User { get; protected set; }
        public virtual bool InUse { get { return User != null; } }

        public virtual void Activate(Proponent proponent)
        {
            User = proponent;
        }

        public event Action OnEnd;
        protected virtual void End()
        {
            if (OnEnd != null) OnEnd();
        }
    }
}