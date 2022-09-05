using ChainBehaviors.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainBehaviors
{
    /// <summary>
    /// Interface to act on <see cref="UnityEngine.Application"/>
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleMiscs + "UnityEngine Application (interface)")]
    public class UnityEngineApplicationInterface : BaseMethod
    {
        public string BuildVersion => Application.version;

        public void ExitApplication()
        {
            TraceCustomMethodName("Exit Application");
            Application.Quit();
        }
    }
}