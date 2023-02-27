namespace analysistools.api.Models.FPY.HELPERS
{
    public class TotalsFPYByProcess
    {
        public List<Dictionary<string, int>> TotalProducedByProcess { get; set; }

        public List<Dictionary<string, int>> TotalFailuresByProcess { get; set; }

        public List<Dictionary<string , double>> TotalFPYbyProcess { get; set; }
    }
}
