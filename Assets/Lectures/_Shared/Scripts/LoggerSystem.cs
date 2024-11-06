using System.Collections.Concurrent;
using TMPro;
using UnityEngine;

namespace Lectures._Shared.Scripts
{
    public class LoggerSystem : MonoBehaviour
    {
        [SerializeField] private TMP_Text logText;
        [SerializeField] private RectTransform logContent;

        #if UNITY_STANDALONE_WIN

        #endif

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

                Instantiate(logText, logContent).text = message;
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