using UnityEngine;

public static class SemitoneToPitchHelper
{
    public static float ToFrequency(this int semitones)
    {
        return Mathf.Pow(1.05946f, semitones);
    }
}
