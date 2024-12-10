using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EventApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PublishController : ControllerBase
	{
		[HttpPost]
		public IActionResult PublishMessage([FromBody] string message)
		{
			var factory = new RabbitMQ.Client.ConnectionFactory()
			{
				HostName = "host.docker.internal",
				Port = 5672,
				UserName = "mau5", // Use appropriate username
				Password = "123"			
			};

		using (var connection = factory.CreateConnection()) // Ensure CreateConnection is used correctly
		using (var channel = connection.CreateModel())
		{
			channel.QueueDeclare(queue: "event_queue",
								 durable: false,
								 exclusive: false,
								 autoDelete: false,
								 arguments: null);
		
			var body = Encoding.UTF8.GetBytes($"{message} {DateTime.Now} ");
		
			channel.BasicPublish(exchange: "",
								 routingKey: "event_queue",
								 basicProperties: null,
								 body: body);
		}

			return Ok("Message published to the queue.");
		}
	}
}
