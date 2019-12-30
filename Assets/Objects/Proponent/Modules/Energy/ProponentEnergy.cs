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

        public event Action OnValueChanged;
        protected virtual void TriggerChange()
        {
            if (OnValueChanged != null) OnValueChanged();
        }

        public LevelCore.ProponentProperty LevelData => Proponent.LevelData;

        public override void Configure(Proponent reference)
        {
            base.Configure(reference);

            Value = LevelData.Energy.Initial;

            StartCoroutine(IncreaseProcedure());
        }

        IEnumerator IncreaseProcedure()
        {
            while(true)
            {
                yield return new WaitForSeconds(LevelData.Energy.Increase.Interval);

                Value += LevelData.Energy.Increase.Value;
            }
        }
    }
}