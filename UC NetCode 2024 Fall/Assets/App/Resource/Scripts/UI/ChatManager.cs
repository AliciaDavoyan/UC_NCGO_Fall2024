using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] TMP_Text textArea;
    [SerializeField] TMP_InputField textInput;

    //private ulong _clientID;
    //public string _playerName;

    // Start is called before the first frame update
    void Start()
    {
        textArea.SetText("");
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    /** public void SetPlayerLabelName(ulong playerName)
    {
        _clientID = playerName;
        _playerName = "Player " + playerName.ToString();
    } **/

    public void SendMessage()
    {
        AddTextServerRpc(textInput.text);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddTextServerRpc(string text)
    {
        AddTextClientRpc(text);
    }



    [ClientRpc]
    private void AddTextClientRpc(string text)
    {
        AddText(text);
    }

    void AddText(string chat)
    {
        string lastText = textArea.text;
        textArea.SetText(lastText + "\n" + chat);
    }

    public override void OnNetworkDespawn()
    {
        textArea.SetText("");
        base.OnNetworkDespawn();
    }
}
