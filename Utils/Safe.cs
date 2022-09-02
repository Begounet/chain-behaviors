using System;

namespace ChainBehaviors
{
    /// <summary>
    /// Allow to decrease defensive code. It also supports UnityEngine.Object null check.
    /// </summary>
    /// <example>
    /// Safe.Execute(audioSource, (audio) => audio.Volume = 0.5f); // Execute only if audioSource is not null
    /// float volume = Safe.Get(audioSource, 1.0f, (audio) => audio.Volume); // Returns audio.Volume or 1.0f is audioSource is null.
    /// </example>
    public static class Safe
    {
        public static TReturn Get<T, TReturn>(T left, TReturn defaultValue, Func<T, TReturn> right)
        {
            if (left != null)
            {
                return right.Invoke(left);
            }
            return defaultValue;
        }

        public static TReturn Get<T, TReturn>(T left, TReturn defaultValue, Func<TReturn> right)
        {
            if (left != null)
            {
                return right.Invoke();
            }
            return defaultValue;
        }

        public static void Execute<T>(T left, Action callback)
        {
            if (left != null)
            {
                callback();
            }
        }

        public static void Execute<T>(T left, Action<T> callback)
        {
            if (left != null)
            {
                callback(left);
            }
        }
    }
}
