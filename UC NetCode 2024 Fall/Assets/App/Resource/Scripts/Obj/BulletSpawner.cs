using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
    [SerializeField] public NetworkVariable<int> _ammo = new NetworkVariable<int>(4);
    [SerializeField] private Transform _startingPoint;
    [SerializeField] private NetworkObject _ProjectilePrefab;

    /** Not the best example, could optimize by creating a pooled projectiles
     Pooling object pooling**/

    [Rpc(SendTo.Server, RequireOwnership = false)] 
    public void FireProjectileRpc(RpcParams rpcParams = default)
    {
        if (_ammo.Value > 0)
        {
            // Spawn out bullet at your starting point Position and use spawn with ownership so we can own the projectile
            NetworkObject newProjectile =
                NetworkManager.Instantiate(_ProjectilePrefab, _startingPoint.position, _startingPoint.rotation);

            newProjectile.SpawnWithOwnership(rpcParams.Receive.SenderClientId);

            _ammo.Value--;
        }
    }
}
