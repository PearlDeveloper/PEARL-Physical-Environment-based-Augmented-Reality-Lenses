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

public class AutoStart : MonoBehaviour
{
    public int targetFrameToPerform = 300;
    public AOIDataManager aOIDataManager;
    public QRAlign qRAlign;

    bool isPerformed = false;
    int currentFrame = 0;

    /// <summary>
    /// 
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("currentFrame " + currentFrame);
        if (currentFrame > targetFrameToPerform && isPerformed == false)
        {
            Debug.Log("autostart perfromed! ");
            isPerformed = true;
            //
            aOIDataManager.loadDimensionViaNetwork("room2042");
            qRAlign.performAlignment();

        }

        if(currentFrame <= targetFrameToPerform)
        {
            currentFrame++;
        }
    }
}
