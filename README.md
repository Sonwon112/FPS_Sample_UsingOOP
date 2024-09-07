# FPS 프로젝트 

## 초기 기획안
- 4종류의 총기
    1. 연발, 1번 총알만 사용가능
    2. 연발/단발, 2번 총알만 사용가능
    3. 연발/점사, 3번 총알만 사용가능
    4. 단발, 4번 총알만 사용가능
- w/a/s/d를 통해 움직이며, 달리기, 점프, 앉기가 가능해야한다.
- 플레이어는 한번에 2개의 총만 들 수 있고, 총알은 각 총알 별로 30발씩(변동이 가능해야 함) 인벤토리 한칸을 차지
- 인벤토리는 총 5칸 (Tab 키를 통해 확인 가능)
- 총 발사 시 이펙트와 사운드가 발생해야한다.
- 필드에 랜덤으로 생성되는 적을 향해 총을 발사하여 처치하여야한다
- 적도 3종류로 색깔별로 체력의 차이가 있어야한다.

1. Gun, Bullet, Enemy라는 상위 클래스, Interactable이라는 Interface 생성
2. Gun, Bullet은 Interactable 인터페이스를 구현하여 플레이어가 필드에 드랍되는 총과 총을 주울 수 있어야한다. 또한 각각 itemKey를 가지고 있어야한다. ItemKey를 통해 총이 어떤 총알을 사용할 수 있는지 구분<br/>
&rarr;총 바꾸기는 어떻게 구현을 할것인지 ? (상호작용 키 꾹 눌러서 변경하기, 상호작용시 버릴 총 선택)
&rarr;인벤토리에서 총알 버리기 및, 총알 변경하기 ? (상호작용시 버릴 총알 선택)
&rarr;총알 소모와 인벤토리 업데이트 처리 과정도 고려하여 플레이어 인벤토리 작성할 것
3. 플레이어 인벤토리는 배열로 구성하여 itemKey를 0, 아이템의 개수를 1 인덱스를 가지게끔 구성된 배열을 원소로 가짐, 인벤토리 정렬은 itemKey 오름차순으로 정렬할것
4. 총알 별 데미지도 다르게 구현

![FPS](https://github.com/user-attachments/assets/51ea3335-a30d-4464-9191-e3adafcbdfec)
![FPS_순서](https://github.com/user-attachments/assets/3fe07512-b542-4d86-8840-da32be0bf1c0)

## 개발후 기획 점검
- 4종류의 총기 구현 완료 및 확장 가능
    1. 연발, 1번 총알만 사용가능
    2. 연발/단발, 2번 총알만 사용가능
    3. 연발/점사, 3번 총알만 사용가능
    4. 단발, 4번 총알만 사용가능
- w/a/s/d를 통한 앞뒤좌우 이동과 화면 회전만 가능(점프, 앉기 기능이 필요하지 X)
- 플레이어는 한번에 2개의 총만 들 수 있고, 총알은 각 총알 별로 지정된 총알 개수만큼 인벤토리 한칸을 차지
- 인벤토리 UI 미구현
- 총 발사 시 이펙트와 사운드가 발생하게 구현완료
- 필드에 랜덤으로 생성되는 적을 향해 총을 발사하여 처치하게끔 구현완료, 현재 지정된 적을 처치시 승리, 지정된 수만큼 적이 생성되면 패배하게끔 구현
- 적은 4종류로 색깔별로 체력의 차이가 있게 구현완료
- 승리 조건은 일정 수의 적을 처치, 패배 조건은 적이 지정된 숫자만큼 생성되면 패배

## 개발된 스크립트 클래스 구조
![FPS Real](https://github.com/user-attachments/assets/2f50dec8-ea89-4d28-a26f-900f1c030c97)

## 핵심 코드
### 플레이어가 상호작용 구현 부분
```C#
 void CallInteract()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward, Color.green, 10f);
        if(Physics.Raycast(transform.position,transform.forward, out hit, 10f))
        {   
            if(hit.collider.gameObject != null)
            {
                GameObject target = hit.collider.gameObject;
                Interactable interact = target.GetComponentInParent<Interactable>();
                if (interact != null){
                    ItemType type = interact.Interact(isHold); // 상호작용 후 상호작용한 대상이 어떤 타입인지 받아옴
                    switch(type)
                    {
                        case ItemType.GUN: // 상호작용 대상이 총일때
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
                        case ItemType.BULLET: // 상호작용 대상이 아이템일때
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
                        case ItemType.HOLD: // 총과 상호작용시 총 변경 상황
                                requireHold = true;
                            break;
                    }
                   
                }
            }

        }
    }

```
### 총 발사(연발, 단발, 점사)
```C#
public void Shot()
    {
        //Debug.Log(doneShot);
        if (doneShot) return; 
        // 단발, 점사 시 한번의 사격이 끝났을 경우 더이상 쏘지 못하게 구현
        if (reloadedCnt == 0) return;
        // 장전된 총알이 없을 때 쏘지 못하게 구현
        // 점사 및 연발의 경우에도 지정된 딜레이만큼으 속도로 총알을 이어서 쏘게 구현
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
```


## 개발 후 느낀점
- Unity에서는 Script를 객체지향적으로 Interface와 상위클래스로 통합하여 작성하는 것도 중요하지만, 스크립트는 일종의 컴포넌트(모듈)로 적용되기 때문에 동일한 스크립트여도 public 변수를 적절히 활용하면 동일한 코드에서도 다르게 동작이 되게끔 구현이 가능해집니다. 
- public을 통해 객체를 주입하거나, Find 함수를 통해 현재 Scene에 생성되어있는 객체를 찾아 주입하는 방식이 기존에는 아무 생각이 없었지만 의존 주입을 통해서 하고 있다는 것을 알게 되었습니다.
- 처음으로 프로젝트에서 enum과 interface를 적용하여 개발하였는데 재활용성과 편의성이 올라간다는 것을 알았고 앞으로 개발시에 Enum과 Interface를 적절히 활용할 것 같습니다.
- 개발하다가 생긴 의문점은 GameManager가 해야할 역할이 다양하고 많다보니 이것을 코드에서 분리하여서 써야할지 하나로 통합하여도 되는지가 의문이긴합니다. 현재 프로젝트에서는 처리해야할 일이 많지 않아 하나로 통합하여 관리중입니다.
- 또한 캡슐화를 통해 직접 변수에 접근하지 않고 getter, setter를 통해 처리하게끔하여서 객체 지향 프로그래밍의 원칙을 지키려고 했습니다.
- 기획과 설계에 최대한 따라 개발하지만 기능 구현을 위해 벗어나게 혹은 추가하여서 작업을 할수 밖에 없다는 것을 한번더 느꼈습니다.
- 이번 개발을 하면서 PlayOnShot()과 Time.timeScale 기능을 추가적으로 알게 되었습니다.

## 아쉬운 점
&nbsp;클래스 다이어그램을 정리하면서 UI를 표시하기 위해 OnTriggerEnter와 OnTriggerExit가 여러부분에 중복 코딩 된것을 보고 별도 스크립트로 분리하였다면 조금더 좋았을 것 같습니다.
&nbsp; 기획에서 놓친 부분인 인벤토리 구현 처럼 UI 부분이 조금 아쉽게 느껴졌고 이부분에 대해서 조금더 공부해봐야겠다고 생각하였습니다.

## 동작 화면
![img1](https://github.com/user-attachments/assets/3eb3e8cd-1d55-4b7e-bdbc-370f7d0d448b)<br/>
총와 총알을 주울수 있게 구현
![img2](https://github.com/user-attachments/assets/8f572785-84e0-41e2-8aba-375896d20402)<br/>
총과 총알을 줍고 장전한 모습
![img3](https://github.com/user-attachments/assets/8292e8a5-68c2-42a9-9984-2422a9c76dfa)<br/>
게임을 시작하고 적이 스폰된 모습
![img4](https://github.com/user-attachments/assets/fb7ff3f0-90db-463c-9aaa-b060ffaba5d4)<br/>
승리 조건을 충족하고 승리한 모습
[실행영상](https://drive.google.com/file/d/1s29mgxBuUjLFOk1KKdascTfdwgj7h8VC/view?usp=sharing)
