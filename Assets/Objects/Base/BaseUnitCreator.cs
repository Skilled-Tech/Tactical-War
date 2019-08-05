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
	public class BaseUnitCreator : Base.Reference
	{
        public class Procedure
        {
            public UnitData Data { get; protected set; }

            public float Duration { get; protected set; }

            public float Timer { get; protected set; }

            public float Rate { get { return Timer / Duration; } }

            public event Action<UnitData> OnCompletion;

            public IEnumerator Coroutine()
            {
                while(Timer > 0f)
                {
                    Timer = Mathf.MoveTowards(Timer, 0f, Time.deltaTime);

                    yield return new WaitForEndOfFrame();
                }

                if (OnCompletion != null) OnCompletion(Data);
            }

            public Procedure(UnitData data)
            {
                this.Data = data;

                Duration = Timer = data.DeploymentTime;
            }
        }

        public virtual Procedure ReadyUp(UnitData data)
        {
            if(Proponent.Funds.CanAfford(data.Cost))
            {
                Proponent.Funds.Take(data.Cost);

                var procedure = new Procedure(data);

                StartCoroutine(procedure.Coroutine());

                procedure.OnCompletion += (UnitData val)=> Spawn(val);

                return procedure;
            }
            else
            {
                Debug.LogWarning("Base trying to create a Unit that the Proponent can't afford, Proponent: " + Proponent.name);
                return null;
            }
        }

        public virtual Unit Spawn(UnitData data)
        {
            var instance = Instantiate(data.Prefab, transform.position, transform.rotation);

            var unit = instance.GetComponent<Unit>();

            unit.Init(Proponent);

            return unit;
        }
    }
}