using Lunson.BLL;
using Lunson.DAL;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            //string a = "312331212.4353.5435345";
            //Console.WriteLine(a.GetExtension());
            //ValidateTickets();

            //string normalParms = "This is Normal Parms"; string outParms = "This is Out Parms"; string refParms = "This is Ref Parms";
            //Console.WriteLine("Now I'm Initialy");
            //Console.WriteLine(normalParms);
            //Console.WriteLine(outParms);
            //Console.WriteLine(refParms);
            //Console.WriteLine("========华丽的分割线=========");
            //TheDifferentOfNormalParmsOutParmsAndRefParms(normalParms, out outParms, ref refParms);
            //Console.WriteLine("Now I'm out of the Function");
            //Console.WriteLine(normalParms);
            //Console.WriteLine(outParms);
            //Console.WriteLine(refParms);
            //Console.ReadLine();
           
            //UseTicket();

            UseTickets();
        }

        public static void TheDifferentOfNormalParmsOutParmsAndRefParms(string normalParms, out string outParms, ref string refParms)
        {
            Console.WriteLine("Now I'm in the Function");
            normalParms = normalParms + " I'm the ext";
            outParms = " I'm the ext";
            refParms = refParms + " I'm the ext";
            Console.WriteLine(normalParms);
            Console.WriteLine(outParms);
            Console.WriteLine(refParms);
            Console.WriteLine("========华丽的分割线=========");
        }

        public static void ValidateTicket()
        {
            Lunson.BLL.Services.TicketService ts = new Lunson.BLL.Services.TicketService();
            var ticketNo = "E7C2973E42E04BC0BB2B18574A3ACE0C";//F91799ED1EAF49838FE5101F8044FAB5//AEDEDA87328349A1A1745D881C294621
            var ticket = new Lunson.Domain.Entities.TicketDetail();
            var result = ts.ValidateTicket(ticketNo, out ticket);
            var b = ticket;
        }

        public static void ValidateTickets()
        {
            Lunson.BLL.Services.TicketService ts = new Lunson.BLL.Services.TicketService();
            var ticketNos = new List<string>();
            ticketNos.Add("E7C2973E42E04BC0BB2B18574A3ACE0C");
            ticketNos.Add("F91799ED1EAF49838FE5101F8044FAB5");
            ticketNos.Add("AEDEDA87328349A1A1745D881C294621");
            ticketNos.Add("HHHHHHHHHHHBBBBBBBBBBBBCCCCCCCCC");
            var unValidatedTickets = new List<Lunson.Domain.Entities.TicketDetail>();
            var result = ts.ValidateTickets(ticketNos, out unValidatedTickets);
        }

        public static void UseTicket()
        {
            Lunson.BLL.Services.TicketService ts = new Lunson.BLL.Services.TicketService();
            var ticketNo = "E7C2973E42E04BC0BB2B18574A3ACE0C";
            var msg = "";
            ts.UseTicket(ticketNo,out  msg);
        
        }

        public static void UseTickets()
        {
            Lunson.BLL.Services.TicketService ts = new Lunson.BLL.Services.TicketService();
            var ticketNos=new List<string>();
            ticketNos.Add("F91799ED1EAF49838FE5101F8044FAB5");
            ticketNos.Add("0B633987A93F4A5186CA410700E55C12");
            ticketNos.Add("BF0E3BB11E1B43E48ABB180A3A991447");
            ticketNos.Add("F1A310339D5645568BDAE82180CE8409");
            var unUsedTickets = new List<Lunson.Domain.Entities.TicketDetail>();
            //var result = ts.UseTickets(ticketNos, out unUsedTickets);        
        }

    }
}
