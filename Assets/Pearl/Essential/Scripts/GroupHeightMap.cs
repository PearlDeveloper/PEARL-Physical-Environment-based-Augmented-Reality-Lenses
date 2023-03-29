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

public class GroupHeightMap
{
    public int[] heightValueByGroup = new int[2];

    public GroupHeightMap()
    {
        heightValueByGroup = new int[2];
        heightValueByGroup[0] = 0;
        heightValueByGroup[1] = 0;
    }
}
