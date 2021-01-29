using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace XMLObject.AirAvailLLSRQ
{
    [XmlRoot(ElementName = "HostCommand", Namespace = "http://services.sabre.com/STL/v01")]
    public class HostCommand
    {
        [XmlAttribute(AttributeName = "LNIATA")]
        public string LNIATA { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SystemSpecificResults", Namespace = "http://services.sabre.com/STL/v01")]
    public class SystemSpecificResults
    {
        [XmlElement(ElementName = "HostCommand", Namespace = "http://services.sabre.com/STL/v01")]
        public HostCommand HostCommand { get; set; }
    }

    [XmlRoot(ElementName = "Success", Namespace = "http://services.sabre.com/STL/v01")]
    public class Success
    {
        [XmlElement(ElementName = "SystemSpecificResults", Namespace = "http://services.sabre.com/STL/v01")]
        public SystemSpecificResults SystemSpecificResults { get; set; }
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

    [XmlRoot(ElementName = "BookingClassAvail", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class BookingClassAvail
    {
        [XmlAttribute(AttributeName = "AggregatedContent")]
        public string AggregatedContent { get; set; }
        [XmlAttribute(AttributeName = "Availability")]
        public string Availability { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlAttribute(AttributeName = "ResBookDesigCode")]
        public string ResBookDesigCode { get; set; }
    }

    [XmlRoot(ElementName = "OperationTime", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OperationTime
    {
        [XmlAttribute(AttributeName = "Fri")]
        public string Fri { get; set; }
        [XmlAttribute(AttributeName = "Mon")]
        public string Mon { get; set; }
        [XmlAttribute(AttributeName = "Sat")]
        public string Sat { get; set; }
        [XmlAttribute(AttributeName = "Sun")]
        public string Sun { get; set; }
        [XmlAttribute(AttributeName = "Thur")]
        public string Thur { get; set; }
        [XmlAttribute(AttributeName = "Tue")]
        public string Tue { get; set; }
        [XmlAttribute(AttributeName = "Weds")]
        public string Weds { get; set; }
    }

    [XmlRoot(ElementName = "OperationTimes", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OperationTimes
    {
        [XmlElement(ElementName = "OperationTime", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OperationTime OperationTime { get; set; }
    }

    [XmlRoot(ElementName = "OperationSchedule", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OperationSchedule
    {
        [XmlElement(ElementName = "OperationTimes", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OperationTimes OperationTimes { get; set; }
    }

    [XmlRoot(ElementName = "DaysOfOperation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class DaysOfOperation
    {
        [XmlElement(ElementName = "OperationSchedule", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OperationSchedule OperationSchedule { get; set; }
    }

    [XmlRoot(ElementName = "DestinationLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class DestinationLocation
    {
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
    }

    [XmlRoot(ElementName = "Equipment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Equipment
    {
        [XmlAttribute(AttributeName = "AirEquipType")]
        public string AirEquipType { get; set; }
    }

    [XmlRoot(ElementName = "FlightDetails", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class FlightDetails
    {
        [XmlAttribute(AttributeName = "Canceled")]
        public string Canceled { get; set; }
        [XmlAttribute(AttributeName = "Charter")]
        public string Charter { get; set; }
        [XmlAttribute(AttributeName = "GroundTime")]
        public string GroundTime { get; set; }
        [XmlAttribute(AttributeName = "TotalTravelTime")]
        public string TotalTravelTime { get; set; }
        [XmlAttribute(AttributeName = "CodeshareBlockDisplay")]
        public string CodeshareBlockDisplay { get; set; }
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
        [XmlElement(ElementName = "BookingClassAvail", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<BookingClassAvail> BookingClassAvail { get; set; }
        [XmlElement(ElementName = "DaysOfOperation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public DaysOfOperation DaysOfOperation { get; set; }
        [XmlElement(ElementName = "DestinationLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public DestinationLocation DestinationLocation { get; set; }
        [XmlElement(ElementName = "Equipment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public Equipment Equipment { get; set; }
        [XmlElement(ElementName = "FlightDetails", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public FlightDetails FlightDetails { get; set; }
        [XmlElement(ElementName = "MarketingAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public MarketingAirline MarketingAirline { get; set; }
        [XmlElement(ElementName = "OriginLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OriginLocation OriginLocation { get; set; }
        [XmlAttribute(AttributeName = "ArrivalDateTime")]
        public string ArrivalDateTime { get; set; }
        [XmlAttribute(AttributeName = "DOT_Ind")]
        public string DOT_Ind { get; set; }
        [XmlAttribute(AttributeName = "DepartureDateTime")]
        public string DepartureDateTime { get; set; }
        [XmlAttribute(AttributeName = "FlightNumber")]
        public string FlightNumber { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlAttribute(AttributeName = "SmokingAllowed")]
        public string SmokingAllowed { get; set; }
        [XmlAttribute(AttributeName = "StopQuantity")]
        public string StopQuantity { get; set; }
        [XmlAttribute(AttributeName = "eTicket")]
        public string ETicket { get; set; }
        [XmlElement(ElementName = "Meal", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<Meal> Meal { get; set; }
        [XmlElement(ElementName = "DisclosureAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public DisclosureAirline DisclosureAirline { get; set; }
    }

    [XmlRoot(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OriginDestinationOption
    {
        [XmlElement(ElementName = "FlightSegment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public FlightSegment FlightSegment { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
    }

    [XmlRoot(ElementName = "Meal", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Meal
    {
        [XmlAttribute(AttributeName = "MealCode")]
        public string MealCode { get; set; }
    }

    [XmlRoot(ElementName = "DisclosureAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class DisclosureAirline
    {
        [XmlElement(ElementName = "Text", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "CompanyShortName")]
        public string CompanyShortName { get; set; }
    }

    [XmlRoot(ElementName = "OriginDestinationOptions", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OriginDestinationOptions
    {
        [XmlElement(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<OriginDestinationOption> OriginDestinationOption { get; set; }
        [XmlAttribute(AttributeName = "OriginTimeZone")]
        public string OriginTimeZone { get; set; }
        [XmlAttribute(AttributeName = "TimeZoneDifference")]
        public string TimeZoneDifference { get; set; }
    }

    [XmlRoot(ElementName = "OTA_AirAvailRS", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OTA_AirAvailRS
    {
        [XmlElement(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL/v01")]
        public ApplicationResults ApplicationResults { get; set; }
        [XmlElement(ElementName = "OriginDestinationOptions", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OriginDestinationOptions OriginDestinationOptions { get; set; }
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


