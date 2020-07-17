using System;
using UnityEngine;
using Unity.Entities;

public struct PlayEvent : IComponentData
{
    public float timeAfterStart;
    public float volume;
}

public struct ClipReference : IComponentData
{
    public int index;
    public float pitchAmount;
}

public struct TriggerNowTag : IComponentData { }

public struct TriggeredTag : IComponentData { }