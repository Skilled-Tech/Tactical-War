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
	public class AIProponent : Proponent
	{
        protected override void Start()
        {
            base.Start();

            StartCoroutine(Procedure());
        }

        BaseUnitsCreator.Deployment deployment;

        IEnumerator Procedure()
        {
            while(Base.Health.Value > 0f)
            {
                if (Base.Units.Count < Enemey.Base.Units.Count + 4)
                {
                    if (Base.Units.Creator.CanDeploy(Age.Value.Units[0]))
                    {
                        deployment = Base.Units.Creator.Deploy(Age.Value.Units.First());

                        yield return deployment.Coroutine;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else
                    yield return null;
            }
        }

        void OnDeploymentComplete(BaseUnitsCreator.Deployment obj)
        {
            deployment = null;
        }
    }
}