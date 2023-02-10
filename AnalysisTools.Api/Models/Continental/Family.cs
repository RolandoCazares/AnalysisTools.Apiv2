using analysistools.api.Models.Optical;
using analysistools.api.Models.Tickets;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.Continental
{
    /// <summary>
    /// Product family (GBCM, LDM, FordGen3, ...)
    /// </summary>
    public class Family
    {        
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<OpticalStation> OpticalStations { get; set; }
        [JsonIgnore]
        public List<TicketServer> TicketServers{ get; set; }
    }
}
