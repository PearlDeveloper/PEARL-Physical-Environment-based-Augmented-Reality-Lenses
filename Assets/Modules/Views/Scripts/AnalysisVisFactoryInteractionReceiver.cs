// ------------------------------------------------------------------------------------
// <copyright file="AnalysisVisFactoryInteractionReceiver.cs" company="Technische Universität Dresden">
//      Copyright (c) Technische Universität Dresden.
//      Licensed under the MIT License.
// </copyright>
// <author>
//      Wolfgang Büschel
// </author>
// ------------------------------------------------------------------------------------

using System.Collections.Generic;
using IMLD.MixedRealityAnalysis.Core;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace IMLD.MixedRealityAnalysis.Views
{
    /// <summary>
    /// This Unity component handles the interaction logic for the visualization factory.
    /// </summary>
    public class AnalysisVisFactoryInteractionReceiver : MonoBehaviour
    {
        public Interactable CenterDataBtn;
        public GameObject ConditionLabel;
        public GameObject DataLoadingGroup;
        public GameObject GeneralSettingsGroup;
        public GameObject HoloButtonPrefab;
        public GameObject InformationLabel;
        public GameObject ShowDataSetsBtn;
        public GameObject StudySessionLabel;
        public VisButton VisButtonPrefab;
        public GameObject VisButtonsGroup;
        private bool isStudyLoaded = false;
        private int selectedStudyCondition = -1;
        private int selectedStudySession = -1;
        private readonly List<GameObject> studyButtons = new List<GameObject>();
        private readonly List<VisButton> visButtons = new List<VisButton>();

        [PunRPC]
        public void OnApplyButtonSync()
        {
            List<int> sessions = new List<int>();
            List<int> conditions = new List<int>();
            if (selectedStudyCondition != -1)
            {
                conditions.Add(selectedStudyCondition);
            }
            else
            {
                for (int i = 0; i < Services.DataManager().CurrentStudy.Conditions.Count; i++)
                {
                    conditions.Add(i);
                }
            }

            if (selectedStudySession >= 0)
            {
                sessions.Add(selectedStudySession);
            } 
            else if(selectedStudySession == -1)
            {
                for (int i = 0; i < Services.DataManager().CurrentStudy.Sessions.Count; i++)
                {
                    sessions.Add(i);
                }
            }
            else if (selectedStudySession == -2) // add sessions, groupPH
            {
                for (int i = 0; i < 3; i++)
                {
                    sessions.Add(i);
                }
            }
            else if (selectedStudySession == -3) // add sessions, groupOH
            {
                for (int i = (int)(Services.DataManager().CurrentStudy.Sessions.Count / 2.0f); 
                    i < Services.DataManager().CurrentStudy.Sessions.Count; i++)
                {
                    sessions.Add(i);
                }
            }

            Services.StudyManager().UpdateSessionFilter(sessions, conditions); // update in study manager, also sends it to other clients

        }

        /// <summary>
        /// Handles pressing of the Apply button.
        /// </summary>
        public void OnApplyButton()
        {
            OnApplyButtonSync();
            GetComponent<PhotonView>().RPC("OnApplyButtonSync", RpcTarget.Others);
        }

        /// <summary>
        /// Handles pressing of the Back button in the main study panel.
        /// </summary>
        public void OnBackButton()
        {
            ShowDataSetsBtn.SetActive(true);
            if (isStudyLoaded)
            {
                selectedStudySession = -1;
                StudySessionLabel.GetComponent<TextMeshPro>().text = "all";
                    //Services.DataManager().CurrentStudy.Sessions[selectedStudySession].Name;
                selectedStudyCondition = 0;
                ConditionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Conditions[selectedStudyCondition];
                InformationLabel.SetActive(true);
                GeneralSettingsGroup.SetActive(true);
                VisButtonsGroup.SetActive(true);
                DataLoadingGroup.SetActive(false);
            }
            else
            {
                InformationLabel.SetActive(false);
                GeneralSettingsGroup.SetActive(false);
                VisButtonsGroup.SetActive(false);
                DataLoadingGroup.SetActive(false);
                ShowDataSetsBtn.SetActive(true);
            }
        }

        /// <summary>
        /// Gets called by toggling the center data button on.
        /// </summary>
        public void OnCenterDataButtonDisabled()
        {
            Services.VisManager().CenterData(false);
        }

        /// <summary>
        /// Gets called by toggling the center data button off.
        /// </summary>
        public void OnCenterDataButtonEnabled()
        {
            Services.VisManager().CenterData(true);
        }

        [PunRPC]
        public void OnLoadDataButtonSync(int id)
        {
            Services.StudyManager().LoadStudy(id);
        }

        /// <summary>
        /// Handles pressing of the Load button for a specific data set.
        /// </summary>
        /// <param name="id">The id of the study that should be loaded.</param>
        public void OnLoadDataButton(int id)
        {
            OnLoadDataButtonSync(id);
            GetComponent<PhotonView>().RPC("OnLoadDataButtonSync", RpcTarget.Others, id);
        }

        /// <summary>
        /// Handles pressing the Next Condition button.
        /// </summary>
        public void OnNextConditionButton()
        {
            if (selectedStudyCondition == -1)
            {
                // currently "all conditions", go to first
                selectedStudyCondition = 0;
                ConditionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Conditions[selectedStudyCondition];
            }
            else if (selectedStudyCondition < (Services.DataManager().CurrentStudy.Conditions.Count - 1))
            {
                // currently one of the conditions but not the last, go to next
                selectedStudyCondition++;
                ConditionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Conditions[selectedStudyCondition];
            }
            else
            {
                // currently the last condition, go to "all"
                selectedStudyCondition = -1;
                ConditionLabel.GetComponent<TextMeshPro>().text = "all";
            }
        }

        /// <summary>
        /// Handles pressing the Next Session button.
        /// </summary>
        public void OnNextStudySessionButton()
        {
            if (selectedStudySession == -1)
            {
                // currently "all sessions", go to first
                selectedStudySession = 0;
                StudySessionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Sessions[selectedStudySession].Name;

            }

            else if (selectedStudySession == -2)
            {
                selectedStudySession = -1;
                StudySessionLabel.GetComponent<TextMeshPro>().text = "all";

            }

            else if (selectedStudySession == -3)
            {
                selectedStudySession = -2;
                StudySessionLabel.GetComponent<TextMeshPro>().text = "groupPH";
            }

            else if (selectedStudySession < (Services.DataManager().CurrentStudy.Sessions.Count - 1))
            {
                // currently one of the sessions but not the last, go to next
                selectedStudySession++;
                StudySessionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Sessions[selectedStudySession].Name;
            }

            else if (selectedStudySession == (Services.DataManager().CurrentStudy.Sessions.Count - 1))
            {
                // currently the last session, go to -3, "groupOH"
                selectedStudySession = -3;
                StudySessionLabel.GetComponent<TextMeshPro>().text = "groupOH";
            }
        }

        /// <summary>
        /// Handles pressing the Previous Condition button.
        /// </summary>
        public void OnPreviousConditionButton()
        {
            if (selectedStudyCondition == -1)
            {
                // currently "all conditions", go to last
                selectedStudyCondition = Services.DataManager().CurrentStudy.Conditions.Count - 1;
                ConditionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Conditions[selectedStudyCondition];
            }
            else if (selectedStudyCondition > 0)
            {
                // currently one of the conditions but not the first, got to previous
                selectedStudyCondition--;
                ConditionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Conditions[selectedStudyCondition];
            }
            else
            {
                // currently the first condition, got to "all"
                selectedStudyCondition = -1;
                ConditionLabel.GetComponent<TextMeshPro>().text = "all";
            }
        }

        /// <summary>
        /// Handles pressing the Previous Session button.
        /// </summary>
        public void OnPreviousStudySessionButton()
        {
            if (selectedStudySession == -1)
            {
                selectedStudySession = -2;
                StudySessionLabel.GetComponent<TextMeshPro>().text = "groupPH";
            }
            else if (selectedStudySession == -2)
            {
                selectedStudySession = -3;
                StudySessionLabel.GetComponent<TextMeshPro>().text = "groupOH";
            }
            else if (selectedStudySession == -3)
            {
                // currently "groupOH", go to last
                selectedStudySession = Services.DataManager().CurrentStudy.Sessions.Count - 1;
                StudySessionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Sessions[selectedStudySession].Name;
            }
            else if (selectedStudySession > 0)
            {
                // currently one of the sessions but not the first, got to previous
                selectedStudySession--;
                StudySessionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Sessions[selectedStudySession].Name;
            }
            else if (selectedStudySession == 0)
            {
                // currently the first session, go to "all"
                selectedStudySession = -1;
                StudySessionLabel.GetComponent<TextMeshPro>().text = "all";
            }
        }

        /// <summary>
        /// Handles pressing the Show Datasets Button. Displays the available datasets in the main study panel, deactivates study controls and vis buttons.
        /// </summary>
        public void OnShowDataSetButtons()
        {
            ShowDataSetsBtn.SetActive(false);
            GeneralSettingsGroup.SetActive(false);
            VisButtonsGroup.SetActive(false);
            DataLoadingGroup.SetActive(true);
        }

        private void InitializeDataBaseButtons()
        {
            int counter = 0;
            foreach (var study in Services.DataManager().StudyList)
            {
                var studyButton = Instantiate(HoloButtonPrefab, DataLoadingGroup.transform);
                studyButton.name = "StudyButton";
                studyButton.transform.localPosition = new Vector3(0.1f + (0.08f * counter), -0.02f, -0.005f); // display buttons next to each other
                studyButton.transform.localScale = new Vector3(2, 2, 1);
                var helper = studyButton.GetComponent<ButtonConfigHelper>();
                if (helper)
                {
                    helper.SeeItSayItLabelEnabled = false;
                    helper.MainLabelText = study.StudyName;
                    helper.IconSet = ShowDataSetsBtn.GetComponent<ButtonConfigHelper>().IconSet;
                    helper.SetSpriteIconByName("IconDB");
                    helper.OnClick.AddListener(() => OnLoadDataButton(study.Id));
                }

                studyButton.SetActive(true);
                studyButtons.Add(studyButton);

                counter++;
            }
        }

        private void OnDataCentered(bool isCentering)
        {
            CenterDataBtn.IsToggled = isCentering;
        }

        private void OnSessionFilterChange()
        {
            //var sessions = Services.StudyManager().CurrentStudySessions;
            //var conditions = Services.StudyManager().CurrentStudyConditions;

            //if (sessions != null && sessions.Count == 1)
            //{
            //    selectedStudySession = sessions[0];
            //    StudySessionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Sessions[selectedStudySession].Name;
            //}
            //else
            //{
            //    selectedStudySession = -1;
            //    StudySessionLabel.GetComponent<TextMeshPro>().text = "all";
            //}

            //if (conditions != null && conditions.Count == 1)
            //{
            //    selectedStudyCondition = conditions[0];
            //    ConditionLabel.GetComponent<TextMeshPro>().text = Services.DataManager().CurrentStudy.Conditions[selectedStudyCondition];
            //}
            //else
            //{
            //    selectedStudyCondition = -1;
            //    ConditionLabel.GetComponent<TextMeshPro>().text = "all";
            //}
        }

        private void OnStudyLoaded(int index)
        {
            isStudyLoaded = true;
            InformationLabel.GetComponent<TextMeshPro>().text = "Study: [" + Services.DataManager().CurrentStudy.StudyName + "] Sessions: [" + Services.DataManager().CurrentStudy.Sessions.Count + "] Conditions: [" + Services.DataManager().CurrentStudy.Conditions.Count + "]";

            OnBackButton();

            // show the vis at the beginning 
            foreach (var btn in visButtons)
                btn.GetComponent<VisButton>().OnEnableVis();
            OnApplyButton();
        }

        public bool fbLoadDataset = false;
        public bool fbEnableAllVis = false;
        public bool fbDisableAllVis = false;
        public bool fbEnableAvatarTrails = false;
        public bool fbDisableAvatarTrails = false;

        void fDisableAllVis()
        {
            foreach (var btn in visButtons)
                btn.GetComponent<VisButton>().OnDisableVis();
            OnApplyButton();
        }

        void fEnableAllVis()
        {
            foreach (var btn in visButtons)
                btn.GetComponent<VisButton>().OnEnableVis();
            OnApplyButton();
        }

        void fEnableAvatarTrails()
        {
            visButtons[0].GetComponent<VisButton>().OnEnableVis();
            visButtons[1].GetComponent<VisButton>().OnEnableVis();
            visButtons[2].GetComponent<VisButton>().OnDisableVis();
            OnApplyButton();
        }

        void fDisableAvatarTrails()
        {
            visButtons[0].GetComponent<VisButton>().OnDisableVis();
            visButtons[1].GetComponent<VisButton>().OnDisableVis();
            visButtons[2].GetComponent<VisButton>().OnEnableVis();
            OnApplyButton();
        }

        void fLoadDataset()
        {
            OnLoadDataButton(0);
        }

        public void Update()
        {
            if (fbDisableAllVis)
            {
                fbDisableAllVis = false;
                fDisableAllVis();
            }

            if (fbEnableAvatarTrails)
            {
                fbEnableAvatarTrails = false;
                fEnableAvatarTrails();
            }

            if (fbDisableAvatarTrails)
            {
                fbDisableAvatarTrails = false;
                fDisableAvatarTrails();
            }

            if (fbEnableAllVis)
            {
                fbEnableAllVis = false;
                fEnableAllVis();
            }

            if (fbLoadDataset)
            {
                fbLoadDataset = false;
                fLoadDataset();
            }
        }

        private void OnVisualizationCreated(VisProperties settings)
        {
            foreach (var visButton in visButtons)
            {
                if (visButton.VisType == (VisType)settings.VisType)
                {
                    visButton.SetActive(true);
                    return;
                }
            }
        }

        private void OnVisualizationDeleted(VisProperties settings)
        {
            foreach (var visButton in visButtons)
            {
                if (visButton.VisType == (VisType)settings.VisType)
                {
                    visButton.SetActive(false);
                    return;
                }
            }
        }

        int viewIDStart = 60;

        private void Start()
        {
            if (Services.DataManager().IsInitialized)
            {
                InitializeDataBaseButtons();
                ShowDataSetsBtn.SetActive(false);
                InformationLabel.SetActive(false);
            }
            else
            {
                Services.DataManager().StudyListReady.AddListener(() =>
                {
                    InitializeDataBaseButtons();
                    ShowDataSetsBtn.SetActive(false);
                    InformationLabel.SetActive(false);
                });
            }

            // generate visualization buttons
            Vector3 z_offset = new Vector3(0f, 0f, -0.005f);
            foreach (var prefab in Services.VisManager().VisualizationPrefabs)
            {
                if (prefab.Is3D)
                {
                    var visButton = Instantiate(VisButtonPrefab, VisButtonsGroup.transform);
                    visButton.VisType = prefab.VisType;
                    visButton.AnchorId = -1;
                    visButton.transform.position += z_offset;
                    var helper = visButton.SpawnButton.GetComponent<ButtonConfigHelper>();
                    visButton.GetComponent<PhotonView>().ViewID = viewIDStart;
                    viewIDStart++;

                    if (helper)
                    {
                        helper.SeeItSayItLabelEnabled = false;
                        helper.MainLabelText = prefab.VisType.ToString();
                        helper.SetSpriteIconByName("Icon3D");
                    }
                    visButtons.Add(visButton);
                }
            }

            var collection = VisButtonsGroup.gameObject.GetComponent<GridObjectCollection>();
            collection.UpdateCollection();

            Services.StudyManager().SessionFilterEventBroadcast.AddListener(OnSessionFilterChange);
            Services.StudyManager().StudyChangeBroadcast.AddListener(OnStudyLoaded);
            Services.VisManager().VisualizationCreatedEventBroadcast.AddListener(OnVisualizationCreated);
            Services.VisManager().VisualizationDeletedEventBroadcast.AddListener(OnVisualizationDeleted);
            Services.VisManager().DataCenteringEventBroadcast.AddListener(OnDataCentered);

            // default vis 
            ShowDataSetsBtn.SetActive(false);
            GeneralSettingsGroup.SetActive(false);
            VisButtonsGroup.SetActive(false);
            DataLoadingGroup.SetActive(true);
        }
    }
}