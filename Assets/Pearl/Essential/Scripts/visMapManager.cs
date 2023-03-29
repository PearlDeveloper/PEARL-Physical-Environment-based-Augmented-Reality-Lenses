// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class visMapManager : MonoBehaviour
{
    public GameObject OriginAnchor;
    public MeshRenderer visMapMeshRenderer; 
    public AOIDataManager aoiDataManager;
    public GameObject positiveAOIPool;
    public GameObject negativeAOIPool;
    public GameObject ANDgroupedAOIPool;
    public GameObject FloorIndicator;
    public int resolutionXZ = 1000;
    public float pointStep = 0.1f;
    public float cellSize = 0.2f;

    public TextMeshPro adjustmentInfoBoard;
    public GameObject textPrefeb;
    public float objectMaskRadius = 0.5f;
    public bool useGradient = false;
    public Graph G = new Graph();
    public Gradient gradient;
    public Gradient paceGradient;
    public float TexDim = 10;
    public Color TargetColor01;
    public Color TargetColor02;
    public GameObject[] Referents;
    public Vector3 bpMin = new Vector3(-1, 0, -2.5f);
    public Vector3 bpMax = new Vector3(9, 0, 2.5f);
    public Color LeftHSVStart = Color.HSVToRGB(240 / 360.0f, 85 / 100.0f, 100 / 100.0f);
    public Color LeftHSVEnd = Color.HSVToRGB(240 / 360.0f, 85 / 100.0f, 100 / 100.0f);
    public Color RightHSVStart = Color.HSVToRGB(355 / 360.0f, 83 / 100.0f, 81 / 100.0f);
    public Color RightHSVEnd = Color.HSVToRGB(355 / 360.0f, 83 / 100.0f, 81 / 100.0f);
    [HideInInspector] public List<GameObject> dots = new List<GameObject>();
    [HideInInspector] public Texture2D visMap;

    int boneSpacePixel = 50;
    int borderWidthPixel = 2;
    float arrowWidthWorldSpace = 0.02f;
    float angleInDegree = 150.0f;
    float offsetXZ = 5 - 4;
    Color defaultColor;
    Vector2 floorTexOffset = new Vector2(1, 5);
    List<Color> colorTable = new List<Color>();

    //
    void Start()
    {
        //showDonutVis();
        defaultColor = Random.ColorHSV();
        defaultColor.a = 0;
        colorTable.Add(LeftHSVStart);
        colorTable.Add(LeftHSVEnd);
        colorTable.Add(Color.yellow);
        colorTable.Add(Color.blue);
        colorTable.Add(Color.green);
        colorTable.Add(Color.white);

        TestData.Add(new Vector3(1.842364f, -0.6517117f, 1.807563f));
        TestData.Add(new Vector3(0.1599952f, - 0.4824266f, 3.080786f));
        TestData.Add(new Vector3(2.014836f, - 0.688972f, 5.407819f));
        TestData.Add(new Vector3(-0.004444987f, - 0.6230808f, 6.71341f));
        TestData.Add(new Vector3(-1.10362f, - 0.3413818f, 4.525807f));
        TestData.Add(new Vector3(-2.243156f, - 0.1685691f, 2.417832f));

        G = new Graph();

        //Vector2 c = toTextureCoordinate(new Vector3(0, 0, 0));
        //Vector3 invP = visMapMeshRenderer.transform.InverseTransformPoint(new Vector3(0, 0, 0));
        //Vector3 invP01 = visMapMeshRenderer.transform.InverseTransformPoint(new Vector3(1, 0, 0));
        //Vector3 invP02 = visMapMeshRenderer.transform.InverseTransformPoint(new Vector3(2, 0, 0));
        //Vector3 iinvP = new Vector3((float)(invP.x + 0.5) * resolutionXZ, 0, (float)(invP.y + 0.5) * resolutionXZ);
        //Vector3 invP2 = OriginAnchor.transform.InverseTransformPoint(new Vector3(0, 0, 0));
    }
    void Update()
    {
        
    }

    // helper functions 
    Vector2 toTextureCoordinate(Vector2 worldCoordinate)
    {
        //Vector3 worldCoordinatevec3 = new Vector3(worldCoordinate.x, 0, worldCoordinate.y);
        //Vector2 r = new Vector2();
        //r.x = (OriginAnchor.transform.InverseTransformPoint(worldCoordinatevec3).x * OriginAnchor.transform.localScale.x + floorTexOffset.x) * resolutionXZ / TexDim;
        //r.y = (OriginAnchor.transform.InverseTransformPoint(worldCoordinatevec3).z * OriginAnchor.transform.localScale.z + floorTexOffset.y) * resolutionXZ / TexDim;
        //return r;


        Vector3 worldCoordinatevec3 = new Vector3(worldCoordinate.x, 0, worldCoordinate.y);
        Vector2 r = new Vector2();
        r.x = (visMapMeshRenderer.transform.InverseTransformPoint(worldCoordinatevec3).x + 0.5f) * resolutionXZ;
        r.y = (visMapMeshRenderer.transform.InverseTransformPoint(worldCoordinatevec3).y + 0.5f) * resolutionXZ;
        return r;
    }
    Vector2 toTextureCoordinate(Vector3 worldCoordinate)
    {
        //Vector2 r = new Vector2();
        //r.x = (OriginAnchor.transform.InverseTransformPoint(worldCoordinate).x * OriginAnchor.transform.localScale.x + floorTexOffset.x) * resolutionXZ / TexDim;
        //r.y = (OriginAnchor.transform.InverseTransformPoint(worldCoordinate).z * OriginAnchor.transform.localScale.z + floorTexOffset.y) * resolutionXZ / TexDim;
        //return r;

        Vector2 r = new Vector2();
        r.x = (visMapMeshRenderer.transform.InverseTransformPoint(worldCoordinate).x + 0.5f) * resolutionXZ;
        r.y = (visMapMeshRenderer.transform.InverseTransformPoint(worldCoordinate).y + 0.5f) * resolutionXZ;
        return r;
    }

    Vector3 toWorldCoordinate(Vector2 textureCoordi)
    {
        //Vector3 localCoordi = new Vector3(
        //    (textureCoordi.x / resolutionXZ * TexDim - floorTexOffset.x) / OriginAnchor.transform.localScale.x,
        //    (FloorIndicator.transform.position.y + 0.01f) / OriginAnchor.transform.localScale.y, 
        //    (textureCoordi.y / resolutionXZ * TexDim - floorTexOffset.y) / OriginAnchor.transform.localScale.z);

        //Vector3 worldCoordi = OriginAnchor.transform.TransformPoint(localCoordi);

        //return worldCoordi;

        Vector3 localCoordi = new Vector3(
            textureCoordi.x / resolutionXZ * TexDim,
            FloorIndicator.transform.position.y,
            textureCoordi.y / resolutionXZ * TexDim);

        Vector3 worldCoordi = visMapMeshRenderer.transform.TransformPoint(localCoordi);

        return worldCoordi;
    }

    float toTextureDist(float worldDist)
    {
        return worldDist * resolutionXZ / TexDim;
    }
    public void resetCanvas()
    {
        // reset all pixels 
        visMap = new Texture2D(resolutionXZ, resolutionXZ);
        for (int i = 0; i < resolutionXZ; i++)
            for (int j = 0; j < resolutionXZ; j++)
                visMap.SetPixel(i, j, defaultColor);
    }
    public void submitCurrentTexture()
    {
        visMapMeshRenderer.material.SetTexture("_MainTex", visMap);
        visMap.Apply(true);
    }
    public void saveVisMapToDisk()
    {
        // write to file on disk 
        byte[] bytes = visMap.EncodeToPNG();
        var dirPath = Application.dataPath + "/Generated/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "visMap" + ".png", bytes);
    }
    public void computeDonutVis(Vector2 center, float innerRadius, float outerRadius, float holoSize, float patternSpace, Color c)
    {
        int centerX = Mathf.RoundToInt(center.x);
        int centerY = Mathf.RoundToInt(center.y);

        int range = Mathf.RoundToInt(outerRadius);

        for (int i = centerX - range; i < centerX + range; i++)
        {
            for (int j = centerY - range; j < centerY + range; j++)
            {
                if (
                    Mathf.Sqrt(
                        (Mathf.Abs(i - centerX) * Mathf.Abs(i - centerX)
                            + Mathf.Abs(j - centerY) * Mathf.Abs(j - centerY))) < outerRadius
                    && Mathf.Sqrt(
                        (Mathf.Abs(i - centerX) * Mathf.Abs(i - centerX)
                            + Mathf.Abs(j - centerY) * Mathf.Abs(j - centerY))) > innerRadius)
                    visMap.SetPixel(i, j, c);

                //if (
                //    Mathf.Sqrt(
                //        (Mathf.Abs(i - centerX) * Mathf.Abs(i - centerX)
                //            + Mathf.Abs(j - centerY) * Mathf.Abs(j - centerY))) < (outerRadius - holoSize)
                //            && Mathf.Sqrt( (Mathf.Abs(i - centerX) * Mathf.Abs(i - centerX)
                //                + Mathf.Abs(j - centerY) * Mathf.Abs(j - centerY))) > (innerRadius + holoSize)
                //                && (i % patternSpace > (patternSpace / 2.0f)))
                //    visMap.SetPixel(i, j, defaultColor);
            }
        }
    }
    public void computeDotVis(Vector2 center, float Radius, float holoSize, float patternSpace, Color c)
    {
        int centerX = Mathf.RoundToInt(center.x);
        int centerY = Mathf.RoundToInt(center.y);

        int range = Mathf.RoundToInt(Radius);

        for (int i = centerX - range; i < centerX + range; i++)
        {
            for (int j = centerY - range; j < centerY + range; j++)
            {
                if (
                    Mathf.Sqrt(
                        (Mathf.Abs(i - centerX) * Mathf.Abs(i - centerX)
                            + Mathf.Abs(j - centerY) * Mathf.Abs(j - centerY))) < Radius)
                    visMap.SetPixel(i, j, c);
                //if (
                //    Mathf.Sqrt(
                //        (Mathf.Abs(i - centerX) * Mathf.Abs(i - centerX)
                //            + Mathf.Abs(j - centerY) * Mathf.Abs(j - centerY))) < (Radius - holoSize) 
                //                && (i % patternSpace > (patternSpace / 2.0f)))
                //    visMap.SetPixel(i, j, defaultColor);
            }
        }
    }
    public void computeHeatmap()
    {
        // do not filter when loading traj. 

        if (resolutionXZ > 50) resolutionXZ = 50;

        // loop all pixels 
        visMap = new Texture2D(resolutionXZ, resolutionXZ);
        float[,] pointsInCell = new float[resolutionXZ, resolutionXZ];
        float maxPointsInCell = 0;
        for (int i = 0; i < resolutionXZ; i++)
        {
            for (int j = 0; j < resolutionXZ; j++)
            {
                // initialize
                visMap.SetPixel(i, j, Random.ColorHSV());

                /* compute world position 
                 * from 0,499 -> 0,1 -> scale -> offset -> world pose 
                 * ((float) i / resolutionXZ ) * 10 + 3
                 * 10m x 10m space 
                 */
                Vector3 worldPoseOfThePixel = new Vector3(((float)i / resolutionXZ) * 10 - 1,
                    0, ((float)j / resolutionXZ) * 10 - 1);

                // loop all traj. points to collect pointsInCell
                //foreach (Transform traj_i in tubeParameterHelper.transform)
                //{
                //    TubeGeneratorFromObj data_traj_i = traj_i.GetComponent<TubeGeneratorFromObj>();
                //    if (!data_traj_i)
                //        continue;
                //    // loop all tubes 
                //    for (int tube_i = 0; tube_i < data_traj_i.polylines.Length; tube_i++)
                //    {
                //        // loop all points 
                //        for (int point_i = 0; point_i < data_traj_i.polylines[tube_i].Length; point_i += 1)
                //        {
                //            Vector3 projectedPolylinePoint = data_traj_i.polylines[tube_i][point_i];
                //            projectedPolylinePoint.y = 0;
                //            if (Vector3.Distance(projectedPolylinePoint, worldPoseOfThePixel) < cellSize)
                //            {
                //                pointsInCell[i, j] += 1;
                //            }
                //        }
                //    }
                //}

                // compute max value 
                if (pointsInCell[i, j] > maxPointsInCell)
                    maxPointsInCell = pointsInCell[i, j];

            }
        }

        //
        Debug.Log("Max Points In Cell: " + maxPointsInCell);

        // normalize & ignore zero point areas 
        for (int i = 0; i < resolutionXZ; i++)
        {
            for (int j = 0; j < resolutionXZ; j++)
            {
                pointsInCell[i, j] = pointsInCell[i, j] / maxPointsInCell;
                Color c = Color.HSVToRGB(1 - pointsInCell[i, j], 1.0f, 0.6f);
                if (pointsInCell[i, j] == 0)
                    c.a = 0;
                visMap.SetPixel(i, j, c);
            }
        }

        submitCurrentTexture();
        saveVisMapToDisk();
    }

    // 3D Dot based 
    void drawDotsBetweenTwoPointsOnGround(Vector3 startingPoint, Vector3 endPoint)
    {
        //startingPoint = new Vector3(startingPoint.x, FloorIndicator.transform.position.y, startingPoint.z);
        //endPoint = new Vector3(endPoint.x, FloorIndicator.transform.position.y, endPoint.z);

        //float accuDist = 0;
        //Vector3 directedDistBetween = endPoint - startingPoint;
        //Vector3 normalizedDir = Vector3.Normalize(directedDistBetween);
        //float dist = Vector3.Magnitude(directedDistBetween);
        //Vector3 currentOffset = startingPoint;
        //while (accuDist < dist)
        //{
        //    GameObject dot = Instantiate(dotPrefeb, currentOffset, Quaternion.LookRotation(normalizedDir));//
        //    dots.Add(dot);
        //    currentOffset += pointStep * normalizedDir;
        //    accuDist += pointStep;
        //}
    }
    public void removeDots()
    {
        dots.Clear();
    }


    // attached vis 
    public void updateDataVis()
    {
        foreach (GameObject fb in aoiDataManager.AOIs)
            fb.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization();
    }

    [PunRPC]
    public void show2DBarChartRPC()
    {
        foreach (Transform aoi in positiveAOIPool.transform)
        {
            aoi.Find("AOI").GetComponent<AOIVisControl>().show2DBarChart();
        }
        foreach (Transform aoi in negativeAOIPool.transform)
        {
            aoi.Find("AOI").GetComponent<AOIVisControl>().show2DBarChart();
        }
        foreach (Transform aoi in ANDgroupedAOIPool.transform)
        {
            aoi.Find("AOI").GetComponent<AOIVisControl>().show2DBarChart();
        }
    }

    public void show2DBarChart()
    {
        GetComponent<PhotonView>().RPC("show2DBarChartRPC",RpcTarget.All);
    }

    public void show2DLineChart()
    {
        foreach (GameObject fb in aoiDataManager.AOIs)
            fb.transform.Find("AOI").GetComponent<AOIVisControl>().show2DBarChart_Lines();
    }

    [PunRPC]
    public void showTimeTableRPC()
    {
        foreach (Transform aoi in positiveAOIPool.transform)
        {
            aoi.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
        }
        foreach (Transform aoi in negativeAOIPool.transform)
        {
            aoi.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
        }
        foreach (Transform aoi in ANDgroupedAOIPool.transform)
        {
            aoi.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
        }
    }

    public void showTimeTable()
    {
        GetComponent<PhotonView>().RPC("showTimeTableRPC", RpcTarget.All);
    }
    public void situatedVisHideAll()
    {
        foreach (GameObject fb in aoiDataManager.AOIs)
        {
            fb.transform.Find("AOI").GetComponent<AOIVisControl>().hideAll();
        }
    }

    //
    void plotLine(int x0, int y0, int x1, int y1, int width, Color c)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2; /* error value e_xy */

        for (; ; )
        {
            //Vector2 bpMinLocal = toTextureCoordinate(OriginAnchor.transform.TransformPoint(bpMin));
            //Vector2 bpMaxLocal = toTextureCoordinate(OriginAnchor.transform.TransformPoint(bpMax));

            //int minX = Mathf.RoundToInt(bpMinLocal.x);
            //int maxX = Mathf.RoundToInt(bpMaxLocal.x);
            //int minZ = Mathf.RoundToInt(bpMinLocal.y);
            //int maxZ = Mathf.RoundToInt(bpMaxLocal.y);

            if (y0 < 0.25f * resolutionXZ || y0 > 0.75f * resolutionXZ) // for room 2042
                break;

            /* loop */
            visMap.SetPixel(x0, y0, c);
            for (int i = 1; i < width; i++)
            {
                visMap.SetPixel(x0, y0 + i, c);
                visMap.SetPixel(x0, y0 - i, c);
                visMap.SetPixel(x0 + i, y0, c);
                visMap.SetPixel(x0 - i, y0, c);
            }


            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
            if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
        }
    }
    //
    void plotLine(int x0, int y0, int x1, int y1, int wS, int wE, Color c)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2; /* error value e_xy */

        int xo = x0, yo = y0;

        float dist = Vector2.Distance(new Vector2(x0, y0), new Vector2(x1, y1));

        for (; ; )
        {  /* loop */
            float startDist = Vector2.Distance(new Vector2(x0, y0), new Vector2(xo, yo));
            float p = startDist / dist;

            float w = (1 - p) * wS + p * wE;

            visMap.SetPixel(x0, y0, c);
            for (int i = 1; i < w; i++)
            {
                visMap.SetPixel(x0, y0 + i, c);
                visMap.SetPixel(x0, y0 - i, c);
                visMap.SetPixel(x0 + i, y0, c);
                visMap.SetPixel(x0 - i, y0, c);
            }


            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
            if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
        }
    }
    void plotQuadBezierSeg(int x0, int y0, int x1, int y1, int x2, int y2, int depth, Color c)
    {
        int sx = x2 - x1, sy = y2 - y1;
        long xx = x0 - x1, yy = y0 - y1, xy;         /* relative values for checks */
        double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

        Debug.Assert(xx * sx <= 0 && yy * sy <= 0);  /* sign of gradient must not change */

        if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
        { /* begin with longer part */
            x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;  /* swap P0 P2 */
        }
        if (cur != 0)
        {                                    /* no straight line */
            xx += sx; xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
            yy += sy; yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
            xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
            if (cur * sx * sy < 0)
            {                           /* negated curvature? */
                xx = -xx; yy = -yy; xy = -xy; cur = -cur;
            }
            dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;             /* differences 1st degree */
            dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;
            xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
            do
            {
                visMap.SetPixel(x0, y0, c);
                for(int i = 1; i < depth; i++)
                {
                    visMap.SetPixel(x0, y0 + i, c);
                    visMap.SetPixel(x0, y0 - i, c);
                    visMap.SetPixel(x0 + i, y0, c);
                    visMap.SetPixel(x0 - i, y0, c);
                }

                if (x0 == x2 && y0 == y2) return;  /* last pixel -> curve finished */
               // y1 = ;                  /* save value for test of y step */
                if (2 * err > dy) { x0 += sx; dx -= xy; err += dy += yy; } /* x step */
                if (2 * err < dx) { y0 += sy; dy -= xy; err += dx += xx; } /* y step */
            } while (dy < dx);           /* gradient negates -> algorithm fails */
        }
        plotLine(x0, y0, x2, y2, depth, c);                  /* plot remaining part to end */
    }
    void plotCubicBezierSeg(Vector2 start, Vector2 end, Vector2 startControl, Vector2 endControl, int segments, int width, Color c) 
    {
        start = toTextureCoordinate(start);
        end = toTextureCoordinate(end);
        startControl = toTextureCoordinate(startControl);
        endControl = toTextureCoordinate(endControl);

        float p = 1.0f / segments;
        float q = p;
        Vector2 lastPoint = start;
        for (int i = 1; i < segments; i++, p += q)
        {
            Vector2 currPoint = p * p * p * (end + 3.0f * (startControl - endControl) - start) +
                      3.0f * p * p * (start - 2.0f * startControl + endControl) +
                      3.0f * p * (startControl - start) + start;
            plotLine(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y), 
                Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y), width, c);
            lastPoint = currPoint;
        }
        plotLine(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y),
                Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y), width, c);
    }
    void plotFishBoneFlowWithQuadBezierSeg(
        Vector2 startLocalSpace, Vector2 endLocalspace, Vector2 startControlPointLocalspace, Vector2 endControlPointLocalSpace,
        int segments, int width, int arrowLength, int boneSpace, float angleOffset, int gid)
    {

        float p = 1.0f / segments;
        float q = p;
        Vector2 lastPoint = startLocalSpace;
        Color currColor = Color.white;
        for (int i = 1; i < segments; i++, p += q)
        {
            Vector2 currPoint = p * p * p * (endLocalspace + 3.0f * (startControlPointLocalspace - endControlPointLocalSpace) - startLocalSpace) +
                      3.0f * p * p * (startLocalSpace - 2.0f * startControlPointLocalspace + endControlPointLocalSpace) +
                      3.0f * p * (startControlPointLocalspace - startLocalSpace) + startLocalSpace;
            float deltaX = currPoint.x - lastPoint.x;
            float deltaY = currPoint.y - lastPoint.y;
            Vector2 dirL = new Vector2(
                Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY, 
                Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);
            Vector2 dirR = new Vector2(
                Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY,
                Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);
            dirL.Normalize();
            dirR.Normalize();
            Vector2 epL = currPoint + arrowLength * dirL;
            Vector2 epR = currPoint + arrowLength * dirR;

            if (gid == 0)
                currColor = Color.Lerp(LeftHSVStart, TargetColor01, p);
            if (gid == 1)
                currColor = Color.Lerp(LeftHSVEnd, TargetColor02, p);

            if (segments % boneSpace == 0)
            {
                plotLine(Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y),
                    Mathf.RoundToInt(epL.x), Mathf.RoundToInt(epL.y), width, currColor);
                plotLine(Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y), 
                    Mathf.RoundToInt(epR.x), Mathf.RoundToInt(epR.y), width, currColor);
            }
            plotLine(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y), 
                Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y), width, currColor);

            lastPoint = currPoint;
        }
        plotLine(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y),
                Mathf.RoundToInt(endLocalspace.x), Mathf.RoundToInt(endLocalspace.y), width, currColor);






        //int sx = x2 - x1, sy = y2 - y1;
        //long xx = x0 - x1, yy = y0 - y1, xy;         /* relative values for checks */
        //double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

        //if (!(xx * sx <= 0 && yy * sy <= 0))
        //    return;
        //Debug.Assert(xx * sx <= 0 && yy * sy <= 0);  /* sign of gradient must not change */

        //int lastX = x0;
        //int lastY = y0;
        //int numOfPointsDrawn = 0;
        //int nx = 0, ny = 0;
        //int deltaX =0, deltaY = 0;

        ////if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
        ////{ /* begin with longer part */
        ////    x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;  /* swap P0 P2 */
        ////}
        //if (cur != 0)
        //{                                    /* no straight line */
        //    xx += sx; xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
        //    yy += sy; yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
        //    xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
        //    if (cur * sx * sy < 0)
        //    {                           /* negated curvature? */
        //        xx = -xx; yy = -yy; xy = -xy; cur = -cur;
        //    }
        //    dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;             /* differences 1st degree */
        //    dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;
        //    xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
        //    do
        //    {
        //        visMap.SetPixel(x0, y0, c);
        //        for (int i = 1; i < width; i++)
        //        {
        //            visMap.SetPixel(x0, y0 + i, c);
        //            visMap.SetPixel(x0, y0 - i, c);
        //            visMap.SetPixel(x0 + i, y0, c);
        //            visMap.SetPixel(x0 - i, y0, c);
        //        }

        //        if (numOfPointsDrawn > 0)
        //        {
        //            deltaX = x0 - lastX;
        //            deltaY = y0 - lastY;

        //            //nx = -deltaY;
        //            //ny = deltaX;

        //            int dirL_x = Mathf.RoundToInt(Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY);
        //            int dirL_y = Mathf.RoundToInt(Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);

        //            int dirR_x = Mathf.RoundToInt(Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY);
        //            int dirR_y = Mathf.RoundToInt(Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);

        //            Vector2 dirL = new Vector2(dirL_x, dirL_y);
        //            dirL.Normalize();
        //            Vector2 dirR = new Vector2(dirR_x, dirR_y);
        //            dirR.Normalize();

        //            Vector2 targetEndPoint = arrowLength * dirL + new Vector2(x0, y0);
        //            Vector2 targetEndPointOpposite = arrowLength * dirR + new Vector2(x0, y0);

        //            if (numOfPointsDrawn % boneSpace == 0)
        //            {
        //                plotLine(x0, y0, 
        //                    Mathf.RoundToInt(targetEndPoint.x), 
        //                    Mathf.RoundToInt(targetEndPoint.y), width, c);
        //                plotLine(x0, y0,
        //                    Mathf.RoundToInt(targetEndPointOpposite.x),
        //                    Mathf.RoundToInt(targetEndPointOpposite.y), width, c);
        //                lastX = x0;
        //                lastY = y0;
        //            }
        //        }

        //        numOfPointsDrawn++;

        //        if (x0 == x2 && y0 == y2) return;  /* last pixel -> curve finished */
        //        // y1 = ;                  /* save value for test of y step */
        //        if (2 * err > dy) { x0 += sx; dx -= xy; err += dy += yy; } /* x step */
        //        if (2 * err < dx) { y0 += sy; dy -= xy; err += dx += xx; } /* y step */
        //    } while (dy < dx);           /* gradient negates -> algorithm fails */
        //}
        //plotLine(x0, y0, x2, y2, width, c);                  /* plot remaining part to end */
    }
    void plotBidirectionalArrowPatternFlowWithQuadBezierSeg(int x0, int y0, int x1, int y1, int x2, int y2,
        int width, int arrowWidth, int leftDist, int rightDist, int boneSpace, float angleOffset
            , Color LeftHSVStart, Color LeftHSVEnd, Color RightHSVStart, Color RightHSVEnd, Color borderColor)
    {

        Gradient LG = new Gradient();
        Gradient RG = new Gradient();
        GradientColorKey[] CKL;
        GradientColorKey[] CKR;
        GradientAlphaKey[] alphaKey;
        CKL = new GradientColorKey[2];
        CKL[0].color = LeftHSVStart;
        CKL[0].time = 0.0f;
        CKL[1].color = LeftHSVEnd;
        CKL[1].time = 1.0f;
        CKR = new GradientColorKey[2];
        CKR[0].color = RightHSVStart;
        CKR[0].time = 0.0f;
        CKR[1].color = RightHSVEnd;
        CKR[1].time = 1.0f;
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        LG.SetKeys(CKL, alphaKey);
        RG.SetKeys(CKR, alphaKey);
        //LG.Evaluate(0.25f)

        int sx = x2 - x1, sy = y2 - y1;
        long xx = x0 - x1, yy = y0 - y1, xy;         /* relative values for checks */
        double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

        Debug.Assert(xx * sx <= 0 && yy * sy <= 0);  /* sign of gradient must not change */

        Vector2 startPoint = new Vector2(x0, y0);
        Vector2 endPoint = new Vector2(x2, y2);
        float wholeDist = Vector2.Distance(startPoint, endPoint);
        int lastX = x0;
        int lastY = y0;
        int numOfPointsDrawn = 0;
        int nx = 0, ny = 0;
        int deltaX = 0, deltaY = 0;
        Vector2 lastpL = new Vector2(), lastpR = new Vector2();
        int numOfTargetPoints = 0;
        bool lastDoPlot = true;

        //if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
        //{ /* begin with longer part */
        //    x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;  /* swap P0 P2 */
        //}
        if (cur != 0)
        {                                    /* no straight line */
            xx += sx; xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
            yy += sy; yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
            xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
            if (cur * sx * sy < 0)
            {                           /* negated curvature? */
                xx = -xx; yy = -yy; xy = -xy; cur = -cur;
            }
            dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;             /* differences 1st degree */
            dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;
            xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
            do
            {
                visMap.SetPixel(x0, y0, borderColor);
                for (int i = 1; i < width; i++)
                {
                    visMap.SetPixel(x0, y0 + i, borderColor);
                    visMap.SetPixel(x0, y0 - i, borderColor);
                    visMap.SetPixel(x0 + i, y0, borderColor);
                    visMap.SetPixel(x0 - i, y0, borderColor);
                }

                if (numOfPointsDrawn > 0)
                {
                    deltaX = x0 - lastX;
                    deltaY = y0 - lastY;

                    //nx = -deltaY;
                    //ny = deltaX;

                    Vector3 localDir = new Vector3(deltaX, 0, deltaY);
                    localDir.Normalize();
                    Vector3 worldDir = OriginAnchor.transform.TransformDirection(localDir);

                    int dirL_x = Mathf.RoundToInt(Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY);
                    int dirL_y = Mathf.RoundToInt(Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);

                    int dirR_x = Mathf.RoundToInt(Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY);
                    int dirR_y = Mathf.RoundToInt(Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);

                    Vector2 dirL = new Vector2(dirL_x, dirL_y);
                    dirL.Normalize();
                    Vector2 dirR = new Vector2(dirR_x, dirR_y);
                    dirR.Normalize();

                    Vector2 pML = leftDist * dirL + new Vector2(x0, y0);
                    Vector2 pMR = - rightDist * dirL + new Vector2(x0, y0);
                    Vector2 pL = pML - leftDist * dirR;
                    Vector2 pR = pMR + rightDist * dirR;

                    float currDist = Vector2.Distance(new Vector2(x0, y0), startPoint);
                    float percentage = currDist / wholeDist;

                    bool doPlot = true;
                    if (currDist < toTextureDist(objectMaskRadius)) 
                    {
                        doPlot = false;
                    }
                    //
                    if(lastDoPlot == false && doPlot == true)
                    {
                        GameObject o = Instantiate(textPrefeb, toWorldCoordinate(new Vector2(x0, y0)), Quaternion.LookRotation(worldDir,Vector3.up));
                        o.transform.Find("TextPrefeb").GetComponent<TextMeshPro>().text = "Incoming: 2; Outgoing: 4";
                    }

                    if (numOfPointsDrawn % boneSpace == 0)
                    {

                        if (doPlot)
                        {
                            // plot the bi - dir. arrow  - L + R
                            plotLine(
                                Mathf.RoundToInt(x0), Mathf.RoundToInt(y0),
                                Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y), arrowWidth, LG.Evaluate(percentage));
                            plotLine(
                                Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y),
                                Mathf.RoundToInt(pL.x), Mathf.RoundToInt(pL.y), arrowWidth, LG.Evaluate(percentage));

                            plotLine(
                                Mathf.RoundToInt(x0), Mathf.RoundToInt(y0),
                                Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y), arrowWidth, RG.Evaluate(percentage));
                            plotLine(
                                Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y),
                                Mathf.RoundToInt(pR.x), Mathf.RoundToInt(pR.y), arrowWidth, RG.Evaluate(percentage));
                        }

                        // plot the border 
                        if (numOfTargetPoints > 0)
                        {
                            if (doPlot)
                            {
                                plotLine(
                                    Mathf.RoundToInt(lastpL.x),
                                    Mathf.RoundToInt(lastpL.y),
                                    Mathf.RoundToInt(pL.x),
                                    Mathf.RoundToInt(pL.y), width, borderColor);

                                plotLine(
                                    Mathf.RoundToInt(lastpR.x),
                                    Mathf.RoundToInt(lastpR.y),
                                    Mathf.RoundToInt(pR.x),
                                    Mathf.RoundToInt(pR.y), width, borderColor);
                            }
                        }
                        lastpL = pL;
                        lastpR = pR;
                        numOfTargetPoints++;
                        lastX = x0;
                        lastY = y0;
                    }
                    lastDoPlot = doPlot;
                }

                numOfPointsDrawn++;

                if (x0 == x2 && y0 == y2) return;  /* last pixel -> curve finished */
                // y1 = ;                  /* save value for test of y step */
                if (2 * err > dy) { x0 += sx; dx -= xy; err += dy += yy; } /* x step */
                if (2 * err < dx) { y0 += sy; dy -= xy; err += dx += xx; } /* y step */
            } while (dy < dx);           /* gradient negates -> algorithm fails */
        }
        //plotLine(x0, y0, x2, y2, width, Color.HSVToRGB(HSV.x, HSV.y, HSV.z));                  /* plot remaining part to end */
    }
    void plotArrowPatternFlowWithQuadBezierSeg(int x0, int y0, int x1, int y1, int x2, int y2,
        int depth, int arrowLength, int boneSpace, float angleOffset, Vector3 HSV, Color borderColor)
    {
        int sx = x2 - x1, sy = y2 - y1;
        long xx = x0 - x1, yy = y0 - y1, xy;         /* relative values for checks */
        double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

        Debug.Assert(xx * sx <= 0 && yy * sy <= 0);  /* sign of gradient must not change */

        Vector2 startPoint = new Vector2(x0, y0);
        Vector2 endPoint = new Vector2(x2, y2);
        float wholeDist = Vector2.Distance(startPoint, endPoint);
        int lastX = x0;
        int lastY = y0;
        int numOfPointsDrawn = 0;
        int nx = 0, ny = 0;
        int deltaX = 0, deltaY = 0;
        Vector2 lastTargetEndPoint = new Vector2(), lastlastTargetEndPointOpposite = new Vector2();
        int numOfTargetPoints = 0;

        //if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
        //{ /* begin with longer part */
        //    x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;  /* swap P0 P2 */
        //}
        if (cur != 0)
        {                                    /* no straight line */
            xx += sx; xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
            yy += sy; yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
            xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
            if (cur * sx * sy < 0)
            {                           /* negated curvature? */
                xx = -xx; yy = -yy; xy = -xy; cur = -cur;
            }
            dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;             /* differences 1st degree */
            dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;
            xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
            do
            {
                //visMap.SetPixel(x0, y0, c);
                //for (int i = 1; i < depth; i++)
                //{
                //    visMap.SetPixel(x0, y0 + i, c);
                //    visMap.SetPixel(x0, y0 - i, c);
                //    visMap.SetPixel(x0 + i, y0, c);
                //    visMap.SetPixel(x0 - i, y0, c);
                //}

                if (numOfPointsDrawn > 0)
                {
                    deltaX = x0 - lastX;
                    deltaY = y0 - lastY;

                    //nx = -deltaY;
                    //ny = deltaX;

                    int dirL_x = Mathf.RoundToInt(Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY);
                    int dirL_y = Mathf.RoundToInt(Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);

                    int dirR_x = Mathf.RoundToInt(Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY);
                    int dirR_y = Mathf.RoundToInt(Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);

                    Vector2 dirL = new Vector2(dirL_x, dirL_y);
                    dirL.Normalize();
                    Vector2 dirR = new Vector2(dirR_x, dirR_y);
                    dirR.Normalize();

                    Vector2 targetEndPoint = arrowLength * dirL + new Vector2(x0, y0);
                    Vector2 targetEndPointOpposite = arrowLength * dirR + new Vector2(x0, y0);

                    float percentage = Vector2.Distance(new Vector2(x0, y0), startPoint) / wholeDist;

                    if (numOfPointsDrawn % boneSpace == 0)
                    {
                        // plot the arrow 
                        plotLine(x0, y0,
                            Mathf.RoundToInt(targetEndPoint.x),
                            Mathf.RoundToInt(targetEndPoint.y), depth, Color.HSVToRGB(HSV.x, HSV.y, percentage));
                        plotLine(x0, y0,
                            Mathf.RoundToInt(targetEndPointOpposite.x),
                            Mathf.RoundToInt(targetEndPointOpposite.y), depth, Color.HSVToRGB(HSV.x, HSV.y, percentage));

                        if (numOfTargetPoints > 0)
                        {
                            plotLine(
                                Mathf.RoundToInt(lastTargetEndPoint.x),
                                Mathf.RoundToInt(lastTargetEndPoint.y),
                                Mathf.RoundToInt(targetEndPoint.x),
                                Mathf.RoundToInt(targetEndPoint.y), depth, borderColor);

                            plotLine(
                                Mathf.RoundToInt(lastlastTargetEndPointOpposite.x),
                                Mathf.RoundToInt(lastlastTargetEndPointOpposite.y),
                                Mathf.RoundToInt(targetEndPointOpposite.x),
                                Mathf.RoundToInt(targetEndPointOpposite.y), depth, borderColor);
                        }
                        lastTargetEndPoint = targetEndPoint;
                        lastlastTargetEndPointOpposite = targetEndPointOpposite;
                        numOfTargetPoints++;
                        lastX = x0;
                        lastY = y0;
                    }
                }

                numOfPointsDrawn++;

                if (x0 == x2 && y0 == y2) return;  /* last pixel -> curve finished */
                // y1 = ;                  /* save value for test of y step */
                if (2 * err > dy) { x0 += sx; dx -= xy; err += dy += yy; } /* x step */
                if (2 * err < dx) { y0 += sy; dy -= xy; err += dx += xx; } /* y step */
            } while (dy < dx);           /* gradient negates -> algorithm fails */
        }
        plotLine(x0, y0, x2, y2, depth, Color.HSVToRGB(HSV.x, HSV.y, HSV.z));                  /* plot remaining part to end */
    }
    void plotTimeRulerWithQuadBezierSeg(int x0, int y0, int x1, int y1, int x2, int y2,
       int depth, int arrowLength, int boneSpace, int drawSpace, float angleOffset, Color c)
    {
        int sx = x2 - x1, sy = y2 - y1;
        long xx = x0 - x1, yy = y0 - y1, xy;         /* relative values for checks */
        double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

        Debug.Assert(xx * sx <= 0 && yy * sy <= 0);  /* sign of gradient must not change */

        int lastX = x0;
        int lastY = y0;
        int numOfPointsDrawn = 0;
        int lastDrawnPointIndex = 0;
        int nx = 0, ny = 0;
        int deltaX = 0, deltaY = 0;
        Vector2 lastTargetEndPoint = new Vector2(), lastlastTargetEndPointOpposite = new Vector2();
        int numOfTargetPoints = 0;

        if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
        { /* begin with longer part */
            x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;  /* swap P0 P2 */
        }
        if (cur != 0)
        {                                    /* no straight line */
            xx += sx; xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
            yy += sy; yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
            xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
            if (cur * sx * sy < 0)
            {                           /* negated curvature? */
                xx = -xx; yy = -yy; xy = -xy; cur = -cur;
            }
            dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;             /* differences 1st degree */
            dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;
            xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
            do
            {
                //visMap.SetPixel(x0, y0, c);
                //for (int i = 1; i < depth; i++)
                //{
                //    visMap.SetPixel(x0, y0 + i, c);
                //    visMap.SetPixel(x0, y0 - i, c);
                //    visMap.SetPixel(x0 + i, y0, c);
                //    visMap.SetPixel(x0 - i, y0, c);
                //}

                if (numOfPointsDrawn > 0)
                {
                    deltaX = x0 - lastX;
                    deltaY = y0 - lastY;

                    nx = -deltaY;
                    ny = deltaX;

                    Vector2 n = new Vector2(nx, ny);

                    //int dirL_x = Mathf.RoundToInt(Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY);
                    //int dirL_y = Mathf.RoundToInt(Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);

                    //int dirR_x = Mathf.RoundToInt(Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY);
                    //int dirR_y = Mathf.RoundToInt(Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);

                    //Vector2 dirL = new Vector2(dirL_x, dirL_y);
                    //dirL.Normalize();
                    //Vector2 dirR = new Vector2(dirR_x, dirR_y);
                    //dirR.Normalize();
                    n.Normalize();

                    Vector2 targetEndPoint = arrowLength * n + new Vector2(x0, y0);
                    Vector2 targetEndPointOpposite = - arrowLength * n + new Vector2(x0, y0);

                    // 
                    //visMap.SetPixel(Mathf.RoundToInt(targetEndPoint.x), Mathf.RoundToInt(targetEndPoint.y), c);
                    //for (int i = 1; i < depth; i++)
                    //{
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPoint.x), Mathf.RoundToInt(targetEndPoint.y) + i, c);
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPoint.x), Mathf.RoundToInt(targetEndPoint.y) - i, c);
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPoint.x) + i, Mathf.RoundToInt(targetEndPoint.y), c);
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPoint.x) - i, Mathf.RoundToInt(targetEndPoint.y), c);
                    //}
                    //visMap.SetPixel(Mathf.RoundToInt(targetEndPointOpposite.x), Mathf.RoundToInt(targetEndPointOpposite.y), c);
                    //for (int i = 1; i < depth; i++)
                    //{
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPointOpposite.x), Mathf.RoundToInt(targetEndPointOpposite.y) + i, c);
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPointOpposite.x), Mathf.RoundToInt(targetEndPointOpposite.y) - i, c);
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPointOpposite.x) + i, Mathf.RoundToInt(targetEndPointOpposite.y), c);
                    //    visMap.SetPixel(Mathf.RoundToInt(targetEndPointOpposite.x) - i, Mathf.RoundToInt(targetEndPointOpposite.y), c);
                    //}

                    boneSpace = Random.Range(5, 40);



                    if (numOfPointsDrawn - lastDrawnPointIndex > boneSpace)
                    {
                        //plotLine(x0, y0,
                        //    Mathf.RoundToInt(targetEndPoint.x),
                        //    Mathf.RoundToInt(targetEndPoint.y), depth, c);
                        //plotLine(x0, y0,
                        //    Mathf.RoundToInt(targetEndPointOpposite.x),
                        //    Mathf.RoundToInt(targetEndPointOpposite.y), depth, c);
                        if (numOfTargetPoints > 0)
                        {
                            plotLine(
                                Mathf.RoundToInt(lastTargetEndPoint.x),
                                Mathf.RoundToInt(lastTargetEndPoint.y),
                                Mathf.RoundToInt(targetEndPoint.x),
                                Mathf.RoundToInt(targetEndPoint.y), depth, c);

                            plotLine(
                                Mathf.RoundToInt(lastlastTargetEndPointOpposite.x),
                                Mathf.RoundToInt(lastlastTargetEndPointOpposite.y),
                                Mathf.RoundToInt(targetEndPointOpposite.x),
                                Mathf.RoundToInt(targetEndPointOpposite.y), depth, c);
                        }
                        lastTargetEndPoint = targetEndPoint;
                        lastlastTargetEndPointOpposite = targetEndPointOpposite;
                        numOfTargetPoints++;

                        plotLine(
                            Mathf.RoundToInt(targetEndPoint.x),
                            Mathf.RoundToInt(targetEndPoint.y),
                            Mathf.RoundToInt(targetEndPointOpposite.x),
                            Mathf.RoundToInt(targetEndPointOpposite.y), depth, Color.red);
                        lastDrawnPointIndex = numOfPointsDrawn;

                        lastX = x0;
                        lastY = y0;
                    }

                }

                numOfPointsDrawn++;

                if (x0 == x2 && y0 == y2) return;  /* last pixel -> curve finished */
                // y1 = ;                  /* save value for test of y step */
                if (2 * err > dy) { x0 += sx; dx -= xy; err += dy += yy; } /* x step */
                if (2 * err < dx) { y0 += sy; dy -= xy; err += dx += xx; } /* y step */
            } while (dy < dx);           /* gradient negates -> algorithm fails */
        }
        plotLine(x0, y0, x2, y2, depth, c);                  /* plot remaining part to end */
    }

    /*
     * Vector2 startLocalSpace, Vector2 endLocalspace, Vector2 startControlPointLocalspace, Vector2 endControlPointLocalSpace,
        int segments, int width, int arrowLength, int boneSpace, float angleOffset, Color c
     */
    void plotDoubleArrowPatternFlowWithQuadBezierSeg(
        Vector2 startLocalSpace, Vector2 endLocalspace, Vector2 startControlPointLocalspace, Vector2 endControlPointLocalSpace,
        int segments, int width, int arrowWidth, int leftDist, int rightDist, int boneSpace, float angleOffset
            ,Color LeftHSVStart, Color LeftHSVEnd, Color RightHSVStart, Color RightHSVEnd, Color borderColor)
    {
        Gradient LG = new Gradient();
        Gradient RG = new Gradient();
        GradientColorKey[] CKL;
        GradientColorKey[] CKR;
        GradientAlphaKey[] alphaKey;
        CKL = new GradientColorKey[2];
        CKL[0].color = LeftHSVStart;
        CKL[0].time = 0.0f;
        CKL[1].color = LeftHSVEnd;
        CKL[1].time = 1.0f;
        CKR = new GradientColorKey[2];
        CKR[0].color = RightHSVStart;
        CKR[0].time = 0.0f;
        CKR[1].color = RightHSVEnd;
        CKR[1].time = 1.0f;
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        LG.SetKeys(CKL, alphaKey);
        RG.SetKeys(CKR, alphaKey);
        //LG.Evaluate(0.25f)


        float p = 1.0f / segments;
        float q = p;
        Vector2 lastPoint = startLocalSpace;
        Vector2 lastpL = startLocalSpace;
        Vector2 lastpR = startLocalSpace;
        for (int i = 1; i < segments; i++, p += q)
        {
            Vector2 currPoint = p * p * p * (endLocalspace + 3.0f * (startControlPointLocalspace - endControlPointLocalSpace) - startLocalSpace) +
                      3.0f * p * p * (startLocalSpace - 2.0f * startControlPointLocalspace + endControlPointLocalSpace) +
                      3.0f * p * (startControlPointLocalspace - startLocalSpace) + startLocalSpace;
            float deltaX = currPoint.x - lastPoint.x;
            float deltaY = currPoint.y - lastPoint.y;
            Vector2 dirL = new Vector2(
                Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY,
                Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);
            Vector2 dirR = new Vector2(
                Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY,
                Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);
            dirL.Normalize();
            dirR.Normalize();
            Vector2 pML = currPoint + dirL * leftDist; 
            Vector2 pMR = currPoint + dirR * rightDist;
            Vector2 pL = pML - dirR * leftDist;
            Vector2 pR = pMR - dirL * rightDist;

            float wholeDist = Vector2.Distance(startLocalSpace, endLocalspace);
            float currDist = Vector2.Distance(currPoint, startLocalSpace);
            float endDist = Vector2.Distance(currPoint, endLocalspace);
            float percentage = (float)i / segments;

            bool doPlot = true;
            if (currDist < toTextureDist(objectMaskRadius)|| endDist < toTextureDist(objectMaskRadius))
            {
                doPlot = false;
            }

            if (doPlot)
            {
                plotLine( // the middle line 
                    Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y), 
                    Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y), width, borderColor);
                if (segments % boneSpace == 0)
                {
                    if (useGradient){
                        // plot the W shape 
                        plotLine(
                            Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y),
                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y), arrowWidth, width, LG.Evaluate(percentage));
                        plotLine(
                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y),
                            Mathf.RoundToInt(pL.x), Mathf.RoundToInt(pL.y), arrowWidth, width, LG.Evaluate(percentage));

                        plotLine(
                            Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y),
                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y), arrowWidth, width, RG.Evaluate(percentage));
                        plotLine(
                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y),
                            Mathf.RoundToInt(pR.x), Mathf.RoundToInt(pR.y), arrowWidth, width, RG.Evaluate(percentage));
                    }
                    else
                    {
                        // plot the W shape 
                        plotLine(
                            Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y),
                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y), arrowWidth, width, LeftHSVStart);
                        plotLine(
                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y),
                            Mathf.RoundToInt(pL.x), Mathf.RoundToInt(pL.y), arrowWidth, width, LeftHSVStart);

                        plotLine(
                            Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y),
                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y), arrowWidth, width, RightHSVStart);
                        plotLine(
                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y),
                            Mathf.RoundToInt(pR.x), Mathf.RoundToInt(pR.y), arrowWidth, width, RightHSVStart);
                    }

                    if (i > 2) // plot border lines 
                    {
                        plotLine(
                            Mathf.RoundToInt(lastpL.x),
                            Mathf.RoundToInt(lastpL.y),
                            Mathf.RoundToInt(pL.x),
                            Mathf.RoundToInt(pL.y), width, borderColor);

                        plotLine(
                            Mathf.RoundToInt(lastpR.x),
                            Mathf.RoundToInt(lastpR.y),
                            Mathf.RoundToInt(pR.x),
                            Mathf.RoundToInt(pR.y), width, borderColor);
                    }
                }
            }

            lastpL = pL;
            lastpR = pR;
            lastPoint = currPoint;
        }
        //plotLine(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y),
        //        Mathf.RoundToInt(endLocalspace.x), Mathf.RoundToInt(endLocalspace.y), width, borderColor);





        //int sx = x2 - x1, sy = y2 - y1;
        //long xx = x0 - x1, yy = y0 - y1, xy;         /* relative values for checks */
        //double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

        //Debug.Assert(xx * sx <= 0 && yy * sy <= 0);  /* sign of gradient must not change */

        //Vector2 startPoint = new Vector2(x0, y0);
        //Vector2 endPoint = new Vector2(x2, y2);
        //float wholeDist = Vector2.Distance(startPoint, endPoint);
        //int lastX = x0;
        //int lastY = y0;
        //int numOfPointsDrawn = 0;
        //int nx = 0, ny = 0;
        //int deltaX = 0, deltaY = 0;
        //Vector2 lastpL = new Vector2(), lastpR = new Vector2();
        //int numOfTargetPoints = 0;
        //bool lastDoPlot = true;

        ////if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
        ////{ /* begin with longer part */
        ////    x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;  /* swap P0 P2 */
        ////}
        //if (cur != 0)
        //{                                    /* no straight line */
        //    xx += sx; xx *= sx = x0 < x2 ? 1 : -1;           /* x step direction */
        //    yy += sy; yy *= sy = y0 < y2 ? 1 : -1;           /* y step direction */
        //    xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
        //    if (cur * sx * sy < 0)
        //    {                           /* negated curvature? */
        //        xx = -xx; yy = -yy; xy = -xy; cur = -cur;
        //    }
        //    dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;             /* differences 1st degree */
        //    dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;
        //    xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
        //    do
        //    {
        //        if (numOfPointsDrawn > 0)
        //        {
        //            deltaX = x0 - lastX;
        //            deltaY = y0 - lastY;

        //            //nx = -deltaY;
        //            //ny = deltaX;

        //            Vector3 localDir = new Vector3(deltaX, 0, deltaY);
        //            localDir.Normalize();
        //            Vector3 worldDir = OriginAnchor.transform.TransformDirection(localDir);

        //            int dirL_x = Mathf.RoundToInt(Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY);
        //            int dirL_y = Mathf.RoundToInt(Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);

        //            int dirR_x = Mathf.RoundToInt(Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY);
        //            int dirR_y = Mathf.RoundToInt(Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);

        //            Vector2 dirL = new Vector2(dirL_x, dirL_y);
        //            dirL.Normalize();
        //            Vector2 dirR = new Vector2(dirR_x, dirR_y);
        //            dirR.Normalize();

        //            Vector2 pML = new Vector2(x0, y0) + dirL * leftDist;
        //            Vector2 pL = pML - dirR * leftDist;
        //            Vector2 pMR = new Vector2(x0, y0) + dirR * rightDist;
        //            Vector2 pR = pMR - dirL * rightDist;

        //            float currDist = Vector2.Distance(new Vector2(x0, y0), startPoint);
        //            float percentage = currDist / wholeDist;

        //            bool doPlot = true;
        //            if (currDist < toTextureDist(objectMaskRadius))
        //            {
        //                doPlot = false;
        //            }
        //            //
        //            if (lastDoPlot == false && doPlot == true)
        //            {
        //                //GameObject o = Instantiate(textPrefeb, toWorldCoordinate(new Vector2(x0, y0)), Quaternion.LookRotation(worldDir, Vector3.up));
        //                //o.transform.Find("TextPrefeb").GetComponent<TextMeshPro>().text = "Incoming: 2; Outgoing: 4";
        //            }

        //            if (numOfPointsDrawn % boneSpace == 0)
        //            {

        //                if (doPlot)
        //                {
        //                    if (useGradient)
        //                    {
        //                        // plot the bi - dir. arrow  - L + R
        //                        plotLine(
        //                            Mathf.RoundToInt(x0), Mathf.RoundToInt(y0),
        //                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y), arrowWidth, width, LG.Evaluate(percentage));
        //                        plotLine(
        //                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y),
        //                            Mathf.RoundToInt(pL.x), Mathf.RoundToInt(pL.y), arrowWidth, width, LG.Evaluate(percentage));

        //                        plotLine(
        //                            Mathf.RoundToInt(x0), Mathf.RoundToInt(y0),
        //                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y), arrowWidth, width, RG.Evaluate(percentage));
        //                        plotLine(
        //                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y),
        //                            Mathf.RoundToInt(pR.x), Mathf.RoundToInt(pR.y), arrowWidth, width,  RG.Evaluate(percentage));
        //                    }
        //                    else
        //                    {
        //                        // plot the bi - dir. arrow  - L + R
        //                        plotLine(
        //                            Mathf.RoundToInt(x0), Mathf.RoundToInt(y0),
        //                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y), arrowWidth, width, LeftHSVStart);
        //                        plotLine(
        //                            Mathf.RoundToInt(pML.x), Mathf.RoundToInt(pML.y),
        //                            Mathf.RoundToInt(pL.x), Mathf.RoundToInt(pL.y), arrowWidth, width, LeftHSVStart);

        //                        plotLine(
        //                            Mathf.RoundToInt(x0), Mathf.RoundToInt(y0),
        //                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y), arrowWidth, width, RightHSVStart);
        //                        plotLine(
        //                            Mathf.RoundToInt(pMR.x), Mathf.RoundToInt(pMR.y),
        //                            Mathf.RoundToInt(pR.x), Mathf.RoundToInt(pR.y), arrowWidth, width, RightHSVStart);
        //                    }

        //                }

        //                // plot the border 
        //                if (numOfTargetPoints > 0)
        //                {
        //                    if (doPlot)
        //                    {
        //                        plotLine(
        //                            Mathf.RoundToInt(lastpL.x),
        //                            Mathf.RoundToInt(lastpL.y),
        //                            Mathf.RoundToInt(pL.x),
        //                            Mathf.RoundToInt(pL.y), width, borderColor);

        //                        plotLine(
        //                            Mathf.RoundToInt(lastpR.x),
        //                            Mathf.RoundToInt(lastpR.y),
        //                            Mathf.RoundToInt(pR.x),
        //                            Mathf.RoundToInt(pR.y), width, borderColor);
        //                    }
        //                }
        //                lastpL = pL;
        //                lastpR = pR;
        //                numOfTargetPoints++;
        //                lastX = x0;
        //                lastY = y0;
        //            }
        //            lastDoPlot = doPlot;
        //        }

        //        visMap.SetPixel(x0, y0, borderColor);
        //        for (int i = 1; i < width; i++)
        //        {
        //            visMap.SetPixel(x0, y0 + i, borderColor);
        //            visMap.SetPixel(x0, y0 - i, borderColor);
        //            visMap.SetPixel(x0 + i, y0, borderColor);
        //            visMap.SetPixel(x0 - i, y0, borderColor);
        //        }

        //        numOfPointsDrawn++;

        //        if (x0 == x2 && y0 == y2) return;  /* last pixel -> curve finished */
        //        // y1 = ;                  /* save value for test of y step */
        //        if (2 * err > dy) { x0 += sx; dx -= xy; err += dy += yy; } /* x step */
        //        if (2 * err < dx) { y0 += sy; dy -= xy; err += dx += xx; } /* y step */
        //    } while (dy < dx);           /* gradient negates -> algorithm fails */
        //}
        ////plotLine(x0, y0, x2, y2, width, Color.HSVToRGB(HSV.x, HSV.y, HSV.z));                  /* plot remaining part to end */
    }

    void plotPaceViewFlowWithQuadBezierSeg(
    Vector2 startLocalSpace, Vector2 endLocalspace, Vector2 startControlPointLocalspace, Vector2 endControlPointLocalSpace,
    int segments, int width, int arrowLength, int boneSpace, float angleOffset, 
    Color lowestSpeedColor, Color middleSpeedColor, Color highestSpeedColor, List<float> speedMap, float highestSpeedPercentage)
    {

        float p = 1.0f / segments;
        float q = p;
        Vector2 lastPoint = startLocalSpace;
        for (int i = 1; i < segments; i++, p += q)
        {
            Vector2 currPoint = p * p * p * (endLocalspace + 3.0f * (startControlPointLocalspace - endControlPointLocalSpace) - startLocalSpace) +
                      3.0f * p * p * (startLocalSpace - 2.0f * startControlPointLocalspace + endControlPointLocalSpace) +
                      3.0f * p * (startControlPointLocalspace - startLocalSpace) + startLocalSpace;
            float deltaX = currPoint.x - lastPoint.x;
            float deltaY = currPoint.y - lastPoint.y;
            Vector2 dirL = new Vector2(
                Mathf.Cos(angleOffset) * deltaX - Mathf.Sin(angleOffset) * deltaY,
                Mathf.Sin(angleOffset) * deltaX + Mathf.Cos(angleOffset) * deltaY);
            Vector2 dirR = new Vector2(
                Mathf.Cos(-angleOffset) * deltaX - Mathf.Sin(-angleOffset) * deltaY,
                Mathf.Sin(-angleOffset) * deltaX + Mathf.Cos(-angleOffset) * deltaY);
            dirL.Normalize();
            dirR.Normalize();
            Vector2 epL = currPoint + arrowLength * dirL;
            Vector2 epR = currPoint + arrowLength * dirR;

            // seg. interpolation 
            //float s = 0;
            //for(int b = 0; b < speedMap.Count - 1; b++)
            //{
            //    if ((p < ((float)(b + 1) / speedMap.Count)) && (p > ((float)b / speedMap.Count)))
            //    {
            //       // s = Mathf.Lerp(speedMap[b], speedMap[b + 1], p);
            //        s = speedMap[b];
            //    }
            //}

            float s;
            if (p < highestSpeedPercentage)
                s = p / highestSpeedPercentage;
            else
                s = 1 - (p - 0.6f) / 0.4f;

            //Color c;
            //if(s <0.5f)
            //    c = Color.Lerp(lowestSpeedColor, middleSpeedColor, s);
            //else
            //    c = Color.Lerp(middleSpeedColor, highestSpeedColor, s);

            Color c = paceGradient.Evaluate(s);

            plotLine(
                Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y), 
                Mathf.RoundToInt(epL.x), Mathf.RoundToInt(epL.y), width, c);

            //plotLine(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y), Mathf.RoundToInt(currPoint.x), Mathf.RoundToInt(currPoint.y), width, c);

            lastPoint = currPoint;
        }
        //plotLine(Mathf.RoundToInt(lastPoint.x), Mathf.RoundToInt(lastPoint.y), Mathf.RoundToInt(endLocalspace.x), Mathf.RoundToInt(endLocalspace.y), width, c);
    }



    public void showALLBDFVPara()
    {
        adjustmentInfoBoard.text = "";
        adjustmentInfoBoard.text += "boneSpacePixel = " + boneSpacePixel + "\n";
        adjustmentInfoBoard.text += "arrowWidthWorldSpace = " + arrowWidthWorldSpace + "\n";
        adjustmentInfoBoard.text += "angleInDegree = " + angleInDegree + "\n";
        adjustmentInfoBoard.text += "borderWidthPixel = " + borderWidthPixel + "\n";
    }

    public void more_boneSpacePixel()
    {
        boneSpacePixel += 10;
        adjustmentInfoBoard.text = "boneSpacePixel = " + boneSpacePixel;
    }

    public void less_boneSpacePixel()
    {
        boneSpacePixel -= 10;
        adjustmentInfoBoard.text = "boneSpacePixel = " + boneSpacePixel;
    }
    public void more_arrowWidthWorldSpace()
    {
        arrowWidthWorldSpace += 0.01f;
        adjustmentInfoBoard.text = "arrowWidthWorldSpace = " + arrowWidthWorldSpace;
    }

    public void less_arrowWidthWorldSpace()
    {
        arrowWidthWorldSpace -= 0.01f;
        adjustmentInfoBoard.text = "arrowWidthWorldSpace = " + arrowWidthWorldSpace;
    }

    public void more_angleInDegree()
    {
        angleInDegree += 10;
        adjustmentInfoBoard.text = "angleInDegree = " + angleInDegree;
    }

    public void less_angleInDegree()
    {
        angleInDegree -= 10;
        adjustmentInfoBoard.text = "angleInDegree = " + angleInDegree;
    }

    public void more_borderWidthPixel()
    {
        borderWidthPixel += 2;
        adjustmentInfoBoard.text = "borderWidthPixel = " + borderWidthPixel;
    }

    public void less_borderWidthPixel()
    {
        borderWidthPixel -= 2;
        adjustmentInfoBoard.text = "borderWidthPixel = " + borderWidthPixel;
    }


    // draw functions - warping the plot functions 
    public void drawCurveBetweenTwoPointsOnVisMap(Vector3 startingPoint, Vector3 endPoint, bool isOdd, Vector2 controlPointOffset)
    {
        // 
        Vector2 startingPointI = toTextureCoordinate(startingPoint);
        Vector2 endPointI = toTextureCoordinate(endPoint);

        // flip the control point when odd 
        if (isOdd)
        {
            plotQuadBezierSeg(
                Mathf.RoundToInt(startingPointI.x) , Mathf.RoundToInt(startingPointI.y),
                Mathf.RoundToInt(endPointI.x) + Mathf.RoundToInt(controlPointOffset.x), Mathf.RoundToInt(startingPointI.y) + Mathf.RoundToInt(controlPointOffset.y),
                Mathf.RoundToInt(endPointI.x), Mathf.RoundToInt(endPointI.y), 2, Color.HSVToRGB(122 / 360.0f, 0.48f, 0.58f));
        }
        else
        {
            plotQuadBezierSeg(
                Mathf.RoundToInt(startingPointI.x), Mathf.RoundToInt(startingPointI.y),
                Mathf.RoundToInt(startingPointI.x) + Mathf.RoundToInt(controlPointOffset.x), Mathf.RoundToInt(endPointI.y) + Mathf.RoundToInt(controlPointOffset.y),
                Mathf.RoundToInt(endPointI.x), Mathf.RoundToInt(endPointI.y), 2, Color.HSVToRGB(122 / 360.0f, 0.48f, 0.58f));
        }
    }
    public void drawFishBoneFlowBetweenTwoPointsOnVisMap(AOIEdge edge)
    {
        //
        Vector3 dir = edge.e - edge.s;
        Vector3 perpendicularVector = new Vector3(-dir.z, 0, dir.x);
        Vector3 controlPointWorldSpace = edge.s + 0.5f * (edge.e - edge.s) + perpendicularVector * edge.offsetL;

        //
        Vector2 startingPointI = toTextureCoordinate(edge.s);
        Vector2 endPointI = toTextureCoordinate(edge.e);
        Vector2 cpPointI = toTextureCoordinate(controlPointWorldSpace);

        //
        plotFishBoneFlowWithQuadBezierSeg(
            startingPointI, endPointI, cpPointI, cpPointI,
            20, // segments
            2, // width 
            10, // arrow length
            2, // bone space
            130.0f / 180.0f * Mathf.PI, // angle offset 
            edge.groupID); // color 
        
    }
    public void drawArrowPatternFlowBetweenTwoPointsOnVisMap(Vector3 startingPoint, Vector3 endPoint, bool isOdd)
    {
        // 
        Vector2 startingPointI = toTextureCoordinate(startingPoint);
        Vector2 endPointI = toTextureCoordinate(endPoint);

        // 
        int startingPointX = Mathf.RoundToInt(startingPointI.x);
        int startingPointZ = Mathf.RoundToInt(startingPointI.y);
        int endPointX = Mathf.RoundToInt(endPointI.x);
        int endPointZ = Mathf.RoundToInt(endPointI.y);

        plotArrowPatternFlowWithQuadBezierSeg(
            startingPointX, startingPointZ,
            endPointX, startingPointZ,
            endPointX, endPointZ,
            2, 10, 20, 130.0f / 180.0f * Mathf.PI, new Vector3(122 / 360.0f, 0.48f, 0.58f), Color.HSVToRGB(0,0,0.4f));
    }
    public void drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(Vector3 startingPoint, Vector3 endPoint, bool isOdd)
    {
        // 
        Vector2 startingPointI = toTextureCoordinate(startingPoint);
        Vector2 endPointI = toTextureCoordinate(endPoint);

        // 
        int startingPointX = Mathf.RoundToInt(startingPointI.x);
        int startingPointZ = Mathf.RoundToInt(startingPointI.y);
        int endPointX = Mathf.RoundToInt(endPointI.x);
        int endPointZ = Mathf.RoundToInt(endPointI.y);

        plotBidirectionalArrowPatternFlowWithQuadBezierSeg(
            startingPointX, startingPointZ,
            endPointX, startingPointZ,
            endPointX, endPointZ,
            borderWidthPixel, // width border 
            Mathf.RoundToInt(arrowWidthWorldSpace * resolutionXZ / TexDim), // width 
            Mathf.RoundToInt(0.1f * resolutionXZ / TexDim), Mathf.RoundToInt(0.35f * resolutionXZ / TexDim), // left - right distance  
            boneSpacePixel, // bone space
            angleInDegree / 180.0f * Mathf.PI, // angle 
            LeftHSVStart, LeftHSVEnd, RightHSVStart, RightHSVEnd,
            Color.HSVToRGB(0, 0, 0.4f)); // color 
    }
    public void drawDoubleArrowPatternFlowBetweenTwoPointsOnVisMap(AOIEdge edge, float radio)
    {
        //
        Vector3 dir = edge.e - edge.s;
        Vector3 perpendicularVector = new Vector3(-dir.z, 0, dir.x);
        Vector3 controlPointWorldSpace = edge.s + 0.5f * (edge.e - edge.s) + perpendicularVector * edge.offsetL;

        //
        Vector2 startingPointI = toTextureCoordinate(edge.s);
        Vector2 endPointI = toTextureCoordinate(edge.e);
        Vector2 cpPointI = toTextureCoordinate(controlPointWorldSpace);

        //float radio = Random.Range(0.05f, 0.95f);
        float entireWidth = 0.2f;

        plotDoubleArrowPatternFlowWithQuadBezierSeg(
            startingPointI, endPointI, cpPointI, cpPointI,
            20, // segments
            2, // width 
            2, // arrow length
            Mathf.RoundToInt(entireWidth * radio * resolutionXZ / TexDim), Mathf.RoundToInt(entireWidth * (1- radio) * resolutionXZ / TexDim), // left - right distance  
            1, // bone space
            45.0f / 180.0f * Mathf.PI, // angle offset 
            LeftHSVStart, LeftHSVEnd, RightHSVStart, RightHSVEnd, // arrow colors 
            Color.HSVToRGB(0, 0, 0.4f)); // border color 
    }
    public void drawTimeRulerBetweenTwoPointsOnVisMap(Vector3 startingPoint, Vector3 endPoint, bool isOdd)
    {
        // 
        Vector2 startingPointI = toTextureCoordinate(startingPoint);
        Vector2 endPointI = toTextureCoordinate(endPoint);

        // 
        int startingPointX = Mathf.RoundToInt(startingPointI.x);
        int startingPointZ = Mathf.RoundToInt(startingPointI.y);
        int endPointX = Mathf.RoundToInt(endPointI.x);
        int endPointZ = Mathf.RoundToInt(endPointI.y);

        plotTimeRulerWithQuadBezierSeg(
            startingPointX, startingPointZ,
            endPointX, startingPointZ,
            endPointX, endPointZ,
            2, 10, 
            20, // bone space 
            5, // drawn space 
            90.0f / 180.0f * Mathf.PI, Color.HSVToRGB(122 / 360.0f, 0.48f, 0.58f));
    }
    public void drawApproachCurves(Vector3 center, float r, Vector2 offset0, Vector2 offset1, Vector2 offset2, Vector2 offset3, int segments, int width, Color c)
    {
        float x = center.x, y = center.z; // take z from the center!
        plotCubicBezierSeg(new Vector2(x - r, y), new Vector2(x, y + r),
            new Vector2(x - r, y + r) + offset0,
            new Vector2(x - r, y + r) + offset0, segments, width, c);
        plotCubicBezierSeg(new Vector2(x, y + r), new Vector2(x + r, y),
            new Vector2(x + r, y + r) + offset1,
            new Vector2(x + r, y + r) + offset1, segments, width, c);
        plotCubicBezierSeg(new Vector2(x + r, y), new Vector2(x, y - r),
            new Vector2(x + r, y - r) + offset2,
            new Vector2(x + r, y - r) + offset2, segments, width, c);
        plotCubicBezierSeg(new Vector2(x, y - r), new Vector2(x - r, y), 
            new Vector2(x - r, y - r) + offset3, 
            new Vector2(x - r, y - r) + offset3, segments, width, c);
    }
    public void drawPaceViewBetweenTwoPointsOnVisMap(AOIEdge edge, float r)
    {
        //
        Vector3 dir = edge.e - edge.s;
        Vector3 perpendicularVector = new Vector3(-dir.z, 0, dir.x);
        Vector3 controlPointWorldSpace = edge.s + 0.5f * (edge.e - edge.s) + perpendicularVector * edge.offsetL;

        //
        Vector2 startingPointI = toTextureCoordinate(edge.s);
        Vector2 endPointI = toTextureCoordinate(edge.e);
        Vector2 cpPointI = toTextureCoordinate(controlPointWorldSpace);

        //
        plotPaceViewFlowWithQuadBezierSeg(
            startingPointI, endPointI, cpPointI, cpPointI,
            400, // segments
            2, // width 
            25, // arrow length
            1, // bone space
            90.0f / 180.0f * Mathf.PI, // angle offset 
            LeftHSVStart, LeftHSVEnd, RightHSVStart, // color 
            edge.speedMapTestData, 
            r); 
    }

    //
    List<Vector3> TestData = new List<Vector3>();

    //
    public void showConnections3D()
    {
        //for (int i = 0; i < aoiDataManager.AOIs.Count - 1; i++) 
        //{
        //    drawDotsBetweenTwoPointsOnGround(
        //        aoiDataManager.AOIs[i].transform.position, aoiDataManager.AOIs[i+1].transform.position);
        //}
    }
  
    public void showApproachVisTest()
    {
        resetCanvas();

        drawApproachCurves(new Vector2(1, 1), 1,
            new Vector2(0.2f, -0.2f),
            new Vector2(-0.2f + 0.3f, -0.2f + 0.3f),
            new Vector2(-0.2f, 0.2f),
            new Vector2(0.2f, 0.2f),
            10, 2, Color.green);

        drawApproachCurves(new Vector2(1, 1), 1.1f,
            new Vector2(0.2f - 0.3f, -0.2f + 0.3f),
            new Vector2(-0.2f, -0.2f),
            new Vector2(-0.2f, 0.2f),
            new Vector2(0.2f, 0.2f),
            10, 2, Color.red);

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showApproachVis_dataFromAOI()
    {
        resetCanvas();
        foreach (GameObject fb in aoiDataManager.AOIs)
        {
            Vector3 centeredPosition = new Vector3( 
                fb.transform.Find("AOI").transform.position.x,
                fb.transform.position.y, 
                fb.transform.Find("AOI").transform.position.z);

            drawApproachCurves(centeredPosition, 1,
                new Vector2(0.2f, -0.2f),
                new Vector2(-0.2f + 0.3f, -0.2f + 0.3f),
                new Vector2(-0.2f, 0.2f),
                new Vector2(0.2f, 0.2f),
                10, 2, Color.green);

            drawApproachCurves(centeredPosition, 1.1f,
                new Vector2(0.2f - 0.3f, -0.2f + 0.3f),
                new Vector2(-0.2f, -0.2f),
                new Vector2(-0.2f, 0.2f),
                new Vector2(0.2f, 0.2f),
                10, 2, Color.red);
        }
        submitCurrentTexture();
        saveVisMapToDisk();
    }

    public void showFlow_ArrowPatternTest()
    {
        resetCanvas();
        drawArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(1, 0, 1), new Vector3(2, 0, 2), true);
        drawArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(2, 0, 2), new Vector3(3, 0, 1), false);
        drawArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(3, 0, 1), new Vector3(4, 0, 2), true);
        drawArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(4, 0, 2), new Vector3(3, 0, 4), false);
        drawArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(3, 0, 4), new Vector3(2.5f, 0, 2), true);
        drawArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(2.5f, 0, 2), new Vector3(1.5f, 0, 4), false);
        drawArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(1.5f, 0, 4), new Vector3(1, 0, 1), true);
        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showFlow_ArrowPattern_dataFromAOI()
    {
        resetCanvas();
        for (int i = 0; i < aoiDataManager.AOIs.Count - 1; i++)
        {
            drawArrowPatternFlowBetweenTwoPointsOnVisMap(
                aoiDataManager.AOIs[i].transform.Find("AOI").transform.position,
                aoiDataManager.AOIs[i + 1].transform.Find("AOI").transform.position, i % 2 == 0);
        }
        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showTimeRulerTest()
    {
        resetCanvas();

        drawTimeRulerBetweenTwoPointsOnVisMap(new Vector3(1, 0, 1), new Vector3(2, 0, 2), true);
        drawTimeRulerBetweenTwoPointsOnVisMap(new Vector3(2, 0, 2), new Vector3(3, 0, 1), false);
        drawTimeRulerBetweenTwoPointsOnVisMap(new Vector3(3, 0, 1), new Vector3(4, 0, 2), true);
        drawTimeRulerBetweenTwoPointsOnVisMap(new Vector3(4, 0, 2), new Vector3(3, 0, 4), false);
        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showTimeRuler_dataFromAOI()
    {
        resetCanvas();
        for (int i = 0; i < aoiDataManager.AOIs.Count - 1; i++)
        {
            drawTimeRulerBetweenTwoPointsOnVisMap(
                aoiDataManager.AOIs[i].transform.Find("AOI").transform.position,
                aoiDataManager.AOIs[i + 1].transform.Find("AOI").transform.position, i % 2 == 0);
        }
        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showFlow_BidirectionalTest()
    {
        resetCanvas();

        drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(1, 0, 1), new Vector3(2, 0, 2), true);
        drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(2, 0, 2), new Vector3(3, 0, 1), false);
        drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(3, 0, 1), new Vector3(4, 0, 2), true);
        drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(4, 0, 2), new Vector3(3, 0, 4), false);
        drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(3, 0, 4), new Vector3(2.5f, 0, 2), true);
        drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(2.5f, 0, 2), new Vector3(1.5f, 0, 4), false);
        drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(new Vector3(1.5f, 0, 4), new Vector3(1, 0, 1), true);

        //for (int i = 0;i< TestData.Count - 1; i++)
        //    drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(TestData[i], TestData[i + 1], i % 2 == 0);

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showFlow_Bidirectional_dataFromAOI() {
        resetCanvas();
        for (int i = 0; i < aoiDataManager.AOIs.Count - 1; i++)
        {
            drawBidirectionalArrowPatternFlowBetweenTwoPointsOnVisMap(
                aoiDataManager.AOIs[i].transform.Find("AOI").transform.position,
                aoiDataManager.AOIs[i + 1].transform.Find("AOI").transform.position, i % 2 == 0);
        }
        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showDonutVis()
    {
        resetCanvas();
        int colorIndex = 0;
        foreach (GameObject fb in aoiDataManager.AOIs)
        {
            float randomRadius = Random.Range(80, 120);
            Vector2 center = toTextureCoordinate(fb.transform.Find("AOI").transform.position);
            computeDonutVis(center, randomRadius / 2.0f, randomRadius, 4, 6, colorTable[colorIndex++]);
        }
        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showDotVis()
    {
        resetCanvas();
        int colorIndex = 0;
        foreach (GameObject fb in aoiDataManager.AOIs)
        {
            float randomRadius = Random.Range(80, 120);
            Vector2 center = toTextureCoordinate(fb.transform.Find("AOI").transform.position);
            computeDotVis(center, randomRadius, 4, 6, colorTable[colorIndex++]);
        }
        submitCurrentTexture();
        saveVisMapToDisk();
    }

    //
    public void AOIFilmingMode()
    {
        foreach(var aoi in aoiDataManager.AOIs)
        {
            aoi.transform.Find("AOI").GetComponent<AOIApperancemanager>().applyVis(false, false, null, false, false, false, false);
        }
    }

    //
    public void showFlow_FishBoneTest()
    {
        resetCanvas();

        G.resetGraph();
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[1 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[2 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[3 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[4 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[6 - 1]));


        G.insertAOIEdge(new AOIEdge(Referents[4 - 1], Referents[6 - 1]));

        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[6 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[6 - 1], Referents[5 - 1]));

        G.insertAOIEdge(new AOIEdge(Referents[2 - 1], Referents[5 - 1]));

        //
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[6 - 1], 1));
        G.insertAOIEdge(new AOIEdge(Referents[4 - 1], Referents[6 - 1], 1));
        foreach (var e in G.AOIEdges)
            drawFishBoneFlowBetweenTwoPointsOnVisMap(e);

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showFlow_FishBone_dataFromAOI()
    {
        //resetCanvas();
        //for (int i = 0; i < aoiDataManager.AOIs.Count - 1; i++) {
        //    drawFishBoneFlowBetweenTwoPointsOnVisMap(
        //        aoiDataManager.AOIs[i].transform.Find("AOI").transform.position,
        //        aoiDataManager.AOIs[i+1].transform.Find("AOI").transform.position, i%2 == 0);
        //}
        ////resetPixelsAccordingToAOi();
        //submitCurrentTexture();
        //saveVisMapToDisk();
    }

    //
    public void showFlow_DoubleArrow_Test()
    {
        resetCanvas();

        G.resetGraph();
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[1 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[2 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[3 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[4 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[6 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[4 - 1], Referents[6 - 1]));
        foreach (var e in G.AOIEdges)
            drawDoubleArrowPatternFlowBetweenTwoPointsOnVisMap(e, Random.Range(0.05f, 0.95f));

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showFlow_DoubleArrow_dataFromAOI()
    {
        resetCanvas();
        //for (int i = 0; i < aoiDataManager.AOIs.Count - 1; i++)
        //{
        //    drawDoubleArrowPatternFlowBetweenTwoPointsOnVisMap(
        //        aoiDataManager.AOIs[i].transform.Find("AOI").transform.position,
        //        aoiDataManager.AOIs[i + 1].transform.Find("AOI").transform.position, i % 2 == 0);
        //}
        submitCurrentTexture();
        saveVisMapToDisk();
    }

    //
    public void showFlow_PaceView_Test()
    {
        resetCanvas();

        G.resetGraph();
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[1 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[2 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[3 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[4 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[6 - 1]));
        G.insertAOIEdge(new AOIEdge(Referents[4 - 1], Referents[6 - 1]));
        foreach (var e in G.AOIEdges)
            drawPaceViewBetweenTwoPointsOnVisMap(e, Random.Range(0.2f, 0.8f));

        submitCurrentTexture();
        saveVisMapToDisk();     
    }
    public void showFlow_PaceView_DataFromAOI()
    {
        resetCanvas();

        //buildGraph_Test();
        //foreach (var e in G.edges)
        //    drawFishBoneFlowBetweenTwoPointsOnVisMap(e);

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showFlow_PaceView_DataFromAOIAndTraj()
    {
        resetCanvas();

        //buildGraph_Test();
        //foreach (var e in G.edges)
        //    drawFishBoneFlowBetweenTwoPointsOnVisMap(e);

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    
    public void plotApproachViewGivenCenter(Vector3 center, GroupHeightMap[] heightMap, float maxEnterTime, float pdirDist)
    {
        float numOfDirs = heightMap.Length;
        float angleDelta = (360.0f / numOfDirs) / 180.0f * Mathf.PI;
        float invisibleDist = 0.1f;
        
        for (int i = 0; i < numOfDirs; i++) 
        {
            Vector3 dir = new Vector3(Mathf.Cos(angleDelta * i), 0, Mathf.Sin(angleDelta * i));
            Vector3 pdir = new Vector3(-Mathf.Sin(angleDelta * i), 0, Mathf.Cos(angleDelta * i));
            dir.Normalize();
            pdir.Normalize();

            if(heightMap[i].heightValueByGroup[0] > heightMap[i].heightValueByGroup[1])
            {
                for (int j = 0; j < 100; j++) // 
                {
                    float dirDist = invisibleDist + (float) j / 100 * heightMap[i].heightValueByGroup[0] / maxEnterTime;

                    Vector3 L = center + dirDist * dir + pdir * pdirDist;
                    Vector3 R = center + dirDist * dir - pdir * pdirDist; 

                    Vector3 LLocal = toTextureCoordinate(L);
                    Vector3 RLocal = toTextureCoordinate(R);

                    plotLine(
                        Mathf.RoundToInt(LLocal.x), Mathf.RoundToInt(LLocal.y),
                        Mathf.RoundToInt(RLocal.x), Mathf.RoundToInt(RLocal.y), 2, LeftHSVStart);
                }
                for (int j = 0; j < 100; j++) // 
                {
                    float dirDist = invisibleDist + (float)j / 100 * heightMap[i].heightValueByGroup[1] / maxEnterTime;

                    Vector3 L = center + dirDist * dir + pdir * pdirDist;
                    Vector3 R = center + dirDist * dir - pdir * pdirDist;

                    Vector3 LLocal = toTextureCoordinate(L);
                    Vector3 RLocal = toTextureCoordinate(R);

                    plotLine(
                        Mathf.RoundToInt(LLocal.x), Mathf.RoundToInt(LLocal.y),
                        Mathf.RoundToInt(RLocal.x), Mathf.RoundToInt(RLocal.y), 2, LeftHSVEnd);
                }
            }else{
                for (int j = 0; j < 100; j++) // 
                {
                    float dirDist = invisibleDist + (float)j / 100 * heightMap[i].heightValueByGroup[1] / maxEnterTime;

                    Vector3 L = center + dirDist * dir + pdir * pdirDist;
                    Vector3 R = center + dirDist * dir - pdir * pdirDist;

                    Vector3 LLocal = toTextureCoordinate(L);
                    Vector3 RLocal = toTextureCoordinate(R);

                    plotLine(
                        Mathf.RoundToInt(LLocal.x), Mathf.RoundToInt(LLocal.y),
                        Mathf.RoundToInt(RLocal.x), Mathf.RoundToInt(RLocal.y), 2, LeftHSVEnd);
                }
                for (int j = 0; j < 100; j++) // 
                {
                    float dirDist = invisibleDist + (float)j / 100 * heightMap[i].heightValueByGroup[0] / maxEnterTime;

                    Vector3 L = center + dirDist * dir + pdir * pdirDist;
                    Vector3 R = center + dirDist * dir - pdir * pdirDist;

                    Vector3 LLocal = toTextureCoordinate(L);
                    Vector3 RLocal = toTextureCoordinate(R);

                    plotLine(
                        Mathf.RoundToInt(LLocal.x), Mathf.RoundToInt(LLocal.y),
                        Mathf.RoundToInt(RLocal.x), Mathf.RoundToInt(RLocal.y), 2, LeftHSVStart);
                }
            }


            //plotLine(
            //    Mathf.RoundToInt(centerLocal.x), Mathf.RoundToInt(centerLocal.y), 
            //    Mathf.RoundToInt(targetLocal.x), Mathf.RoundToInt(targetLocal.y), 2, Color.green);
        }

    }
    public float pDist = 0.04f;
    //
    public void showVis_ApproachView_Test()
    {
        resetCanvas();

        //foreach (var refer in Referents){
        //    List<GroupHeightMap> heightMap = new List<GroupHeightMap>();
        //    int numDirs = 24;
        //    for (int d = 0; d < numDirs; d++) // directions 
        //    {
        //        GroupHeightMap ght = new GroupHeightMap();
        //        ght.heightValueByGroup.Add(Random.Range(0.1f, 0.8f));
        //        ght.heightValueByGroup.Add(Random.Range(0.1f, 0.8f));
        //        heightMap.Add(ght);
        //    }
        //    plotApproachViewGivenCenter(refer.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position, heightMap, pDist);
        //}

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showVis_ApproachView_DataFromAOI()
    {
        resetCanvas();

        //buildGraph_Test();
        //foreach (var e in G.edges)
        //    drawFishBoneFlowBetweenTwoPointsOnVisMap(e);

        submitCurrentTexture();
        saveVisMapToDisk();
    }
    public void showVis_ApproachView_DataFromAOIAndTraj()
    {
        resetCanvas();

        //buildGraph_Test();
        //foreach (var e in G.edges)
        //    drawFishBoneFlowBetweenTwoPointsOnVisMap(e);

        submitCurrentTexture();
        saveVisMapToDisk();
    }

}
