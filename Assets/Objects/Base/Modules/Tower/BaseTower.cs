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

using UnityEngine.EventSystems;

namespace Game
{
	public class BaseTower : Base.Module, IPointerClickHandler
	{
		[SerializeField]
        protected BaseTowerSlot[] slots;
        public BaseTowerSlot[] Slots { get { return slots; } }

        public event Action OnClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null) OnClick();
        }
    }
}