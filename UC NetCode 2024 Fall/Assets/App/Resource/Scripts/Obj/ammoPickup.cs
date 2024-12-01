using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ammoPickup : NetworkBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (!IsServer) return;
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<BulletSpawner>()._ammo.Value++;

            Destroy(gameObject);
        }
    }
}
