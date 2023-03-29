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

public class FlowManager : MonoBehaviour
{
    public visMapManager visMapManager;
    public AOIDataManager aOIDataManager;
    public GameObject ANDGroupedPool;
    public bool TrySubmitGroup = false;
    [HideInInspector]public List<int> OrderedSelectedIDs = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (TrySubmitGroup)
        {
            TrySubmitGroup = false;
            submitGroup();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void submitGroup()
    {
        foreach(int id in OrderedSelectedIDs)
        {
            aOIDataManager.AOIs[id].transform.Find("AOI").GetComponent<PropertyManager>().switchPropertyRemote((int)Property.GROUPED);
            aOIDataManager.AOIs[id].transform.parent = ANDGroupedPool.transform; // put them to AND grouped pool 
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void resetAOISelection()
    {
        OrderedSelectedIDs.Clear();
        foreach (var aoi in aOIDataManager.AOIs)
            aoi.transform.Find("AOI").GetComponent<PropertyManager>().InGroupID.text = "-";
    }

    /// <summary>
    /// 
    /// </summary>
    public void plotFlowAccordingToSelectedAOIs()
    {
        visMapManager.resetCanvas();
        for (int i = 0; i < OrderedSelectedIDs.Count - 1; i++)
        {
            visMapManager.drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(
                aOIDataManager.AOIs[i].transform.position, aOIDataManager.AOIs[i+1].transform.position, i%2 == 0);
        }
        visMapManager.submitCurrentTexture();
        visMapManager.saveVisMapToDisk();

        resetAOISelection(); // reset after submission 
    }
}
