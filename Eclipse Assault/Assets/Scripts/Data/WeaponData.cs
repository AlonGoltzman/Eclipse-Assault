using System;

namespace Data
{
    /// <summary>
    /// A data class used to identify a weapon data file for the store.
    /// </summary>
    [Serializable]
    public class WeaponData 
    {
        public string GUID;
        public string Name;
        public string Description;
        public int Price;
        public float Damage;
        public float RateOfFire;

        public override string ToString()
        {
            return string.Format("GUID:{0},Name:{1},Price:{2},Damage:{3},Rate Of Fire:{4} per second.\nDescription:{5}", GUID,Name, Price, Damage, RateOfFire, Description);
        }

        public string AdditionalDetails()
        {
            return string.Format("Damage:{0}.\nRate of fire:{1}.", string.Format("{0:0.##}", Damage), string.Format("{0:0.##}",RateOfFire));
        }
    }

}