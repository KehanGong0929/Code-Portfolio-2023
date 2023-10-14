using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public int space = 20;
    public List<Item> Items = new List<Item>();
    public bool Add(Item item)
    {
        bool existing = false;
        if (Items.Count >= space)
        {
            Debug.Log("Not enough room.");
            return false;
        }

        //Item copyItem = Instantiate(item);
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].name == item.name)
            {
                ++Items[i].amount;
                existing = true;
            }
        }

        if (!existing)
        {
            Items.Add(item);
            item.amount = 1;
        }

        if (onItemChangedCallBack != null)
            onItemChangedCallBack.Invoke();
        return true;
    }

    public void Remove(Item item)
    {
        if (item.amount > 1)
        {
            item.amount--;
        }
        else
        {
            item.amount = 0;
            Items.Remove(item);
        }
        if (onItemChangedCallBack != null)
            onItemChangedCallBack.Invoke();
    }
}
