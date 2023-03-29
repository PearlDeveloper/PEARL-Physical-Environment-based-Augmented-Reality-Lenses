// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SampleQRCodes
{
    public class QRCodesVisualizer : MonoBehaviour
    {
        public GameObject qrCodePrefab_origin;
        public GameObject qrCodePrefab_objects;
        public GameObject positiveFilterBoxesPool;
        public AOIDataManager AOIDataManager;
        public TextMeshPro debuginfo;
        public System.Collections.Generic.SortedDictionary<string, GameObject> qrCodesObjects;

        private SortedDictionary<System.Guid, GameObject> qrCodesObjectsList;
        private bool clearExisting = false;

        struct ActionData
        {
            public enum Type
            {
                Added,
                Updated,
                Removed
            };
            public Type type;
            public Microsoft.MixedReality.QR.QRCode qrCode;

            public ActionData(Type type, Microsoft.MixedReality.QR.QRCode qRCode) : this()
            {
                this.type = type;
                qrCode = qRCode;
            }
        }

        private Queue<ActionData> pendingActions = new Queue<ActionData>();

        // Use this for initialization
        void Start()
        {
            Debug.Log("QRCodesVisualizer start");
            qrCodesObjectsList = new SortedDictionary<System.Guid, GameObject>();
            qrCodesObjects = new System.Collections.Generic.SortedDictionary<string, GameObject>();

            QRCodesManager.Instance.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;
            if (qrCodePrefab_origin == null)
            {
                throw new System.Exception("Prefab not assigned");
            }
        }
        private void Instance_QRCodesTrackingStateChanged(object sender, bool status)
        {
            if (!status)
            {
                clearExisting = true;
            }
        }

        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeAdded");

            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeUpdated");

            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeRemoved");

            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Removed, e.Data));
            }
        }

        void InstantinateQRObjects(ActionData action)
        {
            GameObject qrCodeObject = null;

            //
            for (int i = 1; i < 10; i++)
            {
                if (action.qrCode.Data.Equals("obj0" + i.ToString()))
                {
                    //
                    qrCodeObject = Instantiate(qrCodePrefab_objects, new Vector3(0, 0, 0), Quaternion.identity);

                    // scale of possible 
                    if (AOIDataManager.AOIDimsAndName.ContainsKey(action.qrCode.Data))
                    {
                        qrCodeObject.GetComponent<ScaleController>()
                            .AdjustScaleAccordingToDimension(AOIDataManager.AOIDimsAndName[action.qrCode.Data]);
                    }

                    // move to pool
                    qrCodeObject.transform.parent = positiveFilterBoxesPool.transform;

                    // add to list 
                    AOIDataManager.AOIs.Add(qrCodeObject);
                    qrCodeObject.transform.Find("AOI").GetComponent<PropertyManager>().GlobalID.text = (AOIDataManager.AOIs.Count + 1).ToString();
                    if (debuginfo) debuginfo.text = action.qrCode.Data + " recognized as QRCode!, ActionData.Type.Added\n" + debuginfo.text;
                }
            }
            //
            for (int i = 10; i < 21; i++) // current limit is 20
            {
                if (action.qrCode.Data.Equals("obj" + i.ToString()))
                {
                    //
                    qrCodeObject = Instantiate(qrCodePrefab_objects, new Vector3(0, 0, 0), Quaternion.identity);

                    // scale of possible 
                    if (AOIDataManager.AOIDimsAndName.ContainsKey(action.qrCode.Data))
                    {
                        qrCodeObject.GetComponent<ScaleController>()
                            .AdjustScaleAccordingToDimension(AOIDataManager.AOIDimsAndName[action.qrCode.Data]);
                    }

                    // move to pool
                    qrCodeObject.transform.parent = positiveFilterBoxesPool.transform;

                    // add to list 
                    AOIDataManager.AOIs.Add(qrCodeObject);
                    qrCodeObject.transform.Find("AOI").GetComponent<PropertyManager>().GlobalID.text = (AOIDataManager.AOIs.Count + 1).ToString();
                    if (debuginfo) debuginfo.text = action.qrCode.Data + " recognized as QRCode!, ActionData.Type.Added\n" + debuginfo.text;
                }
            }
            //
            if (action.qrCode.Data.Equals("origin"))
            {
                qrCodeObject = Instantiate(qrCodePrefab_origin, new Vector3(0, 0, 0), Quaternion.identity);
                if (debuginfo) debuginfo.text = "origin QR Code recognized!, ActionData.Type.Added\n" + debuginfo.text;
            }
            //
            if (action.qrCode.Data.Equals("floor"))
            {
                qrCodeObject = Instantiate(qrCodePrefab_origin, new Vector3(0, 0, 0), Quaternion.identity);
                if (debuginfo) debuginfo.text = "floor QR Code recognized!, ActionData.Type.Added\n" + debuginfo.text;
            }

            // main logic 
            if (qrCodeObject != null && !qrCodesObjects.ContainsKey(action.qrCode.Data))
            {
                qrCodeObject.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                qrCodeObject.GetComponent<QRCode>().qrCode = action.qrCode;
                qrCodesObjectsList.Add(action.qrCode.Id, qrCodeObject);
                qrCodesObjects.Add(action.qrCode.Data, qrCodeObject);
            }
        }

        private void HandleEvents()
        {
            lock (pendingActions)
            {
                while (pendingActions.Count > 0)
                {
                    var action = pendingActions.Dequeue();
                    if (action.type == ActionData.Type.Added)
                    {
                        InstantinateQRObjects(action);
                    }
                    else if (action.type == ActionData.Type.Updated)
                    {
                        if (!qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            InstantinateQRObjects(action);
                        }
                    }
                    else if (action.type == ActionData.Type.Removed)
                    {
                        if (qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            Destroy(qrCodesObjectsList[action.qrCode.Id]);
                            qrCodesObjectsList.Remove(action.qrCode.Id);
                        }
                    }
                }
            }
            if (clearExisting)
            {
                clearExisting = false;
                foreach (var obj in qrCodesObjectsList)
                {
                    Destroy(obj.Value);
                }
                qrCodesObjectsList.Clear();

            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleEvents();
        }
    }
}
