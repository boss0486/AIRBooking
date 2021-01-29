using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace XMLObject.FareLLSRQ
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

    [XmlRoot(ElementName = "Airline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Airline
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "Brand", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Brand
    {
        [XmlAttribute(AttributeName = "BrandCode")]
        public string BrandCode { get; set; }
        [XmlAttribute(AttributeName = "BrandName")]
        public string BrandName { get; set; }
        [XmlAttribute(AttributeName = "ProgramCode")]
        public string ProgramCode { get; set; }
        [XmlAttribute(AttributeName = "ProgramID")]
        public string ProgramID { get; set; }
        [XmlAttribute(AttributeName = "ProgramName")]
        public string ProgramName { get; set; }
        [XmlAttribute(AttributeName = "SystemCode")]
        public string SystemCode { get; set; }
    }

    [XmlRoot(ElementName = "Fare", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Fare
    {
        [XmlAttribute(AttributeName = "Amount")]
        public string Amount { get; set; }
    }

    [XmlRoot(ElementName = "OneWayRoundTrip", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OneWayRoundTrip
    {
        [XmlAttribute(AttributeName = "Ind")]
        public string Ind { get; set; }
    }

    [XmlRoot(ElementName = "Rule", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Rule
    {
        [XmlElement(ElementName = "Category", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<string> Category { get; set; }
    }

    [XmlRoot(ElementName = "AdditionalInformation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class AdditionalInformation
    {
        [XmlElement(ElementName = "AdvancePurchase", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string AdvancePurchase { get; set; }
        [XmlElement(ElementName = "Airline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public Airline Airline { get; set; }
        [XmlElement(ElementName = "Brand", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public Brand Brand { get; set; }
        [XmlElement(ElementName = "Cabin", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string Cabin { get; set; }
        [XmlElement(ElementName = "CabinName", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string CabinName { get; set; }
        [XmlElement(ElementName = "ExpirationDate", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string ExpirationDate { get; set; }
        [XmlElement(ElementName = "Fare", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<Fare> Fare { get; set; }
        [XmlElement(ElementName = "OneWayRoundTrip", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OneWayRoundTrip OneWayRoundTrip { get; set; }
        [XmlElement(ElementName = "Rule", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public Rule Rule { get; set; }
        [XmlElement(ElementName = "TicketDate", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string TicketDate { get; set; }
        [XmlAttribute(AttributeName = "Acknowledgement")]
        public string Acknowledgement { get; set; }
        [XmlAttribute(AttributeName = "Constructed")]
        public string Constructed { get; set; }
        [XmlAttribute(AttributeName = "FareType")]
        public string FareType { get; set; }
        [XmlAttribute(AttributeName = "FareVendor")]
        public string FareVendor { get; set; }
        [XmlAttribute(AttributeName = "Net")]
        public string Net { get; set; }
        [XmlAttribute(AttributeName = "Private")]
        public string Private { get; set; }
        [XmlAttribute(AttributeName = "PrivateInd")]
        public string PrivateInd { get; set; }
        [XmlAttribute(AttributeName = "ResBookDesigCode")]
        public string ResBookDesigCode { get; set; }
        [XmlAttribute(AttributeName = "RoutingNumber")]
        public string RoutingNumber { get; set; }
        [XmlAttribute(AttributeName = "YY")]
        public string YY { get; set; }
    }

    [XmlRoot(ElementName = "BaseFare", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class BaseFare
    {
        [XmlAttribute(AttributeName = "Amount")]
        public string Amount { get; set; }
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
    }

    [XmlRoot(ElementName = "Category22", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Category22
    {
        [XmlAttribute(AttributeName = "Ind")]
        public string Ind { get; set; }
    }

    [XmlRoot(ElementName = "PassengerType", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class PassengerType
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "FareBasis", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class FareBasis
    {
        [XmlElement(ElementName = "AdditionalInformation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public AdditionalInformation AdditionalInformation { get; set; }
        [XmlElement(ElementName = "BaseFare", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public BaseFare BaseFare { get; set; }
        [XmlElement(ElementName = "Category22", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public Category22 Category22 { get; set; }
        [XmlElement(ElementName = "PassengerType", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public PassengerType PassengerType { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
    }

    [XmlRoot(ElementName = "OtherAirlinesInMarket", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OtherAirlinesInMarket
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "AdditionalVendorInfo", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class AdditionalVendorInfo
    {
        [XmlElement(ElementName = "OtherAirlinesInMarket", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<OtherAirlinesInMarket> OtherAirlinesInMarket { get; set; }
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
        [XmlAttribute(AttributeName = "DepartureDateTime")]
        public string DepartureDateTime { get; set; }
    }

    [XmlRoot(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OriginDestinationOption
    {
        [XmlElement(ElementName = "FlightSegment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public FlightSegment FlightSegment { get; set; }
    }

    [XmlRoot(ElementName = "HeaderInformation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class HeaderInformation
    {
        [XmlElement(ElementName = "AdditionalVendorInfo", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public AdditionalVendorInfo AdditionalVendorInfo { get; set; }
        [XmlElement(ElementName = "CurrencyCode", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string CurrencyCode { get; set; }
        [XmlElement(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OriginDestinationOption OriginDestinationOption { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<string> Text { get; set; }
    }

    [XmlRoot(ElementName = "Routing", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Routing
    {
        [XmlElement(ElementName = "Text", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "GlobalInd")]
        public string GlobalInd { get; set; }
        [XmlAttribute(AttributeName = "Number")]
        public string Number { get; set; }
    }

    [XmlRoot(ElementName = "FareRS", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class FareRS
    {
        [XmlElement(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL/v01")]
        public ApplicationResults ApplicationResults { get; set; }
        [XmlElement(ElementName = "FareBasis", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<FareBasis> FareBasis { get; set; }
        [XmlElement(ElementName = "HeaderInformation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public HeaderInformation HeaderInformation { get; set; }
        [XmlElement(ElementName = "Routing", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public Routing Routing { get; set; }
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
