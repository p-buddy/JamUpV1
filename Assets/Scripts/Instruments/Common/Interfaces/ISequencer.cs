public interface ISequencer : ITrackItem
{
    ISample Current { get; }
    ISample MoveNext();
    ISample BackToStart();
    void PitchAllUp(int numberOfSemitones);
    void PitchAllDown(int numberOfSemitones);
}
