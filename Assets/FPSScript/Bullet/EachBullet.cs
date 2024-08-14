using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachBullet : MonoBehaviour
{
    private int damage = 0;
    private float currTime;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > 10f)
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.takeDamage(damage);
        }
    }

    public void setDamage(int damage)
    {
        this.damage = damage;
    }

    public int getDamage()
    {
        return this.damage;
    }
}
