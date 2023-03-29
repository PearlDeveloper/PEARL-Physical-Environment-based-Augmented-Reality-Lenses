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
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ComputingTask
{
    SEGMENT, // 0
    FILTER, // 1
    Stacked3DBarChart, // 2
    Situated2DbarChart, // 3
    DoubleArrowView, // 4
    FishBoneView, // 5
    PaceView, // 6
    ApproachView, // 7
    GanttChart, // 8
    NONE // 
}

public class ComputingManager: MonoBehaviour
{
    public GameObject positiveAOIPool;
    public GameObject negativeAOIPool;
    public GameObject ANDGroupedAOIPool;
    public GameObject InactiveAOIPool;
    public GameObject FloorIndicator;

    public GameObject originAnchor;
    public visMapManager mVisMapManager;
    public MRMotionVisManager mRMotionVisManager;
    public heatMapManager heatMapManager;
    public AOIDataManager aOIDataManager;

    public Material transparentMat;
    public Material originalMat;
    public int pointStep = 50;
    public int segPointStep = 1;

    Vis3DTrajectories trajView = null;

    public bool fbAdjustAOIsForCapturing = false;
    public bool fbPutTestAOIsToPools = false;
    public bool fbReasoningThePool = false;
    public bool fbSegAllTrajByAOIs = false;
    public bool fbComputeAndApplyToStacked3DBarChart = false;
    public bool fbComputeAndApply2DbarChart = false;
    public bool fbComputeAndApplyDoubleArrowVis = false;
    public bool fbComputeAndApplyToFishBoneView = false;
    public bool fbComputeAndApplyPaceView = false;
    public bool fbComputeAndAppyApproachView = false;
    public bool fbCountOverallTimeSpan = false;
    public bool fbComputeHeatMap = false;
    public bool fbHideAllTraj = false;
    public bool fbShowAllTraj = false;
    public bool fbShowHeadTraj = false;

    public ComputingTask currComputingTask = ComputingTask.SEGMENT;

    //
    public GameObject LinePrefab;

    public int NG = 9;

    //public Transform Anchor;

    // Start is called before the first frame update
    void Start()
    {
        putAOIsToDatamanagerList();

        //currComputingTask = ComputingTask.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        if (fbPutTestAOIsToPools)
        {
            fbPutTestAOIsToPools = false;
            putTestAOIsToPools();
        }

        if (fbAdjustAOIsForCapturing)
        {
            fbAdjustAOIsForCapturing = false;
            adjustAOIsForCapturing();
        }

        if (fbReasoningThePool)
        {
            fbReasoningThePool = false;
            performComputation((int)ComputingTask.FILTER);
        }

        if (fbSegAllTrajByAOIs)
        {
            fbSegAllTrajByAOIs = false;
            //performComputation((int)ComputingTask.SEGMENT);
            StartCoroutine(segAllTrajByAOIs());
        }

        if (fbComputeHeatMap)
        {
            fbComputeHeatMap = false;
            heatMapManager.computeHeatmap();
        }

        if (fbHideAllTraj)
        {
            fbHideAllTraj = false;
            hideAllTraj();
        }

        if (fbShowAllTraj)
        {
            fbShowAllTraj = false;
            showAllTraj();
        }

        if (fbShowHeadTraj)
        {
            fbShowHeadTraj = false;
            bShowHeadTraj();
        }

        if (fbComputeAndApplyToStacked3DBarChart)
        {
            fbComputeAndApplyToStacked3DBarChart = false;
            performComputation((int)ComputingTask.Stacked3DBarChart);
        }

        if (fbComputeAndApplyDoubleArrowVis)
        {
            fbComputeAndApplyDoubleArrowVis = false;
            performComputation((int)ComputingTask.DoubleArrowView);
        }

        if (fbComputeAndApply2DbarChart)
        {
            fbComputeAndApply2DbarChart = false;
            performComputation((int)ComputingTask.Situated2DbarChart);
        }

        if (fbComputeAndApplyToFishBoneView)
        {
            fbComputeAndApplyToFishBoneView = false;
            performComputation((int)ComputingTask.FishBoneView);
        }

        if (fbComputeAndApplyPaceView)
        {
            fbComputeAndApplyPaceView = false;
            performComputation((int)ComputingTask.PaceView);
        }

        if (fbComputeAndAppyApproachView)
        {
            fbComputeAndAppyApproachView = false;
            performComputation((int)ComputingTask.ApproachView);
        }

        if (fbCountOverallTimeSpan)
        {
            fbCountOverallTimeSpan = false;
            CountOverallTimeSpan();
        }
    }

    bool PointInOABB(Vector3 point, BoxCollider box)
    {
        point = box.transform.InverseTransformPoint(point) - box.center;

        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);
        if (point.x < halfX && point.x > -halfX &&
           point.y < halfY && point.y > -halfY &&
           point.z < halfZ && point.z > -halfZ)
            return true;
        else
            return false;
    }

    public void putAOIsToDatamanagerList()
    {
        //aOIDataManager.AOIs.Add(Referents[0]);
        //aOIDataManager.AOIs.Add(Referents[1]);
        //aOIDataManager.AOIs.Add(Referents[2]);
        //aOIDataManager.AOIs.Add(Referents[3]);
        //aOIDataManager.AOIs.Add(Referents[4]);
        //aOIDataManager.AOIs.Add(Referents[5]);
    }

    public void putTestAOIsToPools()
    {
        aOIDataManager.markAllAOIs4Computing();
    }

    public void adjustAOIsForCapturing() {
        aOIDataManager.disableWireframes();
    }

    public void reasoningThePool()
    {
        if (!findTrajView()) 
            return;

        for (int s = 0; s < trajView.fData.Count; s++) // reset the visibility at the beginning
        {
            for (int d = 0; d < trajView.fData[s].Count; d++)
            {
                trajView.fData[s][d].GetComponent<CustomTubeRenderer>().tubeVisable = false; // reset the visibility varible 
            }
        }
        foreach (Transform ANDLogicAOI in ANDGroupedAOIPool.transform)
        {
            ANDLogicAOI.transform.Find("AOI").GetComponent<PropertyManager>().tubesPassedBy.Clear();
        }
        foreach (Transform positiveAOI in positiveAOIPool.transform)
        {
            positiveAOI.transform.Find("AOI").GetComponent<PropertyManager>().tubesPassedBy.Clear();
        }
        foreach (Transform negativeAOI in negativeAOIPool.transform)
        {
            negativeAOI.transform.Find("AOI").GetComponent<PropertyManager>().tubesPassedBy.Clear();
        }


        //
        if (ANDGroupedAOIPool.transform.childCount > 1) // do AND operation first 
        {
            foreach(Transform ANDLogicAOI in ANDGroupedAOIPool.transform) // collect data 
            {
                for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
                {
                    // 
                    for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                    {
                        for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                        {
                            AnalysisObject dataSet = Services.DataManager().DataSets[d];
                            var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                            for (int pidx = 0; pidx < infoObjects.Count; pidx += pointStep)
                            {
                                Sample o = infoObjects[pidx];

                                BoxCollider AOICollider = ANDLogicAOI.transform.Find("AOI").transform
                                    .Find("AOIIllustrator").GetComponent<BoxCollider>();

                                if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                                {
                                    if(trajView.fData[s][d]!=null)
                                        ANDLogicAOI.transform.Find("AOI").GetComponent<PropertyManager>().tubesPassedBy.Add(trajView.fData[s][d]);
                                    break; // no need to loop further points, go to next tube! 
                                }
                            }
                        }
                    }
                }
            }

            List<GameObject> tubesInCommon = ANDGroupedAOIPool.transform.GetChild(0).transform.Find("AOI").GetComponent<PropertyManager>().tubesPassedBy;
            foreach (Transform ANDLogicAOI in ANDGroupedAOIPool.transform)
            {
                List<GameObject> tubesPassingBy = ANDLogicAOI.transform.Find("AOI").GetComponent<PropertyManager>().tubesPassedBy;
                List<GameObject> newtubesInCommon = tubesInCommon.Intersect(tubesPassingBy).ToList();
                tubesInCommon = newtubesInCommon;
            }
            foreach (GameObject t in tubesInCommon)
                t.GetComponent<CustomTubeRenderer>().tubeVisable = true;
        }

        foreach (Transform positiveAOI in positiveAOIPool.transform) // do OR operations 
        {
            for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
            {
                // 
                for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                {
                    for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                    {
                        AnalysisObject dataSet = Services.DataManager().DataSets[d];
                        var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                        for (int pidx = 0; pidx < infoObjects.Count; pidx += pointStep)
                        {
                            Sample o = infoObjects[pidx];

                            BoxCollider AOICollider = positiveAOI.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();

                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                            {
                                trajView.fData[s][d].GetComponent<CustomTubeRenderer>().tubeVisable = true;
                                break; 
                            }
                        }
                    }
                }
            }
        }

        foreach (Transform negativeAOI in negativeAOIPool.transform) // absolute negation 
        {
            for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
            {
                // 
                for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                {
                    for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                    {
                        AnalysisObject dataSet = Services.DataManager().DataSets[d];
                        var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                        for (int pidx = 0; pidx < infoObjects.Count; pidx += pointStep)
                        {
                            Sample o = infoObjects[pidx];

                            BoxCollider AOICollider = negativeAOI.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();

                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                            {
                                trajView.fData[s][d].GetComponent<CustomTubeRenderer>().tubeVisable = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        for (int s = 0; s < trajView.fData.Count; s++) // reset the visibility at the beginning
        {
            for (int d = 0; d < trajView.fData[s].Count; d++)
            {
                if(trajView.fData[s][d].GetComponent<CustomTubeRenderer>().tubeVisable == true) 
                {
                    // trajView.fData[s][d].SetActive(true);
                    trajView.fData[s][d].GetComponent<MeshRenderer>().material = originalMat;
                    foreach (Transform c in trajView.fData[s][d].transform) // also change its children 
                    {
                        c.gameObject.GetComponent<MeshRenderer>().material = originalMat;
                        c.gameObject.GetComponent<MeshRenderer>().material.color = c.transform.parent.GetComponent<CustomTubeRenderer>().Color;
                    }
                }
                else
                {
                  //  trajView.fData[s][d].SetActive(false);
                    trajView.fData[s][d].GetComponent<MeshRenderer>().material = transparentMat;
                    foreach (Transform c in trajView.fData[s][d].transform) // also change its children 
                    {
                        c.gameObject.GetComponent<MeshRenderer>().material = transparentMat;
                    }
                }

            }
        }
    }

    public bool findTrajView()
    {
        // APIs of MIRIA:
        // InnerLoop, Sessions: Services.VisManager().Visualizations[i].Settings -> Sessions, Conditions 
        // Outer View, Datasets: Services.DataManager().DataSets
        bool found = false;
        foreach (KeyValuePair<Guid, AbstractView> entry in Services.VisManager().Visualizations)
        {
            if (entry.Value.Settings.VisType == VisType.Trajectory3D) // assume we only have one traj shown in MRMA
            {
                trajView = (Vis3DTrajectories)entry.Value;
                found = true;
            }
        }

        if(found)
            Debug.Log("trajView.Settings.Sessions.Count = " + trajView.Settings.Sessions.Count);

        return found;
    }

    List<GameObject> lineSegments = new List<GameObject>();

    public IEnumerator segAllTrajByAOIs()
    {
        if (!findTrajView())
            yield return null;

        foreach(var line in lineSegments)
        {
            Destroy(line);
        }

        List<List<List<List<Vector3>>>> extractedData = new List<List<List<List<Vector3>>>>(trajView.Settings.Sessions.Count); // session, object, seg, point 

        //
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
        {
            //List<List<List<List<int>>>> segmentsInSessions = new List<List<List<List<int>>>>();
            for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
            {
                List<List<List<Vector3>>> segmentsInDataset = new List<List<List<Vector3>>>(Services.DataManager().DataSets.Count);
                int d = 0;
                // for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                {
                    AnalysisObject dataSet = Services.DataManager().DataSets[d];
                    var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                    bool lastPointInAnyAOI = false;
                    bool pointInAnyAOI = false;
                    List<List<Vector3>> segPoints = new List<List<Vector3>>(); // collection of segments of lines 
                    for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep) { // each point 
                        Sample o = infoObjects[pidx];
                        ////////////////////////////////
                        pointInAnyAOI = false;
                        foreach (Transform aoi in positiveAOIPool.transform)
                        {
                            BoxCollider AOICollider = aoi.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                                pointInAnyAOI = true;
                        }
                        foreach (Transform aoi in negativeAOIPool.transform)
                        {
                            BoxCollider AOICollider = aoi.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                                pointInAnyAOI = true;
                        }
                        foreach (Transform aoi in ANDGroupedAOIPool.transform)
                        {
                            BoxCollider AOICollider = aoi.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                                pointInAnyAOI = true;
                        }
                        ////////////////////////////////
                        if ((lastPointInAnyAOI == false) && (pointInAnyAOI == true)) // enter the AOI
                        {
                            segPoints.Add(new List<Vector3>());
                        }
                        if (pointInAnyAOI == true)  // add to list if in Any AOI
                        {
                            segPoints[segPoints.Count - 1].Add(o.Position);
                        }
                        if ((lastPointInAnyAOI == true) && (pointInAnyAOI == false)) // exit the AOI
                        {

                        }
                        ////////////////////////////////
                        lastPointInAnyAOI = pointInAnyAOI;
                    }
                    segmentsInDataset.Add(segPoints);
                }
                extractedData.Add(segmentsInDataset);

                yield return null; // stops here each frame 
            }
        }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("Time Span For Computing: " + elapsedMs + "ms"); 

        //
        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int s = 0; s < extractedData.Count; s++) {
            for (int d = 0; d < extractedData[s].Count; d++)
            {
                var line = Instantiate(LinePrefab, trajView.transform);
                var lineComponent = line.GetComponent<CustomTubeRenderer>();
                lineComponent.Color = trajView.fData[s][d].GetComponent<CustomTubeRenderer>().Color;
                lineComponent.SetPositions(extractedData[s][d]);
                lineSegments.Add(line);
            }
        }
        watch.Stop();
        elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log("Time Span For Remeshing: " + elapsedMs + "ms");

        //
        for (int s = 0; s < trajView.fData.Count; s++) // 
        {
            for (int d = 0; d < trajView.fData[s].Count; d++)
            {
                trajView.fData[s][d].SetActive(false);
            }
        }
    }


    [PunRPC]
    public void hideAllTrajRPC()
    {
        if (!findTrajView())
            return;

        for (int s = 0; s < trajView.fData.Count; s++) // reset the visibility at the beginning
        {
            for (int d = 0; d < trajView.fData[s].Count; d++)
            {
                trajView.fData[s][d].SetActive(false); // reset the visibility varible 
            }
        }
    }

    [PunRPC]
    public void showHeadTrajRPC()
    {
        if (!findTrajView())
            return;

        for (int s = 0; s < trajView.fData.Count; s++) // reset the visibility at the beginning
        {
            for (int d = 0; d < trajView.fData[s].Count; d++)
            {
                trajView.fData[s][d].SetActive(false); // reset the visibility varible 
            }
        }

        for (int s = 0; s < trajView.fData.Count; s++) // reset the visibility at the beginning
        {
            trajView.fData[s][0].SetActive(true); // reset the visibility varible 
        }
    }

    [PunRPC]
    public void showAllTrajRPC()
    {
        if (!findTrajView())
            return;

        for (int s = 0; s < trajView.fData.Count; s++) // reset the visibility at the beginning
        {
            for (int d = 0; d < trajView.fData[s].Count; d++)
            {
                trajView.fData[s][d].SetActive(true); // reset the visibility varible 
            }
        }
    }

    public void hideAllTraj()
    {
        GetComponent<PhotonView>().RPC("hideAllTrajRPC", RpcTarget.All);
    }

    public void showHeadTraj()
    {
        GetComponent<PhotonView>().RPC("showHeadTrajRPC", RpcTarget.All);
    }

    public void showAllTraj()
    {
        GetComponent<PhotonView>().RPC("showAllTrajRPC", RpcTarget.All);
    }

    void bShowHeadTraj()
    {
        if (!findTrajView())
            return;

        for (int s = 0; s < trajView.fData.Count; s++) // reset the visibility at the beginning
        {
            trajView.fData[s][0].SetActive(true); // reset the visibility varible 
            trajView.fData[s][1].SetActive(false); // reset the visibility varible 
            trajView.fData[s][2].SetActive(false); // reset the visibility varible 
        }
    }

    /////////////////////////////////////////////
    ///
    ///////////// -> graph 
    public IEnumerator ComputeAndApplyDoubleArrowVis()
    {
        if (!findTrajView())
            yield return null; // exit here each frame 

        //
        mVisMapManager.resetCanvas();

        if (mVisMapManager.G == null)
            mVisMapManager.G = new Graph();
        mVisMapManager.G.resetGraph();

        for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
        {
            // 
            for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
            {
                //for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                int d = 0;
                {
                    AnalysisObject dataSet = Services.DataManager().DataSets[d];
                    var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                    bool WaitingForTheNextAOI = false;
                    GameObject startAOI = null;
                    GameObject endAOI = null;
                    for (int pidx = 1; pidx < infoObjects.Count; pidx += pointStep)
                    {

                        Sample o = infoObjects[pidx];
                        Vector3 CurrentPoint = infoObjects[pidx].Position;
                        Vector3 LastPoint = infoObjects[pidx - 1].Position;
                        //////////////// Check All AOIs, if any happends ///
                        foreach (Transform aoi in positiveAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    if (s < NG)
                                        e.repeatTimesInGroup[0]++;
                                    if (s >= NG)
                                        e.repeatTimesInGroup[1]++;
                                    mVisMapManager.G.insertAndMergeIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                }
                                break;
                            }
                        }
                        foreach (Transform aoi in negativeAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    if (s < NG)
                                        e.repeatTimesInGroup[0]++;
                                    if (s >= NG)
                                        e.repeatTimesInGroup[1]++;
                                    mVisMapManager.G.insertAndMergeIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                }
                                break;
                            }
                        }
                        foreach (Transform aoi in ANDGroupedAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    if (s < NG)
                                        e.repeatTimesInGroup[0]++;
                                    if (s >= NG)
                                        e.repeatTimesInGroup[1]++;
                                    mVisMapManager.G.insertAndMergeIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            yield return null; // exit here each frame 
        }
       
        foreach (AOIEdge e in mVisMapManager.G.DataEdges)
        {
            //if (e.repeatTimesInGroup[0] > 0 && e.repeatTimesInGroup[1] > 0) 
                mVisMapManager.drawDoubleArrowPatternFlowBetweenTwoPointsOnVisMap(e, 
                    (float)e.repeatTimesInGroup[0] /(e.repeatTimesInGroup[1] + e.repeatTimesInGroup[0] + 1));
        }

        mVisMapManager.submitCurrentTexture();
        mVisMapManager.saveVisMapToDisk();
        mRMotionVisManager.setVis(1);
    }

    public IEnumerator ComputeAndApplyToFishBoneView()
    {
        if (!findTrajView())
            yield return null; // exit here each frame 

        //
        mVisMapManager.resetCanvas();

        if (mVisMapManager.G == null)
            mVisMapManager.G = new Graph();
        mVisMapManager.G.resetGraph();

        for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
        {
            // 
            for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
            {
                // for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                int d = 0;
                {
                    AnalysisObject dataSet = Services.DataManager().DataSets[d];
                    var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                    bool WaitingForTheNextAOI = false;
                    GameObject startAOI = null;
                    GameObject endAOI = null;
                    for (int pidx = 1; pidx < infoObjects.Count; pidx += pointStep)
                    {

                        Sample o = infoObjects[pidx];
                        Vector3 CurrentPoint = infoObjects[pidx].Position;
                        Vector3 LastPoint = infoObjects[pidx - 1].Position;
                        //////////////// Check All AOIs, if any happends ///
                        foreach (Transform aoi in positiveAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    if (s < NG)
                                        e.repeatTimesInGroup[0]++;
                                    if (s >= NG)
                                        e.repeatTimesInGroup[1]++;
                                    mVisMapManager.G.insertAndMergeIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                }
                                break;
                            }
                        }
                        foreach (Transform aoi in negativeAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    if (s < NG)
                                        e.repeatTimesInGroup[0]++;
                                    if (s >= NG)
                                        e.repeatTimesInGroup[1]++;
                                    mVisMapManager.G.insertAndMergeIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                }
                                break;
                            }
                        }
                        foreach (Transform aoi in ANDGroupedAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    if (s < NG)
                                        e.repeatTimesInGroup[0]++;
                                    if (s >= NG)
                                        e.repeatTimesInGroup[1]++;
                                    mVisMapManager.G.insertAndMergeIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            yield return null; // exit here each frame 
        }

        mVisMapManager.G.fromDataEdgeToAOIEdges();  

        foreach (var e in mVisMapManager.G.AOIEdges)
            mVisMapManager.drawFishBoneFlowBetweenTwoPointsOnVisMap(e);

        mVisMapManager.submitCurrentTexture();
        mVisMapManager.saveVisMapToDisk();
        mRMotionVisManager.setVis(1);
    }

    public IEnumerator ComputeAndApplyPaceView()
    {
        if (!findTrajView())
            yield return null; // exit here each frame 

        //
        mVisMapManager.resetCanvas();

        if (mVisMapManager.G == null)
            mVisMapManager.G = new Graph();
        mVisMapManager.G.resetGraph();

        for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
        {
            // 
            for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
            {
                int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                {
                    AnalysisObject dataSet = Services.DataManager().DataSets[d];
                    var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                    bool WaitingForTheNextAOI = false;
                    GameObject startAOI = null;
                    GameObject endAOI = null;
                    float maxSpeedInBetween = 0;
                    int startIndex = -1;
                    int endIndex = -1;
                    int maxSpeedIndex = -1;
                    for (int pidx = 1; pidx < infoObjects.Count; pidx += pointStep)
                    {

                        Sample o = infoObjects[pidx];
                        Vector3 CurrentPoint = infoObjects[pidx].Position;
                        Vector3 LastPoint = infoObjects[pidx - 1].Position;
                        //////////////// Check All AOIs, if any happends ///
                        foreach (Transform aoi in positiveAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false; 
                            if (o.Speed > maxSpeedInBetween)
                            {
                                maxSpeedInBetween = o.Speed;
                                maxSpeedIndex = pidx;
                            }
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startIndex = pidx;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endIndex = pidx;
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    e.highestSpeedPercentage = (float)(maxSpeedIndex - startIndex) / (endIndex - startIndex);
                                    e.someTrajFound = true;
                                    mVisMapManager.G.insertAndMergePercentageIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                    maxSpeedInBetween = 0;
                                    startIndex = -1;
                                    endIndex = -1;
                                    maxSpeedIndex = -1;
                                }
                                break;
                            }
                        }
                        foreach (Transform aoi in negativeAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            if (o.Speed > maxSpeedInBetween)
                            {
                                maxSpeedInBetween = o.Speed;
                                maxSpeedIndex = pidx;
                            }
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startIndex = pidx;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endIndex = pidx;
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    e.highestSpeedPercentage = (float)(maxSpeedIndex - startIndex) / (endIndex - startIndex);
                                    e.someTrajFound = true;
                                    mVisMapManager.G.insertAndMergePercentageIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                    maxSpeedInBetween = 0;
                                    startIndex = -1;
                                    endIndex = -1;
                                    maxSpeedIndex = -1;
                                }
                                break;
                            }
                        }
                        foreach (Transform aoi in ANDGroupedAOIPool.transform)
                        {
                            bool CurrentPointInAOI = false;
                            bool LastPointInAOI = false;
                            if (o.Speed > maxSpeedInBetween)
                            {
                                maxSpeedInBetween = o.Speed;
                                maxSpeedIndex = pidx;
                            }
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(CurrentPoint), AOICollider))
                                CurrentPointInAOI = true;
                            if (PointInOABB(trajView.transform.TransformPoint(LastPoint), AOICollider))
                                LastPointInAOI = true;
                            if (LastPointInAOI == true && CurrentPointInAOI == false)
                            {
                                WaitingForTheNextAOI = true;
                                startIndex = pidx;
                                startAOI = aoi.gameObject;
                                break;
                            }
                            if (LastPointInAOI == false && CurrentPointInAOI == true)
                            {
                                endIndex = pidx;
                                endAOI = aoi.gameObject;
                                if (WaitingForTheNextAOI && (startAOI != endAOI))
                                {
                                    // Submit here, startAOI and endAOI can not be null
                                    AOIEdge e = new AOIEdge(startAOI, endAOI);
                                    e.offsetL = 0.2f;
                                    e.highestSpeedPercentage = (float)(maxSpeedIndex - startIndex) / (endIndex - startIndex);
                                    e.someTrajFound = true;
                                    mVisMapManager.G.insertAndMergePercentageIfExists(e);

                                    WaitingForTheNextAOI = false;
                                    startAOI = null;
                                    endAOI = null;
                                    maxSpeedInBetween = 0;
                                    startIndex = -1;
                                    endIndex = -1;
                                    maxSpeedIndex = -1;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            yield return null; // exit here each frame 
        }



        //mVisMapManager.resetCanvas();

        //if (mVisMapManager.G == null)
        //    mVisMapManager.G = new Graph();
        //mVisMapManager.G.resetGraph();
        //mVisMapManager.G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[1 - 1]));
        //mVisMapManager.G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[2 - 1]));
        //mVisMapManager.G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[3 - 1]));
        //mVisMapManager.G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[4 - 1]));
        //mVisMapManager.G.insertAOIEdge(new AOIEdge(Referents[5 - 1], Referents[6 - 1]));
        //mVisMapManager.G.insertAOIEdge(new AOIEdge(Referents[4 - 1], Referents[6 - 1]));
        foreach (var e in mVisMapManager.G.DataEdges)
        {
            if(e.someTrajFound)
                mVisMapManager.drawPaceViewBetweenTwoPointsOnVisMap(e, e.highestSpeedPercentage);
        }

        mVisMapManager.submitCurrentTexture();
        mVisMapManager.saveVisMapToDisk();
        mRMotionVisManager.setVis(1);
    }

    ////////// -> AOI 
    public void ComputeAndApplyToStacked3DBarChart()
    {
        if(!findTrajView())
            return;

        //
        foreach (Transform aoi in positiveAOIPool.transform) {
            aoi.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup = new int[2];
            aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0] = 0;
            aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1] = 0;
            for (int s = 0; s < trajView.Settings.Sessions.Count; s++) { 
                for (int c = 0; c < trajView.Settings.Conditions.Count; c++) {
                    int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
                    { 
                        AnalysisObject dataSet = Services.DataManager().DataSets[d];
                        var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                        bool AnyPointInAOI = false;
                        for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
                        {
                            Sample o = infoObjects[pidx];
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                            {
                                AnyPointInAOI = true;
                                break;
                            }
                        }
                        if (AnyPointInAOI)
                        {
                            if (s < NG) 
                            {
                                aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0]++;
                            }
                            if (s >= NG) // 
                            {
                                aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1]++;
                            }
                        }
                    }
                }        
            }        
        }
        foreach (Transform aoi in positiveAOIPool.transform)
        {
            GameObject visObject = aoi.transform.Find("AOI").GetComponent<AOIVisControl>().visList[3];
            visObject.transform.position = new Vector3(
                visObject.transform.position.x, FloorIndicator.transform.position.y, visObject.transform.position.z);
            List<float> heights = new List<float>();
            List<string> texts = new List<string>();
            heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0] * 0.1f);
            heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1] * 0.1f);
            texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0].ToString());
            texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1].ToString());
            visObject.GetComponent<Stacked3DBarController>().updateData(heights, texts);
            //aoi.transform.Find("AOI").GetComponent<AOIVisControl>().show3DBarChart();
            visObject.SetActive(true);
        }

        ////
        //foreach (Transform aoi in negativeAOIPool.transform)
        //{
        //    aoi.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup = new int[3];
        //    for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
        //    {
        //        for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
        //        {
        //            int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
        //            {
        //                AnalysisObject dataSet = Services.DataManager().DataSets[d];
        //                var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
        //                bool AnyPointInAOI = false;
        //                for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
        //                {
        //                    Sample o = infoObjects[pidx];
        //                    BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
        //                    if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
        //                    {
        //                        AnyPointInAOI = true;
        //                        break;
        //                    }
        //                }
        //                if (AnyPointInAOI)
        //                {
        //                    if (s == 0 || s == 1) // user 0,1 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0]++;
        //                    }
        //                    if (s == 1 || s == 2) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1]++;
        //                    }
        //                    if (s == 3 || s == 4) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2]++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //foreach (Transform aoi in negativeAOIPool.transform)
        //{
        //    GameObject visObject = aoi.transform.Find("AOI").GetComponent<AOIVisControl>().visList[3];
        //    visObject.transform.position = new Vector3(
        //        visObject.transform.position.x, FloorIndicator.transform.position.y, visObject.transform.position.z);
        //    List<float> heights = new List<float>();
        //    List<string> texts = new List<string>();
        //    heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0] * 0.1f);
        //    heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1] * 0.1f);
        //    heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2] * 0.1f);
        //    texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0].ToString());
        //    texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1].ToString());
        //    texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2].ToString());
        //    visObject.GetComponent<Stacked3DBarController>().updateData(heights, texts);
        //    visObject.SetActive(true);
        //}

        ////
        //foreach (Transform aoi in ANDGroupedAOIPool.transform)
        //{
        //    aoi.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup = new int[3];
        //    for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
        //    {
        //        for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
        //        {
        //            int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
        //            {
        //                AnalysisObject dataSet = Services.DataManager().DataSets[d];
        //                var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
        //                bool AnyPointInAOI = false;
        //                for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
        //                {
        //                    Sample o = infoObjects[pidx];
        //                    BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
        //                    if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
        //                    {
        //                        AnyPointInAOI = true;
        //                        break;
        //                    }
        //                }
        //                if (AnyPointInAOI)
        //                {
        //                    if (s == 0 || s == 1) // user 0,1 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0]++;
        //                    }
        //                    if (s == 1 || s == 2) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1]++;
        //                    }
        //                    if (s == 3 || s == 4) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2]++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //foreach (Transform aoi in ANDGroupedAOIPool.transform)
        //{
        //    GameObject visObject = aoi.transform.Find("AOI").GetComponent<AOIVisControl>().visList[3];
        //    visObject.transform.position = new Vector3(
        //        visObject.transform.position.x, FloorIndicator.transform.position.y, visObject.transform.position.z);
        //    List<float> heights = new List<float>();
        //    List<string> texts = new List<string>();
        //    heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0] * 0.1f);
        //    heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1] * 0.1f);
        //    heights.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2] * 0.1f);
        //    texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0].ToString());
        //    texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1].ToString());
        //    texts.Add(aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2].ToString());
        //    visObject.GetComponent<Stacked3DBarController>().updateData(heights, texts);
        //    visObject.SetActive(true);
        //}
    }

    public void ComputeAndApply2DbarChart()
    {
        if (!findTrajView())
            return;

        ////
        //foreach (Transform aoi in positiveAOIPool.transform)
        //{
        //    aoi.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup = new int[3];
        //    for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
        //    {
        //        for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
        //        {
        //            int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
        //            {
        //                AnalysisObject dataSet = Services.DataManager().DataSets[d];
        //                var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
        //                bool AnyPointInAOI = false;
        //                for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
        //                {
        //                    Sample o = infoObjects[pidx];
        //                    BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
        //                    if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
        //                    {
        //                        AnyPointInAOI = true;
        //                        break;
        //                    }
        //                }
        //                if (AnyPointInAOI)
        //                {
        //                    if (s == 0 || s == 1) // user 0,1 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0]++;
        //                    }
        //                    if (s == 1 || s == 2) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1]++;
        //                    }
        //                    if (s == 3 || s == 4) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2]++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //
        int id = 0;
        foreach (Transform aoi in positiveAOIPool.transform)
        {
            id++;
            if (id == 1) aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization_OffPeakHours();
            if (id == 4) aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization_OffPeakHours();
            if (id == 5) aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization_OffPeakHours();
            if (id == 2) aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization_PeakHours();
            if (id == 3) aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization_PeakHours();
            if (id == 6) aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization_PeakHours();
            //int nChildren = Random.Range(1, 5);
            //int nTeenagers = Random.Range(1, 5);
            //int nAdults = 10 - nChildren - nTeenagers;
            //aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization(nChildren,nTeenagers,nAdults);

            //aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization(
            //    aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0] + 1,
            //    aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1] + 1,
            //    aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2] + 1);
            aoi.transform.Find("AOI").GetComponent<AOIVisControl>().show2DBarChart();
        }
        ////
        //foreach (Transform aoi in negativeAOIPool.transform)
        //{
        //    aoi.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup = new int[3];
        //    for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
        //    {
        //        for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
        //        {
        //            int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
        //            {
        //                AnalysisObject dataSet = Services.DataManager().DataSets[d];
        //                var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
        //                bool AnyPointInAOI = false;
        //                for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
        //                {
        //                    Sample o = infoObjects[pidx];
        //                    BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
        //                    if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
        //                    {
        //                        AnyPointInAOI = true;
        //                        break;
        //                    }
        //                }
        //                if (AnyPointInAOI)
        //                {
        //                    if (s == 0 || s == 1) // user 0,1 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0]++;
        //                    }
        //                    if (s == 1 || s == 2) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1]++;
        //                    }
        //                    if (s == 3 || s == 4) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2]++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ////
        //foreach (Transform aoi in negativeAOIPool.transform)
        //{
        //    //int nChildren = Random.Range(1, 5);
        //    //int nTeenagers = Random.Range(1, 5);
        //    //int nAdults = 10 - nChildren - nTeenagers;
        //    //aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization(nChildren,nTeenagers,nAdults);

        //    aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization(
        //        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0] + 1,
        //        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1] + 1,
        //        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2] + 1);
        //    aoi.transform.Find("AOI").GetComponent<AOIVisControl>().show2DBarChart();
        //}
        ////
        //foreach (Transform aoi in ANDGroupedAOIPool.transform)
        //{
        //    aoi.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup = new int[3];
        //    for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
        //    {
        //        for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
        //        {
        //            int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
        //            {
        //                AnalysisObject dataSet = Services.DataManager().DataSets[d];
        //                var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
        //                bool AnyPointInAOI = false;
        //                for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
        //                {
        //                    Sample o = infoObjects[pidx];
        //                    BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
        //                    if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
        //                    {
        //                        AnyPointInAOI = true;
        //                        break;
        //                    }
        //                }
        //                if (AnyPointInAOI)
        //                {
        //                    if (s == 0 || s == 1) // user 0,1 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0]++;
        //                    }
        //                    if (s == 1 || s == 2) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1]++;
        //                    }
        //                    if (s == 3 || s == 4) // 
        //                    {
        //                        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2]++;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ////
        //foreach (Transform aoi in ANDGroupedAOIPool.transform)
        //{
        //    //int nChildren = Random.Range(1, 5);
        //    //int nTeenagers = Random.Range(1, 5);
        //    //int nAdults = 10 - nChildren - nTeenagers;
        //    //aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization(nChildren,nTeenagers,nAdults);

        //    aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateDataVisualization(
        //        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[0] + 1,
        //        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[1] + 1,
        //        aoi.transform.Find("AOI").GetComponent<PropertyManager>().numPeoplePassedByForEachGroup[2] + 1);
        //    aoi.transform.Find("AOI").GetComponent<AOIVisControl>().show2DBarChart();
        //}
    }

    public IEnumerator ComputeAndAppyApproachView()
    {
        if (!findTrajView())
            yield return null;
        int numDirs = 24;
        int maxEnterTime = 0;
        float pDist = 0.04f;
        mVisMapManager.resetCanvas();

        //
        maxEnterTime = 0;
        foreach (Transform aoi in positiveAOIPool.transform)
        {
            for (int c = 0; c < numDirs; c++)
            {
                aoi.Find("AOI").GetComponent<PropertyManager>().heightMap[c].heightValueByGroup[0] = 0;
                aoi.Find("AOI").GetComponent<PropertyManager>().heightMap[c].heightValueByGroup[1] = 0;
            }
        }
        foreach (Transform aoi in positiveAOIPool.transform)
        {
            for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
            {
                for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                {
                    int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
                    {
                        AnalysisObject dataSet = Services.DataManager().DataSets[d];
                        var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                        bool AnyPointInAOI = false;
                        bool pointInAOI = false;
                        bool lastPointInAOI = false;
                        for (int pidx = 0; pidx < infoObjects.Count - 1; pidx += segPointStep)
                        {
                            Sample o = infoObjects[pidx];
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            ////////////////////
                            pointInAOI = false;
                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                            {
                                AnyPointInAOI = true;
                                pointInAOI = true;
                            }
                            ////////////////////
                            if ((lastPointInAOI == false) && (pointInAOI == true))
                            {
                                Vector3 approachDir = trajView.transform.TransformPoint(o.Position) - aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position;
                                approachDir.Normalize();
                                float angle = Vector3.SignedAngle(approachDir, new Vector3(1, 0, 0), new Vector3(0, 1, 0));
                                if (angle < 0)
                                    angle = 360.0f + angle;
                                float angleDelta = 360.0f / numDirs;
                                int category = (int) (angle / angleDelta);
                                if (s < NG)
                                {
                                    aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0]++;
                                    if (aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0] > maxEnterTime)
                                        maxEnterTime = aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0];
                                }
                                if (s >= NG)
                                {
                                    aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1]++;
                                    if (aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1] > maxEnterTime)
                                        maxEnterTime = aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1];
                                }

                            }
                            lastPointInAOI = pointInAOI;
                        }
                    }
                }
            }
        }

        //
        yield return null; // exit here each frame 

        Debug.Log("maxEnterTime = " + maxEnterTime);
        foreach (Transform aoi in positiveAOIPool.transform)
        {
            mVisMapManager.plotApproachViewGivenCenter(
                aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position,
                aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap,(float)maxEnterTime, pDist);
        }

        //
        maxEnterTime = 0;
        foreach (Transform aoi in negativeAOIPool.transform)
        {
            for (int c = 0; c < numDirs; c++)
            {
                aoi.Find("AOI").GetComponent<PropertyManager>().heightMap[c].heightValueByGroup[0] = 0;
                aoi.Find("AOI").GetComponent<PropertyManager>().heightMap[c].heightValueByGroup[1] = 0;
            }
        }

        //
        yield return null; // exit here each frame 

        foreach (Transform aoi in negativeAOIPool.transform)
        {
            for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
            {
                for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                {
                    int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
                    {
                        AnalysisObject dataSet = Services.DataManager().DataSets[d];
                        var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                        bool AnyPointInAOI = false;
                        bool pointInAOI = false;
                        bool lastPointInAOI = false;
                        for (int pidx = 0; pidx < infoObjects.Count - 1; pidx += segPointStep)
                        {
                            Sample o = infoObjects[pidx];
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            ////////////////////
                            pointInAOI = false;
                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                            {
                                AnyPointInAOI = true;
                                pointInAOI = true;
                            }
                            ////////////////////
                            if ((lastPointInAOI == false) && (pointInAOI == true))
                            {
                                Vector3 approachDir = trajView.transform.TransformPoint(o.Position) - aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position;
                                approachDir.Normalize();
                                float angle = Vector3.SignedAngle(approachDir, new Vector3(1, 0, 0), new Vector3(0, 1, 0));
                                if (angle < 0)
                                    angle = 360.0f + angle;
                                float angleDelta = 360.0f / numDirs;
                                int category = (int)(angle / angleDelta);
                                if (s < NG)
                                {
                                    aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0]++;
                                    if (aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0] > maxEnterTime)
                                        maxEnterTime = aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0];
                                }
                                if (s >= NG)
                                {
                                    aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1]++;
                                    if (aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1] > maxEnterTime)
                                        maxEnterTime = aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1];
                                }

                            }
                            lastPointInAOI = pointInAOI;
                        }
                    }
                }
            }
        }
        Debug.Log("maxEnterTime = " + maxEnterTime);
        foreach (Transform aoi in negativeAOIPool.transform)
        {
            mVisMapManager.plotApproachViewGivenCenter(
                aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position,
                aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap, (float)maxEnterTime, pDist);
        }

        //
        maxEnterTime = 0;
        foreach (Transform aoi in ANDGroupedAOIPool.transform)
        {
            for (int c = 0; c < numDirs; c++)
            {
                aoi.Find("AOI").GetComponent<PropertyManager>().heightMap[c].heightValueByGroup[0] = 0;
                aoi.Find("AOI").GetComponent<PropertyManager>().heightMap[c].heightValueByGroup[1] = 0;
            }
        }
        foreach (Transform aoi in ANDGroupedAOIPool.transform)
        {
            for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
            {
                for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                {
                    int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++)
                    {
                        AnalysisObject dataSet = Services.DataManager().DataSets[d];
                        var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                        bool AnyPointInAOI = false;
                        bool pointInAOI = false;
                        bool lastPointInAOI = false;
                        for (int pidx = 0; pidx < infoObjects.Count - 1; pidx += segPointStep)
                        {
                            Sample o = infoObjects[pidx];
                            BoxCollider AOICollider = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                            ////////////////////
                            pointInAOI = false;
                            if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                            {
                                AnyPointInAOI = true;
                                pointInAOI = true;
                            }
                            ////////////////////
                            if ((lastPointInAOI == false) && (pointInAOI == true))
                            {
                                Vector3 approachDir = trajView.transform.TransformPoint(o.Position) - aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position;
                                approachDir.Normalize();
                                float angle = Vector3.SignedAngle(approachDir, new Vector3(1, 0, 0), new Vector3(0, 1, 0));
                                if (angle < 0)
                                    angle = 360.0f + angle;
                                float angleDelta = 360.0f / numDirs;
                                int category = (int)(angle / angleDelta);
                                if (s < NG)
                                {
                                    aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0]++;
                                    if (aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0] > maxEnterTime)
                                        maxEnterTime = aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[0];
                                }
                                if (s >= NG)
                                {
                                    aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1]++;
                                    if (aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1] > maxEnterTime)
                                        maxEnterTime = aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap[category].heightValueByGroup[1];
                                }

                            }
                            lastPointInAOI = pointInAOI;
                        }
                    }
                }
            }
        }
        Debug.Log("maxEnterTime = " + maxEnterTime); 
        foreach (Transform aoi in ANDGroupedAOIPool.transform)
        {
            mVisMapManager.plotApproachViewGivenCenter(
                aoi.transform.Find("AOI").transform.Find("AOIIllustrator").transform.position,
                aoi.transform.Find("AOI").GetComponent<PropertyManager>().heightMap, (float)maxEnterTime, pDist);
        }

        //
        yield return null; // exit here each frame 

        //
        mVisMapManager.submitCurrentTexture();
        mVisMapManager.saveVisMapToDisk();
        mRMotionVisManager.setVis(1);
    }

    public void ComputeAndApply2DGanttChart()
    {
        if (!findTrajView())
            return;

        foreach (Transform aoi in positiveAOIPool.transform)
            aoi.transform.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
        foreach (Transform aoi in negativeAOIPool.transform)
            aoi.transform.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
        foreach (Transform aoi in ANDGroupedAOIPool.transform)
            aoi.transform.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();

        if (false)
        {
            List<float> xv = new List<float>();
            List<float> wv = new List<float>();

            xv.Add(0.5f);
            xv.Add(0.5f);
            xv.Add(0.5f);
            xv.Add(0.5f);
            xv.Add(0.5f);
            xv.Add(0.5f);

            wv.Add(0.5f);
            wv.Add(0.5f);
            wv.Add(0.5f);
            wv.Add(0.5f);
            wv.Add(0.5f);
            wv.Add(0.5f);
            //
            foreach (Transform aoi in positiveAOIPool.transform)
            {
                List<List<List<List<Vector3>>>> extractedData = new List<List<List<List<Vector3>>>>(trajView.Settings.Sessions.Count); // session, object, seg, point
                List<List<List<List<int>>>> extractedDataIdx = new List<List<List<List<int>>>>(trajView.Settings.Sessions.Count);
                for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
                {
                    //List<List<List<List<int>>>> segmentsInSessions = new List<List<List<List<int>>>>();
                    for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                    {
                        List<List<List<Vector3>>> segmentsInDataset = new List<List<List<Vector3>>>(Services.DataManager().DataSets.Count);
                        List<List<List<int>>> segmentsInDatasetIdx = new List<List<List<int>>>(Services.DataManager().DataSets.Count);
                        int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                        {
                            AnalysisObject dataSet = Services.DataManager().DataSets[d];
                            var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                            bool lastPointInAnyAOI = false;
                            bool pointInAnyAOI = false;
                            List<List<Vector3>> segPoints = new List<List<Vector3>>(); // collection of segments of lines 
                            List<List<int>> segPointsIdx = new List<List<int>>(); // collection of segments of lines 
                            for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
                            { // each point 
                                Sample o = infoObjects[pidx];
                                ////////////////////////////////
                                pointInAnyAOI = false;

                                BoxCollider AOICollider = aoi.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                                if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                                    pointInAnyAOI = true;
                                ////////////////////////////////
                                if ((lastPointInAnyAOI == false) && (pointInAnyAOI == true)) // enter the AOI
                                {
                                    segPoints.Add(new List<Vector3>());
                                    segPointsIdx.Add(new List<int>());
                                }
                                if (pointInAnyAOI == true)  // add to list if in Any AOI
                                {
                                    segPoints[segPoints.Count - 1].Add(o.Position);
                                    segPointsIdx[segPointsIdx.Count - 1].Add(pidx);
                                }
                                if ((lastPointInAnyAOI == true) && (pointInAnyAOI == false)) // exit the AOI
                                {

                                }
                                ////////////////////////////////
                                lastPointInAnyAOI = pointInAnyAOI;
                            }
                            segmentsInDataset.Add(segPoints);
                            segmentsInDatasetIdx.Add(segPointsIdx);
                        }
                        extractedData.Add(segmentsInDataset);
                        extractedDataIdx.Add(segmentsInDatasetIdx);
                    }
                }

                for (int i = 0; i < wv.Count; i++)
                    wv[i] = (float)extractedData[i][0].Count / trajView.fData[i][0].GetComponent<CustomTubeRenderer>().numOfPoints;
                for (int i = 0; i < xv.Count && extractedDataIdx[i][0].Count > 0 && extractedDataIdx[i][0][0].Count > 0; i++)
                    xv[i] = (float)extractedDataIdx[i][0][0][0] / trajView.fData[i][0].GetComponent<CustomTubeRenderer>().numOfPoints;

                aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateGanttChart(xv, wv);
                aoi.transform.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
            }
            //
            foreach (Transform aoi in negativeAOIPool.transform)
            {
                List<List<List<List<Vector3>>>> extractedData = new List<List<List<List<Vector3>>>>(trajView.Settings.Sessions.Count); // session, object, seg, point
                List<List<List<List<int>>>> extractedDataIdx = new List<List<List<List<int>>>>(trajView.Settings.Sessions.Count);
                for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
                {
                    //List<List<List<List<int>>>> segmentsInSessions = new List<List<List<List<int>>>>();
                    for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                    {
                        List<List<List<Vector3>>> segmentsInDataset = new List<List<List<Vector3>>>(Services.DataManager().DataSets.Count);
                        List<List<List<int>>> segmentsInDatasetIdx = new List<List<List<int>>>(Services.DataManager().DataSets.Count);
                        int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                        {
                            AnalysisObject dataSet = Services.DataManager().DataSets[d];
                            var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                            bool lastPointInAnyAOI = false;
                            bool pointInAnyAOI = false;
                            List<List<Vector3>> segPoints = new List<List<Vector3>>(); // collection of segments of lines 
                            List<List<int>> segPointsIdx = new List<List<int>>(); // collection of segments of lines 
                            for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
                            { // each point 
                                Sample o = infoObjects[pidx];
                                ////////////////////////////////
                                pointInAnyAOI = false;

                                BoxCollider AOICollider = aoi.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                                if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                                    pointInAnyAOI = true;
                                ////////////////////////////////
                                if ((lastPointInAnyAOI == false) && (pointInAnyAOI == true)) // enter the AOI
                                {
                                    segPoints.Add(new List<Vector3>());
                                    segPointsIdx.Add(new List<int>());
                                }
                                if (pointInAnyAOI == true)  // add to list if in Any AOI
                                {
                                    segPoints[segPoints.Count - 1].Add(o.Position);
                                    segPointsIdx[segPointsIdx.Count - 1].Add(pidx);
                                }
                                if ((lastPointInAnyAOI == true) && (pointInAnyAOI == false)) // exit the AOI
                                {

                                }
                                ////////////////////////////////
                                lastPointInAnyAOI = pointInAnyAOI;
                            }
                            segmentsInDataset.Add(segPoints);
                            segmentsInDatasetIdx.Add(segPointsIdx);
                        }
                        extractedData.Add(segmentsInDataset);
                        extractedDataIdx.Add(segmentsInDatasetIdx);
                    }
                }

                for (int i = 0; i < wv.Count; i++)
                    wv[i] = (float)extractedData[i][0].Count / trajView.fData[i][0].GetComponent<CustomTubeRenderer>().numOfPoints;
                for (int i = 0; i < xv.Count; i++)
                    xv[i] = (float)extractedDataIdx[i][0][0][0] / trajView.fData[i][0].GetComponent<CustomTubeRenderer>().numOfPoints;

                aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateGanttChart(xv, wv);
                aoi.transform.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
            }
            //
            foreach (Transform aoi in ANDGroupedAOIPool.transform)
            {
                List<List<List<List<Vector3>>>> extractedData = new List<List<List<List<Vector3>>>>(trajView.Settings.Sessions.Count); // session, object, seg, point
                List<List<List<List<int>>>> extractedDataIdx = new List<List<List<List<int>>>>(trajView.Settings.Sessions.Count);
                for (int s = 0; s < trajView.Settings.Sessions.Count; s++) // for each session, people 
                {
                    //List<List<List<List<int>>>> segmentsInSessions = new List<List<List<List<int>>>>();
                    for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
                    {
                        List<List<List<Vector3>>> segmentsInDataset = new List<List<List<Vector3>>>(Services.DataManager().DataSets.Count);
                        List<List<List<int>>> segmentsInDatasetIdx = new List<List<List<int>>>(Services.DataManager().DataSets.Count);
                        int d = 0; // for (int d = 0; d < Services.DataManager().DataSets.Count; d++) // for each object like head, hands 
                        {
                            AnalysisObject dataSet = Services.DataManager().DataSets[d];
                            var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                            bool lastPointInAnyAOI = false;
                            bool pointInAnyAOI = false;
                            List<List<Vector3>> segPoints = new List<List<Vector3>>(); // collection of segments of lines 
                            List<List<int>> segPointsIdx = new List<List<int>>(); // collection of segments of lines 
                            for (int pidx = 0; pidx < infoObjects.Count; pidx += segPointStep)
                            { // each point 
                                Sample o = infoObjects[pidx];
                                ////////////////////////////////
                                pointInAnyAOI = false;

                                BoxCollider AOICollider = aoi.Find("AOI").transform.Find("AOIIllustrator").GetComponent<BoxCollider>();
                                if (PointInOABB(trajView.transform.TransformPoint(o.Position), AOICollider))
                                    pointInAnyAOI = true;
                                ////////////////////////////////
                                if ((lastPointInAnyAOI == false) && (pointInAnyAOI == true)) // enter the AOI
                                {
                                    segPoints.Add(new List<Vector3>());
                                    segPointsIdx.Add(new List<int>());
                                }
                                if (pointInAnyAOI == true)  // add to list if in Any AOI
                                {
                                    segPoints[segPoints.Count - 1].Add(o.Position);
                                    segPointsIdx[segPointsIdx.Count - 1].Add(pidx);
                                }
                                if ((lastPointInAnyAOI == true) && (pointInAnyAOI == false)) // exit the AOI
                                {

                                }
                                ////////////////////////////////
                                lastPointInAnyAOI = pointInAnyAOI;
                            }
                            segmentsInDataset.Add(segPoints);
                            segmentsInDatasetIdx.Add(segPointsIdx);
                        }
                        extractedData.Add(segmentsInDataset);
                        extractedDataIdx.Add(segmentsInDatasetIdx);
                    }
                }

                for (int i = 0; i < wv.Count; i++)
                    wv[i] = (float)extractedData[i][0].Count / trajView.fData[i][0].GetComponent<CustomTubeRenderer>().numOfPoints;
                for (int i = 0; i < xv.Count; i++)
                    xv[i] = (float)extractedDataIdx[i][0][0][0] / trajView.fData[i][0].GetComponent<CustomTubeRenderer>().numOfPoints;

                aoi.transform.Find("AOI").GetComponent<AOIVisControl>().updateGanttChart(xv, wv);
                aoi.transform.Find("AOI").GetComponent<AOIVisControl>().showTimeTable();
            }
        }
    }

    public void showTrajectories()
    {
        if (!findTrajView())
            return;
        for (int s = 0; s < trajView.Settings.Sessions.Count; s++) {
            int c = 0;

        }
    }

    public void hideTrajectories()
    {
        if (!findTrajView())
            return;
        for (int s = 0; s < trajView.Settings.Sessions.Count; s++) { 
        
        }
    }

    public void CountOverallTimeSpan()
    {
        if (!findTrajView())
            return;
        int estimatedTimeInSecond;
        int estimatedTimeMins;
        int reminedSeconds;
        int totalNumberOfpoints = 0;

        int estimatedTimeInSecondEachSession;
        int numberOfpointsEachSession = 0;
        for (int s = 0; s < trajView.Settings.Sessions.Count; s++)
        {
            numberOfpointsEachSession = 0;
            for (int c = 0; c < trajView.Settings.Conditions.Count; c++)
            {
                AnalysisObject dataSet = Services.DataManager().DataSets[0]; // just check the head 
                var infoObjects = dataSet.GetInfoObjects(trajView.Settings.Sessions[s], trajView.Settings.Conditions[c]);
                for (int pidx = 0; pidx < infoObjects.Count - 1; pidx += 1) {
                    numberOfpointsEachSession++;
                }
            }
            estimatedTimeInSecondEachSession = (int)(numberOfpointsEachSession * (1.0f / 70));
            Debug.Log("numberOfpointsEachSession_" + s + " = " + numberOfpointsEachSession);
            //Debug.Log("Time Span In Seconds_" + s + " = " + estimatedTimeInSecondEachSession + " s");
            Debug.Log("Time Span_" + s + " = " + estimatedTimeInSecondEachSession / 60 + " m " + estimatedTimeInSecondEachSession % 60 + " s");

            totalNumberOfpoints += numberOfpointsEachSession;
        }


        estimatedTimeInSecond = (int)(totalNumberOfpoints * (1.0f / 70));
        estimatedTimeMins = estimatedTimeInSecond / 60;
        reminedSeconds = estimatedTimeInSecond % 60;

        Debug.Log("totalNumberOfpoints = " + totalNumberOfpoints);
        //Debug.Log("Overall Time Span In Seconds = " + estimatedTimeInSecond + " s");
        Debug.Log("Overall Time Span = " + estimatedTimeMins + " m " + reminedSeconds + " s");
    }

    public void ComputeHeatMap()
    {

    }

    [PunRPC]
    public void performComputationSync()
    {
        ComputingTask t = currComputingTask;

        if (t == ComputingTask.SEGMENT)
        {
            segAllTrajByAOIs();
        }
        if (t == ComputingTask.ApproachView)
        {
            ComputeAndAppyApproachView();
        }
        if (t == ComputingTask.DoubleArrowView)
        {
            ComputeAndApplyDoubleArrowVis();
        }
        if (t == ComputingTask.FILTER)
        {
            reasoningThePool();
        }
        if (t == ComputingTask.FishBoneView)
        {
            ComputeAndApplyToFishBoneView();
        }
        if (t == ComputingTask.PaceView)
        {
            ComputeAndApplyPaceView();
        }
        if (t == ComputingTask.Situated2DbarChart)
        {
            ComputeAndApply2DbarChart();
        }
        if (t == ComputingTask.Stacked3DBarChart)
        {
            ComputeAndApplyToStacked3DBarChart();
        }
        if (t == ComputingTask.GanttChart)
        {
            ComputeAndApply2DGanttChart();
        }
    }

    [PunRPC]
    public void performComputationSync(int taskID)
    {
        ComputingTask t = (ComputingTask)taskID;
        currComputingTask = t;
        if (t == ComputingTask.SEGMENT)
        {
            StartCoroutine(segAllTrajByAOIs());
        }
        if (t == ComputingTask.ApproachView)
        {
            StartCoroutine(ComputeAndAppyApproachView());
        }
        if (t == ComputingTask.DoubleArrowView)
        {
            StartCoroutine(ComputeAndApplyDoubleArrowVis());
        }
        if (t == ComputingTask.FILTER)
        {
            reasoningThePool();
        }
        if (t == ComputingTask.FishBoneView)
        {
            StartCoroutine(ComputeAndApplyToFishBoneView());
        }
        if (t == ComputingTask.PaceView)
        {
            StartCoroutine(ComputeAndApplyPaceView());
        }
        if (t == ComputingTask.Situated2DbarChart)
        {
            ComputeAndApply2DbarChart();
        }
        if (t == ComputingTask.Stacked3DBarChart)
        {
            ComputeAndApplyToStacked3DBarChart();
            ComputeAndApplyToStacked3DBarChart();
        }
        if (t == ComputingTask.GanttChart)
        {
            ComputeAndApply2DGanttChart();
        }
    }

    public void performComputation(int taskID)
    {
        performComputationSync(taskID);
        GetComponent<PhotonView>().RPC("performComputationSync", RpcTarget.Others, taskID);
    }

    public void performComputation()
    {
        performComputationSync();
        GetComponent<PhotonView>().RPC("performComputationSync", RpcTarget.Others);
    }
}
