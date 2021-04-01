using ClipManagement;
using MonoBehaviours;
using UnityEngine;

namespace AudioPlayBack
{
    public class PlayBack : IPlayBack
    {
        private readonly IAudioClipPlayer player;
        private readonly double offset;
        private readonly double startingDSPTime;

        public PlayBack(IAudioClipPlayer player, double delay = 0)
        {
            this.player = player;
            startingDSPTime = AudioSettings.dspTime;
            offset = delay;
        }

        #region IPlayBackImplementation
        public double CurrentTrackTime => AudioSettings.dspTime - startingDSPTime + offset;

        public void ScheduleClip(in ClipAliasComponent clipAlias, in PlayEventComponent eventDetails,
            in TrackAliasComponent trackAlias)
        {
            GameManager.Instance.TryFetch(out IClipRegister clipRegister);
            double dspTime = startingDSPTime + (eventDetails.TrackTime - offset);
            
            if (!clipRegister.TryGetClip(clipAlias, out AudioClip clip))
            {
                return;
            }
            
            player.Schedule(clip, dspTime, eventDetails.Pitch, eventDetails.Volume, trackAlias.ID);
        }
        #endregion
    }
}