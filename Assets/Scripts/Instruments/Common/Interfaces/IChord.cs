public interface IChord : INote, ITrackItem
{
    ISample this[int index] { get; }
}