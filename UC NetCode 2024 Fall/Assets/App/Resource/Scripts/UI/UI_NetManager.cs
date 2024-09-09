using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{

    [SerializeField] private Button _serverBttn, _clientBttn, _hostBttn;

    // Start is called before the first frame update
    void Start()
    {
        _serverBttn.onClick.AddListener(ServerClick);
        _clientBttn.onClick.AddListener(ClientClick);
        _hostBttn.onClick.AddListener(HostClick);

    }



    private void ServerClick()
    {
        NetworkManager.Singleton.StartHost();      // Starts the NetworkManager as just a server (that is, no local client).
        this.gameObject.SetActive(false);

    }

    private void ClientClick()
    {
        NetworkManager.Singleton.StartClient();        // Starts the NetworkManager as both a server and a client (that is, has local client)
        this.gameObject.SetActive(false);
    }

    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();      // Starts the NetworkManager as just a client.
        this.gameObject.SetActive(false);
    }
}
