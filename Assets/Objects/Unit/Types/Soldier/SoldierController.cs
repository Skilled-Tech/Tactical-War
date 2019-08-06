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
	public class SoldierController : UnitController
	{
        public Animator Animator { get; protected set; }

        public override void Configure(Unit data)
        {
            base.Configure(data);

            Animator = Dependancy.Get<Animator>(Unit.gameObject);
        }

        public override void Init()
        {
            base.Init();

            Animator.SetFloat("Cycle Offset", Random.Range(0f, 1f));
        }

        void Update()
        {
            if(Index == 0)
            {
                if (Enemy.Base.Units.Count > 0)
                {
                    ProcessTarget(Enemy.Base.Units.First);
                }
                else
                {
                    ProcessTarget(Enemy.Base, Enemy.Base.Entrance.transform.position);
                }
            }
            else
            {
                var target = Base.Units.List[Index - 1];

                if(Navigator.MoveTo(target.transform.position - target.transform.forward * 2f, 0f))
                {
                    Animator.SetBool("Walk", false);
                }
                else
                {
                    Animator.SetBool("Walk", true);
                }
            }
        }

        void ProcessTarget(Entity target)
        {
            ProcessTarget(target, target.transform.position);
        }
        void ProcessTarget(Entity target, Vector3 position)
        {
            if (Navigator.MoveTo(position, 1f))
            {
                Animator.SetBool("Walk", false);

                if (Attack.IsProcessing)
                {

                }
                else
                {
                    Animator.SetTrigger("Attack");
                    Attack.Do(target);
                }
            }
            else
            {
                Animator.SetBool("Walk", true);
            }
        }
    }
}