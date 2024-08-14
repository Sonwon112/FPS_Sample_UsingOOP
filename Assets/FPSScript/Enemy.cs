using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP = 10;
    public float speed = 2;

    private GameObject player;
    private GameObject gameManager;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform.position);
        transform.Translate(Vector3.forward * speed * Time.smoothDeltaTime);
        if (HP <= 0)
        {
            gameManager.GetComponent<GameManager>().discountEnemyCnt();
            Destroy(gameObject);

        }
    }
    public void takeDamage(int damage)
    {   
        HP -= damage;
        //Debug.Log(HP);
    }
}
