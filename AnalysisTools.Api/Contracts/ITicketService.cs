using analysistools.api.Models.Tickets;

namespace analysistools.api.Contracts
{
    public interface ITicketService
    {
        string GetTicket(TicketProduct product);
    }
}
