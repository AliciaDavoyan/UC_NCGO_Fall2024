using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _startBttn, _leaveBttn, _readyBttn;
    [SerializeField] private GameObject _panelPrefab; // The prefab we place inside the contents
    [SerializeField] private GameObject _ContentGO; // Where we are spawning panelPrefabs to
    [SerializeField] private TMP_Text rdyTxt; // upade status to user

    // List of network players
    [SerializeField] private NetworkedPlayerData _networkPlayers;

    public List<GameObject> _PlayerPanels = new List<GameObject>();

    private ulong _myLocalClientID;

    private bool isReady = false;

    private void Start()
    {
        _myLocalClientID = NetworkManager.ServerClientId;

        if (IsServer)
        {
            // Inform server and hide rdy bttn
            rdyTxt.text = "Waiting for Players";
            _readyBttn.gameObject.SetActive(false);
        }
        else
        {
            // Client
            rdyTxt.text = "Not Ready";
            _readyBttn.gameObject.SetActive(true);
        }

        _networkPlayers._allConnectedPlayers.OnListChanged += NetPlayersChanged;
        _leaveBttn.onClick.AddListener(LeaveBttnClick);
        _readyBttn.onClick.AddListener(ClientRdyBttnToggle);

    }

    private void ClientRdyBttnToggle()
    {
        if (IsServer) { return; }

        isReady = !isReady;
        if(isReady)
        {
            rdyTxt.text = "Ready";
        } else
        {
            rdyTxt.text = "Not Ready";
        }
        RdyBttnToggleServerRpc(isReady);
    }

    // RPC call for when bttn is clicked
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RdyBttnToggleServerRpc(bool readyStatus, RpcParams rpcParams = default)
    {
        Debug.Log("From Rdy bttn RPC");
        _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
    }

    private void LeaveBttnClick()
    {
        if(!IsServer)
        {
            QuitLobbyServerRpc();
        }
        else
        {
            foreach (PlayerInfoData playerdata in _networkPlayers._allConnectedPlayers)
            {
                if(playerdata._clientId != _myLocalClientID)
                {
                    KickUserBttn(playerdata._clientId);
                }
            }
            NetworkManager.Shutdown();
            SceneManager.LoadScene(0);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void QuitLobbyServerRpc(RpcParams rpcParams = default)
    {
        KickUserBttn(rpcParams.Receive.SenderClientId);
    }

    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeEvent)
    {
        Debug.Log("Net Players has changed event fired!");
        PopulateLabels();
    }

    // Populate Panels
    [ContextMenu("PopulateLabel")]
    private void PopulateLabels()
    {
        // Clear Panels
        ClearPlayerPanel();

        // Loop all player info from allentworker players and then create new panels
        bool allReady = true; // Used for logic on server

        foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            // Instatiate
            GameObject newPlayerPanel = Instantiate(_panelPrefab, _ContentGO.transform);
            PlayerLabel _playerLabel = newPlayerPanel.GetComponent<PlayerLabel>();

            // Subscribe to kick events on the panels
            _playerLabel.onKickClicked += KickUserBttn;

            // Depending on client vs server, we are going to show/hide kick bttns
            if(IsServer && playerData._clientId != _myLocalClientID)
            {
                // Ensure that we are the host and set active kick buttons that don't match the server
                _playerLabel.SetKickActive(true);

                // While we are at it, ensure servers ready button is hidden, we assume server is always ready
                _readyBttn.GameObject().SetActive(false);
            }
            else
            {
                // Ensure clients don't have set kicked buttons visible, but Ready Bttn is visible
                _playerLabel.SetKickActive(false);
                //_readyBttn.GameObject().SetActive(true);
            }

            // Display info to UI
            _playerLabel.SetPlayerLabelName(playerData._clientId);
            _playerLabel.SetReady(playerData._isPlayerReady);
            _playerLabel.SetPlayerColor(playerData._colorId);
            _PlayerPanels.Add(newPlayerPanel);

            if(playerData._isPlayerReady == false)
            {
                allReady = false;
            }

        }

        // Check if everyone is ready, host should see if its ready or not
        if (IsServer)
        {
            if(allReady)
            {
                if(_networkPlayers._allConnectedPlayers.Count > 1)
                {
                    rdyTxt.text = "Ready to start";
                    _startBttn.gameObject.SetActive(true);
                } else
                {
                    rdyTxt.text = "Empty Lobby";
                }
            }
        }
        else
        {
            _startBttn.gameObject.SetActive(false);
            rdyTxt.text = "Waiting for ready players";
        }

    }

    private void KickUserBttn(ulong kickTarget)
    {
        if (!IsServer || !IsHost) return;

        foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            if (playerData._clientId == kickTarget)
            {
                // Remove player from the list
                //_networkPlayers._allConnectedPlayers.Remove(playerData);

                KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));

                // Remove from the network
                NetworkManager.Singleton.DisconnectClient(kickTarget);
            }
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void KickedClientRpc(RpcParams rpcParams)
    {
        SceneManager.LoadScene(0);
    }

    // Clear Panels
    private void ClearPlayerPanel()
    {
        foreach (GameObject panel in _PlayerPanels)
        {
            Destroy(panel);
        }

        _PlayerPanels.Clear();
    }
}
