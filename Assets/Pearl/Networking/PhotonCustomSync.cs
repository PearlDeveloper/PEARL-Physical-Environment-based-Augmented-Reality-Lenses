// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonCustomSync : MonoBehaviourPun, IPunObservable
{
    public bool SyncTestBool01;
    public bool SyncTestBool02;
    public bool SyncTestBool03;
    public string SyncTestString;
    public float SyncTestFloat;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SyncTestBool01);
            stream.SendNext(SyncTestBool02);
            stream.SendNext(SyncTestBool03);
        }
        else
        {
            SyncTestBool01 = (bool)stream.ReceiveNext();
            SyncTestBool02 = (bool)stream.ReceiveNext();
            SyncTestBool03 = (bool)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
