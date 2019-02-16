using ControllerSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mgmt
{
    public class LevelShop : Level
    {
        public enum ItemType
        {
            Weapon,
            Armor
        }

        /// <summary>
        /// Checks the current state and determines if a player can purchase an item.
        /// </summary>
        /// <param name="Price">The price of the item</param>
        /// <returns></returns>
        public bool CanPurchase(int Price)
        {
            return StateInRuntime.GetInstance().Points >= Price;
        }

        /// <summary>
        /// Purchases an item from the shop.
        /// </summary>
        /// <param name="GUID"></param>
        /// <param name="Type"></param>
        public void Purchase(string GUID, ItemType Type)
        {
            WeaponStats BoughtWeapon = FileMgr.GetInstance().LoadWeaponStats(GUID);
            BoughtWeapon.Purchased = true;
            PlayerState.Bought(BoughtWeapon.Price, GUID);
            PlayerState.Save();
        }

        /// <summary>
        /// Marks a specific item as select.
        /// </summary>
        /// <param name="GUID"></param>
        public void SelectWeapon(string GUID, ItemType Type)
        {
            PlayerState.SelectWeapon(GUID);
            PlayerState.Save();
        }

        /// <summary>
        /// Checks if an item is purchased.
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public bool IsPurchased(string GUID)
        {
            return PlayerState.HasItem(GUID);
        }
    }
}