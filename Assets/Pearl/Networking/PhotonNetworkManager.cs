// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public static PhotonNetworkManager instance;
    public bool bListUsers = false;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();        
        

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = "GUEST " + Random.Range(1, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 20;
        bool success = PhotonNetwork.JoinOrCreateRoom("Pearl", roomOptions, TypedLobby.Default);
        Debug.Log("connected to masterserver with " + success);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Max number: " + PhotonNetwork.CurrentRoom.MaxPlayers);
        Player[] player = PhotonNetwork.PlayerList;

        for (int i = 0; i < player.Length; i++)
        {
            Debug.Log((i).ToString() + " : " + player[i].NickName + " ID = " + player[i].UserId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bListUsers)
        {
            bListUsers = false;
            foreach (var p in PhotonNetwork.PlayerList)
            {
                Debug.Log(p.UserId);
            }
        }
    }
}
