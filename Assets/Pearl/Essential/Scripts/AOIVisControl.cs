// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using u2vis;
using UnityEngine;
using Random = UnityEngine.Random;

public class AOIVisControl : MonoBehaviour
{
    public GameObject visAnchor;
    public GameObject[] visList;
    public VisDataHolder[] visDataList;
    public BaseVisualizationView[] baseVisualizationViews;

    // Start is called before the first frame update
    void Start()
    {
        hideAll();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nChildren"></param>
    /// <param name="nTeenagers"></param>
    /// <param name="nAdults"></param>
    public void updateDataVisualization(int nChildren, int nTeenagers, int nAdults)
    {
        foreach (var visData in visDataList)
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    visData.Data[0].Set(i, String.Format("{0:00}:{1:00}", 8 + i, 0)); //      .Add(i.ToString());
            //    visData.Data[1].Set(i, Random.Range(1,6));           //.Add(r.Next(10));
            //    visData.Data[2].Set(i, 50.0f);           //.Add((float)r.NextDouble());
            //    visData.Data[3].Set(i, true);           //.Add((i % 2) > 0);
            //}
            visData.Data[0].Set(0, "Children");
            visData.Data[0].Set(1, "Teenagers");
            visData.Data[0].Set(2, "Adults");

            //int nChildren = Random.Range(1, 5);
            //int nTeenagers = Random.Range(1, 5);
            //int nAdults = 10 - nChildren - nTeenagers;

            visData.Data[1].Set(0, nChildren);
            visData.Data[1].Set(1, nTeenagers);
            visData.Data[1].Set(2, nAdults);
        }


        // explicit rebuild 
        foreach (var viewer in baseVisualizationViews)
        {
            viewer.Rebuild();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void updateDataVisualization_PeakHours()
    {
        foreach (var visData in visDataList)
        {
            visData.Data[0].Set(0, " 9:00 - 10:00");
            visData.Data[0].Set(1, "10:00 - 11:00");
            visData.Data[0].Set(2, "11:00 - 12:00"); // p
            visData.Data[0].Set(3, "12:00 - 13:00"); // p
            visData.Data[0].Set(4, "13:00 - 14:00"); // p
            visData.Data[0].Set(5, "14:00 - 15:00");

            int num01 = Random.Range(1, 3);
            int num02 = Random.Range(1, 3);
            int num03 = Random.Range(3, 5); // p
            int num04 = Random.Range(3, 5); // p
            int num05 = Random.Range(3, 5); // p
            int num06 = Random.Range(1, 3);

            visData.Data[1].Set(0, num01);
            visData.Data[1].Set(1, num02);
            visData.Data[1].Set(2, num03);
            visData.Data[1].Set(3, num04);
            visData.Data[1].Set(4, num05);
            visData.Data[1].Set(5, num06);
        }


        // explicit rebuild 
        foreach (var viewer in baseVisualizationViews)
        {
            viewer.Rebuild();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void updateDataVisualization_OffPeakHours()
    {
        foreach (var visData in visDataList)
        {
            //for (int i = 0; i < 8; i++)
            //{
            //    visData.Data[0].Set(i, String.Format("{0:00}:{1:00}", 8 + i, 0)); //      .Add(i.ToString());
            //    visData.Data[1].Set(i, Random.Range(1,6));           //.Add(r.Next(10));
            //    visData.Data[2].Set(i, 50.0f);           //.Add((float)r.NextDouble());
            //    visData.Data[3].Set(i, true);           //.Add((i % 2) > 0);
            //}
            visData.Data[0].Set(0, " 9:00 - 10:00");
            visData.Data[0].Set(1, "10:00 - 11:00");
            visData.Data[0].Set(2, "11:00 - 12:00"); // p
            visData.Data[0].Set(3, "12:00 - 13:00"); // p
            visData.Data[0].Set(4, "13:00 - 14:00"); // p
            visData.Data[0].Set(5, "14:00 - 15:00");

            int num01 = Random.Range(3, 5);
            int num02 = Random.Range(3, 5);
            int num03 = Random.Range(1, 3); // p
            int num04 = Random.Range(1, 3); // p
            int num05 = Random.Range(1, 3); // p
            int num06 = Random.Range(3, 5);

            visData.Data[1].Set(0, num01);
            visData.Data[1].Set(1, num02);
            visData.Data[1].Set(2, num03);
            visData.Data[1].Set(3, num04);
            visData.Data[1].Set(4, num05);
            visData.Data[1].Set(5, num06);
        }


        // explicit rebuild 
        foreach (var viewer in baseVisualizationViews)
        {
            viewer.Rebuild();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void updateDataVisualization()
    {
        foreach (var visData in visDataList) {
            //for (int i = 0; i < 8; i++)
            //{
            //    visData.Data[0].Set(i, String.Format("{0:00}:{1:00}", 8 + i, 0)); //      .Add(i.ToString());
            //    visData.Data[1].Set(i, Random.Range(1,6));           //.Add(r.Next(10));
            //    visData.Data[2].Set(i, 50.0f);           //.Add((float)r.NextDouble());
            //    visData.Data[3].Set(i, true);           //.Add((i % 2) > 0);
            //}
            visData.Data[0].Set(0, "Children");
            visData.Data[0].Set(1, "Teenagers");
            visData.Data[0].Set(2, "Adults");

            int nChildren = Random.Range(1, 5);
            int nTeenagers = Random.Range(1, 5);
            int nAdults = 10 - nChildren - nTeenagers;

            visData.Data[1].Set(0, nChildren);
            visData.Data[1].Set(1, nTeenagers);
            visData.Data[1].Set(2, nAdults);
        }


        // explicit rebuild 
        foreach (var viewer in baseVisualizationViews)
        {
            viewer.Rebuild();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xv"></param>
    /// <param name="wv"></param>
    public void updateGanttChart(List<float> xv, List<float> wv)
    {
        GanttChartRandomizer ganttData = this.transform.Find("AttachedVis").transform.Find("GanttChart2D").transform.Find("GanttChart").GetComponent<GanttChartRandomizer>();
        ganttData.updateData(xv,wv);
    }

    /// <summary>
    /// 
    /// </summary>
    public void show2DBarChart()
    {
        foreach(GameObject vis in visList)
            vis.SetActive(false);
        visList[0].SetActive(true);
        //visList[0].transform.position = visAnchor.transform.position;
        //visList[0].transform.rotation = visAnchor.transform.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    public void show2DBarChart_Lines()
    {
        foreach (GameObject vis in visList)
            vis.SetActive(false);
        visList[1].SetActive(true);
        visList[1].transform.position = visAnchor.transform.position;
        visList[1].transform.rotation = visAnchor.transform.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    public void showTimeTable()
    {
        foreach (GameObject vis in visList)
            vis.SetActive(false);
        visList[2].SetActive(true);
        visList[2].transform.position = visAnchor.transform.position;
        visList[2].transform.rotation = visAnchor.transform.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    public void show3DBarChart()
    {
        foreach (GameObject vis in visList)
            vis.SetActive(false);
        visList[3].SetActive(true);
        visList[3].transform.position = visAnchor.transform.position;
        visList[3].transform.rotation = visAnchor.transform.rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    public void hideAll()
    {
        foreach (GameObject vis in visList)
            vis.SetActive(false);
    }
}
