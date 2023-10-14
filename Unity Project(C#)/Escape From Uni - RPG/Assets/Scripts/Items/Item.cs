using UnityEngine;

[CreateAssetMenu(fileName ="New Item",menuName ="Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public int amount = 0;
    public int value;
    public Sprite icon;



    public virtual void Use()
    {
        // use the item
        // Something might happen

        Debug.Log("Using " + name);
        if (itemName == "Key")
        {
            FindObjectOfType<PlayerHealth>().playerHealth++;
        }
    }
}
