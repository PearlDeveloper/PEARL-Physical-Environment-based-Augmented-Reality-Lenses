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

public class SceneObjectRequestHelper : MonoBehaviour
{
    [HideInInspector] public GameObject TubesInTheScene = null;
    [HideInInspector] public GameObject MainCamera = null;
    [HideInInspector] public AOIDataManager aOIDataManager = null;
    //public ANDGroupingModeHelper mANDGroupingModeHelper = null;

    [HideInInspector] public GameObject positiveFilterboxesPool = null;
    [HideInInspector] public GameObject ANDGroupPool = null;
    [HideInInspector] public GameObject InactivePool = null;
    [HideInInspector] public GameObject negativeFilterboxesPool = null;
    [HideInInspector] public FlowManager flowManager = null;
    [HideInInspector] public ComputingManager computingManager = null;

    // Start is called before the first frame update
    void Start()
    {
        tryToFindMainCamera();
        tryToFindFilteringBoxesManager();
        tryToFindPositiveFilterboxesPool();
        tryToFindNegativeFilterboxesPool();
        tryToFindFlowManager();
        tryToFindcomputingManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tryToFindANDGroupPool()
    {
        ANDGroupPool = GameObject.FindGameObjectsWithTag("ANDGroupedAOIs")[0];
    }
    public void tryToFindPositiveFilterboxesPool()
    {
        positiveFilterboxesPool = GameObject.FindGameObjectsWithTag("positiveFilterboxesPool")[0];
    }

    public void tryToFindInactivePool()
    {
        InactivePool = GameObject.FindGameObjectsWithTag("Inactive")[0];
    }

    public void tryToFindNegativeFilterboxesPool()
    {
        negativeFilterboxesPool = GameObject.FindGameObjectsWithTag("negativeFilterboxesPool")[0];
    }

    public void tryToFindMainCamera()
    {
        MainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    }

    public void tryToFindFilteringBoxesManager()
    {
        aOIDataManager = GameObject.FindGameObjectsWithTag("AOIDataManager")[0].GetComponent<AOIDataManager>();
    }

    public void tryToFindFlowManager()
    {
        flowManager = GameObject.FindGameObjectsWithTag("FlowManager")[0].GetComponent<FlowManager>();
    }

    public void tryToFindcomputingManager()
    {
        computingManager = GameObject.FindGameObjectsWithTag("ComputingManager")[0].GetComponent<ComputingManager>();
    }

    public void performImplicitFilter()
    {

    }

    public void updateNodeLinkagesAccordingToTheLogicTree()
    {

    }
}
