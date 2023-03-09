namespace analysistools.api.Models.FPY.HELPERS
{
    public class ReportFPYByProcessAndStation
    {
        public string Var { get; set; }

        public string Name { get; set; }

        public int TotalProduced { get; set; }

        public int TotalFailures { get; set; }

        public int Total { get; set; }

        public double FPY { get; set; }
    }
}
