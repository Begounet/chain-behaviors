using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.ChainBehaviors
{
    public class UnityEngineApplicationInterface : MonoBehaviour
    {
        public string BuildVersion => UnityEngine.Application.version;

        public void ExitApplication()
        {
            UnityEngine.Application.Quit();
        }
    }
}