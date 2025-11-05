#if ENABLE_UNIHper
using System;
using System.Collections.Concurrent;
using IOToolkit;
using UnityEngine.Events;
using UniRx;

public static class IODeviceRXConverter
{
    // 使用线程安全字典缓存不同设备与动作对应的UnityEvent<bool>
    private static readonly ConcurrentDictionary<
        (IODevice device, string actionName),
        UnityEvent<bool>
    > _actionEvents = new ConcurrentDictionary<(IODevice, string), UnityEvent<bool>>();

    private static readonly ConcurrentDictionary<
        (IODevice device, string actionName),
        UnityEvent<float>
    > _axisEvents = new ConcurrentDictionary<(IODevice, string), UnityEvent<float>>();

    // 获取或创建UnityEvent<bool>，仅绑定一次事件
    private static UnityEvent<bool> GetOrCreateDIEvent(IODevice device, string actionName)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));
        if (string.IsNullOrEmpty(actionName))
            throw new ArgumentNullException(nameof(actionName));

        var key = (device, actionName);
        return _actionEvents.GetOrAdd(
            key,
            _ =>
            {
                var evt = new UnityEvent<bool>();
                device.BindAction(actionName, InputEvent.IE_Pressed, () => evt.Invoke(true));
                device.BindAction(actionName, InputEvent.IE_Released, () => evt.Invoke(false));
                return evt;
            }
        );
    }

    /// <summary>
    /// 监听按下和释放事件，true 表示按下，false 表示释放。
    /// 多次调用对同一设备同一动作只绑定一次事件，返回共享的Observable。
    /// </summary>
    public static IObservable<bool> OnActionEventAsObservable(
        this IODevice device,
        string actionName
    )
    {
        var unityEvent = GetOrCreateDIEvent(device, actionName);
        // 使用Publish.RefCount保证多订阅共享事件，不重复绑定监听
        return unityEvent.AsObservable().Publish().RefCount();
    }

    /// <summary>
    /// 仅监听动作按下事件
    /// </summary>
    public static IObservable<bool> OnActionPressedAsObservable(
        this IODevice device,
        string actionName
    )
    {
        return OnActionEventAsObservable(device, actionName).Where(isPressed => isPressed);
    }

    /// <summary>
    /// 仅监听动作释放事件
    /// </summary>
    public static IObservable<bool> OnActionReleasedAsObservable(
        this IODevice device,
        string actionName
    )
    {
        return OnActionEventAsObservable(device, actionName).Where(isPressed => !isPressed);
    }

    private static UnityEvent<float> GetOrCreateADEvent(IODevice device, string axisName)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));
        if (string.IsNullOrEmpty(axisName))
            throw new ArgumentNullException(nameof(axisName));

        var key = (device, axisName);
        return _axisEvents.GetOrAdd(
            key,
            _ =>
            {
                var evt = new UnityEvent<float>();
                device.BindAxis(axisName, _val => evt.Invoke(_val));
                return evt;
            }
        );
    }

    public static IObservable<float> OnAxisAsObservable(this IODevice device, string axisName)
    {
        var unityEvent = GetOrCreateADEvent(device, axisName);
        return unityEvent.AsObservable().Publish().RefCount();
    }
}
#endif
