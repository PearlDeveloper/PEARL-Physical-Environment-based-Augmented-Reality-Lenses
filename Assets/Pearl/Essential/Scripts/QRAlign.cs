// ------------------------------------------------------------------------------------
// <copyright company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Zhongyuan Yu <zhongyuan.yu@tu-dresden.de>
// </author>
// ------------------------------------------------------------------------------------
using SampleQRCodes;
using UnityEngine;

public class QRAlign : MonoBehaviour
{
    public GameObject floorIndicator;
    public QRCodesVisualizer QrVis;
    public GameObject OriginAnchor; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void performAlignment()
    {
        Debug.Log("QrVis.qrCodesObjects Count = " + QrVis.qrCodesObjects.Count);
        Debug.Log("QrVis.qrCodesObjects.ContainsKey(origin)? = " + QrVis.qrCodesObjects.ContainsKey("origin"));
        if (QrVis.qrCodesObjects.ContainsKey("origin"))
        {

            OriginAnchor.transform.position = QrVis.qrCodesObjects["origin"].transform.Find("Axis").transform.position;
            OriginAnchor.transform.rotation = QrVis.qrCodesObjects["origin"].transform.Find("Axis").transform.rotation;
        }
        if (QrVis.qrCodesObjects.ContainsKey("floor"))
        {
            floorIndicator.transform.position = new Vector3(
                floorIndicator.transform.position.x,
                QrVis.qrCodesObjects["floor"].transform.position.y,
                floorIndicator.transform.position.z);
        }
    }
}
