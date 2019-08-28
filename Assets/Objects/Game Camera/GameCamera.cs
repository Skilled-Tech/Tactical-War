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
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        public Transform target;

        new public Camera camera { get; protected set; }

        public GameCameraPanZone PanZone { get; protected set; }

        [SerializeField]
        protected float range;
        public float Range { get { return range; } }

        [SerializeField]
        protected float speed = 1f;
        public float Speed { get { return speed; } }

        [SerializeField]
        protected float deAcceleration = 10f;
        public float DeAcceleration { get { return deAcceleration; } }

        public float XPosition
        {
            get
            {
                return transform.position.x;
            }
            set
            {
                var position = transform.position;

                position.x = Mathf.Clamp(value, -range, range);

                transform.position = position;
            }
        }

        public float Velocity { get; protected set; }

        protected virtual void Start()
        {
            camera = GetComponent<Camera>();

            PanZone = FindObjectOfType<GameCameraPanZone>();

            PanZone.DragEvent += OnDrag;
        }

        void OnDrag(PointerEventData obj)
        {
            Velocity = ScreenDeltaToVelocity(-PanZone.Delta).x;

            XPosition += Velocity * speed;
        }

        protected virtual void Update()
        {
            if(PanZone.PointerDown)
            {
                
            }
            else
            {
                if(Time.timeScale > 0)
                {
                    Velocity = Mathf.MoveTowards(Velocity, 0f, deAcceleration * Time.unscaledDeltaTime);

                    XPosition += Velocity * speed;
                }
            }
        }

        protected virtual Vector3 ScreenDeltaToVelocity(Vector2 delta)
        {
            return ScreenToWorld(delta) - ScreenToWorld(Vector3.zero);
        }

        protected virtual Vector3 ScreenToWorld(Vector3 coordinate)
        {
            return camera.ScreenToWorldPoint(coordinate);
        }
    }
}