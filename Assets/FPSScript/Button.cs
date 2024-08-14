using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Button : MonoBehaviour, Interactable
{
    public ItemType Interact(bool isChange)
    {
        GameManager manager = GetComponentInParent<GameManager>();
        Debug.Log(manager);
        if (manager != null) { manager.setStart(); }
        return ItemType.NONE;
    }

}
