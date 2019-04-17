using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Digitec
{
    public partial class DigitecPriceChecker : Form
    {
        string URL = ""; //TODO: add the digitec article url
        string emailAddress = ""; //TODO: add your email address
        string emailPassword = ""; //TODO: add your email password
        string emailSmtpServer = ""; //TODO: add the smtp server from your email provider
        string currentPrice = ""; //OPTIONAL: enter the price from the website
        
        public DigitecPriceChecker()
        {
            InitializeComponent();
            timer1.Interval = 60000; //60s (60000ms) is enough, please do not go under 10s (10000ms)
            timer1.Tick += Timer1_Tick;
            webBrowser1.Navigated += WebBrowser1_Navigated;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            webBrowser1.Navigate(URL);            
        }

        /// <summary>
        /// "parses" the html and calls sendEmail, if the price has changed
        /// may not work in the future, if digitec changes their website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            try
            {
                string html = webBrowser1.Document.Body.InnerHtml;
                if (html.Contains("<span class=\"product-price-currency\">"))
                {
                    int index = html.IndexOf("<span class=\"product-price-currency\">");
                    html = html.Remove(0, index == -1 ? 1 : index);
                    index = html.IndexOf("</div>");
                    html = html.Remove(index == -1 ? 1 : index);
                    index = html.IndexOf("</span>");
                    html = html.Remove(0, index == -1 ? 1 : index + 8);
                    string price = html;
                    Console.WriteLine(price);
                    if(price != currentPrice)
                    {
                        currentPrice = price;
                        sendEmail(price, emailAddress);
                    }
                }
                
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            timer1.Enabled = button1.Text == "Start";
            button1.Text = button1.Text == "Start" ?"Stop" : "Start";
            webBrowser1.Navigate(URL);
        }

        private void sendEmail(string price, string email)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Host = emailSmtpServer; 
            client.Credentials = new System.Net.NetworkCredential(emailAddress, emailPassword);
            mail.To.Add(new MailAddress(email, "Digitec Price Checker"));
            mail.From = new MailAddress(emailAddress, "Digitec Price Checker");
            mail.Subject = "Digitec price change.";
            mail.Body = "new price: " + price + "<br>" + URL;
            mail.IsBodyHtml = true;
            client.Send(mail);
        }
    }
}
