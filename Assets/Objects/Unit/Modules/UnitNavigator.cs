﻿using System;
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
	public class UnitNavigator : Unit.Module
	{
		[SerializeField]
        protected float speed = 2f;
        public float Speed { get { return speed; } }

        public float XPosition
        {
            get
            {
                return Unit.transform.position.x;
            }
            set
            {
                var position = Unit.transform.position;

                position.x = value;

                Unit.transform.position = position;
            }
        }

        public float DistanceLeft { get; protected set; }

        public float Direction { get; protected set; }

        public virtual bool MoveTo(Vector3 destination, float stoppingDistance)
        {
            return MoveTo(destination.x, stoppingDistance);
        }
        public virtual bool MoveTo(float destination, float stoppingDistance)
        {
            DistanceLeft = Mathf.Abs(destination - Unit.transform.position.x);

            if(DistanceLeft > stoppingDistance)
            {
                var difference = destination - Unit.transform.position.x;

                DistanceLeft = Mathf.Abs(difference);

                Direction = DistanceLeft == 0f ? 0 : Math.Sign(difference);

                XPosition = Mathf.MoveTowards(XPosition, destination, speed * Time.deltaTime);

                Body.CharacterAnimation.SetState(Assets.HeroEditor4D.Common.CharacterScripts.CharacterState.Walk);

                return false;
            }
            else
            {
                Body.CharacterAnimation.SetState(Assets.HeroEditor4D.Common.CharacterScripts.CharacterState.Idle);

                return true;
            }
        }
    }
}