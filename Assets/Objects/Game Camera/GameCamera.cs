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
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        new public Camera camera { get; protected set; }

        public GameCameraPanZone PanZone { get; protected set; }

        [SerializeField]
        protected float range;
        public float Range { get { return range; } }

        [SerializeField]
        protected float speed;
        public float Speed { get { return speed; } }

        protected virtual void Start()
        {
            camera = GetComponent<Camera>();

            PanZone = FindObjectOfType<GameCameraPanZone>();
        }

        protected virtual void Update()
        {
            var position = transform.position;

            position.x -= PanZone.Delta.x * speed * Time.unscaledDeltaTime;
            position.x = Mathf.Clamp(position.x, -range, range);

            transform.position = position;
        }
    }
}