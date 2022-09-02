namespace ChainBehaviors.Proxy
{
    /// <summary>
    /// Base class for all event proxies.
    /// Event proxies are used to redirect an event or group multiple ones.
    /// It can also be used to "convert" a UnityEvent to an AUEEvent for more possibilities.
    /// </summary>
    /// <example>
    /// + [Button] => OnClick (UnityEvent) call
    ///  + Disable All UI : EventProxy (AUEEvent) -> multiple events to disable every UI in scene
    ///  + Start Cinematic : EventProxy (AUEEvent) -> one event to start a cinematic
    /// </example>
    public abstract class BaseEventsProxy : BaseMethod {}
}
