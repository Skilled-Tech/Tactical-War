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
        [SerializeField]
        protected Funds funds;
        public override Funds Funds { get { return funds; } }

        protected override void Start()
        {
            base.Start();

            Base.Tower.Slots[0].Handle.SetActive(false);

            DeployTower(0);
        }

        BaseUnitsCreator.Deployment deployment;

        protected override void Awake()
        {
            base.Awake();

            Funds.Configure();

            Funds.Gold.Value = 99999;
            Funds.XP.Value = 99999;
        }

        void Update()
        {
            if (Base.Units.Count < Enemey.Base.Units.Count + 4)
            {
                if (Base.Units.Creator.CanDeploy(UnitSelection[0]))
                {
                    if(deployment == null)
                        deployment = Base.Units.Creator.Deploy(UnitSelection[Random.Range(0, UnitSelection.Count)]);
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