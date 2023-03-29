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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stacked3DBarController : MonoBehaviour
{
    public GameObject cubePrefeb;
    public GameObject toolTipPrefeb;

    List<Vector3> _dims;
    List<Color> _colorTable;
    List<string> _texts;

    List<GameObject> generatedGameObjectPool;

    // Start is called before the first frame update
    void Start()
    {
        _dims = new List<Vector3>();
        _colorTable = new List<Color>();
        _texts = new List<string>();
        generatedGameObjectPool = new List<GameObject>();

        
        _colorTable.Add(Color.green);
        _colorTable.Add(Color.blue);
        _colorTable.Add(Color.black);
        _colorTable.Add(Color.white);

        //genTestData();
        //applyVis();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void genTestData()
    {
        _dims.Add(new Vector3(0.3f, 0.1f, 0.3f));
        _dims.Add(new Vector3(0.3f, 0.3f, 0.3f));
        _dims.Add(new Vector3(0.3f, 0.2f, 0.3f));

        _texts.Add("Rare");
        _texts.Add("Sometimes");
        _texts.Add("Often");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dims"></param>
    /// <param name="colorTable"></param>
    /// <param name="texts"></param>
    public void updateData(List<Vector3> dims, List<Color> colorTable, List<String> texts)
    {
        _dims = dims;
        _colorTable = colorTable;
        _texts = texts;

        applyVis();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="heights"></param>
    public void updateData(List<float> heights)
    {
        if(_dims == null)
        {
            _dims = new List<Vector3>();
            _colorTable = new List<Color>();
            _texts = new List<string>();
            generatedGameObjectPool = new List<GameObject>();

            
            _colorTable.Add(Color.green);
            _colorTable.Add(Color.blue);
            _colorTable.Add(Color.black);
            _colorTable.Add(Color.white);

            _texts.Add("Rare");
            _texts.Add("Sometimes");
            _texts.Add("Often");
        }

        _dims.Clear();
        _dims.Add(new Vector3(0.3f, heights[0], 0.3f));
        _dims.Add(new Vector3(0.3f, heights[1], 0.3f));
        _dims.Add(new Vector3(0.3f, heights[2], 0.3f));

        applyVis();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="heights"></param>
    /// <param name="texts"></param>
    public void updateData(List<float> heights, List<string> texts)
    {
        if (_dims == null)
        {
            _dims = new List<Vector3>();
            _colorTable = new List<Color>();
            _texts = new List<string>();
            generatedGameObjectPool = new List<GameObject>();

            _colorTable.Add(Color.green);
            _colorTable.Add(Color.blue);
            _colorTable.Add(Color.black);
            _colorTable.Add(Color.white);

            _texts.Add("Rare");
            _texts.Add("Sometimes");
            _texts.Add("Often");
        }
        else
        {
            _dims.Clear();
            _texts.Clear();
            for(int i = 0;i< heights.Count; i++)
            {
                if (heights[i] > 0.01f)
                {
                    _dims.Add(new Vector3(0.3f, heights[i], 0.3f));
                    _texts.Add(texts[i]);
                }
            }
        }

        applyVis();
    }

    /// <summary>
    /// 
    /// </summary>
    void applyVis()
    {
        if(generatedGameObjectPool == null)
            generatedGameObjectPool = new List<GameObject>();
        foreach (var g in generatedGameObjectPool)
        {
            Destroy(g);
        }
        float baseHeight = 0;
        for(int i = 0; i < _dims.Count; i++)
        {
            float currCenterHeight = baseHeight + _dims[i].y / 2.0f;
            Vector3 objectAnchorWS = new Vector3(0, currCenterHeight, 0) + this.transform.position;
            Vector3 tipAnchorWS = new Vector3(0, currCenterHeight, _dims[i].z) + this.transform.position;

            //Vector3 objectAnchorWS = this.transform.TransformPoint(objectAnchorLS);
            //Vector3 tipAnchorWS = this.transform.TransformPoint(tipAnchorLS);

            GameObject g = Instantiate(cubePrefeb, objectAnchorWS, Quaternion.identity);
            GameObject t = Instantiate(toolTipPrefeb, tipAnchorWS, Quaternion.identity);

            g.transform.localScale = _dims[i];
            g.GetComponent<MeshRenderer>().material.color = _colorTable[i];
            t.GetComponent<ToolTip>().ToolTipText = _texts[i];
            t.GetComponent<ToolTipConnector>().Target = g;
            //t.transform.Find("ContentParent").transform.localPosition.x = 0.1f;

            baseHeight += _dims[i].y;
            generatedGameObjectPool.Add(g);
            generatedGameObjectPool.Add(t);

            g.transform.parent = this.transform;
            t.transform.parent = this.transform;
        }
    }
}
