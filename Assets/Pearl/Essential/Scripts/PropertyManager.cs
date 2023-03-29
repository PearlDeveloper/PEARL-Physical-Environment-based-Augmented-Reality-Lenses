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
using TMPro;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public enum Property{ 
    INACTIVE, // 0
    POSITIVE, // 1
    NEGATIVE, // 2
    SELECTED,  // 3
    GROUPED,  // 4
    REGESTERED, // 5
    NUM_OF_PROPERTIES
}

public class PropertyManager : MonoBehaviour
{
    public Property state;
    public int GlobalIDValue;
    public TextMesh GlobalID;
    public TextMesh InGroupID; 
    [HideInInspector] public List<GameObject> tubesPassedBy = new List<GameObject>();
    [HideInInspector] public int[] numPeoplePassedByForEachGroup = new int[2];
    [HideInInspector] public GroupHeightMap[] heightMap = new GroupHeightMap[24];
    PhotonView view;

    //public bool currInAABB = false;
    //public bool lastInAABB = false;
    //public int timesEntered = 0;
    //public float firstEnterTimeStamp = 0;
    //public bool firstTimeEntered = false;
    //public bool secTimeEntered = false;
    //public SceneObjectRequestHelper sceneHelper;
    //public GroupLogicManager groupLogicManager;

    AOIApperancemanager aOIApperancemanager;
    SceneObjectRequestHelper sceneObjectRequestHelper;

    void Start()
    {
        aOIApperancemanager = GetComponent<AOIApperancemanager>();
        sceneObjectRequestHelper = GetComponent<SceneObjectRequestHelper>();

        //GlobalID.text = "ID in AOI List: ";
        //InGroupID.text = "InGroupID: ";

        //if(isActiveFilter == 0)
        //{
        //    filterMesh.transform.GetComponent<MeshRenderer>().material = originFilterMaterial;
        //}
        //globalOrderText.gameObject.SetActive(false);
        heightMap = new GroupHeightMap[24];
        for(int i = 0; i < 24; i++)
        {
            heightMap[i] = new GroupHeightMap();
        }

        assignGlobalID(GlobalIDValue.ToString());

        view = GetComponent<PhotonView>();
    }

    public void assignGlobalID(string t)
    {
        GlobalID.text = t;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public void registerAOIRemote()
    {
        registerAOISync();
        GetComponent<PhotonView>().RPC("registerAOISync", RpcTarget.Others);
    }

    /// <summary>
    /// 
    /// </summary>
    [PunRPC]
    public void registerAOISync()
    {
        state = Property.REGESTERED;
        switchProperty(state);
    }

    /// <summary>
    /// 
    /// </summary>
    public void togglePropertiesSync()
    {
        toggleProperties();
        GetComponent<PhotonView>().RPC("toggleProperties", RpcTarget.Others);
    }

    /// <summary>
    /// for toggle btns 
    /// </summary>
    [PunRPC]
    public void toggleProperties()
    {
        if (state > 0) 
            state = Property.INACTIVE;
        else 
            state = Property.POSITIVE;

        switchProperty(state);
    }

    /// <summary>
    /// for switching btns, apply to apperance + relations
    /// </summary>
    /// <param name="targetProperty"></param>
    public void switchProperty(Property targetProperty)
    {
        state = targetProperty; 
        aOIApperancemanager.adjustVis();
        this.adjustRelations();
    }

    /// <summary>
    /// 
    /// </summary>
    void adjustRelations()
    {
        if (state == Property.INACTIVE)
        {
            //if (!sceneObjectRequestHelper.InactivePool)
            //    sceneObjectRequestHelper.tryToFindInactivePool();
            //this.transform.parent.transform.parent = sceneObjectRequestHelper.InactivePool.transform; // move to a pool that wont be included in computation
        }
        if (state == Property.REGESTERED)
        {
            //if (!sceneObjectRequestHelper.InactivePool)
            //    sceneObjectRequestHelper.tryToFindInactivePool();
            //this.transform.parent.transform.parent = sceneObjectRequestHelper.InactivePool.transform; // move to a pool that wont be included in computation 
        }
        if (state == Property.POSITIVE)
        {
            if (!sceneObjectRequestHelper.positiveFilterboxesPool) 
                sceneObjectRequestHelper.tryToFindPositiveFilterboxesPool();
            this.transform.parent.transform.parent = sceneObjectRequestHelper.positiveFilterboxesPool.transform;
        }
        if (state == Property.NEGATIVE)
        {
            if (!sceneObjectRequestHelper.negativeFilterboxesPool)
                sceneObjectRequestHelper.tryToFindNegativeFilterboxesPool();
            this.transform.parent.transform.parent = sceneObjectRequestHelper.negativeFilterboxesPool.transform;
        }
        if (state == Property.GROUPED)
        {
            if (!sceneObjectRequestHelper.ANDGroupPool)
                sceneObjectRequestHelper.tryToFindANDGroupPool();
            this.transform.parent.transform.parent = sceneObjectRequestHelper.ANDGroupPool.transform;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public void switchPropertyRemote(int target)
    {
        switchProperty(target);
        if (view != null) 
            view.RPC("switchProperty", RpcTarget.Others, target);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    [PunRPC]
    public void switchProperty(int target)
    {
        switchProperty((Property)target);
    }

    /// <summary>
    /// 
    /// </summary>
    public void reset2NFilter()
    {
        //isActiveFilter = 0;
        //if (!sceneHelper.positiveFilterboxesPool) // put to positive pool by default ... 
        //    sceneHelper.tryToFindPositiveFilterboxesPool();
        //this.transform.parent = sceneHelper.positiveFilterboxesPool.transform;
        //filterMesh.transform.GetComponent<MeshRenderer>().material = positiveFilterMaterial;
    }

    /// <summary>
    /// 
    /// </summary>
    public void changeProperty2PositiveFilter()
    {
        //isActiveFilter = 1; // no enough, move to positiveFilterboxesPool!
        //if (!sceneHelper.positiveFilterboxesPool)
        //    sceneHelper.tryToFindPositiveFilterboxesPool();
        //this.transform.parent = sceneHelper.positiveFilterboxesPool.transform;
        //filterMesh.transform.GetComponent<MeshRenderer>().material = positiveFilterMaterial;
    }

    /// <summary>
    /// 
    /// </summary>
    public void changeProperty2NegativeFilter()
    {
        //// do not allow direct negate while in active group 
        //if (groupLogicManager.inActiveGroup)
        //    return;

        ////
        //isActiveFilter = 2; // not enough, move to negativeFilterboxesPool!
        //if (!sceneHelper.negativeFilterboxesPool)
        //    sceneHelper.tryToFindNegativeFilterboxesPool();
        //this.transform.parent = sceneHelper.negativeFilterboxesPool.transform;
        //filterMesh.transform.GetComponent<MeshRenderer>().material = negativeFilterMaterial;
    }

    /// <summary>
    /// 
    /// </summary>
    public void selectSync()
    {
        select();
        GetComponent<PhotonView>().RPC("select", RpcTarget.Others);
    }

    /// <summary>
    /// 
    /// </summary>
    [PunRPC]
    public void select()
    {
        if (sceneObjectRequestHelper.flowManager == null) sceneObjectRequestHelper.tryToFindFlowManager();
        if(state != Property.SELECTED)
        {
            sceneObjectRequestHelper.flowManager.OrderedSelectedIDs.Add(int.Parse(GlobalID.text)); // add to selection list 
            InGroupID.text = (sceneObjectRequestHelper.flowManager.OrderedSelectedIDs.Count).ToString();
            switchProperty(Property.SELECTED);
        }else if (state == Property.SELECTED) {
            sceneObjectRequestHelper.flowManager.OrderedSelectedIDs.Remove(int.Parse(GlobalID.text));
            InGroupID.text = "-";
            switchProperty(Property.INACTIVE);
        }
    }
}
