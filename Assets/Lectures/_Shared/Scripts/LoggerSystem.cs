using System.Collections.Concurrent;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lectures._Shared.Scripts
{
    public class LoggerSystem : MonoBehaviour
    {
        [SerializeField] private TMP_Text logText;
        [SerializeField] private RectTransform logContent;

        private static LoggerSystem _instance;
        private static readonly ConcurrentQueue<string> LOGMessages = new();

        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void Update()
        {
            if (LOGMessages.TryDequeue(out string message))
            {
#if UNITY_SERVER
                Debugger.Log(0, "LoggerSystem", message);
#else
                Instantiate(logText, logContent).text = message;
#endif
            }
        }

        public static void Log(string message)
        {
            if (!_instance)
            {
                Debug.LogError("LoggerSystem instance is not set");
                return;
            }

            Debug.Log(message);
            LOGMessages.Enqueue(message);
        }
    }
}