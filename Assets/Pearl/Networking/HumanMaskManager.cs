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

public class HumanMaskManager : MonoBehaviourPun, IPunObservable
{
    public GameObject LocalPlayer;
    public GameObject OriginAnchor;
    public GameObject RemotePlayer;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && photonView.IsMine)
        {
            Vector3 relativePosition = OriginAnchor.transform.InverseTransformPoint(LocalPlayer.transform.position);
            relativePosition.y = 0;
            stream.SendNext(relativePosition);
        }
        else
        {
            Vector3 relativePosition = (Vector3)stream.ReceiveNext();
            relativePosition.y = -9;
            RemotePlayer.transform.position = OriginAnchor.transform.TransformPoint(relativePosition);
        }
    }
}
