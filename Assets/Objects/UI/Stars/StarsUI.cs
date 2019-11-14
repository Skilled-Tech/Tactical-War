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
	public class StarsUI : UIElement
	{
		[SerializeField]
        protected Toggle[] toggles;
        public Toggle[] Toggles { get { return toggles; } }

        public virtual void Show(int number)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = number > i;
            }

            Show();
        }
    }
}