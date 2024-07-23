namespace Hermes.Domain.Settings;

public class EmailSettings
{
    public string SmtpHost { get; set; } 
    public int SmtpPort { get; set; } 
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    public string PasswordResetEndpoint { get; set; } 
}