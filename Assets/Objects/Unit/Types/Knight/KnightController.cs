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
	public class KnightController : UnitController
	{
        void Update()
        {
            if(Index == 0)
            {
                if (Enemy.Base.Units.Count > 0)
                    ProcessTarget(Enemy.Base.Units.First);
                else
                    ProcessTarget(Enemy.Base, Enemy.Base.Entrance.position);
            }
            else
            {
                var target = Base.Units.List[Index - 1];

                if(Navigator.MoveTo(target.transform.position - target.transform.right * 2f, 0f))
                {
                    
                }
                else
                {
                    
                }
            }
        }

        void ProcessTarget(Entity target)
        {
            ProcessTarget(target, target.transform.position);
        }
        void ProcessTarget(Entity target, Vector3 position)
        {
            if (Attack.IsProcessing)
            {

            }
            else
            {
                if (Navigator.MoveTo(position, 1.5f))
                {
                    if (Attack.IsProcessing)
                    {

                    }
                    else
                    {
                        Attack.Do(target);
                    }
                }
                else
                {
                    Body.Animator.SetBool("Walk", true);
                }
            }
        }
    }
}