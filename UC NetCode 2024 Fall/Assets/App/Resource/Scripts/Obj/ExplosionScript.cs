using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ExplosionScript : NetworkBehaviour
{
    public float _damage = 40;

    // Triggered by animation
    [Rpc(SendTo.Server)]
    public void EndAnimRpc()
    {
        NetworkObject.Despawn();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Player hit!");
            other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
        }
    }
}
