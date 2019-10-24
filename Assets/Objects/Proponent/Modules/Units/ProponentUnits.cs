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
	public class ProponentUnits : Proponent.Module
	{
        public ProponentUnitsSelection Selection { get; protected set; }

        #region List
        public IList<Unit> List { get { return Base.Units.List; } }

        public Unit this[int index] => Base.Units[index];

        public Unit First => Base.Units.First;

        public int Count => Base.Units.Count;
        #endregion

        public Base Base { get { return Proponent.Base; } }

        public override void Configure(Proponent reference)
        {
            base.Configure(reference);

            Selection = Dependancy.Get<ProponentUnitsSelection>(gameObject);
        }
    }
}