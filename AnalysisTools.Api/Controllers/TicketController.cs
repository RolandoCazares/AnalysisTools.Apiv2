using analysistools.api.Contracts;
using analysistools.api.Models.Tickets;
using Microsoft.AspNetCore.Mvc;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Gets ticket by serial number and family
        /// </summary>
        /// <returns>The ticket or tickets in plain text</returns>
        [HttpGet("{family}/{serialNumber}")]
        public ActionResult GetTickets(string family, string serialNumber)
        {
            try
            {
                TicketProduct product = new TicketProduct   
                {
                    SerialNumber = serialNumber,
                    Family = family
                };

                var result = _ticketService.GetTicket(product);
                return Ok(new { ticket = result });
            }
            catch (Exception)
            {
                return BadRequest("No Ticket Found");
            }
        }
    }
}
