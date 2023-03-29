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

public class AOIEdge
{
    public GameObject sAOI;
    public GameObject eAOI;
    public Vector3 s;
    public Vector3 e;
    public float offsetL;
    public int repeatTimes = 0;

    public List<float> speedMapTestData = new List<float>();
    public float highestSpeedPercentage = 0.6f;
    public bool someTrajFound = false;

    public int groupID = 0;
    public List<int> repeatTimesInGroup;

    public AOIEdge()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s_aoi"></param>
    /// <param name="e_aoi"></param>
    public AOIEdge(GameObject s_aoi, GameObject e_aoi)
    {
        sAOI = s_aoi;
        eAOI = e_aoi;

        //s = s_aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position;
        //e = e_aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position;
        s = s_aoi.transform.Find("AOI").transform.position;
        e = e_aoi.transform.Find("AOI").transform.position;
        offsetL = 0.01f; // default, will be overwritten

        speedMapTestData.Add(0);
        speedMapTestData.Add(0.1f);
        speedMapTestData.Add(0.25f);
        speedMapTestData.Add(0.5f);
        speedMapTestData.Add(1);
        speedMapTestData.Add(0.5f);
        speedMapTestData.Add(0.25f);
        speedMapTestData.Add(0.1f);
        speedMapTestData.Add(0);

        repeatTimesInGroup = new List<int>();
        repeatTimesInGroup.Add(0);
        repeatTimesInGroup.Add(0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s_aoi"></param>
    /// <param name="e_aoi"></param>
    /// <param name="g"></param>
    public AOIEdge(GameObject s_aoi, GameObject e_aoi, int g)
    {
        sAOI = s_aoi;
        eAOI = e_aoi;
        s = s_aoi.transform.Find("AOI").transform.position;
        e = e_aoi.transform.Find("AOI").transform.position;
        offsetL = 0.01f; // default, will be overwritten

        speedMapTestData.Add(0);
        speedMapTestData.Add(0.1f);
        speedMapTestData.Add(0.25f);
        speedMapTestData.Add(0.5f);
        speedMapTestData.Add(1);
        speedMapTestData.Add(0.5f);
        speedMapTestData.Add(0.25f);
        speedMapTestData.Add(0.1f);
        speedMapTestData.Add(0);

        groupID = g;
        
        repeatTimesInGroup = new List<int>();
        repeatTimesInGroup.Add(0);
        repeatTimesInGroup.Add(0);
    }
}
