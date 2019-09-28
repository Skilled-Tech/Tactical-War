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
	public abstract class ProponentUnits : Proponent.Module
	{
        #region List
        public IList<Unit> List { get { return Base.Units.List; } }

        public Unit this[int index]
        {
            get
            {
                return List[index];
            }
        }

        public Unit First
        {
            get
            {
                if (List.Count == 0) return null;

                return List.First();
            }
        }

        public int Count { get { return List.Count; } }
        #endregion

        public abstract IList<UnitTemplate> Selection { get; }

        public Base Base { get { return Proponent.Base; } }

        public abstract ItemUpgradesData GetUpgrade(UnitTemplate unit);
	}
}