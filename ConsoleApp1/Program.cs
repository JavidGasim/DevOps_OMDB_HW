using Azure.Storage.Queues;

string connectionString = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
string queueName = "xxxxxx";

QueueClient queueClient = new QueueClient(connectionString, queueName);
await queueClient.CreateIfNotExistsAsync();

if (queueClient.Exists())
{
    while (true)
    {
        Console.WriteLine("Enter the name of movie: ");
        string movieName = Console.ReadLine();

        try
        {
            await queueClient.SendMessageAsync(movieName, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(20));
            Console.WriteLine($"Succesfull Sending: {movieName}");
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    };
}
else
{
    Console.WriteLine("Queue storage problem " + queueName);
}