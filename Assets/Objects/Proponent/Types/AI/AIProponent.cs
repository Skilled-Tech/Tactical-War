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
            return;
            if(Base.IsAlive)
            {
                if (Base.Units.Count < Enemey.Base.Units.Count + 4)
                {
                    if (Base.Units.Creator.CanDeploy(Units.Selection[0]))
                    {
                        if (deployment == null)
                            deployment = Base.Units.Creator.Deploy(Units.Selection[Random.Range(0, Units.Selection.Count)]);
                        else
                        {
                            if (deployment.isComplete) deployment = null;
                        }
                    }
                }
            }
        }

        void OnUnitDeploymentComplete(BaseUnitsCreator.Deployment deployment)
        {
            this.deployment = null;
        }

        void DeployTower(int index)
        {
            Base.Tower.Slots[index].Deploy();
            Base.Tower.Slots[index].Turret.isDeployed = true;
        }
    }
}