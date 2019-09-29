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
    public class BaseUnitsCreator : Base.Module
    {
        public BaseUnits Units { get { return Base.Units; } }

        public List<Deployment> Deployments { get; protected set; }
        [Serializable]
        public class Deployment
        {
            public UnitTemplate Unit { get; protected set; }

            public float Duration { get; protected set; }

            public float Timer { get; protected set; }

            public bool isComplete { get { return Timer == 0f; } }

            public float Rate { get { return Timer / Duration; } }

            public event Action OnTick;
            public event Action<Deployment> OnCompletion;

            public Coroutine Coroutine { get; protected set; }
            public IEnumerator Procedure()
            {
                while (Timer > 0f)
                {
                    Timer = Mathf.MoveTowards(Timer, 0f, Time.deltaTime);

                    yield return new WaitForEndOfFrame();

                    if (OnTick != null) OnTick();
                }

                if (OnCompletion != null) OnCompletion(this);
            }

            public Deployment(MonoBehaviour behaviour, UnitTemplate unit)
            {
                this.Unit = unit;

                Duration = Timer = unit.Deployment.Time;

                Coroutine = behaviour.StartCoroutine(Procedure());
            }
        }

        public override void Configure(Base data)
        {
            base.Configure(data);

            Deployments = new List<Deployment>();
        }

        public virtual bool CanDeploy(UnitTemplate unit)
        {
            return Units.Count < Units.Max && Proponent.Energy.Value >= unit.Deployment.Cost;
        }

        public event Action<Deployment> OnDeployment;
        public virtual Deployment Deploy(UnitTemplate data)
        {
            if (Proponent.Energy.Value < data.Deployment.Cost)
                throw new Exception(Proponent.name + "Base trying to Deploy a Unit that the Proponent can't afford to spend Energy on");

            if (Units.Count >= Units.Max)
                throw new Exception(Proponent.name + " Base trying to deploy a Unit but it's reached it's maximum allowed units count");

            Proponent.Energy.Value -= data.Deployment.Cost;

            Spawn(data);

            var deployment = new Deployment(this, data);

            Deployments.Add(deployment);

            deployment.OnCompletion += OnDeploymentComplete;

            if (OnDeployment != null) OnDeployment(deployment);

            return deployment;
        }

        protected virtual void OnDeploymentComplete(Deployment deployment)
        {
            Deployments.Remove(deployment);
        }

        public event Action<Unit> OnSpawn;
        protected virtual Unit Spawn(UnitTemplate template)
        {
            var instance = Instantiate(template.Prefab, transform.position, transform.rotation);

            instance.name = template.name + " (" + Proponent.name + ")";

            instance.transform.localScale = transform.lossyScale;

            Tools.SetLayer(instance, Proponent.Layer);

            var unit = instance.GetComponent<Unit>();
            
            var data = Proponent.Units.Selection.Find(template);

            unit.Configure(Proponent, data);

            if (OnSpawn != null) OnSpawn(unit);

            return unit;
        }
    }
}