﻿using System;
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
    public abstract class ProponentUnitsSelection : Proponent.Module
    {
        public abstract int Count { get; }

        public IUnitSelectionData First
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i] == null) continue;
                    else return this[i];
                }

                return null;
            }
        }

        public IUnitSelectionData Random
        {
            get
            {
                IUnitSelectionData result = null;

                while(result == null)
                    result = this[UnityEngine.Random.Range(0, Count)];

                return result;
            }
        }

        public abstract IUnitSelectionData this[int index] { get; }

        public virtual IUnitSelectionData Find(UnitTemplate template)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Template == template)
                    return this[i];

            return null;
        }
    }

    public interface IUnitSelectionData
    {
        UnitTemplate Template { get; }

        ItemUpgradesData Upgrade { get; }
    }
}