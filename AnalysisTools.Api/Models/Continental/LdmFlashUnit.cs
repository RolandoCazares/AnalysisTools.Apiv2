namespace analysistools.api.Models.Continental
{
    /// <summary>
    /// Specific object for LDM reflash rework.
    /// </summary>
    public class LdmFlashUnit
    {
        public string SerialNumber { get; set; }
        public string Date { get; set; }
        public int Shift { get; set; }
        public string EOL { get; set; }
        public string Pack { get; set; }
    }
}
