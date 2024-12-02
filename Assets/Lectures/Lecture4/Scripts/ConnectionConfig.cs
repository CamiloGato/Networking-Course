using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lectures.Lecture4.Scripts
{
    [Serializable]
    public class EndpointData
    {
        public string name;
        public string endpoint;
    }

    [CreateAssetMenu(fileName = "ConnectionConfig", menuName = "Connection/Config", order = 0)]
    public class ConnectionConfig : ScriptableObject
    {
        public string url;
        public List<EndpointData> endpoints;
        public string token;

        public string GetEndPoint(string endpointName)
        {
            return endpoints.Find(endpoint => endpoint.name == endpointName)?.endpoint;
        }
    }
}