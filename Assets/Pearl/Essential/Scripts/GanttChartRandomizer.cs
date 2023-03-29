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

public class GanttChartRandomizer : MonoBehaviour
{
    public GameObject[] ganttBars;

    List<float> Xvalues;
    List<float> WidthValues;
    // Start is called before the first frame update
    void Start()
    {
        Xvalues = new List<float>();
        WidthValues = new List<float>();

        foreach (var bar in ganttBars)
        {
            float randomWidth = Random.Range(0.02f, 0.2f);
            float randomX = Random.Range(-0.2f + randomWidth / 2.0f, 0.2f - randomWidth / 2.0f);

            Xvalues.Add(randomX);
            WidthValues.Add(randomWidth);
        }

        applyChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    public void fillRandomData()
    {
        Xvalues = new List<float>();
        WidthValues = new List<float>();

        foreach (var bar in ganttBars)
        {
            float randomWidth = Random.Range(0.02f, 0.2f);
            float randomX = Random.Range(-0.2f + randomWidth / 2.0f, 0.2f - randomWidth / 2.0f);

            Xvalues.Add(randomX);
            WidthValues.Add(randomWidth);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xvalues"></param>
    /// <param name="wvalues"></param>
    // xvalue: 0-1, wvalues: 0-1
    public void updateData(List<float> xvalues, List<float> wvalues)
    {
        if(Xvalues == null)
        {
            fillRandomData();
        }
        for (int i = 0; i < xvalues.Count; i++)
        {
            Xvalues[i] = xvalues[i] * 0.4f - 0.2f;
            WidthValues[i] = wvalues[i] * 0.4f;
        }
        applyChanges();
    }

    /// <summary>
    /// 
    /// </summary>
    public void updateRandomData()
    {
        for (int i = 0; i < ganttBars.Length; i++)
        {
            Xvalues[i] = Random.Range(0.02f, 0.2f);
            WidthValues[i] = Random.Range(-0.2f + Xvalues[i] / 2.0f, 0.2f - Xvalues[i] / 2.0f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void applyChanges()
    {
        for(int i = 0;i<ganttBars.Length;i++) {
            ganttBars[i].transform.localPosition = new Vector3(
                Xvalues[i], ganttBars[i].transform.localPosition.y, ganttBars[i].transform.localPosition.z);     
            ganttBars[i].transform.localScale = new Vector3(
                WidthValues[i], ganttBars[i].transform.localScale.y, ganttBars[i].transform.localScale.z);   
        }
    }
}
