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
    public class GameCameraPanZone : MonoBehaviour, IDragHandler
    {
        public float acceleration = 10f;

        void Update()
        {
            Velocity = Vector3.MoveTowards(Velocity, Vector3.zero, acceleration * Time.deltaTime);
        }

        public Vector3 Velocity { get; protected set; }
        public void OnDrag(PointerEventData eventData)
        {
            Velocity = new Vector3(eventData.delta.x, 0f);
        }
    }
}