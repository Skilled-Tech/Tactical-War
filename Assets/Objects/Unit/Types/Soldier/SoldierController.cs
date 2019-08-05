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
	public class SoldierController : UnitController
	{
        public Animator Animator { get; protected set; }

        [SerializeField]
        protected float speed = 5f;
        public float Speed { get { return speed; } }

        [SerializeField]
        protected float attackRange;
        public float AttackRange { get { return attackRange; } }

        [SerializeField]
        protected float attackDuration;
        public float AttackDuration { get { return attackDuration; } }

        protected virtual void Start()
        {
            Animator = Dependancy.Get<Animator>(Unit.gameObject);

            Target = Level.Instance.Proponents.Enemy;
        }

        public Proponent Target { get; protected set; }

        void Update()
        {
            var distanceToTarget = Vector3.Distance(transform.position, Target.Base.transform.position);

            if(distanceToTarget > attackRange)
            {
                Animator.SetBool("Walk", true);

                var position = transform.position;

                position.x += speed * Time.deltaTime;

                transform.position = position;
            }
            else
            {
                Animator.SetBool("Walk", false);

                if(isAttacking)
                {

                }
                else
                {
                    StartCoroutine(Attack());
                }
            }
        }

        bool isAttacking = false;
        IEnumerator Attack()
        {
            isAttacking = true;

            Animator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackDuration);

            Unit.DoDamage(Target.Base, 40f);

            isAttacking = false;
        }
    }
}