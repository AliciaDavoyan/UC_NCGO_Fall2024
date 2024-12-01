using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthNetScript : NetworkBehaviour
{
    [SerializeField] public float _startingHealth = 100f;
    [SerializeField] public float cooldown = 1.5f;
    [SerializeField] private bool _canDamage = true;
    [SerializeField] private NetworkVariable<float> _Health = new NetworkVariable<float>(100);
    public Image _healthBar;

    private GameScript _gameScript;

    [SerializeField] private NetworkedPlayerData _playerData;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Initalize health
        _Health.Value = _startingHealth;

        // Tap into the on value change
        _Health.OnValueChanged += UpdateHealth;
    }

    private void UpdateHealth(float previousvalue, float newvalue)
    {
        if (_healthBar != null)
        {
            _healthBar.fillAmount = newvalue / _startingHealth;
        }

        if (IsOwner)
        {
            if (newvalue < 0f)
            {
                //Talk to game script;
                FindObjectOfType<GameScript>().PlayerDeathRpc();
                HasDiedRpc();
            }
        }
        // TODO: Sync nework data here
        // _playerData.SetClientHealth(_Health); // And I'm completely lost
    }

    [Rpc(SendTo.Server)]
    public void HasDiedRpc()
    {
        NetworkObject.Despawn();
    }

    // Because we want to have other things damage the player we want ownership false
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void DamageObjRpc(float dmg)
    {
        if (!_canDamage) return;
        _Health.Value -= dmg;
        StartCoroutine(nameof(DamageCooldown));
        Debug.Log($"Damage recieved: {dmg}");
    }

    private IEnumerable DamageCooldown()
    {
        _canDamage = false;
        yield return new WaitForSeconds(cooldown);
        _canDamage = true;
    }
}
