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

using Assets.HeroEditor4D.Common.CharacterScripts;

namespace Game
{
	public class UnitNavigator : Unit.Module
	{
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

        public UnitSpeed Speed => Unit.Speed;

        public virtual bool MoveTo(Vector3 destination, float stoppingDistance)
        {
            return MoveTo(destination.x, stoppingDistance);
        }
        public virtual bool MoveTo(float destination, float stoppingDistance)
        {
            var difference = destination - Unit.transform.position.x;

            DistanceLeft = Mathf.Abs(difference);

            Direction = DistanceLeft == 0f ? 0 : Math.Sign(difference);

            if (DistanceLeft > stoppingDistance)
            {
                XPosition = Mathf.MoveTowards(XPosition, destination, Speed.Value * Time.deltaTime);

                Body.CharacterAnimation.SetState(MovementMethodToCharacterState(Template.MovementMethod));

                return false;
            }
            else
            {
                Body.CharacterAnimation.SetState(CharacterState.Idle);

                return true;
            }
        }

        public static CharacterState MovementMethodToCharacterState(UnitMovementMethod method)
        {
            switch (method)
            {
                case UnitMovementMethod.Walk:
                    return CharacterState.Walk;

                case UnitMovementMethod.Sprint:
                    return CharacterState.Run;
            }

            Debug.LogError("No case defined for converting unit movement method: " + method.ToString() + " to Unit Navigator Base Speed, returning Walk State");
            return CharacterState.Walk;
        }
    }
}