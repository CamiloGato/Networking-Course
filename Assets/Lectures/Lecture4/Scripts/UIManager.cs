using System;
using TMPro;
using UnityEngine;

namespace Lectures.Lecture4.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public RequestManager requestManager;

        [Header("UI Components")] [SerializeField]
        private TMP_InputField userField;

        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_Text responseText;

        public async void Login()
        {
            string user = userField.text;
            string password = passwordField.text;

            string response = await requestManager.LoginAsync(user, password);

            if (response != null)
            {
                responseText.text = $"Token: {response}";
                requestManager.SetToken(response);
            }
            else
            {
                responseText.text = "Login failed";
            }

            print(response);
        }

        public async void CallUserEndPoint()
        {
            string response = await requestManager.CallSecureEndpointAsync("user");

            if (response != null)
            {
                responseText.text = response;
            }
            else
            {
                responseText.text = "Call failed";
            }

            print(response);
        }

        public async void CallAdminEndPoint()
        {
            string response = await requestManager.CallSecureEndpointAsync("admin");

            if (response != null)
            {
                responseText.text = response;
            }
            else
            {
                responseText.text = "Call failed";
            }

            print(response);
        }
    }
}