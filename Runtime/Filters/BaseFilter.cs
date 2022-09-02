using AUE;
using System;
using UnityEngine;

namespace ChainBehaviors.Filters
{
    public abstract class BaseFilter : BaseMethod
    {
        public enum EFilterMode
        {
            Equals,
            NotEquals
        }

        [SerializeField]
        protected EFilterMode _filterMode = EFilterMode.Equals;
    }

    public abstract class BaseFilter<TFilterIdentifier> : BaseFilter
    {
        [Serializable]
        public class FilterAction
        {
            [SerializeField]
            private TFilterIdentifier _filter = default;
            public TFilterIdentifier Filter => _filter;

            [SerializeField]
            private AUEEvent<TFilterIdentifier> _action = null;
            public AUEEvent<TFilterIdentifier> Action => _action;
        }

        [SerializeField]
        private FilterAction[] _filters = null;

        public void Filter(TFilterIdentifier identifier)
        {
            for (int i = 0; i < _filters.Length; ++i)
            {
                if (PassFilter(_filters[i].Filter, identifier))
                {
                    _filters[i].Action.Invoke(identifier);
                }
            }
        }

        private bool PassFilter(TFilterIdentifier identifierA, TFilterIdentifier identifierB)
        {
            switch (_filterMode)
            {
                case EFilterMode.Equals:
                    return identifierA.Equals(identifierB);
                case EFilterMode.NotEquals:
                    return !identifierA.Equals(identifierB);
                default:
                    throw new NotSupportedException($"'{_filterMode}' mode not supported");
            }
        }
    }
}
