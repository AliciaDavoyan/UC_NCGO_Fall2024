using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TnTSpawner : NetworkBehaviour
{
    public NetworkObject TNT;

    [SerializeField] private float tickTime = 6f;
    [SerializeField] private float currentTime = 6f;

    public void FixedUpdate()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            SpawnTnTRpc();
            currentTime = Random.Range(tickTime, tickTime + 5f);
        }
    }

    [ContextMenu("SpawnTNT")]
    [Rpc(SendTo.Server)]
    public void SpawnTnTRpc()
    {
        NetworkObject tnt = NetworkManager.Instantiate(TNT, transform.position, transform.rotation);
        tnt.Spawn(true);
    }
}
