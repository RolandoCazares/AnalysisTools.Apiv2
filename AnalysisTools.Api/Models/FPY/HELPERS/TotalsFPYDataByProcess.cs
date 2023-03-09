namespace analysistools.api.Models.FPY.HELPERS
{
    public class TotalsFPYDataByProcess
    {
        public Dictionary<string, List<object>> ProducedByProcessAndCOMCELLData { get; set; }

        public Dictionary<string, List<object>> FailuresByProcessAndCOMCELLData { get; set; }

        public Dictionary<string, List<object>> ProducedByProcessAndMODELData { get; set; }

        public Dictionary<string, List<object>> FailuresByProcessAndMODELData { get; set; }

        public Dictionary<string, List<object>> Top3FailsByProcess { get; set; }
    }
}
