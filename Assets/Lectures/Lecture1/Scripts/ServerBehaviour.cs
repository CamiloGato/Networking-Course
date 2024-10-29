using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lectures.Lecture1.Scripts
{
    struct ServerUpdateJob : IJobParallelForDefer
    {
        public NetworkDriver.Concurrent Driver;
        public NativeArray<NetworkConnection> Connections;

        public void Execute(int index)
        {
            Assert.IsTrue(Connections[index].IsCreated);

            NetworkEvent.Type cmd;
            while ((cmd = Driver.PopEventForConnection(Connections[index], out DataStreamReader stream)) != NetworkEvent.Type.Empty)
            {
                switch (cmd)
                {
                    case NetworkEvent.Type.Data:
                    {
                        uint number = stream.ReadUInt();
                        Debug.Log("Got the number " + number);
                        number += 2;

                        if (Connections[index].IsCreated)
                        {
                            Driver.BeginSend(NetworkPipeline.Null, Connections[index], out DataStreamWriter writer);
                            writer.WriteUInt(number);
                            Driver.EndSend(writer);
                        }
                        break;
                    }
                    case NetworkEvent.Type.Disconnect:
                    {
                        Debug.Log("Client disconnected from server");
                        Connections[index] = default;
                        break;
                    }
                }
            }
        }
    }

    struct ServerUpdateConnectionsJob : IJob
    {
        public NetworkDriver Driver;
        public NativeList<NetworkConnection> Connections;

        public void Execute()
        {
            // Clean up connections
            for (int i = Connections.Length - 1; i >= 0; i--)
            {
                if (!Connections[i].IsCreated || Connections[i].GetState(Driver) == NetworkConnection.State.Disconnected)
                {
                    Debug.Log("Removing inactive or disconnected connection");
                    Connections.RemoveAtSwapBack(i);
                }
            }

            // Accept new connections
            NetworkConnection connection;
            while ((connection = Driver.Accept()) != default)
            {
                Connections.Add(connection);
                Debug.Log("Accepted a connection");
            }
        }
    }

    public class ServerBehaviour : MonoBehaviour
    {
        private NetworkDriver _driver;
        private NativeList<NetworkConnection> _connections;
        private JobHandle _serverJobHandle;

        private void Start()
        {
            _connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
            NetworkSettings settings = new NetworkSettings();
            settings.WithNetworkConfigParameters(maxConnectAttempts: 10, maxFrameTimeMS: 10);
            _driver = NetworkDriver.Create(settings);

            NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4.WithPort(8080);
            if (_driver.Bind(endpoint) != 0)
            {
                Debug.LogError("Failed to bind to port 8080");
                return;
            }

            _driver.Listen();
        }

        private void OnDestroy()
        {
            if (!_serverJobHandle.IsCompleted)
            {
                _serverJobHandle.Complete();
            }

            if (_driver.IsCreated)
            {
                _driver.Dispose();
            }

            if (_connections.IsCreated)
            {
                _connections.Dispose();
            }
        }

        private void Update()
        {
            _serverJobHandle.Complete();

            ServerUpdateConnectionsJob connectionJob = new ServerUpdateConnectionsJob()
            {
                Driver = _driver,
                Connections = _connections
            };
            JobHandle connectionJobHandle = connectionJob.Schedule();
            connectionJobHandle.Complete();

            ServerUpdateJob serverUpdateJob  = new ServerUpdateJob()
            {
                Driver = _driver.ToConcurrent(),
                Connections = _connections.AsArray()
            };

            _serverJobHandle = serverUpdateJob.Schedule(_connections, 1, _driver.ScheduleUpdate());
            _serverJobHandle.Complete();
        }
    }
}