namespace Utility
{
    public static class SoundFactory
    {
        public static ISample CreateSample(string locationOfSample, bool isMono) =>
            new Sample(locationOfSample, TrackAliasFactory.Create(isMono));
        public static IChord CreateChord(ISample[] samples) =>
            new Chord(samples, TrackAliasFactory.Create(false));
        public static ISequencer CreateSequencer(ISample[] samples, bool isMono) =>
            new Sequencer(samples, TrackAliasFactory.Create(isMono));
    }
}
