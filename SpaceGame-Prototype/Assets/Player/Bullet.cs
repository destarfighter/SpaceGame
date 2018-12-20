using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int Damage = 10;

    public void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(Damage);
        }

        Destroy(gameObject);
    }
}
