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

public class Graph : MonoBehaviour
{
    public List<AOIEdge> AOIEdges = new List<AOIEdge>();
    public List<AOIEdge> DataEdges = new List<AOIEdge>();
    public float AOISimilarityThreshold = 0.01f;
    public float delta = 0.1f;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    int checkHowManyExistingAOIEdge(AOIEdge e)
    {
        int existingAOIEdges = 0;
        foreach(var AOIEdge in AOIEdges)
        {
            if(Vector3.Distance(AOIEdge.s,e.s) < AOISimilarityThreshold && Vector3.Distance(AOIEdge.e, e.e) < AOISimilarityThreshold)
            {
                existingAOIEdges++;
            }
        }
        return existingAOIEdges;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="repeatTimes"></param>
    public void insertAOIEdgeRepeat(AOIEdge e, int repeatTimes) { 
        for(int i = 0; i < repeatTimes; i++)
        {
            insertAOIEdge(e);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public void insertDataEdge(AOIEdge e) // and compute delta at the same time 
    {
        //e.offsetL = (checkHowManyExistingAOIEdge(e) + 1) * delta;
        DataEdges.Add(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public void insertAndMergeIfExists(AOIEdge e)
    {
        bool Exists = false;
        foreach (var AOIEdge in DataEdges)
        {
            if (Vector3.Distance(AOIEdge.s, e.s) < AOISimilarityThreshold && Vector3.Distance(AOIEdge.e, e.e) < AOISimilarityThreshold)
            {
                AOIEdge.repeatTimesInGroup[0] += e.repeatTimesInGroup[0];
                AOIEdge.repeatTimesInGroup[1] += e.repeatTimesInGroup[1];
                Exists = true;
            }
        }
        if (!Exists)
            insertDataEdge(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public void insertAndMergePercentageIfExists(AOIEdge e)
    {
        bool Exists = false;
        foreach (var AOIEdge in DataEdges)
        {
            if (Vector3.Distance(AOIEdge.s, e.s) < AOISimilarityThreshold && Vector3.Distance(AOIEdge.e, e.e) < AOISimilarityThreshold)
            {
                AOIEdge.highestSpeedPercentage = (AOIEdge.highestSpeedPercentage + e.highestSpeedPercentage) / 2.0f;
                Exists = true;
            }
        }
        if (!Exists)
            insertDataEdge(e);
    }

    /// <summary>
    /// 
    /// </summary>
    public void fromDataEdgeToAOIEdges()
    {
        AOIEdges.Clear();
        foreach(var d in DataEdges)
        {
            for (int i = 0; i < d.repeatTimesInGroup[0]; i++)
            {
                AOIEdge rawAOI = new AOIEdge(d.sAOI,d.eAOI);
                rawAOI.groupID = 0;
                insertAOIEdge(rawAOI);
            }
            for (int i = 0; i < d.repeatTimesInGroup[1]; i++)
            {
                AOIEdge rawAOI = new AOIEdge(d.sAOI, d.eAOI);
                rawAOI.groupID = 1;
                insertAOIEdge(rawAOI);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public void insertAOIEdge(AOIEdge e) // and compute delta at the same time 
    {
        e.offsetL = (checkHowManyExistingAOIEdge(e) + 1) * delta;
        AOIEdges.Add(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="keepOffset"></param>
    public void insertAOIEdge(AOIEdge e, bool keepOffset) // and compute delta at the same time 
    {
        if(!keepOffset)
            e.offsetL = (checkHowManyExistingAOIEdge(e) + 1) * delta;
        AOIEdges.Add(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public void insertAOIEdgeOpposite(AOIEdge e) // and compute delta at the same time 
    {
        if (checkHowManyExistingAOIEdge(e) > 0)
            e.offsetL = -delta;
        else
            e.offsetL = delta;
        AOIEdges.Add(e);
    }

    /// <summary>
    /// 
    /// </summary>
    public void resetGraph()
    {
        AOIEdges.Clear();
    }
   
}
