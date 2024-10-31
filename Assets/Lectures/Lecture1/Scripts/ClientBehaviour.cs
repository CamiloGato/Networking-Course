using Lectures._Shared.Scripts;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

namespace Lectures.Lecture1.Scripts
{
    struct ClientUpdateJob : IJob
    {
        public NetworkDriver Driver;
        public NativeArray<NetworkConnection> Connection;
        public NativeArray<byte> Done;

        public void Execute()
        {
            if (!CheckConnection()) return;

            NetworkEvent.Type cmd;
            while ((cmd = Connection[0].PopEvent(Driver, out DataStreamReader stream)) != NetworkEvent.Type.Empty)
            {
                switch (cmd)
                {
                    case NetworkEvent.Type.Connect:
                    {
                        ConnectServer();
                        break;
                    }
                    case NetworkEvent.Type.Data:
                    {
                        ReadData(stream);
                        break;
                    }
                    case NetworkEvent.Type.Disconnect:
                    {
                        DisconnectServer();
                        break;
                    }
                }
            }
        }

        private void DisconnectServer()
        {
            LoggerSystem.Log("Disconnected from server");
            Connection[0] = default;
        }

        private void ReadData(DataStreamReader stream)
        {
            uint value = stream.ReadUInt();
            LoggerSystem.Log("Got the value = " + value);
            Done[0] = 1;

            Connection[0].Disconnect(Driver);
            Connection[0] = default;
        }

        private void ConnectServer()
        {
            LoggerSystem.Log("Connected to server");

            uint value = 1;
            if (Connection[0].IsCreated && Connection[0].GetState(Driver) == NetworkConnection.State.Connected)
            {
                Driver.BeginSend(Connection[0], out DataStreamWriter writer);
                writer.WriteUInt(value);
                Driver.EndSend(writer);
            }
        }

        private bool CheckConnection()
        {
            if (!Connection[0].IsCreated)
            {
                if (Done[0] != 1)
                {
                    LoggerSystem.Log("Failed to connect to server");
                }

                return false;
            }

            return true;
        }
    }

    public class ClientBehaviour : MonoBehaviour
    {
        private NetworkDriver _driver;
        private NativeArray<NetworkConnection> _connection;
        private NativeArray<byte> _done;
        private JobHandle _clientJobHandle;

        private bool _isClientStarted;

        public void StartClient()
        {
            _driver = NetworkDriver.Create();
            _connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
            _done = new NativeArray<byte>(1, Allocator.Persistent);

            NetworkEndpoint endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(8080);
            _connection[0] = _driver.Connect(endpoint);

            _isClientStarted = true;
            LoggerSystem.Log("Client sent connection request");
        }

        private void OnDestroy()
        {
            if (!_clientJobHandle.IsCompleted)
            {
                _clientJobHandle.Complete();
            }

            if (_driver.IsCreated)
            {
                _driver.Dispose();
            }

            if (_connection.IsCreated)
            {
                _connection.Dispose();
            }

            if (_done.IsCreated)
            {
                _done.Dispose();
            }
        }

        private void Update()
        {
            if (!_isClientStarted) return;

            _clientJobHandle.Complete();

            if (_done[0] == 1) return;

            ClientUpdateJob clientUpdateJob = new ClientUpdateJob
            {
                Driver = _driver,
                Connection = _connection,
                Done = _done
            };

            _clientJobHandle = clientUpdateJob.Schedule(_driver.ScheduleUpdate());
        }
    }
}