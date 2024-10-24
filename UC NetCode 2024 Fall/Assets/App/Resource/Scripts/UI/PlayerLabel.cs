using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerLabel : MonoBehaviour
{

    [SerializeField] private TMP_Text _playerText;
    [SerializeField] private Button _kickBttn;
    [SerializeField] private RawImage _ReadyStatusImg, _PlayerColorImg;

    public event Action<ulong> onKickClicked;
    private ulong _clientID;


    private void OnEnable()
    {
        _kickBttn.onClick.AddListener((BttnKick_Clicked));
    }

    public void SetPlayerLabelName(ulong playerName)
    {
        _clientID = playerName;
        _playerText.text = "Player " + playerName.ToString();
    }

    private void BttnKick_Clicked()
    {
        onKickClicked?.Invoke(_clientID);
    }

    private void setKickActivated(bool isOn)
    {
        _kickBttn.gameObject.SetActive(isOn);
    }

    private void SetReady(bool ready)
    {
        if (ready)
        {
            _ReadyStatusImg.color = Color.green;
        } 
        else
        {
            _ReadyStatusImg.color = Color.red;
        }
    }

    public void SetPlayerColor(Color color)
    {
        _PlayerColorImg.color = color;
    }

}
