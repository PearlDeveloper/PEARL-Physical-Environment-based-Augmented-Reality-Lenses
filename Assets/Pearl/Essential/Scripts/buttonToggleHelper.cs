// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonToggleHelper : MonoBehaviour
{
    public GameObject[] toggleButttons;
    bool isShown = false;

    // Start is called before the first frame update
    void Start()
    {
        isShown = this.GetComponent<Interactable>().IsToggled;
        foreach (GameObject tb in toggleButttons)
            tb.SetActive(isShown);
    }

    /// <summary>
    /// 
    /// </summary>
    public void toggleTheButtons()
    {
        isShown = this.GetComponent<Interactable>().IsToggled;

        foreach(GameObject tb in toggleButttons)
            tb.SetActive(isShown);
    }
}
