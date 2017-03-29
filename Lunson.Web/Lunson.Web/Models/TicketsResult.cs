using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web.Models
{
    public class TicketsResult
    {
        public bool Result;
        public List<TicketDetail> AllowTickets;
        public List<TicketDetail> InvalidateTickets;
        public string Message;
    }
}