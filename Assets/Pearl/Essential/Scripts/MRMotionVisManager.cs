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

public class MRMotionVisManager : MonoBehaviour
{
    public GameObject[] visList;
    public visMapManager visMapManager;
    // Start is called before the first frame update

    public void setVis(int idx)
    {
        foreach (var vis in visList)
            vis.SetActive(false);
        visList[idx].SetActive(true);

        if(idx == 3) // situated
        {

        }
        else
        {
            visMapManager.situatedVisHideAll();
        }
    }

    // disable all 
    public void resetVis()
    {
        foreach (var vis in visList)
            vis.SetActive(false);
        visMapManager.situatedVisHideAll();
    }
}
