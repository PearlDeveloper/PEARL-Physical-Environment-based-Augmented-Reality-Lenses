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

public class AOIApperancemanager : MonoBehaviour
{
    [SerializeField] GameObject AOIIllustrator;
    [SerializeField] GameObject rigsRoot;
    [SerializeField] GameObject targetRigPositionsRoot;
    [SerializeField] GameObject textRoot;
    [SerializeField] GameObject contextMenu;
    [SerializeField] GameObject targetMenuPosition;
    [SerializeField] GameObject VisualMask;

    [SerializeField] GameObject boundingBox; // gray corners 
    [SerializeField] Material positiveFilterMaterial; // green corners 
    [SerializeField] Material negativeFilterMaterial; // red corners 
    [SerializeField] Material inGroupMaterial; // yellow corners 
    [SerializeField] Material registeredMaterial; // gray corners 

    [SerializeField] Material greyBBox;
    [SerializeField] Material highlightedBBox;
    [SerializeField] Material ingroupBBox;

    [HideInInspector] public bool showContextMenuInActive = false;

    PropertyManager propertyManager;

    // Start is called before the first frame update
    void Start()
    {
        propertyManager = GetComponent<PropertyManager>();

        adjustVis();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateMenuPosition()
    {
        contextMenu.transform.position = targetMenuPosition.transform.position;
    }

    public void updateRigPositions()
    {
        if (!rigsRoot.activeSelf) return;
        if (rigsRoot.transform.childCount != targetRigPositionsRoot.transform.childCount) return;
        for (int i = 0; i < rigsRoot.transform.childCount; i++)
            rigsRoot.transform.GetChild(i).transform.position = targetRigPositionsRoot.transform.GetChild(i).transform.position;
    }

    void applyMaterialToRigs(Material mat)
    {
        for (int i = 0; i < rigsRoot.transform.childCount; i++)
            rigsRoot.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = mat;
    }

    public void applyVis(bool bboxVis, bool contextMenuVis)
    {
        boundingBox.SetActive(bboxVis);
        contextMenu.SetActive(contextMenuVis);
    }

    public void applyVis(bool bboxVis, bool rigsVis, Material rigMat, bool textVis, bool contextMenuVis, bool highlightBBox, bool groupedBBox)
    {
        boundingBox.SetActive(bboxVis);
        rigsRoot.SetActive(rigsVis);
        textRoot.SetActive(textVis);
        if (rigsVis)
            applyMaterialToRigs(rigMat);
        contextMenu.SetActive(contextMenuVis);
        
        boundingBox.GetComponent<MeshRenderer>().material = greyBBox;
        if (highlightBBox)
            boundingBox.GetComponent<MeshRenderer>().material = highlightedBBox;
        if (groupedBBox)
            boundingBox.GetComponent<MeshRenderer>().material = ingroupBBox; // higher priority 
    }

    public void adjustVis()
    {
        if(propertyManager.state == Property.INACTIVE)
        {
            applyVis(false, false, null, false, showContextMenuInActive, false, false);
        }
        if (propertyManager.state == Property.POSITIVE)
        {
            applyVis(false, true, positiveFilterMaterial, true, true, false, false);
        }
        if (propertyManager.state == Property.NEGATIVE)
        {
            applyVis(false, true, negativeFilterMaterial, true, true, false, false);
        }
        if (propertyManager.state == Property.SELECTED)
        {
            applyVis(true, true, inGroupMaterial, true, true, true, false);
        }
        if (propertyManager.state == Property.GROUPED)
        {
            applyVis(true, true, inGroupMaterial, true, true, true, true);
        }
        if (propertyManager.state == Property.REGESTERED)
        {
            applyVis(true, false, registeredMaterial, false, true, false, false);
        }
    }
}
