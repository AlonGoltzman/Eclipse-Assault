using System;
using UnityEngine;

namespace ControllerSO
{

    [CreateAssetMenu(menuName ="Weapon")]
    public class WeaponStats : ScriptableObject
    {
        public string WeaponGUID = Guid.NewGuid().ToString();
        public int Price;
        public bool Purchased;

        public bool DefaultWeapon;

        public float FireRate;
        public float DamagePerShot;

        public GameObject Projectile;
        public bool ConsecutiveShot;

        public string ItemName;
        public string ItemDescription;    

        public string AdditionalDetails()
        {
            return string.Format("\n\t<color=Red>Damage</color>:{0}\n\t<color=Orange>Rate of fire</color>:{1}", string.Format("{0:0.##}", DamagePerShot), FireRate <= 0? "Constant" : string.Format("{0:0.##}",FireRate));
        }
    }

}