using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.ChainBehaviors
{
    public class Printer : MonoBehaviour
    {
        public void Log(string message) => Debug.Log(message);
        public void Log(object message) => Debug.Log(message);
        public void Log(bool message) => Debug.Log(message);
        public void LogWarning(object message) => Debug.LogWarning(message);
        public void LogError(object message) => Debug.LogError(message);
    }
}