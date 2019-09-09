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
        int jewels;
        public int Jewels { get { return jewels; } }

        public int Get(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.Gold:
                    return gold;
                case CurrencyType.Jewels:
                    return jewels;
            }

            throw new NotImplementedException();
        }

        public static bool IsSufficient(Currency requirement, Currency proposal)
        {
            return IsSufficient(requirement, proposal.gold, proposal.jewels);
        }
        public static bool IsSufficient(Currency requirement, int gold, int xp)
        {
            if (requirement.gold > gold) return false;

            if (requirement.jewels > xp) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType()) return false;

            var currency = (Currency)obj;

            if (gold != currency.gold) return false;
            if (jewels != currency.jewels) return false;

            return true;
        }

        #region Operators
        public static bool operator == (Currency one, Currency two)
        {
            return one.Equals(two);
        }
        public static bool operator != (Currency one, Currency two)
        {
            return !one.Equals(two);
        }

        public static bool operator > (Currency one, Currency two)
        {
            if (one.gold < two.gold) return false;
            if (one.jewels < two.jewels) return false;

            return true;
        }
        public static bool operator < (Currency one, Currency two)
        {
            if (one.gold > two.gold) return false;
            if (one.jewels > two.jewels) return false;

            return true;
        }

        public static Currency operator * (Currency currency, float number)
        {
            return new Currency(Mathf.RoundToInt(currency.gold * number), Mathf.RoundToInt(currency.jewels * number));
        }
        #endregion

        public override int GetHashCode()
        {
            return gold.GetHashCode() ^ jewels.GetHashCode();
        }

        public override string ToString()
        {
            return FormatText(gold, jewels);
        }

        public static string FormatText(int gold, int jewels)
        {
            var text = "";

            if (gold > 0)
                text += gold.ToString("N0") + " GD";

            if (jewels > 0)
            {
                if (text.Length > 0) text += ", ";

                text += jewels.ToString("N0") + " JL";
            }

            return text;
        }

        public Currency(int gold, int jewels)
        {
            this.gold = gold;
            this.jewels = jewels;
        }
        public Currency(Dictionary<string, uint> Prices)
        {
            if (Prices.ContainsKey(CurrencyCodes.Gold))
                gold = (int)Prices[CurrencyCodes.Gold];
            else
                gold = 0;

            if (Prices.ContainsKey(CurrencyCodes.Jewels))
                jewels = (int)Prices[CurrencyCodes.Jewels];
            else
                jewels = 0;
        }
    }

    public enum CurrencyType
    {
        Gold, Jewels
    }

    public static class CurrencyCodes
    {
        public const string Gold = "GD";
        public const string Jewels = "JL";
    }
}