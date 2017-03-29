using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LonsunTicket
{
    public partial class TicketMsgItem : UserControl
    {
        public TicketMsgItem()
        {
            InitializeComponent();
            CreatedOn = DateTime.Now;
        }

        public DateTime CreatedOn { get;private set; }

    }
}
