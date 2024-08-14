using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] EnemyList;
    public float term = 100f;
    public int oneTimeSpawnCnt = 5;
    public int winCondition = 10;
    public int defeatCondition = 80;
    public TMP_Text text;
    public GameObject WinOrDeafeatCanvas;


    private bool start = false;

    private float prevTime = 0f;
    private float currTime = 0f;
    private int enemyCnt = 0;
    private int killEnemyCnt = 0;

    private TMP_Text GameEndText;
    private Image GameEndBackground;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        GameEndText = WinOrDeafeatCanvas.transform.Find("GameState").GetComponent<TMP_Text>();
        GameEndBackground = WinOrDeafeatCanvas.transform.Find("background").GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if(killEnemyCnt >= winCondition)
        {
            GameEnd("WIN");
        }
        if(enemyCnt >= defeatCondition)
        {
            GameEnd("DEFEAT");
        }

        if (start)
        {
            //Debug.Log(prevTime + ", " + currTime);
            if (prevTime + term < currTime)
            {
                prevTime = currTime;
                for (int i = 0; i < oneTimeSpawnCnt; i++)
                {
                    float xPos = Random.Range(0, 2); // 0 : + 康开, 1 : - 康开
                    float zPos = Random.Range(0, 2); // 0 : + 康开, 1 : - 康开
                    switch (xPos)
                    {
                        case 0:
                            xPos = Random.Range(10, 48);
                            break;
                        case 1:
                            xPos = Random.Range(-47, -9);
                            break;
                    }
                    switch (zPos)
                    {
                        case 0:
                            zPos = Random.Range(10, 48);
                            break;
                        case 1:
                            zPos = Random.Range(-47, -9);
                            break;
                    }
                    Vector3 pos = new Vector3(xPos, 1.2f, zPos);
                    int index = Random.Range(0, EnemyList.Count());
                    Instantiate(EnemyList[index], pos, new Quaternion());
                }
                enemyCnt += 10;

            }
        }
    }

    public void setStart()
    {

        start = !start;

        switch (start)
        {
            case true:
                text.text = "stop";
                break;
            case false:
                text.text = "start";
                break;
        }

    }

    void GameEnd(string state) {
        setStart();
        ClearEnemy();
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GameEndText.text = state;
        switch(state)
        {
            case "WIN":
                GameEndBackground.color = new Color(0f, 0f, 1f, 0.5f);
                break;
            case "DEFEAT":
                GameEndBackground.color = new Color(1f, 0f, 0f, 0.5f);
                break;
        }
        WinOrDeafeatCanvas.SetActive(true);
    }

    public void discountEnemyCnt()
    {
        enemyCnt -= 1;
        killEnemyCnt += 1;
    }

    void ClearEnemy()
    {
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemyObj in enemy)
        {
            Destroy(enemyObj);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

}
