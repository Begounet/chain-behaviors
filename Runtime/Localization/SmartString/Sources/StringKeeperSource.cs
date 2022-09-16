#if USE_UNITY_LOCALIZATION
using ChainBehaviors.Text;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace ChainBehaviors.Localization.SmartString
{
    /// <summary>
    /// Allow to use <see cref="StringKeeper"/> as SmartString sources.
    /// </summary>
    public class StringKeeperSource : ISource
    {
        public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
        {
            var selector = selectorInfo.SelectorText;
            var formatDetails = selectorInfo.FormatDetails;

            if (int.TryParse(selector, out var selectorValue))
            {
                // Argument Index:
                // Just like String.Format, the arg index must be in-range,
                // should be the first item, and shouldn't have any operator:
                if (selectorInfo.SelectorIndex == 0
                    && selectorValue < formatDetails.OriginalArgs.Count
                    && selectorInfo.SelectorOperator == "")
                {
                    object arg = formatDetails.OriginalArgs[selectorValue];
                    if (arg is StringKeeper sk)
                    {
                        selectorInfo.Result = sk.String;
                        return true;

                    }
                }
            }
            return false;
        }
    }
}
#endif