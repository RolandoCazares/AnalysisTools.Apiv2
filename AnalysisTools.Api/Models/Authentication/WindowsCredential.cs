using analysistools.api.Models.Optical;
using analysistools.api.Models.Tickets;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.Authentication
{
    /// <summary>
    /// Windows credentials to access test stations.
    /// </summary>
    public class WindowsCredential
    {        
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public List<OpticalStation> OpticalStations { get; set; }
        [JsonIgnore]
        public List<TicketServer> TicketServers { get; set; }
    }
}
