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

public class GanttLineControl : MonoBehaviour
{
    public float speed = 0.005f / 60;

    public GameObject indicator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = new Vector3(
            indicator.transform.localPosition.x + speed, indicator.transform.localPosition.y, indicator.transform.localPosition.z);
        if (targetPosition.x > 0.25f)
            targetPosition.x = -0.25f;
        indicator.transform.localPosition = targetPosition;
    }
}
