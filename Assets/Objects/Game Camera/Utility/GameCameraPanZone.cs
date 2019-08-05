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
    public class GameCameraPanZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Vector2 Input { get; protected set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            CalculateInput(eventData.position.x, eventData.position.y);
        }

        public void OnDrag(PointerEventData eventData)
        {
            CalculateInput(eventData.position.x, eventData.position.y);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Input = Vector2.zero;
        }

        public void CalculateInput(float x, float y)
        {
            Input = new Vector2()
            {
                x = -(Screen.width / 2 - x) / Screen.width * 2,
                y = -(Screen.height / 2 - y) / Screen.height * 2,
            };
        }
    }
}