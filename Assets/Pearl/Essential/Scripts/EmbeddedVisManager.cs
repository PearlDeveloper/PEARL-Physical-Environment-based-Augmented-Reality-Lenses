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
using TMPro;
using UnityEngine;

public class EmbeddedVisManager : MonoBehaviour
{
    public AOIDataManager aoiDataManager;

    public GameObject visPrefeb3DBoxes;
    public GameObject visPrefeb3DBox;
    public GameObject visPrefeb2DBar;

    public GameObject visObjectPool;
    public GameObject FloorIndicator;

    public float minHeightStacked = 0.6f;
    public float maxHeightStacked = 1.8f;

    public TextMeshPro heightText;

    public void showALL()
    {
        heightText.text = "minHeightStacked: " + minHeightStacked + "\n";
        heightText.text = "maxHeightStacked: " + maxHeightStacked + "\n";
    }

    public void less_MinHeight()
    {
        minHeightStacked -= 0.1f;
        heightText.text = "minHeightStacked: " + minHeightStacked;
    }
    public void more_MinHeight()
    {

        minHeightStacked += 0.1f;
        heightText.text = "minHeightStacked: " + minHeightStacked;
    }
    public void less_MaxHeight()
    {
        maxHeightStacked -= 0.1f;
        heightText.text = "maxHeightStacked: " + maxHeightStacked;
    }
    public void more_MaxHeight()
    {
        maxHeightStacked += 0.1f;
        heightText.text = "maxHeightStacked: " + maxHeightStacked;
    }

    void instantiateVisObjects(GameObject prefeb)
    {
        // 
        foreach(Transform visObj in visObjectPool.transform)
            Destroy(visObj.gameObject);

        //
        foreach(GameObject fb in aoiDataManager.AOIs)
        {
            float randomHeightA = Random.Range(0.2f, 0.6f);
            float randomHeightB = Random.Range(0.2f, 0.6f);
            float randomHeightC = Random.Range(0.2f, 0.6f);
            float centerX = fb.transform.Find("AOI").transform.position.x;
            float centerZ = fb.transform.Find("AOI").transform.position.z;

            //GameObject visObject = Instantiate(prefeb, new Vector3(centerX, FloorIndicator.transform.position.y + randomHeight / 2.0f, centerZ), Quaternion.identity);
            //visObject.transform.localScale = new Vector3(
            //    visObject.transform.localScale.x,
            //    randomHeight, 
            //    visObject.transform.localScale.z);

            GameObject visObject = Instantiate(prefeb, new Vector3(centerX, FloorIndicator.transform.position.y, centerZ), Quaternion.identity);
            List<float> heights = new List<float>();
            heights.Add(randomHeightA);
            heights.Add(randomHeightB);
            heights.Add(randomHeightC);
            visObject.GetComponent<Stacked3DBarController>().updateData(heights);

            visObject.transform.parent = visObjectPool.transform;
        }
    }

    public void showSuperimposedBoxes()
    {
        instantiateVisObjects(visPrefeb3DBoxes);
    }
    public void showSuperimposed3DBox()
    {
        instantiateVisObjects(visPrefeb3DBox);
    }
    public void showSuperimposed2DBars()
    {
        instantiateVisObjects(visPrefeb2DBar);
    }

    public void increaseHeight()
    {
        //heightText.text = "Height: \n" + (FloorIndicator.transform.position.y + 0.1f);
        //FloorIndicator.transform.position = new Vector3(
        //    FloorIndicator.transform.position.x,
        //    FloorIndicator.transform.position.y + 0.1f, 
        //    FloorIndicator.transform.position.z);
    }

    public void decreaseHeight()
    {
        //heightText.text = "Height: \n" + (FloorIndicator.transform.position.y - 0.1f);
        //FloorIndicator.transform.position = new Vector3(
        //    FloorIndicator.transform.position.x,
        //    FloorIndicator.transform.position.y - 0.1f,
        //    FloorIndicator.transform.position.z);
    }
}
