#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using AUE;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ChainBehaviors.Utils;

namespace ChainBehaviors
{
    /// <summary>
    /// Trigger an event according to the index value.
    /// </summary>
    [AddComponentMenu(CBConstants.ModuleControlsPath + "Int Selector")]
    public class IntSelector : BaseMethod
    {
        [System.Serializable]
        private class Selection
        {
            [SerializeField]
#if ODIN_INSPECTOR
            [TableColumnWidth(20)]
#endif
            private int _index;
            public int Index => _index;

            [SerializeField]
            [Tooltip("The argument is the index value")]
            private AUEEvent<int> _selected;
            public AUEEvent<int> Selected => _selected;
        }

        [SerializeField]
#if ODIN_INSPECTOR
        [TableList]
#endif
        private Selection[] _selects;

        public void Select(int index)
        {
            Trace(("index", index));

            Selection selection = _selects.FirstOrDefault((select) => select.Index == index);
            if (selection != null)
            {
                selection.Selected.Invoke(index);
            }
        }
    }
}