using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// A data class used to identify a weapon data file for the store.
    /// </summary>
    [Serializable]
    public class WeaponData 
    {
        public string Name;
        public string Description;
        public int Price;
        public float Damage;
        public float RateOfFire;

        public override string ToString()
        {
            return string.Format("Name:{0},Price:{1},Damage:{2},Rate Of Fire:{3} per second.\nDescription:{4}", Name, Price, Damage, RateOfFire, Description);
        }

        public string AdditionalDetails()
        {
            return string.Format("Damage:{0}.\nRate of fire:{1}.", string.Format("{0:0.##}", Damage), string.Format("{0:0.##}",RateOfFire));
        }
    }

}