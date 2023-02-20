using System.Text.Json.Serialization;

namespace analysistools.api.Models.FPY
{
    public class ProducedUnitFPY
    {
        public string ID { get; set; }

        public DateTime DIA { get; set; }

        public string MODELO { get; set; }

        public string ESTACION { get; set; }

        public string PROCESO { get; set; }

        public string PRODUCTO { get; set; }

        public int CANTIDAD { get; set; }

    }
}
