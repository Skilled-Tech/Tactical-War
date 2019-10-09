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
using Newtonsoft.Json.Linq;

using PlayFab;

namespace Game
{
    [Serializable]
	public class ItemsUnitsCore : ItemsCore.Module
	{
        #region List
        public List<UnitTemplate> List { get; protected set; }

        public UnitTemplate this[int index] { get { return List[index]; } }

        public int Count { get { return List.Count; } }
        #endregion

        public override void Configure()
        {
            base.Configure();

            List = new List<UnitTemplate>();

            for (int i = 0; i < Items.List.Count; i++)
            {
                if (Items[i] is UnitTemplate)
                    List.Add(Items[i] as UnitTemplate);
            }
        }

        public virtual UnitTemplate Find(string ID)
        {
            for (int i = 0; i < List.Count; i++)
                if (List[i].ID == ID)
                    return List[i];

            return null;
        }
    }
}