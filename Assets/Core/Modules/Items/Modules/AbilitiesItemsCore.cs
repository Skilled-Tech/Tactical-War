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
	public class AbilitiesItemsCore : ItemsCore.Module
	{
        #region List
        public List<AbilityTemplate> List { get; protected set; }

        public AbilityTemplate this[int index] { get { return List[index]; } }

        public int Count { get { return List.Count; } }
        #endregion

        public override void Configure()
        {
            base.Configure();

            List = new List<AbilityTemplate>();

            for (int i = 0; i < Items.List.Count; i++)
            {
                if (Items[i] is AbilityTemplate)
                    List.Add(Items[i] as AbilityTemplate);
            }
        }

        public virtual AbilityTemplate Find(string ID)
        {
            for (int i = 0; i < List.Count; i++)
                if (List[i].ID == ID)
                    return List[i];

            return null;
        }
    }
}