using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models;
using analysistools.api.Models.Tickets;
using System.Text;

namespace analysistools.api.Services
{
    /// <summary>
    /// Ticket services allows to search for ICT short tickets.
    /// </summary>
    public class TicketService : ITicketService
    {
        private AppDbContext _context;
        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get short ticket from product.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Short ticket as a string.</returns>
        public string GetTicket(TicketProduct product)
        {
            try
            {
                var family = _context.Families.FirstOrDefault(f => f.Name == product.Family);
                if (family == null) return null;
                var ticketServer = _context.TicketServers.FirstOrDefault(t => t.FamilyId == family.Id);
                if (ticketServer == null) return null;
                string targetTicketDirectory = ticketServer.GetSourceTicketDirectory();
                string searchPattern = $"*{product.SerialNumber}*";
                string ticket = SearchTicketOnServer(targetTicketDirectory, searchPattern);
                return String.IsNullOrEmpty(ticket) ? "No ticket available" : ticket;
            }
            catch (Exception exception)
            {
                return $"Not available tickets: \r\n{exception.Message}";
            }
        }

        /// <summary>
        /// Join multiple ticket if they exist.
        /// </summary>
        /// <param name="ticketPaths"></param>
        /// <returns>Merged tickets with separator line.</returns>
        private string JoinTickets(string[] ticketPaths)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string ticket in ticketPaths)
            {
                stringBuilder.Append(File.ReadAllText(ticket));
                stringBuilder.Append("\r\n========================\r\n");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Search for tickets in the server.
        /// </summary>
        /// <param name="targetTicketDirectory"></param>
        /// <param name="searchPattern"></param>
        /// <returns>One signle ticket as string.</returns>
        private string SearchTicketOnServer(string targetTicketDirectory, string searchPattern)
        {
            string result = string.Empty;
            if (Directory.Exists(targetTicketDirectory))
            {
                string[] foundTickets = Directory.GetFiles(targetTicketDirectory, searchPattern);
                result = JoinTickets(foundTickets);
            }
            if (result == string.Empty)
            {
                result = "Not tickets available";
            }
            return result;
        }
    }
}
