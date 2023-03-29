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

// for sub menu
public class VisHelper : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        target.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void hideSubMenusRemote()
    {
        GetComponent<PhotonView>().RPC("hideSubMenusRemoteSync", RpcTarget.All);
    }

    [PunRPC]
    public void hideSubMenusRemoteSync()
    {
        target.SetActive(false);
    }

    public void toggleSubMenuVisRemote()
    {
        GetComponent<PhotonView>().RPC("toggleSubMenuVisSync", RpcTarget.All);
    }

    [PunRPC]
    public void toggleSubMenuVisSync()
    {
        target.SetActive(!target.activeSelf);
    }
}
