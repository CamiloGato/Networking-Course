using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Lectures.Lecture4.Scripts
{
    public class RequestManager : MonoBehaviour
    {
        [SerializeField] private ConnectionConfig config;

        public void SetToken(string token)
        {
            var tokenDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(token);
            config.token = tokenDict["token"];
        }

        public async UniTask<string> LoginAsync(string user, string password)
        {
            string endpoint = config.GetEndPoint("login");
            string url = $"{config.url}/{endpoint}";
            Uri uri = new Uri(url);

            var loginData = new
            {
                Username = user,
                Password = password
            };

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
            print(jsonData);

            using UnityWebRequest request = new UnityWebRequest(uri, "POST");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            return request.result == UnityWebRequest.Result.Success ? request.downloadHandler.text : null;
        }

        public async UniTask<string> CallSecureEndpointAsync(string secureEndpoint)
        {
            string endpoint = config.GetEndPoint(secureEndpoint);
            string url = $"{config.url}/{endpoint}";
            Uri uri = new Uri(url);

            using UnityWebRequest request = new UnityWebRequest(uri);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request.SetRequestHeader("Authorization", $"Bearer {config.token}");

            await request.SendWebRequest();

            return request.result == UnityWebRequest.Result.Success ? request.downloadHandler.text : null;
        }
    }
}