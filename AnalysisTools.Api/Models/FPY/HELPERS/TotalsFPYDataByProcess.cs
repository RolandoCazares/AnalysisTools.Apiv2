namespace analysistools.api.Models.FPY.HELPERS
{
    public class TotalsFPYDataByProcess
    {
        public Dictionary<string, List<object>> ProducedByProcessData { get; set; }

        public Dictionary<string, List<object>> FailuresByProcessData { get; set; }

        public Dictionary<string, List<object>> Top3FailsByProcess { get; set; }
    }
}
