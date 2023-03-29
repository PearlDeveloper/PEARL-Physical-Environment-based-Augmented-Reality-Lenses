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

public class AOIManager : MonoBehaviour
{
    public AOIDataManager aOIDataManager;

    public bool fbApplyProximity = false;
    public float pDist = 4;

    // Update is called once per frame
    void Update()
    {
        if (fbApplyProximity)
        {
            fbApplyProximity = false;

            foreach(var aoi in aOIDataManager.AOIs)
            {
                aoi.transform.Find("AOI").GetComponent<ProximityManager>().proximityDistance = pDist;
            }
        }
    }
}
