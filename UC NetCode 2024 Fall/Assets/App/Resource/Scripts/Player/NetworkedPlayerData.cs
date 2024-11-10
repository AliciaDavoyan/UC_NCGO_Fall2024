using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkedPlayerData : NetworkBehaviour
{
    public NetworkList<PlayerInfoData> _allConnectedPlayers; // Current connected players n game
    private int _players = -1;
    private ulong _serverLocalID;

    private Color[] _PlayerColors = new Color[]
    {
        Color.blue, Color.yellow, Color.green, Color.white, Color.black
    };

    private void Awake()
    {
        // Avoid memory leaks by ini network list here
        _allConnectedPlayers = new NetworkList<PlayerInfoData>(readPerm: NetworkVariableReadPermission.Everyone);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvents;
            _serverLocalID = NetworkManager.ServerClientId;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvents;
        }
        base.OnNetworkDespawn();
    }

    private void OnConnectionEvents(NetworkManager netManager, ConnectionEventData eventData)
    {
        if(eventData.EventType == ConnectionEvent.ClientConnected)
        {
            // When client connects create data
            CreateNewClientData(eventData.ClientId);
        }

        if (eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            RemovePlayerData(FindPlayerInfoData(eventData.ClientId));
            _players--;
        }
    }

    private void CreateNewClientData(ulong clientID)
    {
        // Creating new player info
        PlayerInfoData playerInfoData = new PlayerInfoData(clientID);

        // Future work add or modify name

        // Check to see if server matches parameter clientID
        if(_serverLocalID == clientID)
        {
            // If we are the hose, assume we are always ready!
            playerInfoData._isPlayerReady = true;
        } else
        {
            // Clients are set to false
            playerInfoData._isPlayerReady = false;
        }

        _players++;

        playerInfoData._colorId = _PlayerColors[_players];

        // Add to netList

        _allConnectedPlayers.Add(playerInfoData);
    }

    public void RemovePlayerData(PlayerInfoData playerData)
    {
        _allConnectedPlayers.Remove(playerData);
    }

    public PlayerInfoData FindPlayerInfoData(ulong clientID)
    {
        return _allConnectedPlayers[FindPlayerIndex(clientID)];
    }

    private int FindPlayerIndex(ulong clientID)
    {
        int myMatch = -1;

        for (int i = 0; i < _allConnectedPlayers.Count; i++)
        {
            if (clientID == _allConnectedPlayers[i]._clientId)
            {
                myMatch = i;
            }
        }
        return myMatch;
    }

    public void UpdateReadyClient(ulong clientID, bool isReady)
    {
        int idx = FindPlayerIndex(clientID);

        if (idx == -1) { return; }

        // Grab info, change it, and pass it back to networkList
        PlayerInfoData playerInfo = new PlayerInfoData();

        // Copy data
        playerInfo = _allConnectedPlayers[idx];
        
        // Change Status
        playerInfo._isPlayerReady = isReady;

        // Update new status to list
        _allConnectedPlayers[idx] = playerInfo;

    }

}
