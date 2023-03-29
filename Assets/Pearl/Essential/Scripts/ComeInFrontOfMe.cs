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

public class ComeInFrontOfMe : MonoBehaviour
{
    public List<Transform> LocalUIAnchors;
    Transform playerEyePose;
    List<bool> AnchorOccupied;
    bool isInFront = false;

    // Start is called before the first frame update
    void Start()
    {
        playerEyePose = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }

    /// <summary>
    /// UI solution for large spaces 
    /// </summary>
    public void comeToMe()
    {
        this.gameObject.SetActive(true);

        Vector3 targetPosition = playerEyePose.transform.TransformPoint(LocalUIAnchors[0].transform.position);
        targetPosition.y = playerEyePose.transform.position.y;

        Quaternion t = playerEyePose.transform.rotation;
        Vector3 rot = t.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        Quaternion targetRotation = Quaternion.Euler(rot);

        this.transform.position = targetPosition;
        this.transform.rotation = targetRotation;
    }
}
