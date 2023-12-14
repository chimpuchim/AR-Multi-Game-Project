using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("login UI")] 
        [SerializeField] private InputField _playerNameInput;
        [SerializeField] private GameObject _loginUIGameObject;
    [Header("loading UI")] 
        [SerializeField] private GameObject _loadingUIGameObject;
        [SerializeField] private Text _loadingText;
        [SerializeField] private bool _showConnectionStatus;
    [Header("lobby UI")] 
        [SerializeField] private GameObject _lobbyUIGameObject;
        [SerializeField] private GameObject _3DUIGameObject;
        
        
    #region Unity Methods

        private void Awake()
        {
            if (PhotonNetwork.IsConnected)
            {
                _loadingUIGameObject.SetActive(false);
                _lobbyUIGameObject.SetActive(true);
                _3DUIGameObject.SetActive(true);
                _loginUIGameObject.SetActive(false);
                
                return;
            }
            
            _loadingUIGameObject.SetActive(false);
            _lobbyUIGameObject.SetActive(false);
            _3DUIGameObject.SetActive(false);
            _loginUIGameObject.SetActive(true);
        }

        private void Update()
        {
            if(_showConnectionStatus)
                _loadingText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
        }

        #endregion

    
    #region UI Callback Methods

        public void OnEnterGameBtnClick()
        {
            string playerName = _playerNameInput.text;

            if (!string.IsNullOrEmpty(playerName))
            {
                _loadingUIGameObject.SetActive(true);
                _lobbyUIGameObject.SetActive(false);
                _3DUIGameObject.SetActive(false);
                _loginUIGameObject.SetActive(false);

                _showConnectionStatus = true;
                
                if (!PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.LocalPlayer.NickName = playerName;
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
            else
            {
                Debug.Log("Nhập tên đi!!");
            }
        }
        
        public void OnQuickMatchButtonClicked()
        {
            SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
        }

    #endregion


    #region Photon Callback

        public override void OnConnected()
        {
            Debug.Log("Connected!");
        }

        public override void OnConnectedToMaster()
        {
            _loadingUIGameObject.SetActive(false);
            _lobbyUIGameObject.SetActive(true);
            _3DUIGameObject.SetActive(true);
            _loginUIGameObject.SetActive(false);
        }

        #endregion
}
