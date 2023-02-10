using analysistools.api.Models.Authentication;
using analysistools.api.Models.Continental;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.Tickets
{
    /// <summary>
    /// Ticket server where the tickets are stored.
    /// </summary>
    public class TicketServer
    {        
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string SourceTicketDirectory { get; set; }
        public int FamilyId { get; set; }
        [JsonIgnore]
        public Family Family { get; set; }
        public int CredentialId { get; set; }
        [JsonIgnore]
        public WindowsCredential Credential { get; set; }

        public string GetSourceTicketDirectory()
        {
            return string.Format($"\\\\{IpAddress}\\{SourceTicketDirectory}");
        }
    }
}
