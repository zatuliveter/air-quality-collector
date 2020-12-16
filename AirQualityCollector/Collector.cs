using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirQualityCollector
{
	class Collector
	{
		private static readonly HttpClient client = new HttpClient();
		private static readonly StationDA stationDA = new StationDA();
		private static readonly StationDataDA stationDataDA = new StationDataDA();
		
		public async Task CollectAsync(CancellationToken cancellationToken)
		{
			while (true)
			{
				try
				{
					await CollectOnceAsync(cancellationToken);
				}
				catch (Exception e)
				{
					Console.WriteLine($"ServicesHost start error. {e}");
				}

				bool cancelled = cancellationToken.WaitHandle.WaitOne(TimeSpan.FromMinutes(10));

				if (cancelled) return;
			}
		}

		public async Task CollectOnceAsync(CancellationToken cancellationToken)
		{
			foreach (string stationId in await GetStationIdsAsync())
			{
				Console.WriteLine($"Data for: {stationId}");
				
				var stationDataResponse = await client.GetAsync($"https://airnet.waqi.info/airnet/feed/hourly/{stationId}", cancellationToken);
				var stationDataResponseString = await stationDataResponse.Content.ReadAsStringAsync();

				dynamic stationDataObj = JsonConvert.DeserializeObject(stationDataResponseString);

				var stationDto = new StationDto
				{
					StationId = stationDataObj.meta.id,
					AddressStr = stationDataObj.loiq.display_name,
					Name = stationDataObj.meta.name,
					CurrentStatus = stationDataObj.meta.status,
					Lat = stationDataObj.meta.geo[0],
					Lon = stationDataObj.meta.geo[1]
				};
				
				await stationDA.SaveStationAsync(stationDto, cancellationToken);
				await SaveStationDataAsync(stationId, DataType.PM25, stationDataObj.data.pm25, cancellationToken);
				await SaveStationDataAsync(stationId, DataType.PM10, stationDataObj.data.pm10, cancellationToken);

				if (cancellationToken.IsCancellationRequested) return;
			}
		}

		private async Task SaveStationDataAsync(string stationId, DataType dataType, dynamic pmData, CancellationToken cancellationToken)
		{
			if (pmData == null) return;

			foreach (dynamic data in pmData)
			{
				var dataDto = new StationDataDto
				{
					StationId = stationId,
					DataType = dataType,
					DateTimeUtc = data.time,
					MaxValue = data.max,
					MinValue = data.min,
					MeanValue = data.mean,
					MeasurementCount = data.count,
					MedianValue = data.median,
					StddevValue = data.stddev
				};
				await stationDataDA.SaveStationAsync(dataDto, cancellationToken);
				if (cancellationToken.IsCancellationRequested) return;
			}
		}

		private async Task<List<string>> GetStationIdsAsync()
		{
			var content = new FormUrlEncodedContent(new Dictionary<string, string>
					{
						{ "bounds", "74.24451074372163,42.64904889783977,75.02114897595281,43.08230650220345" },
						{ "zoom", "14" }
					});

			var response = await client.PostAsync("https://airnet.waqi.info/airnet/map/bounds", content);

			var responseString = await response.Content.ReadAsStringAsync();

			dynamic obj = JsonConvert.DeserializeObject(responseString);

			var ids = new List<string>();

			foreach (dynamic station in obj.data)
			{
				ids.Add(station.x.ToString());
			}

			return ids;
		}
	}
}
