using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{

    [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn, _startBttn;

    [SerializeField] private GameObject _connectionBttnGroup;

    [SerializeField] private SpawnController _mySpawnController;

    // Start is called before the first frame update
    void Start()
    {
        _startBttn.gameObject.SetActive(false);
        if (_serverBttn != null) _serverBttn.onClick.AddListener(ServerClick);
        if (_clientBttn != null) _clientBttn.onClick.AddListener(ClientClick);
        if (_hostBttn != null) _hostBttn.onClick.AddListener(HostClick);
        if (_startBttn != null) _startBttn.onClick.AddListener(StartClick);

    }

    private void StartClick()
    {
        //hook up spawning here.
        if (IsServer)
        { 
            _mySpawnController.SpawnAllPlayers();
            _startBttn.gameObject.SetActive(false);
        }
    }

    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();      // Starts the NetworkManager as just a server (that is, no local client).
        _connectionBttnGroup.SetActive(false);
        _startBttn.gameObject.SetActive(true);

    }

    private void ClientClick()
    {
        NetworkManager.Singleton.StartClient();        // Starts the NetworkManager as both a server and a client (that is, has local client)
        _connectionBttnGroup.SetActive(false);
    }

    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();      // Starts the NetworkManager as just a client.
        _connectionBttnGroup.SetActive(false);
        _startBttn.gameObject.SetActive(true);
    }
}
