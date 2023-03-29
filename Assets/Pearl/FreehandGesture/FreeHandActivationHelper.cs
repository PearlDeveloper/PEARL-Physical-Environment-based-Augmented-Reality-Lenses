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
using TMPro;
using UnityEngine;

public class FreeHandActivationHelper : MonoBehaviour
{
    public GameObject MRTKPlayspace;
    //public GameObject positiveFilterboxesPool;
    //public GameObject negativeFilterboxesPool;
    public GameObject RfunctionIllustratorPrefeb;
    public GameObject LfunctionIllustratorPrefeb;
    public GameObject MainCameraAsHeadPosition;
    //public RealtimeFilteringHelper realtimeFilteringHelper;
    //public tubeParameterHelper tubeParameterHelper;

    public TextMeshPro debuginfo;
    //public GameObject[] References;
    public AOIDataManager aOIDataManager;
    public FlowManager flowManager;

    public float pinchLowerThreshold = 0.05f;
    public float functionDist = 1.0f;
    public int targettimesRCurling = 120;
    public int targettimesLCurling = 120;
    public int targettimesLIndexing = 120;
    public int targettimesRHandOpened = 120;
    public int targettimesLHandOpened = 120;
    public int targetTimesInverseLeftGestureDetected = 120;
    public int tergetTimesPinching = 60;
    public int targetFrameIntervalForSelection = 10;

    public bool showGestureInfo = false;
    public bool showCurrentAvgDistHand = false;
    public bool showCurrentCurls = true;
    public bool logInfo = true;
    public bool allowTwoFingerAlignment = false;

    public GameObject Referents;
    public GameObject OriginAnchor;

    //
    GameObject lastSelectedTube;
    int timesRCurling = 0;
    int timesLCurling = 0;
    int timesLIndexing = 0;
    int timesRHandOpened = 0;
    int timesLHandOpened = 0;
    int timesInverseLeftGestureDetected = 0;
    Vector3[] poseInLastFrames = new Vector3[60];
    GameObject RfunctionIllustrator = null;
    GameObject LfunctionIllustrator = null;
    int timesPinching = 0;
    GameObject pinchingIllus;
    float lastTimeEntered = 0;
    int currFrameSpan = 0;
    bool Rcurled = false;
    bool RLastCurled = false;
    float Rcurl = 0;
    float RcurlThreshold = 0.0f;
    bool Lcurled = false;
    bool Lindexing = false;
    bool Rindexing = false;
    bool LLastCurled = false;
    bool LLastIndexing = false;
    float Lcurl = 0;
    float LcurlThreshold = 0.0f;
    bool RHandopened = false;
    bool RHandLastopened = false;
    float RHandOpenDegree = 0;
    float RHandOpenThreshold = 0.13f;
    bool LHandopened = false;
    bool LHandLastopened = false;
    float LHandOpenDegree = 0;
    float LHandOpenThreshold = 0.13f;

    /* We are using the following joints for computation: 
        ThumbTip Proxy Transform
        IndexTip Proxy Transform
        MiddleTip Proxy Transform
        RingTip Proxy Transform
        PinkyTip Proxy Transform
    
        Wrist Proxy Transform
        IndexKnuckle Proxy Transform
        MiddleKnuckle Proxy Transform
        RingKnuckle Proxy Transform
        PinkyKnuckle Proxy Transform
     */
    Transform LThumbTip = null;
    Transform LIndexTip = null;
    Transform LMiddleTip = null;
    Transform LRingTip = null;
    Transform LPinkyTip = null;

    Transform RThumbTip = null;
    Transform RIndexTip = null;
    Transform RMiddleTip = null;
    Transform RRingTip = null;
    Transform RPinkyTip = null;

    /*21.07.2022 */
    Transform LWrist = null;
    Transform LIndexKnuckle = null;
    Transform LMiddleKnuckle = null;
    Transform LRingKnuckle = null;
    Transform LPinkyKnuckle = null;

    Transform RWrist = null;
    Transform RIndexKnuckle = null;
    Transform RMiddleKnuckle = null;
    Transform RRingKnuckle = null;
    Transform RPinkyKnuckle = null;

    Transform LPalm = null;
    Transform RPalm = null;


    public void toggleGestureInfofunc()
    {
        showGestureInfo = !showGestureInfo;
    }

    // Start is called before the first frame update
    void Start()
    {
        Microsoft.MixedReality.Toolkit.Input.PointerUtils.SetHandRayPointerBehavior(
            Microsoft.MixedReality.Toolkit.Input.PointerBehavior.AlwaysOff);

        foreach (Transform r in Referents.transform)
        {
            aOIDataManager.AOIs.Add(r.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ListenToGestures();
    }

    void LindexingContinues()
    {
        timesLIndexing++;
        if (timesLIndexing > targettimesLIndexing)
        {
            if (!allowTwoFingerAlignment) return;

            // here we reuse the LfunctionIllustratorPrefeb 
            //if (LPalm && !LfunctionIllustrator)
            //    LfunctionIllustrator = Object.Instantiate(LfunctionIllustratorPrefeb, LPalm);
            Debug.Log("LindexingContinues detected! timesLIndexing > targettimesLIndexing!, Excuting Alignment! ");

            if (!RIndexTip) return;

            //
            OriginAnchor.transform.position = LIndexTip.transform.position;

            //
            Vector3 worldZ = new Vector3(0, 0, 1);
            Vector3 worldY = new Vector3(0, 1, 0);
            //Vector3 currentEulerAngle = OriginAnchor.transform.rotation.eulerAngles;

            //Vector3 rot = LPalm.transform.rotation.eulerAngles;
            //rot.x = 0;
            //rot.y += 90;
            //rot.z = 0;
            //OriginAnchor.transform.rotation = Quaternion.Euler(rot); 
            Vector3 newZ = Vector3.Normalize(RIndexTip.transform.position - LIndexTip.transform.position);
            Quaternion newRot = Quaternion.LookRotation(newZ, worldY);
            Vector3 rot = newRot.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            OriginAnchor.transform.rotation = Quaternion.Euler(rot); // new rotation, just rotate around y!

            timesLIndexing = 0;
        }
    }

    void LCurlingContinues()
    {
        timesLCurling++;
        if (timesLCurling > targettimesLCurling)
        {
            // here we reuse the LfunctionIllustratorPrefeb 
            if (LPalm && !LfunctionIllustrator)
                LfunctionIllustrator = Object.Instantiate(LfunctionIllustratorPrefeb, LPalm);

            timesLCurling = 0;
        }
    }

    // from close to open hand: 
    void RCurlingContinues()
    {
        timesRCurling++;
        if (timesRCurling > targettimesRCurling)
        {
            if (showGestureInfo && showCurrentCurls)
                debuginfo.text = "Curling Detected!\n" + debuginfo.text;

            //
            if (RPalm && !RfunctionIllustrator)
                RfunctionIllustrator = Object.Instantiate(RfunctionIllustratorPrefeb, RPalm);

            timesRCurling = 0;
        }

    }

    void LFistReleaseDetected()
    {
        if (LfunctionIllustrator)
        {
            Object.Destroy(LfunctionIllustrator);
            LfunctionIllustrator = null;
            timesLCurling = 0;
        }
    }

    void LIndexingReleaseDetected()
    {
        if (LfunctionIllustrator)
        {
            Object.Destroy(LfunctionIllustrator);
            LfunctionIllustrator = null;
            timesLIndexing = 0;
        }
    }

    //
    void RFistReleaseDetected()
    {
        if (RfunctionIllustrator && !LfunctionIllustrator)
        {
            if (logInfo)
                Debug.Log("Object.Destroy(RfunctionIllustrator);");
            Object.Destroy(RfunctionIllustrator);
            RfunctionIllustrator = null;
            timesRCurling = 0;

            ////
            //foreach (Transform c in positiveFilterboxesPool.transform)
            //{
            //    if (!c.gameObject.activeSelf)
            //        continue;
            //    // try to find bbox illustrator
            //    GameObject bbox_obj = c.transform.Find("BoundingBoxIllustrator").gameObject;
            //    if (bbox_obj)
            //    {
            //        // BoxCollider bbox = bbox_obj.GetComponent<BoxCollider>();
            //        if (Vector3.Distance(bbox_obj.transform.position, RIndexTip.transform.position) < functionDist)
            //        {
            //            //
            //            c.GetComponent<ActivationHelper>().performActivationDisableToObjects();
            //            c.GetComponent<PropertyManager>().changeProperty2PositiveFilter();
            //        }
            //    }
            //}

            GameObject nearestAOI = null;
            float dist = float.MaxValue;
            foreach (GameObject aoi in aOIDataManager.AOIs)
            {
                if (Vector3.Distance(aoi.transform.position, RIndexTip.transform.position) < dist)
                {
                    nearestAOI = aoi;
                    dist = Vector3.Distance(aoi.transform.position, RIndexTip.transform.position);
                }
            }
            if (nearestAOI)
                nearestAOI.transform.Find("AOI").GetComponent<PropertyManager>().switchPropertyRemote((int)Property.REGESTERED);
        }
    }

    // 
    void RHandOpenedContinues()
    {
        timesRHandOpened++;
        if (timesRHandOpened > targettimesRHandOpened)
        {
            //debuginfo.text = "RfunctionIllustrator Instantiated! \n" + debuginfo.text;
            //
            if (RPalm && !RfunctionIllustrator)
                RfunctionIllustrator = Object.Instantiate(RfunctionIllustratorPrefeb, RPalm);

            timesRHandOpened = 0;
        }
    }

    void LHandOpenedContinues()
    {
        timesLHandOpened++;
        if (timesLHandOpened > targettimesLHandOpened)
        {
            //debuginfo.text = "RfunctionIllustrator Instantiated! \n" + debuginfo.text;
            //
            if (LPalm && !LfunctionIllustrator)
                LfunctionIllustrator = Object.Instantiate(LfunctionIllustratorPrefeb, LPalm);

            timesLHandOpened = 0;
        }
    }

    void LHandOpen2Neutral()
    {
        if (LfunctionIllustrator)
        {
            if (logInfo) Debug.Log("Object.Destroy(LfunctionIllustrator); called!");
            Object.Destroy(LfunctionIllustrator);
            LfunctionIllustrator = null;
            timesLHandOpened = 0;
        }
    }

    void RHandOpen2Neutral()
    {
        if (logInfo) Debug.Log("RHandOpen2Neutral entered!");
        if (RfunctionIllustrator)
        {
            if (logInfo) Debug.Log("Object.Destroy(RfunctionIllustrator); called!");
            Object.Destroy(RfunctionIllustrator);
            RfunctionIllustrator = null;
            timesRHandOpened = 0;

            //
            //foreach (Transform c in positiveFilterboxesPool.transform)
            //{
            //    if (!c.gameObject.activeSelf)
            //        continue;
            //    // try to find bbox illustrator
            //    GameObject bbox_obj = c.transform.Find("BoundingBoxIllustrator").gameObject;
            //    if (bbox_obj)
            //    {
            //        // BoxCollider bbox = bbox_obj.GetComponent<BoxCollider>();
            //        if (Vector3.Distance(bbox_obj.transform.position, RIndexTip.transform.position) < functionDist)
            //        {
            //            //
            //            c.GetComponent<ActivationHelper>().performInversedActivationDisableToObjects();
            //            c.GetComponent<PropertyManager>().reset2NFilter();
            //        }
            //    }
            //}

            ////
            //foreach (Transform c in negativeFilterboxesPool.transform)
            //{
            //    if (!c.gameObject.activeSelf)
            //        continue;
            //    // try to find bbox illustrator
            //    GameObject bbox_obj = c.transform.Find("BoundingBoxIllustrator").gameObject;
            //    if (bbox_obj)
            //    {
            //        // BoxCollider bbox = bbox_obj.GetComponent<BoxCollider>();
            //        if (Vector3.Distance(bbox_obj.transform.position, RIndexTip.transform.position) < functionDist)
            //        {
            //            //
            //            c.GetComponent<ActivationHelper>().performInversedActivationDisableToObjects();
            //            c.GetComponent<PropertyManager>().reset2NFilter();
            //        }
            //    }
            //}

            GameObject nearestAOI = null;
            float dist = float.MaxValue;
            foreach (GameObject aoi in aOIDataManager.AOIs)
            {
                if (Vector3.Distance(aoi.transform.position, RIndexTip.transform.position) < dist)
                {
                    nearestAOI = aoi;
                    dist = Vector3.Distance(aoi.transform.position, RIndexTip.transform.position);
                }
            }
            if (nearestAOI)
                nearestAOI.transform.Find("AOI").GetComponent<PropertyManager>().switchPropertyRemote((int)Property.INACTIVE);
        }
    }

    public void toggleSelection()
    {
        // 
        FindHandSkeleton();

        //
        if (!pinchingIllus && RIndexTip)
        {
            pinchingIllus = Object.Instantiate(RfunctionIllustratorPrefeb, RIndexTip);
            pinchingIllus.transform.localScale = new Vector3(4, 4, 4);
        }
        else
            Object.Destroy(pinchingIllus);
    }



    // Filtering Control
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

    void updateLHandOpenHandState()
    {
        //
        LHandopened = false;

        //
        if (LThumbTip && LIndexTip && LMiddleTip && LRingTip && LPinkyTip)
        {
            float d1 = Vector3.Distance(LThumbTip.transform.position, LIndexTip.transform.position);
            float d2 = Vector3.Distance(LThumbTip.transform.position, LMiddleTip.transform.position);
            float d3 = Vector3.Distance(LThumbTip.transform.position, LRingTip.transform.position);
            float d4 = Vector3.Distance(LThumbTip.transform.position, LPinkyTip.transform.position);
            LHandOpenDegree = (d1 + d2 + d3 + d4) / 4.0f;
            if (showGestureInfo && showCurrentAvgDistHand)
                debuginfo.text = "LHandOpenDegree = " + LHandOpenDegree + "\n" + debuginfo.text;
        }

        //
        if (LHandOpenDegree > LHandOpenThreshold)
        {
            LHandopened = true;
        }
        else
        {
            LHandopened = false;
        }
    }

    void updateRHandOpenHandState()
    {
        if (RThumbTip && RIndexTip && RMiddleTip && RRingTip && RPinkyTip)
        {
            float d1 = Vector3.Distance(RThumbTip.transform.position, RIndexTip.transform.position);
            float d2 = Vector3.Distance(RThumbTip.transform.position, RMiddleTip.transform.position);
            float d3 = Vector3.Distance(RThumbTip.transform.position, RRingTip.transform.position);
            float d4 = Vector3.Distance(RThumbTip.transform.position, RPinkyTip.transform.position);
            RHandOpenDegree = (d1 + d2 + d3 + d4) / 4.0f;
            if (showGestureInfo && showCurrentAvgDistHand)
                debuginfo.text = "RHandOpenDegree = " + RHandOpenDegree + "\n" + debuginfo.text;
            if (logInfo) Debug.Log("RHandOpenDegree = " + RHandOpenDegree);
        }

        //
        if (RHandOpenDegree > RHandOpenThreshold)
        {
            RHandopened = true;
        }
        else
        {
            RHandopened = false;
        }
    }

    //
    void updateLHandCurlingState()
    {
        // RcurlThreshold
        Lcurled = false;
        Lindexing = false;
        float LIndexCurl = 0;
        float LMiddleCurl = 0;
        float LRingCurl = 0;
        float LPinkyCurl = 0;

        if (LWrist &&
            LIndexKnuckle && LIndexTip &&
            LMiddleKnuckle && LMiddleTip &&
            LRingKnuckle && LRingTip &&
            LPinkyKnuckle && LPinkyTip)
        {
            LIndexCurl = Vector3.Dot((LIndexKnuckle.transform.position - LIndexTip.transform.position),
                (LIndexKnuckle.transform.position - LWrist.transform.position));
            LMiddleCurl = Vector3.Dot((LMiddleKnuckle.transform.position - LMiddleTip.transform.position),
                (LMiddleKnuckle.transform.position - LWrist.transform.position));
            LRingCurl = Vector3.Dot((LRingKnuckle.transform.position - LRingTip.transform.position),
                (LRingKnuckle.transform.position - LWrist.transform.position));
            LPinkyCurl = Vector3.Dot((LPinkyKnuckle.transform.position - LPinkyTip.transform.position),
                (LPinkyKnuckle.transform.position - LWrist.transform.position));

            if (showGestureInfo && showCurrentCurls)
            {
                debuginfo.text = "LIndexCurl = " + LIndexCurl + "\n" + debuginfo.text;
                debuginfo.text = "LMiddleCurl = " + LMiddleCurl + "\n" + debuginfo.text;
                debuginfo.text = "LRingCurl = " + LRingCurl + "\n" + debuginfo.text;
                debuginfo.text = "LPinkyCurl = " + LPinkyCurl + "\n" + debuginfo.text;
            }
            if (logInfo)
            {
                Debug.Log("LIndexCurl = " + LIndexCurl);
                Debug.Log("LMiddleCurl = " + LMiddleCurl);
                Debug.Log("LRingCurl = " + LRingCurl);
                Debug.Log("LPinkyCurl = " + LPinkyCurl);
            }
        }

        // or, diff. thresholds? 
        if (LIndexCurl > LcurlThreshold &&
            LMiddleCurl > LcurlThreshold &&
            LRingCurl > LcurlThreshold &&
            LPinkyCurl > LcurlThreshold)
        {
            Lcurled = true;
        }
        else
        {
            Lcurled = false;
        }

        //
        if (LIndexCurl < LcurlThreshold &&
            LMiddleCurl > LcurlThreshold &&
            LRingCurl > LcurlThreshold &&
            LPinkyCurl > LcurlThreshold)
        {
            Lindexing = true;
        }
        else
        {
            Lindexing = false;
        }
    }

    //
    void updateRHandCurlingState()
    {
        // RcurlThreshold
        float RIndexCurl = 0;
        float RMiddleCurl = 0;
        float RRingCurl = 0;
        float RPinkyCurl = 0;

        if (RWrist &&
            RIndexKnuckle && RIndexTip &&
            RMiddleKnuckle && RMiddleTip &&
            RRingKnuckle && RRingTip &&
            RPinkyKnuckle && RPinkyTip)
        {
            RIndexCurl = Vector3.Dot((RIndexKnuckle.transform.position - RIndexTip.transform.position),
                (RIndexKnuckle.transform.position - RWrist.transform.position));
            RMiddleCurl = Vector3.Dot((RMiddleKnuckle.transform.position - RMiddleTip.transform.position),
                (RMiddleKnuckle.transform.position - RWrist.transform.position));
            RRingCurl = Vector3.Dot((RRingKnuckle.transform.position - RRingTip.transform.position),
                (RRingKnuckle.transform.position - RWrist.transform.position));
            RPinkyCurl = Vector3.Dot((RPinkyKnuckle.transform.position - RPinkyTip.transform.position),
                (RPinkyKnuckle.transform.position - RWrist.transform.position));

            if (showGestureInfo && showCurrentCurls)
            {
                debuginfo.text = "RIndexCurl = " + RIndexCurl + "\n" + debuginfo.text;
                debuginfo.text = "RMiddleCurl = " + RMiddleCurl + "\n" + debuginfo.text;
                debuginfo.text = "RRingCurl = " + RRingCurl + "\n" + debuginfo.text;
                debuginfo.text = "RPinkyCurl = " + RPinkyCurl + "\n" + debuginfo.text;
            }

            if (logInfo)
            {
                Debug.Log("RIndexCurl = " + RIndexCurl);
                Debug.Log("RMiddleCurl = " + RMiddleCurl);
                Debug.Log("RRingCurl = " + RRingCurl);
                Debug.Log("RPinkyCurl = " + RPinkyCurl);
                Debug.Log("RIndexCurl = " + RIndexCurl);
                Debug.Log("RMiddleCurl = " + RMiddleCurl);
            }


        }

        // or, diff. thresholds? 
        if (RIndexCurl > RcurlThreshold &&
            RMiddleCurl > RcurlThreshold &&
            RRingCurl > RcurlThreshold &&
            RPinkyCurl > RcurlThreshold)
        {
            Rcurled = true;
        }
        else
        {
            Rcurled = false;
        }
    }

    void checkAndToggle()
    {
        ////
        //foreach (Transform c in positiveFilterboxesPool.transform)
        //{
        //    if (!c.gameObject.activeSelf)
        //        continue;
        //    // try to find bbox illustrator
        //    GameObject bbox_obj = c.transform.Find("inactiveMesh").gameObject;
        //    if (bbox_obj)
        //    {
        //        PropertyManager cp = c.GetComponent<PropertyManager>();
        //        cp.currInAABB = false;
        //        BoxCollider bbox = bbox_obj.GetComponent<BoxCollider>();
        //        if(PointInOABB(RIndexTip.transform.position, bbox))
        //        {
        //            cp.currInAABB = true;
        //        }

        //        // 
        //        if (cp.lastInAABB == false && cp.currInAABB == true)
        //        {
        //            cp.timesEntered++;
        //            Debug.Log("timesEntered++, = " + cp.timesEntered);
        //            debuginfo.text = "timesEntered++, = " + cp.timesEntered + "\n" + debuginfo.text;
        //        }

        //        // first time enter 
        //        if (cp.timesEntered == 1 && !cp.firstTimeEntered)
        //        {
        //            cp.firstEnterTimeStamp = Time.time;
        //            Debug.Log("first time enter !, timestamp = " + cp.firstEnterTimeStamp);
        //            cp.firstTimeEntered = true;
        //        }

        //        // second time enter 
        //        if (cp.timesEntered == 2 && !cp.secTimeEntered)
        //        {
        //            Debug.Log("second time enter !, time = " + Time.time);
        //            if ((Time.time - cp.firstEnterTimeStamp) < 2)
        //            {
        //                Debug.Log("toggleFilterProperty called !");
        //                c.GetComponent<PropertyManager>().changeProperty2NegativeFilter();
        //            }

        //            //
        //            cp.currInAABB = false;
        //            cp.lastInAABB = false;
        //            cp.timesEntered = 0;
        //            cp.firstEnterTimeStamp = 0;
        //            cp.firstTimeEntered = false;
        //            cp.secTimeEntered = false;
        //        }

        //        // reset if gave up the second try longer than 2 seconds
        //        if (Time.time - cp.firstEnterTimeStamp > 3)
        //        {
        //            cp.currInAABB = false;
        //            cp.lastInAABB = false;
        //            cp.timesEntered = 0;
        //            cp.firstEnterTimeStamp = 0;
        //            cp.firstTimeEntered = false;
        //            cp.secTimeEntered = false;
        //        }
        //        cp.lastInAABB = cp.currInAABB;
        //    }
        //}

        ////
        //foreach (Transform c in negativeFilterboxesPool.transform)
        //{
        //    if (!c.gameObject.activeSelf)
        //        continue;
        //    // try to find bbox illustrator
        //    GameObject bbox_obj = c.transform.Find("inactiveMesh").gameObject;
        //    if (bbox_obj)
        //    {
        //        PropertyManager cp = c.GetComponent<PropertyManager>();
        //        cp.currInAABB = false;
        //        BoxCollider bbox = bbox_obj.GetComponent<BoxCollider>();
        //        if (PointInOABB(RIndexTip.transform.position, bbox))
        //        {
        //            cp.currInAABB = true;
        //        }

        //        // 
        //        if (cp.lastInAABB == false && cp.currInAABB == true)
        //        {
        //            cp.timesEntered++;
        //            Debug.Log("timesEntered++, = " + cp.timesEntered);
        //        }

        //        // first time enter 
        //        if (cp.timesEntered == 1 && !cp.firstTimeEntered)
        //        {
        //            cp.firstEnterTimeStamp = Time.time;
        //            Debug.Log("first time enter !, timestamp = " + cp.firstEnterTimeStamp);
        //            cp.firstTimeEntered = true;
        //        }

        //        // second time enter 
        //        if (cp.timesEntered == 2 && !cp.secTimeEntered)
        //        {
        //            Debug.Log("second time enter !, time = " + Time.time);
        //            if ((Time.time - cp.firstEnterTimeStamp) < 2)
        //            {
        //                Debug.Log("toggleFilterProperty called !");
        //                c.GetComponent<PropertyManager>().changeProperty2PositiveFilter();
        //            }

        //            //
        //            cp.currInAABB = false;
        //            cp.lastInAABB = false;
        //            cp.timesEntered = 0;
        //            cp.firstEnterTimeStamp = 0;
        //            cp.firstTimeEntered = false;
        //            cp.secTimeEntered = false;
        //        }

        //        // reset if gave up the second try longer than 2 seconds
        //        if (Time.time - cp.firstEnterTimeStamp > 3)
        //        {
        //            cp.currInAABB = false;
        //            cp.lastInAABB = false;
        //            cp.timesEntered = 0;
        //            cp.firstEnterTimeStamp = 0;
        //            cp.firstTimeEntered = false;
        //            cp.secTimeEntered = false;
        //        }
        //        cp.lastInAABB = cp.currInAABB;
        //    }
        //}
    }


    void ListenToGestures()
    {
        // 
        FindHandSkeleton();

        /*
         * Listen To LeftHand Gestures 
         */
        if (LThumbTip && LIndexTip)
        {
            // Update States
            updateLHandCurlingState();
            updateLHandOpenHandState();

            /*
             * FIST: fist for a while, release to activate filterboxes that in range  
             */
            if (Lcurled)
            {
                LCurlingContinues();
            }
            if ((LLastCurled == true) && (Lcurled == false) && RLastCurled == false && Rcurled == false)
            {
                LFistReleaseDetected();
            }

            LLastCurled = Lcurled;


            /*
             * Indexing 
             */
            if (Lindexing) 
            {
                LindexingContinues();
            }
            if((LLastIndexing == true) && (Lindexing == false))
            {
                LIndexingReleaseDetected();
            }
            LLastIndexing = Lindexing;

            /*
            * OPENHAND: open hand for a while, release to deactivate filterboxes that in range 
            *     not stable due to the lack of frames... 
            */
            if (LHandopened)
            {
                LHandOpenedContinues();
            }
            if ((LHandLastopened == true) && (LHandopened == false) && RHandopened == false && RHandLastopened == false)
            {
                if (showGestureInfo)
                    debuginfo.text = "L OPEN to close hand detected! \n" + debuginfo.text;
                if (logInfo)
                    Debug.Log("L OPEN to close hand detected!");
                LHandOpen2Neutral();
            }
            LHandLastopened = LHandopened;
        }

        /*
         * Listen To RightHand Gestures 
         */
        if (RThumbTip && RIndexTip)
        {
            updateRHandCurlingState();// Update States
            updateRHandOpenHandState();

            /*
             * FIST: fist for a while, release to activate filterboxes that in range  
             */
            if (Rcurled)
            {
                RCurlingContinues();
            }
            if ((RLastCurled == true) && (Rcurled == false) && LLastCurled == false && Lcurled == false)
            {
                if (showGestureInfo)
                    debuginfo.text = "Fist to release hand detected! \n" + debuginfo.text;
                if (logInfo)
                    Debug.Log("Fist to release hand detected!");
                RFistReleaseDetected();
            }
            RLastCurled = Rcurled;

            /*
             * OPENHAND: open hand for a while, release to deactivate filterboxes that in range 
             *     not stable due to the lack of frames... 
             */
            if (RHandopened)
            {
                RHandOpenedContinues();
            }
            if ((RHandLastopened == true) && (RHandopened == false) && LHandopened == false && LHandLastopened == false)
            {
                if (showGestureInfo)
                    debuginfo.text = "OPEN to close hand detected! \n" + debuginfo.text;
                if (logInfo)
                    Debug.Log("OPEN to close hand detected!");
                RHandOpen2Neutral();
            }
            RHandLastopened = RHandopened;

            //
            checkAndToggle();
        }

        /*
         * TWO HANDs
         * FIST for the peak function 
         */
        if (RfunctionIllustrator && LfunctionIllustrator && Lcurled && Rcurled)
        {
            //
            if (logInfo) Debug.Log("two hands fisting! ");

            //
            GameObject nearestAOI = null;
            float dist = float.MaxValue;
            foreach (GameObject aoi in aOIDataManager.AOIs)
            {
                if (Vector3.Distance(aoi.transform.position, RIndexTip.transform.position) < dist)
                {
                    nearestAOI = aoi;
                    dist = Vector3.Distance(aoi.transform.position, RIndexTip.transform.position);
                }
            }
            if (nearestAOI)
            {
                Transform frontHolder = nearestAOI.transform.Find("AOI").transform.Find("Holders").transform.Find("front").transform;
                Transform backHolder = nearestAOI.transform.Find("AOI").transform.Find("Holders").transform.Find("back").transform;
                Vector3 boxDir = frontHolder.position - backHolder.position;
                boxDir.Normalize();
                frontHolder.position = MainCameraAsHeadPosition.transform.position - 0.3f * boxDir;
            }
        }

        /*
         * TWO HANDs
         * clap gesture 
         */
        if (RfunctionIllustrator && LfunctionIllustrator && LHandopened && RHandopened)
        {
            Object.Destroy(LfunctionIllustrator);
            LfunctionIllustrator = null;
            Object.Destroy(RfunctionIllustrator);
            RfunctionIllustrator = null;
            timesLHandOpened = 0;
            timesRHandOpened = 0;

            //
            if (logInfo) Debug.Log("two hands clapping! ");
            flowManager.submitGroup();
        }
    }

    void FindHandSkeleton()
    {
        // HandObject
#if UNITY_EDITOR
        var handObject_LHand = MRTKPlayspace.transform.Find("Left_RiggedHandLeft(Clone)"); // diff name than in the simulator, Left_RiggedHandLeft(Clone)
        var handObject_RHand = MRTKPlayspace.transform.Find("Right_RiggedHandRight(Clone)");
#else
        var handObject_LHand = MRTKPlayspace.transform.Find("Left_HandSkeleton(Clone)"); // diff name than in the simulator 
        var handObject_RHand = MRTKPlayspace.transform.Find("Right_ObsoleteHandSkeleton(Clone)");
#endif
        // Palm
        if (handObject_LHand)
            LPalm = handObject_LHand.transform.Find("Palm Proxy Transform"); // was IndexTip Proxy Transform
        if (handObject_RHand)
            RPalm = handObject_RHand.transform.Find("Palm Proxy Transform");

        /*
            ThumbTip Proxy Transform
            IndexTip Proxy Transform
            MiddleTip Proxy Transform
            RingTip Proxy Transform
            PinkyTip Proxy Transform
         */
        // ThumbTip
        if (handObject_LHand)
            LThumbTip = handObject_LHand.transform.Find("ThumbTip Proxy Transform");
        if (handObject_RHand)
            RThumbTip = handObject_RHand.transform.Find("ThumbTip Proxy Transform");

        // IndexTip
        if (handObject_LHand)
            LIndexTip = handObject_LHand.transform.Find("IndexTip Proxy Transform");
        if (handObject_RHand)
            RIndexTip = handObject_RHand.transform.Find("IndexTip Proxy Transform");

        // MiddleTip
        if (handObject_LHand)
            LMiddleTip = handObject_LHand.transform.Find("MiddleTip Proxy Transform");
        if (handObject_RHand)
            RMiddleTip = handObject_RHand.transform.Find("MiddleTip Proxy Transform");

        // RingTip
        if (handObject_LHand)
            LRingTip = handObject_LHand.transform.Find("RingTip Proxy Transform");
        if (handObject_RHand)
            RRingTip = handObject_RHand.transform.Find("RingTip Proxy Transform");

        // PinkyTip
        if (handObject_LHand)
            LPinkyTip = handObject_LHand.transform.Find("PinkyTip Proxy Transform");
        if (handObject_RHand)
            RPinkyTip = handObject_RHand.transform.Find("PinkyTip Proxy Transform");

        /*
            Wrist Proxy Transform
            IndexKnuckle Proxy Transform
            MiddleKnuckle Proxy Transform
            RingKnuckle Proxy Transform
            PinkyKnuckle Proxy Transform
         */

        // Wrist
        if (handObject_LHand)
            LWrist = handObject_LHand.transform.Find("Wrist Proxy Transform");
        if (handObject_RHand)
            RWrist = handObject_RHand.transform.Find("Wrist Proxy Transform");

        // IndexKnuckle
        if (handObject_LHand)
            LIndexKnuckle = handObject_LHand.transform.Find("IndexKnuckle Proxy Transform");
        if (handObject_RHand)
            RIndexKnuckle = handObject_RHand.transform.Find("IndexKnuckle Proxy Transform");

        // MiddleKnuckle
        if (handObject_LHand)
            LMiddleKnuckle = handObject_LHand.transform.Find("MiddleKnuckle Proxy Transform");
        if (handObject_RHand)
            RMiddleKnuckle = handObject_RHand.transform.Find("MiddleKnuckle Proxy Transform");

        // RingKnuckle
        if (handObject_LHand)
            LRingKnuckle = handObject_LHand.transform.Find("RingKnuckle Proxy Transform");
        if (handObject_RHand)
            RRingKnuckle = handObject_RHand.transform.Find("RingKnuckle Proxy Transform");

        // PinkyKnuckle
        if (handObject_LHand)
            LPinkyKnuckle = handObject_LHand.transform.Find("PinkyKnuckle Proxy Transform");
        if (handObject_RHand)
            RPinkyKnuckle = handObject_RHand.transform.Find("PinkyKnuckle Proxy Transform");
    }
}