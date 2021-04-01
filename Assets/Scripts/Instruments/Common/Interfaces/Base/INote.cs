public interface INote
{
    #region Play Events
    void PlayAt(float time, float volume);
    
    void PlayNow(float volume);
    #endregion Play Events

    #region Pitch Events
    void ResetPitch();
    
    void PitchUp(int numberOfSemitones);
    
    void PitchDown(int numberOfSemitones);
    #endregion Pitch Events

    #region Trim/Length Events
    void TrimStart(double timeToTrim);
    
    void TrimEnd(double timeToTrim);
    
    void Trim(double timeToTrimFromStart, double timeToTrimFromEnd);
    
    void ResetLength();
    #endregion Trim/Length Events
}
