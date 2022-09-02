

# Chain Behaviors

A set of components allowing to make common operations via generic components instead of rewriting little specific scripts.

[Changelog](Documentation~/Changelog.md)

## Dependencies

*Minimum version Unity required: 2019.3*

- [UniTask](https://github.com/Cysharp/UniTask) (Required)
- [App Tools](https://gitea.apperture.fr:3062/Apperture/app-tools) (Required)
- [Adv Unity Event](https://gitea.apperture.fr:3062/Apperture/adv-unity-event) (Required)
- [ScriptableObject Architecture](https://intra.persistant.fr:3000/Apperture/scriptableobject-architecture) (Optional)
- [SO Architecture Extras](https://intra.persistant.fr:3000/Apperture/so-architecture-extras) (Optional)

## Concept

The main concept to catch behind the Chain Behaviors is that each one component make a little task and forward its result (or not) to another component to make a chain of behavior to execute.

![](Documentation~/Resources/Concept01.gif)

In this example, we have:

* Toggle: when click the toggle, it trigger the `OnValueChanged` event and send its result to a `If` behavior.
  * `If`: just split a bool value to 2 distinct events. Here, it makes 2 different call according to the toggle being switch on or off.
    * If on, call "> Set Red Light Color": the `Color Setter` behavior task is to trigger an event with the associated color. In our case, red.
    * If off, call "> Set Blue Light Color": the `Color Setter` will then trigger an event with the blue color.
      * The `Color Setter`,via the event, will change the light color.

This is a simple example but we can go way more complex! So it is also important to stay clear!

You may also want to be careful about the number of GameObjects you are going to create since it can grow pretty fast and make your scene heavier.

---

## Features

- (Almost) all chain behaviors have a "Log Trace", allowing to display that a method has been called (and can provide details about the call and arguments used).

  **You must add `CHAINBEHAVIOR_METHOD_TRACE` to your Scripting Define Symbols to enable log. Remove it if you want to remove these method calls to save performance.**

- "Debug Break" allows to break when Visual Studio is attached. Just goes up in the stacktrace pile to get more details on what's happening on a specific behavior.

### Behaviors

* Animation
  * `AnimatorClipPlayer`: Allow to play a specific clip on an Animator
  * `SetAnimatorParameter`: Provide a way to feed an Animator parameter via "Submit*" methods
* Audio
  * `AudioClipWaveformVisualizer`: Generate a texture representing an `AudioClip` waveform
  * `AudioListenerCaptureController`: Capture the audio from an `AudioListener` in a file
* Controls
  * Branch
    * `If`
    * `Int Selector`: trigger an event according to the int value entry
    * `Numeric Condition`: check a numeric condition on 2 floats
    * `Toggle Gate Controller`: act as a gate. If opened, the event can be executed, otherwise nothing happens
    * `Validity Checker`: check an object is valid (null). Unity Objects are supported.
  * `Caster`: cast an type in another (Odin is required)
  * `Color Setter`: trigger an event with a color
  * `Delay`: trigger an event after X seconds
  * `Gradient Color Setter`: trigger an event with a color from a gradient color, according to the normalized value passed as argument
  * `Instance Synchronizator`: instantiates a prefab attached to a GameObject. Not really a behavior, more than an utility class.
  * `Once Per Application Run`: trigger an event only once in an application run
* Graphics
  * `Graphics Settings Controller`: allow to change graphics settings
* IO
  * `Byte Buffer To File`: save a byte[] to a file
  * `Directory Deleter`
  * `File Stream Creator`
  * `Object To Asset`: save a `UnityEngine.Object` as an asset (editor only)
* Microphone
  * `Microphone Capture Controller`: record the microphone and save it to a file
* Proxies
  * Events: they allow to forward or group events calls. They are mainly used to put things in order or convert a `UnityEvent` to an `AUEEvent`
  * UnityObjectProxy: allow to bind a GameObject as a direct reference or from a prefab
* XR
  * `VR Mode Watcher`: at start, trigger an event telling if VR is enabled.

---

## Extends ChainBehaviors

* Create a new script and inherit from `BaseMethod`
* Do you what you want

### Logs

**You must add `CHAINBEHAVIOR_METHOD_TRACE` to your Scripting Define Symbols to enable log. Remove it if you want to remove these method calls to save performance.**

Inheriting from `BaseMethod` allows to use `Trace` and `TraceCustomMethodName` in your own behavior.

* `Trace`: log  `{GameObject Name}({Behavior name})` and optional details like arguments. You may want to use it when your behavior has only one method.
* `TraceCustomMethodName`: log `{GameObject Name}({Behavior name}):{Custom Method Name}` and optional details like arguments. You may want to use it when you have several method than can be called and want to precise which method has been called.

```csharp
Trace();
Trace(("value", value));
Trace(("key", key),
     ("value", value));

TraceCustomMethodName("MyCustomMethod");
TraceCustomMethodName("MyCustomMethod", ("key", key), ("value", value), ("target", myGameObject));
```

*Note that `UnityEngine.Object` passed as argument generate an hyperlink in editor so you can just click on it from the console to ping it.*