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
    public class GameCameraPanZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        public int? PointerID { get; protected set; }
        public bool PointerDown { get { return PointerID.HasValue; } }

        public Vector2 Delta { get; protected set; }

        public event Action<PointerEventData> PointerDownEvent;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (PointerID.HasValue)
            {

            }
            else
            {
                PointerID = eventData.pointerId;

                Delta = eventData.delta;

                if (PointerDownEvent != null) PointerDownEvent(eventData);
            }
        }

        public event Action<PointerEventData> DragEvent;
        public void OnDrag(PointerEventData eventData)
        {
            if(PointerID.HasValue)
            {
                if(PointerID.Value == eventData.pointerId)
                {
                    Delta = eventData.delta;

                    if (DragEvent != null) DragEvent(eventData);
                }
            }
            else
            {

            }
        }

        public event Action<PointerEventData> DragEndEvent;
        public void OnEndDrag(PointerEventData eventData)
        {
            if (PointerID.HasValue)
            {
                if (PointerID.Value == eventData.pointerId)
                {
                    Delta = eventData.delta;

                    if (DragEndEvent != null) DragEndEvent(eventData);
                }
            }
            else
            {

            }
        }

        public event Action<PointerEventData> PointerUpEvent;
        public void OnPointerUp(PointerEventData eventData)
        {
            if (PointerID.HasValue)
            {
                if (PointerID.Value == eventData.pointerId)
                {
                    Delta = eventData.delta;

                    PointerID = null;

                    if (PointerUpEvent != null) PointerUpEvent(eventData);
                }
            }
            else
            {

            }
        }
    }
}