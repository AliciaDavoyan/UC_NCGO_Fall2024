using ParrelSync.NonCore;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileObj : NetworkBehaviour
{
    [SerializeField] float _speed = 40f;
    [SerializeField] private float _damage = 10;
    [SerializeField] private float destructTime = 5f;

    // TODO: Hook up projectile to pool system
    /** private ObjectPool<ProjectileObj> _projectilePool;
     // I can't tell where to go from here, I've watched like 5 videos and looked at the documentation but it just confused me more
    I'm not sure what I'm meant to do here if not to use object pooling
    **/

public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // _projectilePool = new ObjectPool<ProjectileObj>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, false, 200, 100_000);
        GetComponent<Rigidbody>().velocity = this.transform.forward * _speed;
        StartCoroutine(AutoDestruct());

    }



    private void OnCollisionEnter(Collision other)
    {
        //make sure its a player and that the bullet doesn't match the owner, meaning we can't do friendly fire.
        if (other.gameObject.tag.Equals("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId != this.OwnerClientId)
        {
            other.gameObject.GetComponent<HealthNetScript>().DamageObjRpc(_damage);
        }
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(destructTime);
        this.NetworkObject.Despawn();
    }

}
