// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using u2vis;
using UnityEngine;

public class VisDataModifier : MonoBehaviour
{
    VisDataHolder visDataHolder;
    // Start is called before the first frame update
    void Start()
    {
        visDataHolder = GetComponent<VisDataHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateVisData()
    {
        for (int i = 0; i < 5; i++)
        {
            visDataHolder.Data[0].Set(i, String.Format("{0:00}:{1:00}.{2:00}", i, 0, 0)); //      .Add(i.ToString());
            visDataHolder.Data[1].Set(i, 50);           //.Add(r.Next(10));
            visDataHolder.Data[2].Set(i, 50.0f);           //.Add((float)r.NextDouble());
            visDataHolder.Data[3].Set(i, true);           //.Add((i % 2) > 0);
        }
    }
}
