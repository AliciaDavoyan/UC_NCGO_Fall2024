using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] TMP_Text textArea;
    [SerializeField] TMP_InputField textInput;
    //[SerializeField] private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>();
    [SerializeField] private NetworkedPlayerData networkedPlayerData;

    //NetworkedPlayerData _playerData;
    //NetworkManger netManager;
    //public NetworkList<PlayerInfoData> _allConnectedPlayers;

    //private ulong _clientID;
    public string _playerName;

    // Start is called before the first frame update
    void Start()
    {
        textArea.SetText("");
        //_allConnectedPlayers = new NetworkList<PlayerInfoData>(readPerm: NetworkVariableReadPermission.Everyone);
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    /**
     public void SetPlayerLabelName(string playerName)
    {
        //_clientID = _playerData.FindPlayerInfoData.ClientID;
        _playerName = "Player " + OwnerClientId;
    }  **/

    public void SendMessage()
    {
        AddTextServerRpc(textInput.text);
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    private void AddTextServerRpc(string text, RpcParams rpcParams = default)
    {
        AddTextClientRpc(networkedPlayerData.GetPlayerName(rpcParams.Receive.SenderClientId) + " :" + text);
    }



    [Rpc(SendTo.ClientsAndHost)]
    private void AddTextClientRpc(string text)
    {
        AddText(text);
    }

    void AddText(string chat)
    {
        string lastText = textArea.text;
        textArea.SetText(lastText + "\n" + _playerName +  chat);
    }

    public override void OnNetworkDespawn()
    {
        textArea.SetText("");
        base.OnNetworkDespawn();
    }
}
