using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace XMLObject.AirOTA_AirBookRS
{
	[XmlRoot(ElementName = "Success", Namespace = "http://services.sabre.com/STL/v01")]
	public class Success
	{
		[XmlAttribute(AttributeName = "timeStamp")]
		public string TimeStamp { get; set; }
	}

	[XmlRoot(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL/v01")]
	public class ApplicationResults
	{
		[XmlElement(ElementName = "Success", Namespace = "http://services.sabre.com/STL/v01")]
		public Success Success { get; set; }
		[XmlAttribute(AttributeName = "status")]
		public string Status { get; set; }
	}

	[XmlRoot(ElementName = "DestinationLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
	public class DestinationLocation
	{
		[XmlAttribute(AttributeName = "LocationCode")]
		public string LocationCode { get; set; }
	}

	[XmlRoot(ElementName = "MarketingAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
	public class MarketingAirline
	{
		[XmlAttribute(AttributeName = "Code")]
		public string Code { get; set; }
		[XmlAttribute(AttributeName = "FlightNumber")]
		public string FlightNumber { get; set; }
	}

	[XmlRoot(ElementName = "OriginLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
	public class OriginLocation
	{
		[XmlAttribute(AttributeName = "LocationCode")]
		public string LocationCode { get; set; }
	}

	[XmlRoot(ElementName = "FlightSegment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
	public class FlightSegment
	{
		[XmlElement(ElementName = "DestinationLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
		public DestinationLocation DestinationLocation { get; set; }
		[XmlElement(ElementName = "MarketingAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
		public MarketingAirline MarketingAirline { get; set; }
		[XmlElement(ElementName = "OriginLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
		public OriginLocation OriginLocation { get; set; }
		[XmlAttribute(AttributeName = "ArrivalDateTime")]
		public string ArrivalDateTime { get; set; }
		[XmlAttribute(AttributeName = "DepartureDateTime")]
		public string DepartureDateTime { get; set; }
		[XmlAttribute(AttributeName = "FlightNumber")]
		public string FlightNumber { get; set; }
		[XmlAttribute(AttributeName = "NumberInParty")]
		public string NumberInParty { get; set; }
		[XmlAttribute(AttributeName = "ResBookDesigCode")]
		public string ResBookDesigCode { get; set; }
		[XmlAttribute(AttributeName = "Status")]
		public string Status { get; set; }
		[XmlAttribute(AttributeName = "eTicket")]
		public string ETicket { get; set; }
	}

	[XmlRoot(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
	public class OriginDestinationOption
	{
		[XmlElement(ElementName = "FlightSegment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
		public List<FlightSegment> FlightSegment { get; set; }
	}

	[XmlRoot(ElementName = "OTA_AirBookRS", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
	public class OTA_AirBookRS
	{
		[XmlElement(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL/v01")]
		public ApplicationResults ApplicationResults { get; set; }
		[XmlElement(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
		public OriginDestinationOption OriginDestinationOption { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		[XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xs { get; set; }
		[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xsi { get; set; }
		[XmlAttribute(AttributeName = "stl", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Stl { get; set; }
		[XmlAttribute(AttributeName = "Version")]
		public string Version { get; set; }
	}

}
