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
        public float BaseSpeed
        {
            get
            {
                switch (Template.MovementMethod)
                {
                    case UnitMovementMethod.Walk:
                        return 1.6f;

                    case UnitMovementMethod.Sprint:
                        return 3f;
                }

                Debug.LogError("No case defined for converting unit movement method: " + Template.MovementMethod.ToString() + " to Unit Navigator Base Speed, returning 0");
                return 0f;
            }
        }
        public float Speed
        {
            get
            {
                return BaseSpeed * Unit.TimeScale.Rate;
            }
        }

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
            var difference = destination - Unit.transform.position.x;

            DistanceLeft = Mathf.Abs(difference);

            Direction = DistanceLeft == 0f ? 0 : Math.Sign(difference);

            if (DistanceLeft > stoppingDistance)
            {
                XPosition = Mathf.MoveTowards(XPosition, destination, Speed * Time.deltaTime);

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