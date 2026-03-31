using System;
using UnityEngine;

public static class UIEvents
{
    public static Action<string, Color?> OnShowNotification;
    public static void SendNotification(string msg, Color col) => OnShowNotification?.Invoke(msg, col);

    public static Action<string> OnRequestInteract;
    public static Action OnHideInteract;

    public static void TriggerInteract(string msg) => OnRequestInteract?.Invoke(msg);
    public static void TriggerHideInteract() => OnHideInteract?.Invoke();
}