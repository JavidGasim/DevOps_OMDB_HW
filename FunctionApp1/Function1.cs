using System;
using System.Net.Http.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FunctionApp1
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(
           new ConfigurationOptions
           {
               EndPoints = { { "asdfasdfasdfasd"} },
               User = "default",
               Password = "xxxxxxxxxxxxxxxxxxx"
           }
       );

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task Run([QueueTrigger("moivesqueue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            try
            {
                string? queueMessage = message.Body.ToString();
                _logger.LogInformation($"Message: {queueMessage}");

                string omdbApiKey = "xxxxxxxx";
                string omdbApiUrl = $"http://www.omdbapi.com/?apikey={omdbApiKey}&t={queueMessage}";

                var response = await _httpClient.GetFromJsonAsync<MovieResponse>(omdbApiUrl);

                if (response == null || string.IsNullOrEmpty(response.Title) || string.IsNullOrEmpty(response.Poster))
                {
                    _logger.LogWarning("Problem in OMDB Api");
                    return;
                }

                _logger.LogInformation($"Film Name: {response.Title}, Film Poster: {response.Poster}");

                var db = _redis.GetDatabase();

                string movieKey = response.Title;
                string guidKey = Guid.NewGuid().ToString();

                await db.StringSetAsync(movieKey, response.Poster);
                await db.ListLeftPushAsync("images", response.Poster);

                _logger.LogInformation($"Succesfully: {movieKey}, Poster: {response.Poster}");
            }
            catch (Exception ex) { _logger.LogError($"Error: {ex.Message}"); }
        }
    }
}
