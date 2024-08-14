using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour, Interactable
{
    public ItemType itemType = ItemType.GUN;
    public int itemKey;
    public int damage = 1;
    public float shotPow = 100;
    public GameObject eachBullet;
    public GameObject shotEffect;
    public AudioClip shotSound;
    public float shotDelay = 3f;

    public GunMode[] gunModeList;

    private GunMode currGunMode;
    private int currGunModeIndex = 0;
    private bool doneShot;
    private const int burstShotCnt = 3;
    private int currShotCnt = 0;
    private float prevShotTime = 0f;
    private float currShotTime = 0f;

    public int MAX_RELOADED_CNT = 30;
    private int reloadedCnt = 0;

    private GameObject canvas;
    private TMP_Text description;
    private GameObject InteractArea;
    private bool holdState = false;
    private GameObject bulletSpawn;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    private void Start()
    {   
        canvas = transform.Find("Canvas").gameObject;
        description = canvas.transform.Find("Description").gameObject.GetComponent<TMP_Text>();
        InteractArea = transform.Find("InteractArea").gameObject;
        bulletSpawn = transform.Find("bulletSpawn").gameObject;
        audioSource = GetComponent<AudioSource>();
        currGunMode = gunModeList[currGunModeIndex];
        //Debug.Log(description);
    }
    public ItemType Interact(bool isChange)
    {   
        if(!holdState)
            callDestory();
        else if (isChange)
        {
            holdState = false;
            callDestory();
        }
        else return ItemType.HOLD;
        //InteractArea.SetActive(false);
        return itemType;
    }

    public void Shot()
    {
        //Debug.Log(doneShot);
        if (doneShot) return;
        if (reloadedCnt == 0) return;
        if (currShotTime == 0f || prevShotTime+shotDelay < currShotTime) {
            reloadedCnt--;
            GameObject bulletTmp = Instantiate(eachBullet, bulletSpawn.transform);
            bulletTmp.GetComponent<EachBullet>().setDamage(damage);
            bulletTmp.GetComponent<Rigidbody>().AddForce(bulletTmp.transform.forward * shotPow, ForceMode.Impulse);
            GameObject effectTmp = Instantiate(shotEffect, bulletSpawn.transform);
            Destroy(effectTmp, 3);
            audioSource.Stop();
            audioSource.Play();
            switch (currGunMode)
            {
                case GunMode.SHOT:
                    doneShot = true;
                    break;
                case GunMode.BURST:
                    if (currShotCnt >= burstShotCnt-1) doneShot = true;
                    else currShotCnt++;
                    break;
                case GunMode.VOLLEY:
                    break;
            }
            prevShotTime = currShotTime;
        }
        currShotTime += Time.deltaTime;
        
    }

    public void setDoneShot(bool doneShot)
    {
        this.doneShot = doneShot;
        currShotCnt = 0;
        currShotTime = 0f;
    }
    
    void Reload()
    {

    }

    public string getGunMode()
    {
        string result = "";
        switch (currGunMode)
        {
            case GunMode.SHOT:
                result = "shot";
                break;
                
            case GunMode.BURST:
                result = "burst";
                break;
                
            case GunMode.VOLLEY:
                result = "volley";
                break;
        }
        return result;
    }
    public string changeGunMode()
    {
        currGunModeIndex++;
        if(currGunModeIndex >= gunModeList.Length)
        {
            currGunModeIndex = 0;
        }
        currGunMode = gunModeList[currGunModeIndex];
        return getGunMode();
    }
    public int GetItemKey()
    {
        return itemKey;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            if(player != null)
            {
                int len = player.getGunSlotLength();
                if(len == player.getMaxGunSlot())
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

    public void callDestory()
    {
        Destroy(gameObject);
    }

    public void HandOn()
    {
        InteractArea.SetActive(false);
    }

   
    public void setHold(bool hold)
    {
        holdState = hold;
    }
    public bool isHold()
    {
        return holdState;
    }

    public int getReloadedCnt()
    {
        return reloadedCnt;
    }

    public int getMaxReloadedCnt()
    {
        return MAX_RELOADED_CNT;
    }

    public void setReloadedCnt(int reloadedCnt)
    {
        this.reloadedCnt = reloadedCnt;
    }
}
