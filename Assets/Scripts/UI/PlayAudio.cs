using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayAudio : MonoBehaviour
{
    public void StartPlaying()
    {
        AudioController.Instance.StartPlaying();
    }
}
