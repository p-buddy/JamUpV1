using System;
using MonoBehaviours.Utility;
using UnityEngine;

public interface IAudioClipPlayer : IRetrievableFromGameManager
{
    void Schedule(AudioClip clip, double when, float pitch, float volume, Guid trackID, bool isMono = true);
    void Now(AudioClip clip, float pitch, float volume, Guid trackID, bool isMono = true);
}
