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
using TMPro;

namespace Game
{
	public class LevelEndUI : LevelMenu.Element
	{
		[SerializeField]
        protected TMP_Text info;
        public TMP_Text Info { get { return info; } }

        public virtual void Show(Proponent winner)
        {
            if (winner is PlayerProponent)
            {
                info.text = "You Won";
            }
            else
            {
                info.text = "You Lost";
            }

            Show();
        }
    }
}