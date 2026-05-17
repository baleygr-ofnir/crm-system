using api.Data.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace azfunc.Functions;


public class CustomerChangeNotifier
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CustomerChangeNotifier> _logger;

    public CustomerChangeNotifier(
        IConfiguration configuration,
        ILogger<CustomerChangeNotifier> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [Function($"CustomerChangeNotifier")]
    public async Task Run(
        [CosmosDBTrigger(
            databaseName: "%DatabaseName%",
            containerName: "%ContainerName%",
            Connection = "CosmosDbConnection",
            CreateLeaseContainerIfNotExists = true)]
        IReadOnlyList<Customer>? customers)
    {
        if (customers != null && customers.Count > 0)
        {
            foreach (var customer in customers)
            {
                _logger.LogInformation(
                    "Customer record modified: {CustomerID} | Name: {CustomerName}",
                    customer.Id,
                    customer.Name);

                await SendMail(customer);
            }
        }
    }

    public async Task SendMail(Customer customer)
    {
        var smtpHost = _configuration["SmtpHost"] ??
                       throw new ArgumentNullException("", "SmtpHost is missing");
        var smtpPort = _configuration.GetValue<int>("SmtpPort");
        var smtpUsername = _configuration["SmtpUsername"] ??
                           throw new ArgumentNullException("", "SmtpUsername is missing");
        var smtpPassword = _configuration["SmtpPassword"] ??
                           throw new ArgumentNullException("", "SmtpPassword is missing");
        var targetEmail = _configuration["TargetEmail"] ??
                          throw new ArgumentNullException("", "TargetEmail is missing");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("CRM Change Notifier",smtpUsername));
        message.To.Add(new MailboxAddress("Receiver", targetEmail));
        message.Subject = $"Customer {customer.Id} changed";

        string htmlTable = $@"
            <h2>Customer Change Notification</h2>
            <table border='1' cellpadding='8' style='border-collapse: collapse; font-family: Arial, sans-serif;'>
                <tr><th style='background-color:#f2f2f2; text-align: left;'>Customer Detail</th><th style='background-color: #f2f2f2; text-align: left;'>Value</th></tr>
                <tr><td><strong>ID</strong></td><td>{customer.Id}</td></tr>
                <tr><td><strong>Name</strong></td><td>{customer.Name}</td></tr>
                <tr><td><strong>Title</strong></td><td>{customer.Title}</td></tr>
                <tr><td><strong>Email</strong></td><td>{customer.Email}</td></tr>
                <tr><td><strong>Phone</strong></td><td>{customer.Phone}</td></tr>
                <tr><td><strong>Address</strong></td><td>{customer.Address}</td></tr>
                <tr><th style='background-color:#f2f2f2; text-align: left;'>Sales Representative Detail</th><th style='background-color: #f2f2f2; text-align: left;'>Value</th></tr>
                <tr><td><strong>Name</strong></td><td>{customer.SalesRep.Name}</td></tr>
                <tr><td><strong>Email</strong></td><td>{customer.SalesRep.Email}</td></tr>
                <tr><td><strong>Phone</strong></td><td>{customer.SalesRep.Phone}</td></tr>
            </table>";
        message.Body = new TextPart("html")
        {
            Text = htmlTable
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(smtpUsername, smtpPassword);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}