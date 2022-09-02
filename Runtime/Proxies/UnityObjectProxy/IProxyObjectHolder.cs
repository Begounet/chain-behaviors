namespace ChainBehaviors.Proxy
{
    /// <summary>
    /// Inherit from it to specificy a UnityEngine.Object to be a proxy owner.
    /// </summary>
    /// <typeparam name="T">Type of the proxy</typeparam>
    public interface IProxyObjectHolder<T> where T : UnityEngine.Object
    {
        T Proxy { get; set; }
    }
}
