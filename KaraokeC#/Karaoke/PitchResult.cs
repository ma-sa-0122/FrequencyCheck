namespace Karaoke
{
    public class PitchResult
    {
        // TimeSec: latency-adjusted timestamp (used for plotting)
        public double TimeSec { get; set; }
        // RawTimeSec: raw detection time (getCurrentTime() - halfFrame) before latency subtraction - used for latency estimation
        public double RawTimeSec { get; set; }
        public double Pitch { get; set; }
    }
}
