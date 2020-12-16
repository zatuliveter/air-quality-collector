using System;

namespace AirQualityCollector
{
	public class StationDto
	{
		public string StationId { get; set; }
		public string Name { get; set; }
		public float Lat { get; set; }
		public float Lon { get; set; }
		public string AddressStr { get; set; }
		public string CurrentStatus { get; set; }
	}

	public class StationDataDto
	{
		public string StationId { get; set; }
		public DataType DataType { get; set; }
		public DateTime DateTimeUtc { get; set; }
		public int MeasurementCount { get; set; }
		public float MaxValue { get; set; }
		public float MinValue { get; set; }
		public float MeanValue { get; set; }
		public float MedianValue { get; set; }
		public float StddevValue { get; set; }
	}

	public enum DataType : byte
	{
		PM25 = 1,
		PM10 = 2
	}
}