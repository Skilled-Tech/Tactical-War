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
	public class ShowRegionStoryOperation : Operation
	{
        [SerializeField]
        protected RegionUI _UI;
        public RegionUI UI { get { return _UI; } }

        public Core Core => Core.Instance;

        public override void Execute()
        {
            Core.UI.Fader.Operate(UI.Region.ShowStory);
        }
    }
}