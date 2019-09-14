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
        protected int cost = 500;
        public int Cost { get { return cost; } }

        public Proponent User { get; protected set; }
        public virtual bool InUse { get { return User != null; } }

        public virtual void Activate(Proponent proponent)
        {
            User = proponent;

            if (User.Energy.Value < cost)
                throw new Exception(proponent.name + " can't afford to use ability " + name);

            User.Energy.Value -= cost;
        }

        public event Action OnEnd;
        protected virtual void End()
        {
            if (OnEnd != null) OnEnd();
        }
    }
}