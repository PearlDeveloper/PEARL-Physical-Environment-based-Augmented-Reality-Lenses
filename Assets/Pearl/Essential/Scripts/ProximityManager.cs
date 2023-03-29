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

public class ProximityManager : MonoBehaviour
{
    bool isShowing = false;
    public float proximityDistance = 2.0f;
    SceneObjectRequestHelper sceneObjectRequestHelper;
    AOIApperancemanager aOIApperancemanager;
    PropertyManager propertyManager;

    // Start is called before the first frame update
    void Start()
    {
        sceneObjectRequestHelper = GetComponent<SceneObjectRequestHelper>();
        aOIApperancemanager = GetComponent<AOIApperancemanager>();
        propertyManager = GetComponent<PropertyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (propertyManager.state != Property.INACTIVE)
        {
            if(Vector3.Distance(this.transform.position, sceneObjectRequestHelper.MainCamera.transform.position) < proximityDistance)
            {
                aOIApperancemanager.applyVis(true, true);
            }
        
            if(Vector3.Distance(this.transform.position, sceneObjectRequestHelper.MainCamera.transform.position) > proximityDistance)
            {
                aOIApperancemanager.applyVis(false, false);
            }
        }
    }
}
