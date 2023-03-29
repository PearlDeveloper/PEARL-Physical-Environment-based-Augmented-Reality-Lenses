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

public class DoublePress: MonoBehaviour
{
    public int timePressedTotalTarget = 2;
    public float targetTimeInbetween = 2.0f;

    float timeStampFirstPress = float.MaxValue;
    float timeStampSecondPress = float.MaxValue;
    int timePressed = 0;

    // Update is called once per frame
    void Update()
    {
        if(timePressed >= timePressedTotalTarget)
        {
            //
            //if((timeStampSecondPress - timeStampFirstPress) < targetTimeInbetween){
                ////
                //if (propertyManager.isActiveFilter == 0) // -> will be moved to gesture operation
                //    activationHelper.performActivationDisableToObjects();
                //if ((propertyManager.isActiveFilter == 2) || (propertyManager.isActiveFilter == 0)) // negative filter 
                //    target = 1; // to positive 
                //if (propertyManager.isActiveFilter == 1) // positive filter 
                //    target = 2; // to negative 

                ////
                //if (target == 1)
                //    propertyManager.changeProperty2PositiveFilter();
                //if (target == 2)
                //    propertyManager.changeProperty2NegativeFilter();

                //Debug.Log("excute once, propertyManager.isActiveFilter=" + propertyManager.isActiveFilter); 

                GetComponent<PropertyManager>().selectSync();

                timeStampSecondPress = 0;
                timeStampFirstPress = 0;
           // }

            //
            timePressed = 0;
        }
    }

    public void pressOnce()
    {

        timePressed++;
        Debug.Log("timePressed++!, timePressed = " + timePressed + " timePressedTotalTarget = " + timePressedTotalTarget);
        if (timePressed == 1)
            timeStampFirstPress = Time.time;
        if (timePressed == 2)
            timeStampSecondPress = Time.time;
    }
}
