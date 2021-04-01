using MonoBehaviours.Utility;

namespace AudioPlayBack
{
    public interface IPlayBack : IRetrievableFromGameManager
    {
        double CurrentTrackTime { get; }
        void ScheduleClip(in ClipAliasComponent clipAlias, in PlayEventComponent eventDetails,
            in TrackAliasComponent trackAlias);
    }
}