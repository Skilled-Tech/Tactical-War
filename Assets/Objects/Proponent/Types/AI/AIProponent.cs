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

            Base.Tower.Slots[0].Handle.SetActive(false);

            return;
            Base.Tower.Slots[0].Deploy();
            Base.Tower.Slots[0].Turret.isDeployed = true;
        }

        BaseUnitsCreator.Deployment deployment;

        void Update()
        {
            if (Base.Units.Count < Enemey.Base.Units.Count + 4)
            {
                if (Base.Units.Creator.CanDeploy(Age.Value.Units[0]))
                {
                    if(deployment == null)
                        deployment = Base.Units.Creator.Deploy(Age.Value.Units.First());
                    else
                    {
                        if (deployment.isComplete) deployment = null;
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