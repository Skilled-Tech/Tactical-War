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
	public class BaseGraphic : Base.Module
	{
        new public const string MenuPath = Base.Module.MenuPath + "Graphic/";

        public GameObject Instance { get; protected set; }

        public override void Init()
        {
            base.Init();

            Create();
        }

        private void Update()
        {
            if(Application.isEditor)
            {
                UpdateState();
            }
        }

        protected virtual void UpdateState()
        {
            Instance.transform.localPosition = Base.LevelData.Graphic.Position;

            Instance.transform.localScale = Vector3.one * Base.LevelData.Graphic.Scale;
        }

        protected virtual void Create()
        {
            Instance = Instantiate(Base.LevelData.Graphic.Prefab, transform);

            Instance.name = Base.LevelData.Graphic.Prefab.name;

            Tools.Layer.Set(Instance, Proponent.Layer);

            Instance.transform.localPosition = Base.LevelData.Graphic.Position;
            Instance.transform.localEulerAngles = Vector3.zero;

            UpdateState();

            var script = Instance.GetComponent<EntityHealthActivation>();

            Modules.Setup(script, Base);
        }
    }
}