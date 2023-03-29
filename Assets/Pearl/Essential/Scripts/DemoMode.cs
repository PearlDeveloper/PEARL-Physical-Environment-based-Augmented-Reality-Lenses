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

public class DemoMode : MonoBehaviour
{
    public GameObject AOIparent;
    public bool fbDemoMode = false;

    // Start is called before the first frame update
    void Start()
    {
        enableDemoMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (fbDemoMode)
        {
            fbDemoMode = false;
            enableDemoMode();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void enableDemoMode()
    {
        foreach(Transform aoi in AOIparent.transform)
        {
            aoi.Find("AOI").GetComponent<AOIApperancemanager>().showContextMenuInActive = false;
            aoi.Find("AOI").GetComponent<ProximityManager>().enabled = false;
        }
    }
}
