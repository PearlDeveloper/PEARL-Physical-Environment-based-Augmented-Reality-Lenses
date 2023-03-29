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

public class UIManager : MonoBehaviour
{
    public GameObject[] panelList;
    public GameObject FunctionBtns;
    int lastActiveUIIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < panelList.Length; i++)
            panelList[i].SetActive(false);
        FunctionBtns.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPanel(int idx)
    {
        for (int i = 0; i < panelList.Length; i++)
            panelList[i].SetActive(false);
        panelList[idx].SetActive(true);
        lastActiveUIIdx = idx;
        FunctionBtns.SetActive(true);
    }

    public void disableAllUI()
    {
        for (int i = 0; i < panelList.Length; i++)
            panelList[i].SetActive(false);
        FunctionBtns.SetActive(false);
    }

    public void showLastActiveUI()
    {
        panelList[lastActiveUIIdx].SetActive(true);
        FunctionBtns.SetActive(true);
    }
}
