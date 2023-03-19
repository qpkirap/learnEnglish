using System;
using System.Globalization;

namespace Util
{
    public static class AmountUtils
    {
        private const string BaseMultipliers = "|k|M|G|T|P|E|Z|Y|kY|MY|GY|TY|PY|EY|ZY|YY|kYY|MYY|GYY|TYY|PYY|EYY|ZYY|YYY";
        private const string DefaultFormat = "{0:0.##}{1}";

        private static string[] multipliers;
        private static string[] defaultMultipliers;

        public static void SetupMultipliersFromString(string multipliersStr)
        {
            if (!string.IsNullOrEmpty(multipliersStr)) multipliers = multipliersStr.Split('|');
        }
        
        public static string ToPrettyString(
            this long amount,
            bool truncate = false,
            string format = DefaultFormat,
            bool useDefaultMultipliers = false)
        {
            return ((double)amount).ToPrettyString(truncate, format, useDefaultMultipliers);
        }

        public static string ToPrettyString(
            this float amount,
            bool truncate = false,
            string format = DefaultFormat,
            bool useDefaultMultipliers = false)
        {
            return ((double)amount).ToPrettyString(truncate, format, useDefaultMultipliers);
        }

        public static string ToPrettyString(
            this double amount,
            bool truncate = false,
            string format = DefaultFormat,
            bool useDefaultMultipliers = false)
        {
            var currentMultipliers = GetMultipliers(useDefaultMultipliers);

            if (amount < 1000)
            {
                var check = Math.Truncate(amount);
                var remainder = (float)Math.Abs(amount - check);

                return remainder > 0 ? amount.ToString("0.0") : amount.ToString("0.");
            }

            int sign = Math.Sign(amount);
            amount = Math.Abs(amount);
            if (truncate) amount = Math.Truncate(amount);

            int multiplier = 0;
            while (amount >= 1000 && (multiplier < currentMultipliers.Length - 1))
            {
                amount *= 0.001;
                multiplier += 1;
            }

            amount *= sign;
            return string.Format(CultureInfo.InvariantCulture, format, amount, currentMultipliers[multiplier]);
        }

        private static string[] GetMultipliers(bool useDefaultMultipliers)
        {
            if (useDefaultMultipliers)
            {
                return defaultMultipliers ?? (defaultMultipliers = BaseMultipliers.Split('|'));
            }

            return multipliers ?? GetMultipliers(true);
        }
    }
}