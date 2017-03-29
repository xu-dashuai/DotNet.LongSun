using Lunson.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lunson.Web.Models;
using Lunson.Domain.Entities;

namespace Lunson.Web.Controllers
{
    public class TicketApiController : ApiController
    {

        private TicketService tsv = new TicketService();

        [HttpPost]
        public TicketsResult GetTicketsByTicketNos([FromBody]FormModel model)
        {
            var tickets = model.ID.Split(',').Where(a => a != null && a.Trim() != "").Select(a => a.Trim()).ToList();

            var allowTickets = new List<TicketDetail>();
            var inVailidateTickets = new List<TicketDetail>();
            var msg = string.Empty;
            allowTickets = tsv.ValidateTickets(tickets, out inVailidateTickets);
            tsv.UseTickets(allowTickets);
            var result = new TicketsResult();
            result.AllowTickets = allowTickets;
            result.InvalidateTickets = inVailidateTickets.Where(a=>a.BuyTime!=null).ToList();
            result.Message = msg;
            if (inVailidateTickets == null || inVailidateTickets.Count > 0)
            {
                result.Result = false;
            }
            else
            {
                result.Result = true;
            }
            return result;

        }

    }


    public class FormModel
    {
        public string ID { get; set; }
    }
}
