using System;
using System.Threading;
using System.Threading.Tasks;

namespace AirQualityCollector
{
	internal class WindowsService
	{
		Collector collector = new Collector();
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		
		public void Start()
		{
			Console.WriteLine("Service starting...");
			var cancellationToken = cancellationTokenSource.Token;
			Task.Run(async () => await collector.CollectAsync(cancellationToken), cancellationToken);
			Console.WriteLine("Service started!");
		}

		public void Stop()
		{
			try
			{
				Console.WriteLine("Service stopping...");
				cancellationTokenSource.Cancel();
				Console.WriteLine("Service stopped!");
			}
			catch (Exception e)
			{
				Console.WriteLine($"ServicesHost start error. {e}");
			}
		}
	}
}