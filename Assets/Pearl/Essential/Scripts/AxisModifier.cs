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
using u2vis;
using UnityEditor;
using UnityEngine;

public class AxisModifier : MonoBehaviour
{
    public bool triggerEvent_rebuildVis = true;
    public bool isTimeAxis = false;
    public bool isUserAxis = false;
    GenericAxisView genericAxisView;

    // Update is called once per frame
    void Update()
    {
        if (triggerEvent_rebuildVis)
        {
            if (isTimeAxis)
            {
                int numOfTicks = 9;
                AxisTick[] ticks = new AxisTick[numOfTicks];
                float delta = 1.0f / numOfTicks;
                for(int i = 0; i < numOfTicks; i++)
                {
                    ticks[i] = new AxisTick(delta * i, string.Format("{0:00}:{1:00}", i, 0)); // 20mins recording 
                }

                genericAxisView = GetComponent<GenericAxisView>();
                genericAxisView.Length = 0.9f;
                //genericAxisView.Ticklength = 0.9f;
                //genericAxisView.AxisPresenter.LabelOrientation = LabelOrientation.Diagonal;
                genericAxisView.RebuildAxis(ticks);
            }
            if (isUserAxis)
            {
                int numOfTicks = 6;
                AxisTick[] ticks = new AxisTick[numOfTicks];
                float delta = 1.0f / (numOfTicks + 2);
                for (int i = 0; i < numOfTicks; i++)
                {
                    ticks[i] = new AxisTick(delta * i + delta, "User" + string.Format("{0:00}", i + 1)); // 
                }

                genericAxisView = GetComponent<GenericAxisView>();
                genericAxisView.Length = 0.9f;
                genericAxisView.RebuildAxis(ticks);
            }

            triggerEvent_rebuildVis = false;
        }
    }
}
