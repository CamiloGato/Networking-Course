using System;
using Unity.Netcode;
using UnityEngine;

namespace Lectures.Lecture2.Scripts
{
    [Serializable]
    public enum ConnectionType
    {
        Host,
        Client,
        Server,
        Shutdown
    }

    [RequireComponent(typeof(NetworkManager))]
    public class SampleNetworkManager : MonoBehaviour
    {
        public bool isServer;
        private NetworkManager _networkManager;

        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
        }

        private void Start()
        {
            if (isServer)
            {
                StartServer();
            }
        }

        #region Connection Methods

        public void CreateConnection(ConnectionType connectionType)
        {
            // Check if connection is already created so shutdown the connection
            if (_networkManager.IsClient || _networkManager.IsServer || _networkManager.IsHost)
            {
                connectionType = ConnectionType.Shutdown;
            }

            switch (connectionType)
            {
                case ConnectionType.Host:
                    StartHost();
                    break;
                case ConnectionType.Client:
                    StartClient();
                    break;
                case ConnectionType.Server:
                    StartServer();
                    break;
                case ConnectionType.Shutdown:
                    _networkManager.Shutdown();
                    break;
            }
        }

        private void StartHost()
        {
            _networkManager.StartHost();
        }

        private void StartClient()
        {
            _networkManager.StartClient();
        }

        private void StartServer()
        {
            _networkManager.StartServer();
        }

        #endregion

        #region Properties Methods

        public string GetMode()
        {
            return _networkManager.IsHost ? "Host" : _networkManager.IsServer ? "Server" : "Client";
        }

        public string GetTransport()
        {
            return _networkManager.NetworkConfig.NetworkTransport.GetType().Name;
        }

        #endregion

        #region MovementMethods

        public void Move()
        {
            if (_networkManager.IsHost)
            {
                foreach (NetworkClient player in _networkManager.ConnectedClientsList)
                {
                    player.PlayerObject.GetComponent<SampleNetworkPlayer>().Move();
                }
            }
            else if (_networkManager.IsServer)
            {
                foreach (NetworkClient player in _networkManager.ConnectedClientsList)
                {
                    player.PlayerObject.GetComponent<SampleNetworkPlayer>().Move();
                }
            }
            else if (_networkManager.IsClient)
            {
                SampleNetworkPlayer player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<SampleNetworkPlayer>();
                player.Move();
            }
        }

        #endregion
    }
}