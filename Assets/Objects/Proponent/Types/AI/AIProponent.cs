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

            DeployTower(0);
        }

        BaseUnitsCreator.Deployment deployment;

        void Update()
        {
            if (Base.Units.Count < Enemey.Base.Units.Count + 4)
            {
                if (Base.Units.Creator.CanDeploy(Age.Value.Units[0]))
                {
                    if(deployment == null)
                        deployment = Base.Units.Creator.Deploy(Age.Value.Units[Random.Range(0, Age.Value.Units.Length)]);
                    else
                    {
                        if (deployment.isComplete) deployment = null;
                    }
                }
            }
        }

        void OnUnitDeploymentComplete(BaseUnitsCreator.Deployment obj)
        {
            deployment = null;
        }

        void DeployTower(int index)
        {
            Base.Tower.Slots[index].Deploy();
            Base.Tower.Slots[index].Turret.isDeployed = true;
        }
    }
}