using ControllerSO;
using Data;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Mgmt.LevelShop;

namespace Mgmt
{
    public class StoreMgr : MonoBehaviour
    {

        private LevelShop CurrentLevel;

        void Start()
        {
            CurrentLevel = (LevelShop)GameConstants.CURRENT_LEVEL;
            GameObject ItemTemplate = Resources.Load(GameConstants.PREFAB_PATH_STORE_ITEM_TEMPLATE, typeof(GameObject)) as GameObject;
            GameObject ItemDisplay = GameObject.Find(GameConstants.UI_STORE_NAME_ITEM_DISPLAY);

            WeaponStats[] weapons = Resources.LoadAll<WeaponStats>("Stats/Weapons");
            foreach (WeaponStats weapon in weapons)
            {

                GameObject CreatedItem = Instantiate(ItemTemplate, ItemDisplay.transform);
                CreatedItem.name = weapon.name + " Item Display";

                for (int i = 0; i < CreatedItem.transform.childCount; i++)
                {
                    GameObject Child = CreatedItem.transform.GetChild(i).gameObject;
                    if (Child.name.Equals(GameConstants.UI_STORE_NAME_ITEM_TEXT_CONTAINER))
                    {
                        for (int j = 0; j < Child.transform.childCount; j++)
                        {
                            GameObject GrandChild = Child.transform.GetChild(j).gameObject;
                            if (GrandChild.name.Equals(GameConstants.UI_STORE_NAME_ITEM_NAME))
                            {
                                GrandChild.GetComponent<Text>().text = weapon.ItemName;
                            }
                            else if (GrandChild.name.Equals(GameConstants.UI_STORE_NAME_ITEM_DESCRIPTION))
                            {
                                GrandChild.GetComponent<Text>().text = string.Format(GameConstants.TEXT_ITEM_DESCRIPTION, weapon.ItemDescription, weapon.AdditionalDetails());
                            }
                            else if (GrandChild.name.Equals(GameConstants.UI_STORE_NAME_ITEM_PRICE))
                            {
                                GrandChild.GetComponent<Text>().text = string.Format("Price: {0}", weapon.Price);
                            }
                        }
                    }
                    else if (Child.name.Equals(GameConstants.UI_STORE_NAME_PURCHASE_BUTTON))
                    {
                        ItemDataHolder ButtonDataHolder = Child.GetComponent<ItemDataHolder>();
                        ButtonDataHolder.GUID = weapon.WeaponGUID;
                        ButtonDataHolder.ItemType = ItemType.Weapon;
                    }
                }

                RevalidateShop();
            }

        }

        public void StartGame()
        {
            SceneManager.LoadScene(GameConstants.LEVEL_NAME_ARENA, LoadSceneMode.Single);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(GameConstants.LEVEL_NAME_MENU, LoadSceneMode.Single);
        }

        /// <summary>
        /// Purchases an item from the store.
        /// </summary>
        public void Purchase()
        {
            string PurchasedItem = EventSystem.current.currentSelectedGameObject.GetComponent<ItemDataHolder>().GUID;
            ItemType PurchasedItemType = EventSystem.current.currentSelectedGameObject.GetComponent<ItemDataHolder>().ItemType;
            CurrentLevel.Purchase(PurchasedItem, PurchasedItemType);
            RevalidateShop();
        }

        /// <summary>
        /// Selects an item from the purchased items.
        /// </summary>
        public void SelectItem()
        {
            string SelectedItem = EventSystem.current.currentSelectedGameObject.GetComponent<ItemDataHolder>().GUID;
            ItemType SelectedItemType = EventSystem.current.currentSelectedGameObject.GetComponent<ItemDataHolder>().ItemType;
            CurrentLevel.SelectWeapon(SelectedItem, SelectedItemType);

            RevalidateShop();
        }

        /// <summary>
        /// Verifies that all buttons display the correct values.
        /// </summary>
        private void RevalidateShop()
        {
            GameObject ItemTemplate = Resources.Load(GameConstants.PREFAB_PATH_STORE_ITEM_TEMPLATE, typeof(GameObject)) as GameObject;
            GameObject ItemDisplay = GameObject.Find(GameConstants.UI_STORE_NAME_ITEM_DISPLAY);

            FileMgr FileManager = FileMgr.GetInstance();
            foreach (Button PurchaseButton in ItemDisplay.GetComponentsInChildren<Button>())
            {
                ItemDataHolder DataHolder = PurchaseButton.GetComponent<ItemDataHolder>();
                WeaponStats Stats = FileManager.LoadWeaponStats(DataHolder.GUID);
                PurchaseButton.GetComponent<Button>().onClick.RemoveAllListeners();
                if ((Stats.DefaultWeapon || CurrentLevel.IsPurchased(DataHolder.GUID)))
                {
                    if (CurrentLevel.PlayerState.SelectedWeapon.WeaponGUID.Equals(Stats.WeaponGUID))
                    {
                        PurchaseButton.GetComponentInChildren<Text>().text = "Selected";
                        PurchaseButton.GetComponent<Button>().interactable = false;
                        DataHolder.Selected = true;
                    }
                    else
                    {
                        PurchaseButton.GetComponentInChildren<Text>().text = "Select Item";
                        PurchaseButton.GetComponent<Button>().interactable = true;
                        DataHolder.Selected = false;
                        PurchaseButton.GetComponent<Button>().onClick.AddListener(SelectItem);
                    }
                }
                else if (!Stats.Purchased && (CurrentLevel.CanPurchase(Stats.Price)))
                {
                    PurchaseButton.GetComponent<Button>().onClick.AddListener(Purchase);
                }
                else
                {
                    PurchaseButton.GetComponentInChildren<Text>().text = "Too Expensive";
                    PurchaseButton.GetComponent<Button>().interactable = false;
                }

            }
        }
    }
}