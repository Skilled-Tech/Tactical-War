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
        public ProponentUnitsSelection Selection { get { return Units.Selection; } }

        protected override void Start()
        {
            base.Start();

            Base.Tower.Slots[0].Handle.SetActive(false);

            DeployTower(0);
        }

        BaseUnitsCreator.Deployment deployment;

        void Update()
        {
            if(Base.IsAlive)
            {
                if (Base.Units.Count < Enemy.Base.Units.Count + 4)
                {
                    var random = Selection.Random;

                    if (Base.Units.Creator.CanDeploy(random.Template))
                    {
                        if (deployment == null)
                            deployment = Base.Units.Creator.Deploy(random.Template);
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