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
	public class Ability : MonoBehaviour
	{
        [SerializeField]
        protected Currency cost = new Currency(500, 100);
        public Currency Cost { get { return cost; } }

        public Proponent User { get; protected set; }

        public virtual void Activate(Proponent proponent)
        {
            User = proponent;

            if (!proponent.Funds.CanAfford(cost))
                throw new Exception(proponent.name + " can't afford ability " + name);

            proponent.Funds.Take(cost);

            Debug.Log(name + " Ability Used");
        }

        public event Action OnEnd;
        protected virtual void End()
        {
            if (OnEnd != null) OnEnd();
        }

        //Utility
        public static List<Unit> Query(IList<Unit> list, Vector3 position, float range)
        {
            List<Unit> targets = new List<Unit>();

            for (int i = 0; i < list.Count; i++)
            {
                var distance = Vector3.Distance(list[i].transform.position, position);

                if (distance <= range)
                    targets.Add(list[i]);
            }

            return targets;
        }
	}
}