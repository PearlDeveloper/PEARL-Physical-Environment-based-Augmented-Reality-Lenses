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
using UnityEngine;

public class VisDataFeeder : MonoBehaviour
{
    VisDataHolder visDataHolder;
    GenericDataPresenter genericDataPresenter;
    // Start is called before the first frame update
    void Start()
    {
        visDataHolder = GetComponent<VisDataHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
