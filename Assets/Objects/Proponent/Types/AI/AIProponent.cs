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
        public Proponent Enemey { get { return Level.Instance.Proponents.Player; } }

        BaseUnitsCreator.Deployment deployment;

        private void Update()
        {
            if(Base.Units.Creator.CanDeploy(age.Units.First()))
            {
                if (Base.Units.Count < Enemey.Base.Units.Count + 1)
                {
                    if (deployment == null)
                    {
                        deployment = Base.Units.Creator.Deploy(age.Units.First());

                        deployment.OnCompletion += OnDeploymentComplete;
                    }
                }
            }
        }

        void OnDeploymentComplete(BaseUnitsCreator.Deployment obj)
        {
            deployment = null;
        }
    }
}