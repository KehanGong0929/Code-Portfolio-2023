using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;
    InventoryManager inventoryManager;
    InventorySlot[] slots;
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        inventoryManager.onItemChangedCallBack += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    void UpdateUI()
    {
        Debug.Log("Updating UI");
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventoryManager.Items.Count)
            {
                slots[i].AddItem(inventoryManager.Items[i]);
            }else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
