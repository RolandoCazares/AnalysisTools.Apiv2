namespace analysistools.api.Models.IDR
{
    public class Failure
    {
        public int Id { get; set; }
        public string Serial_Number { get; set; }
        public string StationID { get; set; }
        public string Test_status_Result { get; set; }
        public DateTime Date_Time { get; set; }
        public string MRK_WERT { get; set; }
        public string Work_Area { get; set; }
        public string Component { get; set; }
        public string ProductModel { get; set; }
        public string ID_TYPE { get; set; }
        public string ComponentPartNumber { get; set; }
        public string Defect_Mode { get; set; }
        public string AnalysisComment { get; set; }
    }
}
