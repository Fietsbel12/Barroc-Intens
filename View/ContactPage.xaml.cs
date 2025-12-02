using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit;
using Windows.UI; // nodig voor Color

namespace BarrocIntens.View
{
    public sealed partial class ContactPage : Page
    {
        private static readonly Color RedColor = Color.FromArgb(255, 255, 0, 0);
        private static readonly Color GreenColor = Color.FromArgb(255, 0, 128, 0);
        private static readonly Color BlackColor = Color.FromArgb(255, 0, 0, 0);

        public ContactPage()
        {
            this.InitializeComponent();
        }

        private async void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? string.Empty;
            string email = EmailBox.Text?.Trim() ?? string.Empty;
            string message = MessageBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(message))
            {
                SetStatus("Vul naam, e-mail en bericht in.", RedColor);
                return;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    SetStatus("Ongeldig e-mailadres.", RedColor);
                    return;
                }
            }
            catch
            {
                SetStatus("Ongeldig e-mailadres.", RedColor);
                return;
            }

            // UI: status en disable knoppen
            SetStatus("bericht wordt gestuurd...", BlackColor);
            if (SendButton != null) SendButton.IsEnabled = false;
            if (GoBack != null) GoBack.IsEnabled = false;

            try
            {
                var emailMessage = new MimeMessage();

                var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? "velva93@ethereal.email";
                var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS"); // verplicht
                var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.ethereal.email";
                var smtpPortStr = Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587";
                var toAddress = Environment.GetEnvironmentVariable("TO_ADDRESS") ?? "velva93@ethereal.email";

                if (string.IsNullOrEmpty(smtpPass))
                {
                    SetStatus("SMTP wachtwoord (SMTP_PASS) is niet ingesteld.", RedColor);
                    return;
                }

                if (!int.TryParse(smtpPortStr, out int smtpPort))
                    smtpPort = 587;

                emailMessage.From.Add(new MailboxAddress("BarrocIntens Contact Form", smtpUser));
                emailMessage.ReplyTo.Add(new MailboxAddress(name, email));
                emailMessage.To.Add(new MailboxAddress("BarrocIntens", toAddress));
                emailMessage.Subject = "Contact Form Message";
                emailMessage.Body = new TextPart("plain") { Text = $"From: {name} <{email}>\n\n{message}" };

                await SendWithMailKitAsync(emailMessage, smtpHost, smtpPort, smtpUser, smtpPass);

                SetStatus("bericht gestuurd ✔", GreenColor);

                // Clear velden
                NameBox.Text = "";
                EmailBox.Text = "";
                MessageBox.Text = "";
            }
            catch (Exception ex)
            {
                SetStatus($"Fout bij verzenden: {ex.Message}", RedColor);
            }
            finally
            {
                if (SendButton != null) SendButton.IsEnabled = true;
                if (GoBack != null) GoBack.IsEnabled = true;
            }
        }

        private async Task SendWithMailKitAsync(MimeMessage message, string host, int port, string user, string pass)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Timeout = 20000;
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(user, pass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame != null && Frame.CanGoBack)
                Frame.GoBack();
        }

        private void SetStatus(string text, Color color)
        {
            StatusText.Text = text;
            StatusText.Foreground = new SolidColorBrush(color);
        }
    }
}
