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
    [Serializable]
	public struct Currency
	{
		[SerializeField]
        int gold;
        public int Gold { get { return gold; } }

        [SerializeField]
        int xp;
        public int XP { get { return xp; } }

        public int Get(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.Gold:
                    return gold;
                case CurrencyType.XP:
                    return xp;
            }

            throw new NotImplementedException();
        }

        public static bool IsSufficient(Currency requirement, Currency proposal)
        {
            return IsSufficient(requirement, proposal.gold, proposal.xp);
        }

        public static bool IsSufficient(Currency requirement, int gold, int xp)
        {
            if (requirement.gold > gold) return false;

            if (requirement.xp > xp) return false;

            return true;
        }

        public Currency(int gold, int xp)
        {
            this.gold = gold;
            this.xp = xp;
        }
    }

    public enum CurrencyType
    {
        Gold, XP
    }
}