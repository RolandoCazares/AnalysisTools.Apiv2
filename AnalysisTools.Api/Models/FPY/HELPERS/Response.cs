namespace analysistools.api.Models.FPY.HELPERS
{
    public class Response
    {
        //public TotalsFPY Totals { get; set; }

        public List<ReportFPY> ReportFPY { get; set; }

        public List<ReportFPYByProcessAndStation> ReportFPYByProcessAndStation { get; set; }

        public List<ReportFPYByProcessAndModel> ReportFPYByProcessAndModel { get; set; }

        //public TotalsFPYByProcess TotalsByProcess { get; set; }
        public Dictionary<string, List<object>> ChartFPYPS { get; set; }

        public TotalsFPYDataByProcess DataByProcess { get; set; }

    }
}
