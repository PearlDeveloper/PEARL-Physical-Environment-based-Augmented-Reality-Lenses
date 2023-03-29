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

public class ScaleController : MonoBehaviour
{
    [SerializeField] GameObject LHolder;
    [SerializeField] GameObject RHolder;
    [SerializeField] GameObject TopHolder;
    [SerializeField] GameObject BottomHolder;
    [SerializeField] GameObject FrontHolder;
    [SerializeField] GameObject BackHolder;
    [SerializeField] GameObject inactiveMesh;
    [SerializeField] GameObject AOIIllustratorObject;
    [SerializeField] int frameInterval = 30;
    [SerializeField] bool showHoldersWhenStart = false;

    float originalDim_x;
    float originalDim_y;
    float originalDim_z;
    int currFrame = 0;
    bool sizeInitialized = false;

    AOIApperancemanager aOIApperancemanager;

    // Start is called before the first frame update
    void Start()
    {
        aOIApperancemanager = GetComponent<AOIApperancemanager>(); // reque components

        originalDim_x = AOIIllustratorObject.transform.localScale.x;
        originalDim_y = AOIIllustratorObject.transform.localScale.y;
        originalDim_z = AOIIllustratorObject.transform.localScale.z;

        if (showHoldersWhenStart)
            showHolders();
        else
            disableHolders();
    }

    /// <summary>
    /// 
    /// </summary>
    public void showHolders()
    {
        if (LHolder.GetComponent<MeshRenderer>())
            LHolder.GetComponent<MeshRenderer>().enabled = true;
        if (RHolder.GetComponent<MeshRenderer>())
            RHolder.GetComponent<MeshRenderer>().enabled = true;

        if (TopHolder.GetComponent<MeshRenderer>())
            TopHolder.GetComponent<MeshRenderer>().enabled = true;
        if (BottomHolder.GetComponent<MeshRenderer>())
            BottomHolder.GetComponent<MeshRenderer>().enabled = true;

        if (FrontHolder.GetComponent<MeshRenderer>())
            FrontHolder.GetComponent<MeshRenderer>().enabled = true;
        if (BackHolder.GetComponent<MeshRenderer>())
            BackHolder.GetComponent<MeshRenderer>().enabled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void disableHolders()
    {
        if (LHolder.GetComponent<MeshRenderer>())
            LHolder.GetComponent<MeshRenderer>().enabled = false;
        if (RHolder.GetComponent<MeshRenderer>())
            RHolder.GetComponent<MeshRenderer>().enabled = false;

        if (TopHolder.GetComponent<MeshRenderer>())
            TopHolder.GetComponent<MeshRenderer>().enabled = false;
        if (BottomHolder.GetComponent<MeshRenderer>())
            BottomHolder.GetComponent<MeshRenderer>().enabled = false;

        if (FrontHolder.GetComponent<MeshRenderer>())
            FrontHolder.GetComponent<MeshRenderer>().enabled = false;
        if (BackHolder.GetComponent<MeshRenderer>())
            BackHolder.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // run once to init 
        if (LHolder.activeSelf && RHolder.activeSelf && !sizeInitialized) {
            AdjustScaleAccordingToHolder();
            sizeInitialized = true;
        }

        //
        if (LHolder.activeSelf && RHolder.activeSelf)
        {
            currFrame++;
            if (currFrame > frameInterval)
            {
                AdjustScaleAccordingToHolder();
                currFrame = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dim"></param>
    public void AdjustScaleAccordingToDimension(Vector3 dim)
    {

        Vector3 newCenterforAOI = new Vector3(
            dim.x * 0.5f,
            dim.y * 0.5f,
            -dim.z * 0.5f);

        Vector3 boxRight = new Vector3(
            -dim.x * 0.5f,
            0,
            0);
        Vector3 boxLeft = new Vector3(
            dim.x * 0.5f,
            0,
            0);

        Vector3 boxTop = new Vector3(
            0,
            dim.y * 0.5f,
            0);
        Vector3 boxBottom = new Vector3(
            0,
            -dim.y * 0.5f,
            0);

        Vector3 boxFront = new Vector3(
            0,
            0,
            dim.z * 0.5f);
        Vector3 boxBack = new Vector3(
            0,
            0,
            -dim.z * 0.5f);

        this.transform.localPosition = newCenterforAOI;

        // adjust real object boundingbox 
        inactiveMesh.transform.localScale = dim; 

        // adjust filterbox 
        AOIIllustratorObject.transform.localScale = dim;

        // 
        RHolder.transform.localPosition = boxRight;
        LHolder.transform.localPosition = boxLeft;
        TopHolder.transform.localPosition = boxTop;
        BottomHolder.transform.localPosition = boxBottom;
        FrontHolder.transform.localPosition = boxFront;
       // FrontPressable.transform.localPosition = boxFront;
        BackHolder.transform.localPosition = boxBack;

        // adjust parameters for further 
        originalDim_x = dim.x;
        originalDim_y = dim.y;
        originalDim_z = dim.z;

        aOIApperancemanager.updateRigPositions();
        aOIApperancemanager.updateMenuPosition();

    }

    /// <summary>
    /// 
    /// </summary>
    public void AdjustScaleAccordingToHolder()
    {
        float newDim_x = Mathf.Abs((RHolder.transform.localPosition.x - LHolder.transform.localPosition.x)); 
        float scaleFactor_x = newDim_x / originalDim_x;
        float newCenter_x = (RHolder.transform.localPosition.x + LHolder.transform.localPosition.x) / 2;

        //
        float newDim_y = Mathf.Abs((TopHolder.transform.localPosition.y - BottomHolder.transform.localPosition.y));
        float scaleFactor_y = newDim_y / originalDim_y;
        float newCenter_y = (TopHolder.transform.localPosition.y + BottomHolder.transform.localPosition.y) / 2;

        //
        float newDim_z = Mathf.Abs((FrontHolder.transform.localPosition.z - BackHolder.transform.localPosition.z));
        float scaleFactor_z = newDim_z / originalDim_z;
        float newCenter_z = (FrontHolder.transform.localPosition.z + BackHolder.transform.localPosition.z) / 2;

        /// adjust the illustrator 
        AOIIllustratorObject.transform.localScale = new Vector3(
            scaleFactor_x * originalDim_x, 
            scaleFactor_y * originalDim_y,
            scaleFactor_z * originalDim_z);

        AOIIllustratorObject.transform.localPosition = new Vector3(
            newCenter_x,
            newCenter_y,
            newCenter_z);

        // keep y 
        BottomHolder.transform.localScale = new Vector3(
            scaleFactor_x * originalDim_x,
            BottomHolder.transform.localScale.y,
            scaleFactor_z * originalDim_z);
        BottomHolder.transform.localPosition = new Vector3(
            newCenter_x,
            BottomHolder.transform.localPosition.y,
            newCenter_z);
        TopHolder.transform.localScale = new Vector3(
            scaleFactor_x * originalDim_x,
            TopHolder.transform.localScale.y,
            scaleFactor_z * originalDim_z);
        TopHolder.transform.localPosition = new Vector3(
            newCenter_x,
            TopHolder.transform.localPosition.y,
            newCenter_z);

        // keep x
        LHolder.transform.localScale = new Vector3(
            LHolder.transform.localScale.x,
            scaleFactor_y * originalDim_y,
            scaleFactor_z * originalDim_z);
        LHolder.transform.localPosition = new Vector3(
            LHolder.transform.localPosition.x,
            newCenter_y,
            newCenter_z);
        RHolder.transform.localScale = new Vector3(
            RHolder.transform.localScale.x,
            scaleFactor_y * originalDim_y,
            scaleFactor_z * originalDim_z);
        RHolder.transform.localPosition = new Vector3(
            RHolder.transform.localPosition.x, // keep the x 
            newCenter_y,
            newCenter_z);

        // keep z
        BackHolder.transform.localScale = new Vector3(
            scaleFactor_x * originalDim_x,
            scaleFactor_y * originalDim_y,
            BackHolder.transform.localScale.z);
        BackHolder.transform.localPosition = new Vector3(
            newCenter_x,
            newCenter_y,
            BackHolder.transform.localPosition.z);
        FrontHolder.transform.localScale = new Vector3(
            scaleFactor_x * originalDim_x,
            scaleFactor_y * originalDim_y,
            FrontHolder.transform.localScale.z);
        FrontHolder.transform.localPosition = new Vector3(
            newCenter_x,
            newCenter_y,
            FrontHolder.transform.localPosition.z);

        if (aOIApperancemanager)
        {
            aOIApperancemanager.updateRigPositions();
            aOIApperancemanager.updateMenuPosition();
        }
    }
}
