using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace LonsunTicket
{
    public partial class TicketApi : Form
    {
        string url = ConfigurationManager.AppSettings["TicketApiUrl"];
        public TicketApi()
        {
            InitializeComponent();
            ttbNo.Select();
            Timer_KeepFocus.Enabled = true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            SetMsg(ttbNo.Text);
            ttbNo.Focus();
        }

        private async void SetMsg(string nos)
        {
            try
            {
                lblStatus.Text = "";
                if (nos.Trim() == "")
                {
                    SetContent("二维码内无数据");
                    return;
                }

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                using (var client = new HttpClient(handler))
                {
                    var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"ID",nos.Trim()}
                });
                    ttbNo.Text = "";
                    ttbError.Text = "";
                    ttbNo.Focus();
                    var response = await client.PostAsync(url, content);
                    //确保HTTP成功状态值
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        lblStatus.Text = response.StatusCode.ToString();
                        var result = await response.Content.ReadAsStringAsync();
                        var ticketResult = JsonConvert.DeserializeObject<TicketsResult>(result);
                        SetContent(ticketResult, nos);
                    }
                    else
                    {
                        lblStatus.Text = response.StatusCode.ToString();
                    }
                }
            }
            catch 
            {
                SetContent("网络异常，无法连接到服务器！");
                PlaySS(Ss.ConnectLose);
            }
        }

        private void SetContent(TicketsResult result,string nos)
        {
            bool isplay = false;
            ClearContent();
            TicketMsgItem item = new TicketMsgItem();
            int ticketCount = nos.Split(',').Select(a => a.Trim()).Where(a => a != "").Distinct().Count();

            item.Controls.Add(GetLabel(string.Format("验票时间：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), DockStyle.Top));
            if (ticketCount > (result.AllowTickets.Count + result.InvalidateTickets.Count))
            {
                item.Controls.Add(GetLabel(string.Format("无效票：{0}张", ticketCount - (result.AllowTickets.Count + result.InvalidateTickets.Count)),DockStyle.Top));
                if (isplay == false)
                {
                    isplay = true;
                    PlaySS(Ss.UnVail);
                }
            }
            if (result.InvalidateTickets.Any())
            {
                item.Controls.Add(GetLabel(string.Format("异常：这{0}张已使用", result.InvalidateTickets.Count), DockStyle.Top));
                if (isplay == false)
                {
                    isplay = true;
                    PlaySS(Ss.Error);
                }
                ttbError.Text = string.Join("\r\n", result.InvalidateTickets.Where(a => a.IsUsed == YesOrNo.Yes).Select(a => a.Name +"\r\n"+a.TicketNO+ "\r\n刷票时间 " + ((DateTime)a.UsedTime).ToString("yyyy-MM-dd HH:mm:ss") + "\r\n"));
                ttbError.Text += "\r\n";
                ttbError.Text += string.Join("\r\n", result.InvalidateTickets.Where(a => a.RefundingTime != null).Select(a => a.Name+"\r\n"+a.TicketNO+"\r\n申请退票时间" + ((DateTime)a.RefundingTime).ToString("yyyy-MM-dd HH:mm:ss") + "\r\n"));

            }
            foreach(var x in result.AllowTickets.GroupBy(a=>a.Name))
            {
                item.Controls.Add(GetLabel(string.Format("{0}：{1}张", x.Key,x.Count()), DockStyle.Top));
                if (isplay == false)
                {
                    isplay = true;
                    PlaySS(Ss.Welcome);
                }
            }



            item.Dock = DockStyle.Top;
            pnContent.Controls.Add(item);
            ttbNo.Focus();
        }

        private void SetContent(string code)
        {
            ClearContent();
            TicketMsgItem item = new TicketMsgItem();

            item.Controls.Add(GetLabel(string.Format("验票时间：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), DockStyle.Top));
            item.Controls.Add(GetLabel(code, DockStyle.Top));

            item.Dock = DockStyle.Top;
            pnContent.Controls.Add(item);

            ttbNo.Focus();
        }

        private Label GetLabel(string code, DockStyle style)
        {
            float fontsize = 20;
            float.TryParse(ConfigurationManager.AppSettings["fontsize"], out fontsize);
            Label label = new Label();
            label.ForeColor = Color.Red;
            label.AutoSize = true;
            label.Text = code;
            label.Padding = new Padding(100, 10, 0, 10);
            label.Font = new Font(ConfigurationManager.AppSettings["fontfamily"], fontsize);
            label.Dock = style;

            return label;
        }

        private void ttbNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetMsg(ttbNo.Text);
                ttbNo.Focus();
            }
        }

        private void ClearContent()
        {
            int count = 5;
            int.TryParse(ConfigurationManager.AppSettings["msgcount"],out count);

            var items = pnContent.Controls.Find("TicketMsgItem", false);
            if (items.Count() > count-1)
            {
                var removes = items.OrderByDescending(a => ((TicketMsgItem)a).CreatedOn).Skip(count - 1);
                foreach (var x in removes)
                {
                    pnContent.Controls.Remove(x);
                }
            }
        }

        private enum Ss
        { 
            Welcome,
            Error,
            ConnectLose,
            UnVail
        }

        private void PlaySS(Ss s)
        {
            switch (s)
            {
                case Ss.UnVail:
                    Mp3Player.Play(Application.StartupPath + "\\unvail.wav");
                    break;
                case Ss.Welcome:
                    Mp3Player.Play(Application.StartupPath + "\\wel.wav");
                    break;
                case Ss.Error:
                    Mp3Player.Play(Application.StartupPath + "\\error.wav");
                    break;
                case Ss.ConnectLose:
                    Mp3Player.Play(Application.StartupPath + "\\lose.wav");
                    break;
                default:
                    return;
            }
        }

        private void Timer_KeepFocus_Tick(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void TicketApi_Activated(object sender, EventArgs e)
        {
            this.ttbNo.Select();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Timer_KeepFocus.Enabled = false;
            this.Close();
        }


    }
}
