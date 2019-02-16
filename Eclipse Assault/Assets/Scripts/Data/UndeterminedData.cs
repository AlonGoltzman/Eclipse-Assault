using Mgmt;
using System;

namespace Data
{
    public class UndeterminedData
    {
        public string Type;

        public Type Determine()
        {
            if (Type.Equals(GameConstants.DATA_WEAPONS))
            {
                return typeof(WeaponData);
            }
            return typeof(UndeterminedData);
        }
    }
}
