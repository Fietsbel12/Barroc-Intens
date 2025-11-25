using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using MailKit.Security;
using MimeKit;

namespace BarrocIntens.View
{
    public sealed partial class ContactPage : Page
    {
        public ContactPage()
        {
            this.InitializeComponent();
        }

        private async void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string message = MessageBox.Text.Trim();

            // Controleer lege velden
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(message))
            {
                StatusText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                StatusText.Text = "Please fill in all fields.";
                return;
            }

            // Controleer geldig e-mailadres
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    StatusText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                    StatusText.Text = "Invalid email address.";
                    return;
                }
            }
            catch
            {
                StatusText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                StatusText.Text = "Invalid email address.";
                return;
            }

            // Maak de e-mail
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("BarrocIntens Contact Form", "velva93@ethereal.email")); // Ethereal account
            emailMessage.ReplyTo.Add(new MailboxAddress(name, email)); // Gebruikers e-mail
            emailMessage.To.Add(new MailboxAddress("BarrocIntens", "velva93@ethereal.email")); // Ontvanger (je Ethereal account zelf)
            emailMessage.Subject = "Contact Form Message";
            emailMessage.Body = new TextPart("plain") { Text = $"From: {name} <{email}>\n\n{message}" };

            try
            {
                StatusText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);
                StatusText.Text = "Sending email...";

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);

                    // Lees wachtwoord uit environment variable
                    string smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASS");

                    if (string.IsNullOrEmpty(smtpPassword))
                    {
                        StatusText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                        StatusText.Text = "SMTP password not set in environment variables!";
                        return;
                    }

                    // Authenticate met environment variable wachtwoord
                    await client.AuthenticateAsync("velva93@ethereal.email", smtpPassword);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                StatusText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
                StatusText.Text = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                StatusText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                StatusText.Text = $"Error sending email: {ex.Message}";
            }
        }
    }
}
