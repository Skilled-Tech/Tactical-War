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
	public class ShowRegionStoryOperation : Operation
	{
        [SerializeField]
        protected RegionUI _UI;
        public RegionUI UI { get { return _UI; } }

        public override void Execute()
        {
            UI.Region.ShowStory();
        }
    }
}