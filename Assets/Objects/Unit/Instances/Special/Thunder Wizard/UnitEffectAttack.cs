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
	public class UnitEffectAttack : UnitAttack.Module
	{
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        protected override void AttackConnected()
        {
            base.AttackConnected();

            if(Leader.Base.Units.Count == 0)
            {
                Perform(Leader.Base);
            }
            else
            {
                Perform(Leader.Base.Units.First);
            }
        }

        protected virtual void Perform(Entity target)
        {
            Vector3 GetPosition(Entity entity) => target.Center + Vector3.down * target.Bounds.extents.y;

            var instance = Instantiate(prefab, GetPosition(target), prefab.transform.rotation);

            DoDamage(target);
        }
    }
}