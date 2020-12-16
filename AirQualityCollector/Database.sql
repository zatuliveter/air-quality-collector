--drop table if exists dbo.StationData 
--drop table if exists dbo.Stations
--go

create table dbo.Stations (
	StationId varchar(128) not null
  , Name nvarchar(1000) not null
  , Lat float not null
  , Lon float not null
  , AddressStr nvarchar(1000) null
  , CurrentStatus varchar(10) null
  , CreatedUtc datetime2(0) not null
  , ModifiedUtc datetime2(0) not null
)
go

create unique clustered index CI_Stations on dbo.Stations ( StationId )
go

create table dbo.StationData (
	StationId varchar(128) not null
  , DataType tinyint not null /* 1 - PM2.5, 2 - PM10 */
  , DateTimeUtc datetime2(0) not null
  , MeasurementCount int not null
  , MaxValue float not null
  , MinValue float not null
  , MeanValue float not null
  , MedianValue float not null
  , StddevValue float not null
  , CreatedUtc datetime2(0) not null
  , ModifiedUtc datetime2(0) not null
  , constraint FK_StationData_Stations foreign key ( StationId ) references dbo.Stations ( StationId )
)

create unique clustered index CI_StationData on dbo.StationData ( StationId, DataType, DateTimeUtc )
go

