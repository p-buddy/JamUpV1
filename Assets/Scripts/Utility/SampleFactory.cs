namespace Utility
{
    public static class SoundFactory
    {
        public static ISample CreateSample(string locationOfSample) =>
            new Sample(locationOfSample, TrackAliasFactory.Create());
        public static IChord CreateChord(ISample[] samples) => new Chord(samples);
        public static ISequencer CreateSequencer(ISample[] samples) => new Sequencer(samples);
    }
}
