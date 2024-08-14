using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour, Interactable
{
    public ItemType itemType = ItemType.BULLET;
    public int itemKey;
    public int MAX_CNT = 90;

    private GameObject canvas;
    private TMP_Text description;
    private int cnt = 30;
    private bool holdState = false;
    // Start is called before the first frame update

    private void Start()
    {
        canvas = transform.Find("Canvas").gameObject;
        description = canvas.transform.Find("Description").gameObject.GetComponent<TMP_Text>();
    }

    public ItemType Interact(bool isChange)
    {
        Destroy(gameObject);
        return itemType;
    }

    public int getItemKey()
    {
        return itemKey;
    }

    public int getCnt()
    {
        return cnt;
    }

    public int getMaxCnt()
    {
        return MAX_CNT;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                int len = player.getGunSlotLength();
                if (len == player.getMaxInventorySlot())
                {
                    description.text = "Hold Press";
                    holdState = true;
                }
                canvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            holdState = false;
            description.text = "Press";
            canvas.SetActive(false);
        }
    }

}
