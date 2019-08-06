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
	public class GameCamera : MonoBehaviour
	{
        public GameCameraPanZone PanZone { get; protected set; }

        [SerializeField]
        protected float range;
        public float Range { get { return range; } }

        [SerializeField]
        protected float speed;
        public float Speed { get { return speed; } }

        protected virtual void Start()
        {
            PanZone = FindObjectOfType<GameCameraPanZone>();
        }

        protected virtual void Update()
        {
            var position = transform.position;

            position.x += PanZone.Input.x * speed * Time.deltaTime;

            position.x = Mathf.Clamp(position.x, -range, range);

            transform.position = position;
        }

        protected virtual void OnDrawGizmos()
        {
            if(Application.isPlaying)
            {

            }
            else
            {
                Gizmos.color = Color.yellow;

                Gizmos.DrawWireSphere(transform.position + Vector3.right * range, 0.25f);

                Gizmos.DrawWireSphere(transform.position - Vector3.right * range, 0.25f);
            }
        }
    }
}