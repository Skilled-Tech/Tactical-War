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
    public class ProponentEnergy : Proponent.Module
    {
        [SerializeField]
        protected int value;
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;

                TriggerChange();
            }
        }

        public event Action OnChanged;
        protected virtual void TriggerChange()
        {
            if (OnChanged != null) OnChanged();
        }
    }
}