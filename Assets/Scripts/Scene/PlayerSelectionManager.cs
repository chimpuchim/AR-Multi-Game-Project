﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] private Transform playerSwitcherTransform;
    [SerializeField] private GameObject[] spinnerTopModels;
    [SerializeField] private int playerSelectionNumber;

    [Header("UI")]
        [SerializeField] private TextMeshProUGUI playerModelType_Text;
        [SerializeField] private Button next_Button;
        [SerializeField] private Button previous_Button;

        [SerializeField] private GameObject uI_Selection;
        [SerializeField] private GameObject uI_AfterSelection;


    #region UNITY Methods

        void Start()
        {
            uI_Selection.SetActive(true);
            uI_AfterSelection.SetActive(false);

            playerSelectionNumber = 0;
        }

        void Update()
        {
            
        }
        
    #endregion

    #region UI Callback Methods
    
        public void NextPlayer()
        {
            playerSelectionNumber += 1;

            if (playerSelectionNumber >= spinnerTopModels.Length)
            {
                playerSelectionNumber = 0;
            }
            Debug.Log(playerSelectionNumber);


            next_Button.enabled = false;
            previous_Button.enabled = false;

            StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform ,90, 1.0f));

            if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
            {
                playerModelType_Text.text = "Attack";

            }
            else
            {
                playerModelType_Text.text = "Defend";

            }
        }

        public void PreviousPlayer()
        {
            playerSelectionNumber -= 1;
            if (playerSelectionNumber < 0)
            {
                playerSelectionNumber = spinnerTopModels.Length - 1;
            }
            Debug.Log(playerSelectionNumber);



            next_Button.enabled = false;
            previous_Button.enabled = false;


            StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));

            if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
            {
                playerModelType_Text.text = "Attack";
            }
            else
            {
                playerModelType_Text.text = "Defend";
            }
        }

        public void OnSelectButtonClicked()
        {
            uI_Selection.SetActive(false);
            uI_AfterSelection.SetActive(true);

            ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp); 
        }

        public void OnReSelectButtonClicked()
        {
            uI_Selection.SetActive(true);
            uI_AfterSelection.SetActive(false);
        }

        public void OnBattleButtonClicked()
        {
            SceneLoader.Instance.LoadScene("Scene_Gameplay");
        }

        public void OnBackButtonClicked()
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }

    #endregion


    #region Private Methods
    
        IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f)
        {
            Quaternion originalRotation = transformToRotate.rotation;
            Quaternion finalRotation = transformToRotate.rotation*Quaternion.Euler(axis*angle);

            float elapsedTime = 0.0f;
            while (elapsedTime < duration)
            {
                transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation,elapsedTime/duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transformToRotate.rotation = finalRotation;

            next_Button.enabled = true;
            previous_Button.enabled = true;
        }

    #endregion
}
