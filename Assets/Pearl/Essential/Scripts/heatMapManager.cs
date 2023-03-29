// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using IMLD.MixedRealityAnalysis.Core;
using IMLD.MixedRealityAnalysis.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class heatMapManager : MonoBehaviour
{
    //
    public MeshRenderer heatMapMeshRenderer;
    public GameObject OriginAnchor;
    public GameObject FloorIndicator;
    public int resolutionXZ = 500;
    public float cellSize = 0.2f;
    public int pointStep = 100;
    public Gradient gradient;

    //
    Texture2D heatMap;


    Vector2 floorTexOffset = new Vector2(1, 5);
    float TexDim = 10;
    float offsetXZ = 5 - 4;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textureCoordi"></param>
    /// <returns></returns>
    Vector3 toWorldCoordinate(Vector2 textureCoordi)
    {
        Vector3 localCoordi = new Vector3(
            (textureCoordi.x / resolutionXZ * TexDim - floorTexOffset.x) / OriginAnchor.transform.localScale.x,
            (FloorIndicator.transform.position.y + 0.01f) / OriginAnchor.transform.localScale.y,
            (textureCoordi.y / resolutionXZ * TexDim - floorTexOffset.y) / OriginAnchor.transform.localScale.z);

        Vector3 worldCoordi = OriginAnchor.transform.TransformPoint(localCoordi);

        return worldCoordi;
    }

    /// <summary>
    /// 
    /// </summary>
    public void computeHeatmap()
    {
        // APIs of MIRIA:
        // InnerLoop, Sessions: Services.VisManager().Visualizations[i].Settings -> Sessions, Conditions 
        // Outer View, Datasets: Services.DataManager().DataSets

        Vis3DTrajectories trajView = null; // try to find 3d traj. 
        foreach (KeyValuePair<Guid, AbstractView> entry in Services.VisManager().Visualizations)
        {
            if (entry.Value.Settings.VisType == VisType.Trajectory3D) // assume we only have one traj shown in MRMA
            {
                trajView = (Vis3DTrajectories)entry.Value;
            }
        }
        if (trajView == null) // if 3D traj. exists 
            return;

        // loop all pixels 
        heatMap = new Texture2D(resolutionXZ, resolutionXZ);
        float[,] pointsInCell = new float[resolutionXZ, resolutionXZ];
        float maxPointsInCell = 0;
        for (int i = 0; i < resolutionXZ; i++)
        {
            for (int j = 0; j < resolutionXZ; j++)
            {
                // initialize
                heatMap.SetPixel(i, j, UnityEngine.Random.ColorHSV());

                /* compute world position 
                 * from 0,499 -> 0,1 -> scale -> offset -> world pose 
                 * ((float) i / resolutionXZ ) * 10 + 3
                 * 10m x 10m space 
                 */
                Vector3 worldPoseOfThePixel = toWorldCoordinate(new Vector2(i, j));

                for (int d = 0; d < Services.DataManager().DataSets.Count; d++) {
                    AnalysisObject dataSet = Services.DataManager().DataSets[d];
                    for (int s = 0; s < trajView.Settings.Sessions.Count; s++) {
                        for (int c = 0; c < trajView.Settings.Conditions.Count; c++) {
                            var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                            for (int pidx = 0; pidx < infoObjects.Count; pidx += pointStep) {
                                Sample o = infoObjects[pidx];
                                Vector3 projectedPolylinePoint = trajView.transform.TransformPoint(o.Position);
                                projectedPolylinePoint.y = 0;
                                worldPoseOfThePixel.y = 0;
                                if (Vector3.Distance(projectedPolylinePoint, worldPoseOfThePixel) < cellSize)
                                {
                                    pointsInCell[i, j] += 1;
                                    if (pointsInCell[i, j] > maxPointsInCell) // compute max value 
                                        maxPointsInCell = pointsInCell[i, j];
                                }
                            }
                        }
                    }
                }
            }
        }

        //
        Debug.Log("Max Points In Cell: " + maxPointsInCell);

        // normalize & ignore zero point areas 
        for (int i = 0; i < resolutionXZ; i++)
        {
            for (int j = 0; j < resolutionXZ; j++)
            {
                pointsInCell[i, j] = (float) pointsInCell[i, j] / maxPointsInCell;  
                Color c = gradient.Evaluate(pointsInCell[i, j]);
                if (pointsInCell[i, j] == 0)
                    c.a = 0;
                heatMap.SetPixel(i, j, c);
            }
        }

        //
        heatMapMeshRenderer.material.SetTexture("_MainTex", heatMap);
        heatMap.Apply();

        // write to file on disk 
        byte[] bytes = heatMap.EncodeToPNG();
        var dirPath = Application.dataPath + "/Generated/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "HeatMap" + ".png", bytes);

    }
}
