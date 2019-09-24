using IEvangelist.GitHub.Services;
using IEvangelist.GitHub.Webhooks.Extensions;
using IEvangelist.GitHub.Webhooks.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.ProfanityFilter
{
    public class WebhookFunction
    {
        const string EventHeader = "X-GitHub-Event";
        const string DeliveryHeader = "X-GitHub-Delivery";
        const string SignatureHeader = "X-Hub-Signature";

        readonly IGitHubPayloadValidator _payloadValidator;
        readonly IGitHubWebhookDispatcher _webhookDispatcher;
        readonly ILogger<WebhookFunction> _logger;

        public WebhookFunction(
            IGitHubPayloadValidator payloadValidator,
            IGitHubWebhookDispatcher webhookDispatcher,
            ILogger<WebhookFunction> logger) =>
            (_payloadValidator, _webhookDispatcher, _logger) = (payloadValidator, webhookDispatcher, logger);

        [FunctionName("ProcessWebhook")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = null)] HttpRequest request)
        {
            try
            {
                if (request is null)
                {
                    _logger.LogError("Opps... something went terribly wrong!");
                }

                var signature = request.Headers.GetValueOrDefault(SignatureHeader);
                using var reader = new StreamReader(request.Body);
                var payloadJson = await reader.ReadToEndAsync();
                _logger.LogInformation(payloadJson);

                if (!_payloadValidator.IsPayloadSignatureValid(
                    Encoding.UTF8.GetBytes(payloadJson),
                    signature))
                {
                    _logger.LogError("Invalid GitHub webhook signature!");
                    return new StatusCodeResult(500);
                }

                var eventName = request.Headers.GetValueOrDefault(EventHeader);
                var deliveryId = request.Headers.GetValueOrDefault(DeliveryHeader);
                _logger.LogInformation($"Processing {eventName} ({deliveryId})");

                await _webhookDispatcher.DispatchAsync(eventName, payloadJson);

                return new OkObjectResult($"Successfully handled the {eventName} event.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new StatusCodeResult(500);
            }
        }
    }
}