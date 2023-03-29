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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AOIDataManager : MonoBehaviour
{
    public GameObject refObjectForPosition;
    public GameObject AOIPrefeb;
    public GameObject positiveAOIsPool; 
    public GameObject negativeAOIsPool;
    public TextMeshPro debuginfo;
    [HideInInspector] public List<GameObject> AOIs = new List<GameObject>(); // keep refernce to all filtering boxes no matter positive or negative 
    [HideInInspector] public List<GameObject> DupCreateAOIs = new List<GameObject>();
    [HideInInspector] public List<GameObject> Nodes = new List<GameObject>();
    [HideInInspector] public Dictionary<string, Vector3> AOIDimsAndName = new Dictionary<string, Vector3>();

    GameObject currAOI;
    string fn;
    string serverUrl;
    string dataLoadingUrl;

    /// <summary>
    /// 
    /// </summary>
    public void ClearAllAOIs()
    {
        foreach(GameObject aoi in AOIs)
            Destroy(aoi);

        // do sth to the positiveAOIsPool!
        foreach (Transform c in positiveAOIsPool.transform)
            Destroy(c);
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateAOI()
    {
        currAOI = Instantiate(AOIPrefeb, refObjectForPosition.transform.position, Quaternion.Euler(180,180,0));
        currAOI.transform.parent = positiveAOIsPool.transform;
        currAOI.transform.Find("AOI").GetComponent<PropertyManager>().assignGlobalID((AOIs.Count + 1).ToString());
        AOIs.Add(currAOI);
        DupCreateAOIs.Add(currAOI);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator LoadDataViaNetwork(string url)
    {
        //
        AOIDimsAndName.Clear();

        //
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Or retrieve results as binary data
            //byte[] byteArray = www.downloadHandler.data;

            // Get bytes from file
            byte[] byteArray = Encoding.UTF8.GetBytes(www.downloadHandler.text);

            // Create memory stream
            MemoryStream stream = new MemoryStream(byteArray);

            // Convert MemoryStream to StreamReader
            StreamReader reader = new StreamReader(stream);

            // current line 
            string line = reader.ReadLine();

            //
            string name = "";

            //
            while (line != null)
            {
                // Trim line (removes whitespaces at beginning and end)
                line = line.Trim();

                // Get words
                string[] words = line.Split(' ');

                // If words is empty, ignore
                if (words.Length > 0)
                {

                    if (words[0].Equals("n"))
                    {
                        name = words[1];
                    }

                    if (words[0].Equals("s"))
                    {
                        Vector3 dim = new Vector3(
                            float.Parse(words[1]),
                            float.Parse(words[2]),
                            float.Parse(words[3])
                        );

                        if (name != "")
                        {
                            if (AOIDimsAndName.ContainsKey(name))
                                AOIDimsAndName[name] = dim;
                            else
                                AOIDimsAndName.Add(name, dim);
                            debuginfo.text = "AOI " + name + " " + dim + "added! \n" + debuginfo.text;
                            name = "";
                        }
                    }
                }

                // Read next line
                line = reader.ReadLine();
            }

            // Close
            reader.Close();

            applyTheUpdatedDimension(); 
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void applyTheUpdatedDimension()
    {
        // loop every dim we have and apply 
        foreach (var item in AOIDimsAndName)
        {
            // search and apply 
            foreach(GameObject aoi in AOIs)
            {
                ScaleController sc = aoi.transform.Find("AOI").GetComponent<ScaleController>();
                if(aoi.GetComponent<QRCode>().qrCode != null && aoi.GetComponent<QRCode>().qrCode.Data != null) // if it is QRCode 
                {
                    if (aoi.GetComponent<QRCode>().qrCode.Data == item.Key)
                    {
                        sc.AdjustScaleAccordingToDimension(item.Value);
                        break;
                    }
                }
                else // if it is Created in the air 
                {
                    if (aoi.transform.Find("AOI").GetComponent<PropertyManager>().GlobalID.text == item.Key)
                    {
                        sc.AdjustScaleAccordingToDimension(item.Value);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void saveDataToFile()
    {
        StreamWriter tw = new StreamWriter(fn, false);

        Debug.Log("num of AOIs = " + AOIs.Count);

        // collect handmade filtering boxes 
        foreach (GameObject aoi in AOIs) {
            // should be activated 
            if (aoi.activeSelf == true) { 
                // is a number otherwise: the qr code object may be null! 
                if(aoi.transform.Find("AOI").GetComponent<PropertyManager>())
                    if (!aoi.transform.Find("AOI").GetComponent<PropertyManager>().GlobalID.text.StartsWith("obj"))
                    {
                        GameObject AOIDim = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").gameObject;
                        Vector3 position = AOIDim.transform.position;
                        Quaternion orientation = AOIDim.transform.rotation;
                        Vector3 scale = AOIDim.transform.localScale;
                        tw.WriteLine("n " + aoi.transform.Find("AOI").GetComponent<PropertyManager>().GlobalID.text); // should be a number ... 
                        tw.WriteLine("v " + position.x + " " + position.y + " " + position.z);
                        tw.WriteLine("o " + orientation.x + " " + orientation.y + " " + orientation.z + " " + orientation.w);
                        tw.WriteLine("s " + scale.x + " " + scale.y + " " + scale.z);
                    }            
            }

        }

        // for QRCodes recognized 
        for(int i = 1;i < 21; i++)
        {
            foreach (GameObject aoi in AOIs)
            {
                if (aoi.activeSelf == true && aoi.GetComponent<QRCode>().qrCode!=null)
                {
                    if (i < 10 && aoi.GetComponent<QRCode>().qrCode.Data!=null)
                    {
                        if (aoi.GetComponent<QRCode>().qrCode.Data.Equals("obj0" + i.ToString()))
                        {
                            GameObject AOIDim = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").gameObject;
                            Vector3 position = AOIDim.transform.position;
                            Quaternion orientation = AOIDim.transform.rotation;
                            Vector3 scale = AOIDim.transform.localScale;
                            tw.WriteLine("n " + "obj0" + i.ToString());
                            tw.WriteLine("v " + position.x + " " + position.y + " " + position.z);
                            tw.WriteLine("o " + orientation.x + " " + orientation.y + " " + orientation.z + " " + orientation.w);
                            tw.WriteLine("s " + scale.x + " " + scale.y + " " + scale.z);
                        }
                    }
                    if( i > 10 && i < 21 && aoi.GetComponent<QRCode>().qrCode.Data != null)
                    {
                        if (aoi.GetComponent<QRCode>().qrCode.Data.Equals("obj" + i.ToString()))
                        {
                            GameObject AOIDim = aoi.transform.Find("AOI").transform.Find("AOIIllustrator").gameObject;
                            Vector3 position = AOIDim.transform.position;
                            Quaternion orientation = AOIDim.transform.rotation;
                            Vector3 scale = AOIDim.transform.localScale;
                            tw.WriteLine("n " + "obj" + i.ToString());
                            tw.WriteLine("v " + position.x + " " + position.y + " " + position.z);
                            tw.WriteLine("o " + orientation.x + " " + orientation.y + " " + orientation.z + " " + orientation.w);
                            tw.WriteLine("s " + scale.x + " " + scale.y + " " + scale.z);
                        }
                    }
                }
            }
        }

        tw.Close();

        debuginfo.text = "written to file! path: " + fn + "\n " + debuginfo.text;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roomName"></param>
    public void SendDataViaNetwork(string roomName)
    {
        //
        //fn = Application.persistentDataPath + "/AOIData_" + roomName + "_"+ Time.time.ToString() + ".txt";
        fn = Application.persistentDataPath + "/" + roomName + ".txt";

        // write to file 
        saveDataToFile();

        // send via network, send as binary file
        (new WebClient()).UploadFile(new Uri(serverUrl), "POST", fn);

        debuginfo.text = "file has been posted to: " + serverUrl + "\n " + debuginfo.text;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roomName"></param>
    public void loadDimensionViaNetwork(string roomName)
    {
        //
        AOIDimsAndName.Clear();

        //
        StartCoroutine(LoadDataViaNetwork(dataLoadingUrl + roomName + ".txt"));
    }

    /// <summary>
    /// 
    /// </summary>
    public void refractLogicTree()
    {
        // the tree
        foreach (GameObject aoi in AOIs) {
            aoi.transform.parent = positiveAOIsPool.transform; // apply them back to the root Node 
        }
        foreach(GameObject n in Nodes)
        {
            Destroy(n); // delete all logical nodes 
        }

        // illu
        //cr.clearAllDots();
    }

    /// <summary>
    /// 
    /// </summary>
    public void markAllAOIs4Computing()
    {
        foreach (GameObject aoi in AOIs)
        {
            aoi.transform.Find("AOI").GetComponent<PropertyManager>().switchPropertyRemote((int)Property.POSITIVE);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void disableWireframes()
    {
        foreach (GameObject aoi in AOIs)
        {
            aoi.transform.Find("AOI").GetComponent<AOIApperancemanager>()
                .applyVis(false, false, null, false, false, false, false);
        }
    }
}
