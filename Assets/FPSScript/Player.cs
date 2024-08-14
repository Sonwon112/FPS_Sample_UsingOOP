using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 1.0f;
    public float rotateSpeed = 1.0f;
    float xRotate, yRotate;
    public GameObject MainCamera;
    public GameObject[] GunList;
    public GameObject[] gunSlotImage;
    public GameObject[] bulletCntTxt;
    public GameObject[] gunModeTxt;
    public Sprite[] gunSlotSprite;
    public float requireHoldTime = 2f;

    private GameObject currGun;


    private const int MAX_GUNSLOT_SIZE = 2;
    private const int MAX_INVENTORYSLOT_SIZE =10;
    private List<int> gunSlot = new List<int> ();
    private int currSelectGunSlot = 0;

    private List<int[]> bulletCntList = new List<int[]> ();

    private List<int[]> inventorySlot = new List<int[]> ();

    private bool isHold = false;
    private bool requireHold = false;
    private float holdTime = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        //gunPos = MainCamera.transform.Find("GunPos").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float mouseRotateX = -Input.GetAxis("Mouse Y") * Time.smoothDeltaTime * rotateSpeed;
        float mouseRotateY = Input.GetAxis("Mouse X") * Time.smoothDeltaTime * rotateSpeed;
        float wheelMovement = Input.GetAxis("Mouse ScrollWheel");
        if (wheelMovement != 0) { changeCurrSelectGunSlot(wheelMovement); }

        // 앞뒤 좌우 무빙
        transform.Translate(Vector3.forward * vertical * speed * Time.smoothDeltaTime);
        transform.Translate(Vector3.right * horizontal * speed * 0.8f * Time.smoothDeltaTime);

        // 캐릭터 회전 및 카메라 회전
        yRotate = yRotate + mouseRotateY;
        xRotate = xRotate + mouseRotateX;

        xRotate = Mathf.Clamp(xRotate, -90, 90);

        //Debug.Log(xRotate + ", " + yRotate);
        MainCamera.transform.eulerAngles = new Vector3(xRotate, MainCamera.transform.eulerAngles.y, 0);
        //Quaternion quat = Quaternion.Euler(new Vector3(0, yRotate, 0));
        transform.eulerAngles = new Vector3(0, yRotate, 0);
            
        if (Input.GetButtonDown("Interact")){
            if(!requireHold) CallInteract();
        }
        if (isHold && Input.GetButton("Interact")) {
            
            holdTime += Time.deltaTime;
            if(holdTime >= requireHoldTime)
            {
                CallInteract();
                requireHold = false;
                isHold = false;
                //Debug.Log("end Hold");
            }
        }else if(requireHold && Input.GetButton("Interact"))
        {
            isHold = true;
            holdTime = 0f;
            //Debug.Log("start Hold");
        }else if(requireHold && Input.GetButtonUp("Interact"))
        {
            isHold = false;
            requireHold = false;
            //Debug.Log("cancle Hold");
        }

        if (Input.GetButton("Fire1")) { CallShot(); }
        if (Input.GetButtonUp("Fire1")) { CallShotEnd(); }
        if (Input.GetButtonDown("Reload")) { CallReload(); }
        if (Input.GetButtonDown("ChangeGunMode")) { CallChangeGunMode(); }
    }

    void CallInteract()
    {
 
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward, Color.green, 10f);
        if(Physics.Raycast(transform.position,transform.forward, out hit, 10f))
        {   
            //Debug.Log(hit.collider.gameObject);
            if(hit.collider.gameObject != null)
            {
                GameObject target = hit.collider.gameObject;
                Interactable interact = target.GetComponentInParent<Interactable>();
                if (interact != null){
                    ItemType type = interact.Interact(isHold);
                    
                    switch(type)
                    {
                        case ItemType.GUN:
                            Gun gun = target.GetComponentInParent<Gun>();
                            if (!gun.isHold())
                            {
                                int slotNum = 0;
                                if (gunSlot.Count < MAX_GUNSLOT_SIZE)
                                {
                                    gunSlot.Add(gun.GetItemKey());
                                    slotNum = gunSlot.IndexOf(gun.GetItemKey());
                                }
                                else
                                {
                                    gunSlot[currSelectGunSlot] = gun.GetItemKey();
                                    slotNum = gunSlot.IndexOf(gun.GetItemKey());
                                }
                                SetGun();
                                SetGunSlotBullet(slotNum);
                                SetGunSlotImage(slotNum);
                                SetGunSlotModeText(slotNum);
                            }
                            break;
                        case ItemType.BULLET:
                            Bullet bullet = target.GetComponentInParent<Bullet>();
                            if(bullet != null)
                            {
                                int itemKey = bullet.getItemKey();
                                int[] tmp = null;
                                for(int i = 0; i < inventorySlot.Count; i++) {
                                    if(itemKey == inventorySlot[i][0])
                                    {
                                        if (inventorySlot[i][1] < bullet.getMaxCnt())
                                            tmp = inventorySlot[i];
                                        break;
                                    }
                                }

                                if (tmp != null)
                                {
                                    tmp[1] = tmp[1] + bullet.getCnt();
                                    if (tmp[1] > bullet.getMaxCnt())
                                    {
                                        inventorySlot.Add(new int[] { tmp[0], tmp[1] - bullet.getMaxCnt() });
                                        tmp[1] = bullet.getMaxCnt();
                                    }
                                }
                                else
                                {
                                    inventorySlot.Add(new int[] {itemKey, bullet.getCnt() });
                                }
                                for(int i = 0; i < gunSlot.Count; i++)
                                {
                                    if(itemKey == gunSlot[i])
                                    {
                                        SetGunSlotBullet(i);
                                    }
                                }
                            }

                            break;
                        case ItemType.HOLD:
                                requireHold = true;
                            break;
                    }
                   
                }
            }

        }
    }
    void CallShot()
    {
        if (currGun == null) return;
        Gun currGunTmp = currGun.GetComponent<Gun>();
        currGunTmp.Shot();
        SetGunSlotBullet(currSelectGunSlot);
    }

    void CallShotEnd()
    {
        if (currGun == null) return;
        Gun currGunTmp = currGun.GetComponent<Gun>();
        currGunTmp.setDoneShot(false);
    }

    void CallReload()
    {
        Gun currGunTmp = currGun.GetComponent<Gun>();
        if (currGunTmp.getReloadedCnt() >= currGunTmp.getMaxReloadedCnt()) return;
        int reloadCnt = currGunTmp.getMaxReloadedCnt() - currGunTmp.getReloadedCnt();
        int total = 0;
        for (int i = inventorySlot.Count - 1; i >= 0; i--)
        {
            total += inventorySlot[i][1];
        }
        if (total == 0) return;
        if (total < reloadCnt) reloadCnt = total;

        for (int i = inventorySlot.Count - 1; i >= 0; i--)
        {
            if (inventorySlot[i][0] == currGunTmp.GetItemKey())
            {
                

                inventorySlot[i][1] -= reloadCnt;
                currGunTmp.setReloadedCnt(currGunTmp.getReloadedCnt() + reloadCnt);
                if (inventorySlot[i][1]<= 0)
                {
                    reloadCnt = inventorySlot[i][1] * -1;
                    inventorySlot.Remove(inventorySlot[i]);
                }
                else
                {
                    break;
                }
            }
            
        }
        SetGunSlotBullet(currSelectGunSlot);
    }

    void CallChangeGunMode()
    {
        if (currGun == null) return;
        Gun currGunTmp = currGun.GetComponent<Gun>();
        currGunTmp.changeGunMode();
        SetGunSlotModeText(currSelectGunSlot);
    }

    private void changeCurrSelectGunSlot(float wheelMovement)
    {
        if(wheelMovement > 0)
        {
            currSelectGunSlot = currSelectGunSlot == 1 ? 0 : 1;
        }
        else
        {
            currSelectGunSlot = currSelectGunSlot == 0 ? 1 : 0;
        }
        SetGun();
    }

    void SetGunSlotBullet(int slotPos)
    {
        int itemKey = gunSlot[slotPos];
        int totalCnt = calcTotalCnt(itemKey);
        Gun gunTmp = null;
        foreach (GameObject tmp in GunList)
        {
            if (tmp.GetComponent<Gun>().GetItemKey() == itemKey) {
                gunTmp = tmp.GetComponent<Gun>();
                break;
            } 
        }
        int reloadeCnt = gunTmp.getReloadedCnt();

        TMP_Text tmp_txt = bulletCntTxt[slotPos].GetComponent<TMP_Text>();
        tmp_txt.text = reloadeCnt + "/" +totalCnt;
        bulletCntTxt[slotPos].SetActive(true);

    }

    void SetGunSlotImage(int slotPos)
    {
        int itemKey = gunSlot[slotPos];
        Sprite gunSprite = gunSlotSprite[itemKey - 1];
        Image tmp_image = gunSlotImage[slotPos].GetComponent<Image>();
        tmp_image.sprite = gunSprite;
        gunSlotImage[slotPos].SetActive(true);
    }

    void SetGunSlotModeText(int slotPos)
    {
        int itemKey = gunSlot[slotPos];
        foreach(GameObject tmp in GunList) { 
            if (tmp.GetComponent<Gun>().GetItemKey() == itemKey)
            {
                gunModeTxt[slotPos].GetComponent<TMP_Text>().text = tmp.GetComponent<Gun>().getGunMode();
                gunModeTxt[slotPos].SetActive(true);
                break;
            }
        }
    }

    void SetGun()
    {
        if (gunSlot.Count > 0)
        {
            int itemKey = gunSlot[currSelectGunSlot];
            foreach(GameObject tmp in GunList)
            {   
                if(tmp.GetComponent<Gun>().GetItemKey() == itemKey)
                {
                    tmp.SetActive(true);
                    currGun = tmp;
                }
                else
                {
                    tmp.SetActive(false);
                }
            }
        }
    }

    int calcTotalCnt(int itemKey)
    {
        int totalCnt = 0;
        for (int i = 0; i < inventorySlot.Count; i++)
        {
            if (inventorySlot[i][0] == itemKey)
            {
                totalCnt += inventorySlot[i][1];
            }
        }
        return totalCnt;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interact")
        {
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interact")
        {
            
        }
    }

    public int getGunSlotLength()
    {
        return gunSlot.Count;
    }

    public int getInventoryLength(int itemKey)
    {
        return inventorySlot.Count;
    }

    public int getMaxGunSlot()
    {
        return MAX_GUNSLOT_SIZE;
    }

    public int getMaxInventorySlot()
    {
        return MAX_INVENTORYSLOT_SIZE;
    }
}
