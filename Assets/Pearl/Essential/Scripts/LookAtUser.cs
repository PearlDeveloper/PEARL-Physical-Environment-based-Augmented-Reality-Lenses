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

public class LookAtUser : MonoBehaviour
{
    public SceneObjectRequestHelper sceneObjectRequestHelper;
    // Start is called before the first frame update
    void Start()
    {
        //sceneObjectRequestHelper = GetComponent<SceneObjectRequestHelper>();
        sceneObjectRequestHelper.tryToFindMainCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneObjectRequestHelper.MainCamera == null)
            sceneObjectRequestHelper.tryToFindMainCamera();
        Transform target = sceneObjectRequestHelper.MainCamera.transform;
        Vector3 targetPostition = new Vector3(target.position.x,
                                       this.transform.position.y,
                                       target.position.z);
        this.transform.LookAt(targetPostition);
        //transform.LookAt(sceneObjectRequestHelper.MainCamera.transform, Vector3.up);
    }
}
