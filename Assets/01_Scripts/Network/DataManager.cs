using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{ 
    private List<Player> players = new List<Player>();
    private int _turn = -1;
    private int _round = 0;
    private PhotonView PV;
    public static DataManager Instance;
    public TextMeshProUGUI Round_TMP;
    public Button NextTurnButton;
    public Button StartGameButton;
    public GameObject PlayerListImage;
    public GameObject PlayerInfoButtonPrefab;
    [field: SerializeField] public bool _isMyTurn { get; private set; } = true;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this.gameObject);
        }

        PV = GetComponent<PhotonView>();
        DontDestroyOnLoad(gameObject);
    }
    
    public int FindMyPlayerIndex()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.PlayerList[i].IsLocal)
            {
                return i;
            }
        }

        return -1;
    }
    public void StartGame()
    {
        PV.RPC(nameof(StartGameRPC), RpcTarget.AllBuffered);
    }
    
    /* 버튼 */
    public void EndTurn()
    {
        if (!_isMyTurn)
        {
            return;
        }
        PV.RPC(nameof(TurnEndRPC), RpcTarget.AllBuffered);
    }
    public void SetPlayerList()
    {
        PV.RPC(nameof(SetPlayerListRPC), RpcTarget.AllBuffered);
    }
    
    private void EndRound()
    {
        _round++;
        Round_TMP.text = "Round: " + _round;
    }

    [PunRPC]
    private void StartGameRPC()
    {
        _turn = PhotonNetwork.PlayerList.Length;
        EndTurn();
        NextTurnButton.gameObject.SetActive(true);
        StartGameButton.gameObject.SetActive(false);
    }
    [PunRPC]
    private void TurnEndRPC()
    {
        _turn += 1;
        if (_turn >= PhotonNetwork.PlayerList.Length)
        {
            EndRound();
            _turn = 0;
        }
        if (FindMyPlayerIndex() == _turn)
        {
            _isMyTurn = true;
            NextTurnButton.gameObject.SetActive(true);
        }
        else
        {
            _isMyTurn = false;
            NextTurnButton.gameObject.SetActive(false);
        }
    }
    [PunRPC]
    private void SetPlayerListRPC()
    {
        players.Clear();
        foreach (Transform child in PlayerListImage.transform)
        {
            Destroy(child.gameObject) ;
        }
        foreach (var i in PhotonNetwork.PlayerList)
        {
            players.Add(i);
            GameObject temp = Instantiate(PlayerInfoButtonPrefab, Vector3.zero, Quaternion.identity);
            temp.transform.parent = PlayerListImage.transform;
            temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.NickName;
        }
       
    }
}