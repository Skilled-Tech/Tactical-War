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

using UnityEngine.Events;

using UnityEngine.EventSystems;

namespace Game
{
    public class GameCameraPanZone : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public Vector2 Delta { get; protected set; }

        public float acceleration = 10f;

        void Update()
        {
            Delta = Vector2.MoveTowards(Delta, Vector2.zero, acceleration * Time.unscaledDeltaTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Delta = Vector2.zero;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            Delta = eventData.delta;
        }
    }
}