using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace XMLObject.AirTicketRq
{
    [XmlRoot(ElementName = "PartyId", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
    public class PartyId
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "From", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
    public class From
    {
        [XmlElement(ElementName = "PartyId", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public PartyId PartyId { get; set; }
    }

    [XmlRoot(ElementName = "To", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
    public class To
    {
        [XmlElement(ElementName = "PartyId", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public PartyId PartyId { get; set; }
    }

    [XmlRoot(ElementName = "Service", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
    public class Service
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "MessageData", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
    public class MessageData
    {
        [XmlElement(ElementName = "MessageId", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string MessageId { get; set; }
        [XmlElement(ElementName = "Timestamp", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string Timestamp { get; set; }
        [XmlElement(ElementName = "RefToMessageId", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string RefToMessageId { get; set; }
    }

    [XmlRoot(ElementName = "MessageHeader", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
    public class MessageHeader
    {
        [XmlElement(ElementName = "From", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public From From { get; set; }
        [XmlElement(ElementName = "To", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public To To { get; set; }
        [XmlElement(ElementName = "CPAId", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string CPAId { get; set; }
        [XmlElement(ElementName = "ConversationId", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string ConversationId { get; set; }
        [XmlElement(ElementName = "Service", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public Service Service { get; set; }
        [XmlElement(ElementName = "Action", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string Action { get; set; }
        [XmlElement(ElementName = "MessageData", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public MessageData MessageData { get; set; }
        [XmlAttribute(AttributeName = "eb", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Eb { get; set; }
        [XmlAttribute(AttributeName = "version", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string MustUnderstand { get; set; }
    }

    [XmlRoot(ElementName = "BinarySecurityToken", Namespace = "http://schemas.xmlsoap.org/ws/2002/12/secext")]
    public class BinarySecurityToken
    {
        [XmlAttribute(AttributeName = "valueType")]
        public string ValueType { get; set; }
        [XmlAttribute(AttributeName = "EncodingType")]
        public string EncodingType { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Security", Namespace = "http://schemas.xmlsoap.org/ws/2002/12/secext")]
    public class Security
    {
        [XmlElement(ElementName = "BinarySecurityToken", Namespace = "http://schemas.xmlsoap.org/ws/2002/12/secext")]
        public BinarySecurityToken BinarySecurityToken { get; set; }
        [XmlAttribute(AttributeName = "wsse", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wsse { get; set; }
    }

    [XmlRoot(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Header
    {
        [XmlElement(ElementName = "MessageHeader", Namespace = "http://www.ebxml.org/namespaces/messageHeader")]
        public MessageHeader MessageHeader { get; set; }
        [XmlElement(ElementName = "Security", Namespace = "http://schemas.xmlsoap.org/ws/2002/12/secext")]
        public Security Security { get; set; }
    }

    [XmlRoot(ElementName = "Success", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
    public class Success
    {
        [XmlAttribute(AttributeName = "timeStamp")]
        public string TimeStamp { get; set; }
    }

    [XmlRoot(ElementName = "Message", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
    public class Message
    {
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SystemSpecificResults", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
    public class SystemSpecificResults
    {
        [XmlElement(ElementName = "Message", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
        public Message Message { get; set; }
    }

    [XmlRoot(ElementName = "Warning", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
    public class Warning
    {
        [XmlElement(ElementName = "SystemSpecificResults", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
        public SystemSpecificResults SystemSpecificResults { get; set; }
        [XmlAttribute(AttributeName = "timeStamp")]
        public string TimeStamp { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
    public class ApplicationResults
    {
        [XmlElement(ElementName = "Success", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
        public Success Success { get; set; }
        [XmlElement(ElementName = "Warning", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
        public List<Warning> Warning { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlAttribute(AttributeName = "ns3", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns3 { get; set; }
        [XmlAttribute(AttributeName = "ns4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns4 { get; set; }
        [XmlAttribute(AttributeName = "ns5", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns5 { get; set; }
        [XmlAttribute(AttributeName = "ns6", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns6 { get; set; }
        [XmlAttribute(AttributeName = "ns7", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns7 { get; set; }
        [XmlAttribute(AttributeName = "ns8", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns8 { get; set; }
        [XmlAttribute(AttributeName = "ns9", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns9 { get; set; }
        [XmlAttribute(AttributeName = "ns10", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns10 { get; set; }
        [XmlAttribute(AttributeName = "ns11", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns11 { get; set; }
        [XmlAttribute(AttributeName = "ns12", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns12 { get; set; }
        [XmlAttribute(AttributeName = "ns13", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns13 { get; set; }
        [XmlAttribute(AttributeName = "ns14", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns14 { get; set; }
        [XmlAttribute(AttributeName = "ns15", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns15 { get; set; }
        [XmlAttribute(AttributeName = "ns16", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns16 { get; set; }
        [XmlAttribute(AttributeName = "ns17", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns17 { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "ItineraryRef", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ItineraryRef
    {
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Source", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Source Source { get; set; }
        [XmlAttribute(AttributeName = "PrimeHostID")]
        public string PrimeHostID { get; set; }
        [XmlAttribute(AttributeName = "InhibitCode")]
        public string InhibitCode { get; set; }
        [XmlAttribute(AttributeName = "AccountingCode")]
        public string AccountingCode { get; set; }
        [XmlAttribute(AttributeName = "AccountingCity")]
        public string AccountingCity { get; set; }
        [XmlAttribute(AttributeName = "AirExtras")]
        public string AirExtras { get; set; }
        [XmlAttribute(AttributeName = "OfficeStationCode")]
        public string OfficeStationCode { get; set; }
        [XmlAttribute(AttributeName = "PartitionID")]
        public string PartitionID { get; set; }
        [XmlAttribute(AttributeName = "TicketingCarrier")]
        public string TicketingCarrier { get; set; }
    }

    [XmlRoot(ElementName = "ContactNumber", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ContactNumber
    {
        [XmlAttribute(AttributeName = "Phone")]
        public string Phone { get; set; }
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "ContactNumbers", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ContactNumbers
    {
        [XmlElement(ElementName = "ContactNumber", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ContactNumber ContactNumber { get; set; }
    }

    [XmlRoot(ElementName = "Email", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Email
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "Address", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Address { get; set; }
        [XmlAttribute(AttributeName = "comment")]
        public string Comment { get; set; }
    }

    [XmlRoot(ElementName = "PersonName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PersonName
    {
        [XmlElement(ElementName = "Email", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Email Email { get; set; }
        [XmlElement(ElementName = "GivenName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string GivenName { get; set; }
        [XmlElement(ElementName = "Surname", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Surname { get; set; }
        [XmlAttribute(AttributeName = "elementId")]
        public string ElementId { get; set; }
        [XmlAttribute(AttributeName = "NameNumber")]
        public string NameNumber { get; set; }
        [XmlAttribute(AttributeName = "WithInfant")]
        public string WithInfant { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CustomerInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class CustomerInfo
    {
        [XmlElement(ElementName = "ContactNumbers", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ContactNumbers ContactNumbers { get; set; }
        [XmlElement(ElementName = "PersonName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<PersonName> PersonName { get; set; }
    }

    [XmlRoot(ElementName = "SignatureLine", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class SignatureLine
    {
        [XmlElement(ElementName = "Text", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "ExpirationDateTime")]
        public string ExpirationDateTime { get; set; }
        [XmlAttribute(AttributeName = "Source")]
        public string Source { get; set; }
    }

    [XmlRoot(ElementName = "MiscInformation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class MiscInformation
    {
        [XmlElement(ElementName = "SignatureLine", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public SignatureLine SignatureLine { get; set; }
    }

    [XmlRoot(ElementName = "BaseFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class BaseFare
    {
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
        [XmlAttribute(AttributeName = "Amount")]
        public string Amount { get; set; }
    }

    [XmlRoot(ElementName = "Tax", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Tax
    {
        [XmlAttribute(AttributeName = "TaxCode")]
        public string TaxCode { get; set; }
        [XmlAttribute(AttributeName = "Amount")]
        public string Amount { get; set; }
    }

    [XmlRoot(ElementName = "TaxBreakdownCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class TaxBreakdownCode
    {
        [XmlAttribute(AttributeName = "TaxPaid")]
        public string TaxPaid { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Taxes", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Taxes
    {
        [XmlElement(ElementName = "Tax", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Tax Tax { get; set; }
        [XmlElement(ElementName = "TaxBreakdownCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<TaxBreakdownCode> TaxBreakdownCode { get; set; }
    }

    [XmlRoot(ElementName = "TotalFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class TotalFare
    {
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
        [XmlAttribute(AttributeName = "Amount")]
        public string Amount { get; set; }
    }

    [XmlRoot(ElementName = "Totals", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Totals
    {
        [XmlElement(ElementName = "BaseFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public BaseFare BaseFare { get; set; }
        [XmlElement(ElementName = "Taxes", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Taxes Taxes { get; set; }
        [XmlElement(ElementName = "TotalFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public TotalFare TotalFare { get; set; }
    }

    [XmlRoot(ElementName = "ItinTotalFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ItinTotalFare
    {
        [XmlElement(ElementName = "BaseFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public BaseFare BaseFare { get; set; }
        [XmlElement(ElementName = "Taxes", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Taxes Taxes { get; set; }
        [XmlElement(ElementName = "TotalFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public TotalFare TotalFare { get; set; }
        [XmlElement(ElementName = "Totals", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Totals Totals { get; set; }
    }

    [XmlRoot(ElementName = "PassengerTypeQuantity", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PassengerTypeQuantity
    {
        [XmlAttribute(AttributeName = "Quantity")]
        public string Quantity { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "Endorsement", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Endorsement
    {
        [XmlElement(ElementName = "Text", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "Endorsements", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Endorsements
    {
        [XmlElement(ElementName = "Endorsement", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<Endorsement> Endorsement { get; set; }
    }

    [XmlRoot(ElementName = "FareBasis", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class FareBasis
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "FareCalculation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class FareCalculation
    {
        [XmlElement(ElementName = "Text", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "BaggageAllowance", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class BaggageAllowance
    {
        [XmlAttribute(AttributeName = "Number")]
        public string Number { get; set; }
    }

    [XmlRoot(ElementName = "MarketingAirline", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class MarketingAirline
    {
        [XmlAttribute(AttributeName = "FlightNumber")]
        public string FlightNumber { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlElement(ElementName = "Banner", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Banner { get; set; }
        [XmlAttribute(AttributeName = "ResBookDesigCode")]
        public string ResBookDesigCode { get; set; }
    }

    [XmlRoot(ElementName = "OriginLocation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class OriginLocation
    {
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
        [XmlAttribute(AttributeName = "TerminalCode")]
        public string TerminalCode { get; set; }
        [XmlAttribute(AttributeName = "Terminal")]
        public string Terminal { get; set; }
    }

    [XmlRoot(ElementName = "ValidityDates", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ValidityDates
    {
        [XmlElement(ElementName = "NotValidAfter", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string NotValidAfter { get; set; }
        [XmlElement(ElementName = "NotValidBefore", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string NotValidBefore { get; set; }
    }

    [XmlRoot(ElementName = "FlightSegment", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class FlightSegment
    {
        [XmlElement(ElementName = "BaggageAllowance", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public BaggageAllowance BaggageAllowance { get; set; }
        [XmlElement(ElementName = "FareBasis", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public FareBasis FareBasis { get; set; }
        [XmlElement(ElementName = "MarketingAirline", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public MarketingAirline MarketingAirline { get; set; }
        [XmlElement(ElementName = "OriginLocation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public OriginLocation OriginLocation { get; set; }
        [XmlElement(ElementName = "ValidityDates", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ValidityDates ValidityDates { get; set; }
        [XmlAttribute(AttributeName = "SegmentNumber")]
        public string SegmentNumber { get; set; }
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }
        [XmlAttribute(AttributeName = "DepartureDateTime")]
        public string DepartureDateTime { get; set; }
        [XmlAttribute(AttributeName = "ConnectionInd")]
        public string ConnectionInd { get; set; }
        [XmlAttribute(AttributeName = "FlightNumber")]
        public string FlightNumber { get; set; }
        [XmlAttribute(AttributeName = "ResBookDesigCode")]
        public string ResBookDesigCode { get; set; }
        [XmlElement(ElementName = "DestinationLocation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public DestinationLocation DestinationLocation { get; set; }
        [XmlElement(ElementName = "Equipment", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Equipment Equipment { get; set; }
        [XmlElement(ElementName = "Meal", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Meal Meal { get; set; }
        [XmlElement(ElementName = "OperatingAirline", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public OperatingAirline OperatingAirline { get; set; }
        [XmlElement(ElementName = "OperatingAirlinePricing", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public OperatingAirlinePricing OperatingAirlinePricing { get; set; }
        [XmlElement(ElementName = "DisclosureCarrier", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public DisclosureCarrier DisclosureCarrier { get; set; }
        [XmlElement(ElementName = "UpdatedArrivalTime", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string UpdatedArrivalTime { get; set; }
        [XmlElement(ElementName = "UpdatedDepartureTime", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string UpdatedDepartureTime { get; set; }
        [XmlElement(ElementName = "Cabin", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Cabin Cabin { get; set; }
        [XmlAttribute(AttributeName = "DayOfWeekInd")]
        public string DayOfWeekInd { get; set; }
        [XmlAttribute(AttributeName = "CodeShare")]
        public string CodeShare { get; set; }
        [XmlAttribute(AttributeName = "NumberInParty")]
        public string NumberInParty { get; set; }
        [XmlAttribute(AttributeName = "StopQuantity")]
        public string StopQuantity { get; set; }
        [XmlAttribute(AttributeName = "ElapsedTime")]
        public string ElapsedTime { get; set; }
        [XmlAttribute(AttributeName = "SmokingAllowed")]
        public string SmokingAllowed { get; set; }
        [XmlAttribute(AttributeName = "AirMilesFlown")]
        public string AirMilesFlown { get; set; }
        [XmlAttribute(AttributeName = "IsPast")]
        public string IsPast { get; set; }
        [XmlAttribute(AttributeName = "eTicket")]
        public string ETicket { get; set; }
        [XmlAttribute(AttributeName = "ArrivalDateTime")]
        public string ArrivalDateTime { get; set; }
        [XmlAttribute(AttributeName = "SegmentBookedDate")]
        public string SegmentBookedDate { get; set; }
        [XmlAttribute(AttributeName = "SpecialMeal")]
        public string SpecialMeal { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Location", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Location
    {
        [XmlAttribute(AttributeName = "Origin")]
        public string Origin { get; set; }
        [XmlAttribute(AttributeName = "Destination")]
        public string Destination { get; set; }
    }

    [XmlRoot(ElementName = "Dates", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Dates
    {
        [XmlAttribute(AttributeName = "DepartureDateTime")]
        public string DepartureDateTime { get; set; }
        [XmlAttribute(AttributeName = "ArrivalDateTime")]
        public string ArrivalDateTime { get; set; }
    }

    [XmlRoot(ElementName = "FlightSegmentNumbers", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class FlightSegmentNumbers
    {
        [XmlElement(ElementName = "FlightSegmentNumber", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string FlightSegmentNumber { get; set; }
    }

    [XmlRoot(ElementName = "FareComponent", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class FareComponent
    {
        [XmlElement(ElementName = "Location", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Location Location { get; set; }
        [XmlElement(ElementName = "Dates", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Dates Dates { get; set; }
        [XmlElement(ElementName = "FlightSegmentNumbers", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public FlightSegmentNumbers FlightSegmentNumbers { get; set; }
        [XmlAttribute(AttributeName = "TicketDesignator")]
        public string TicketDesignator { get; set; }
        [XmlAttribute(AttributeName = "GoverningCarrier")]
        public string GoverningCarrier { get; set; }
        [XmlAttribute(AttributeName = "FareBasisCode")]
        public string FareBasisCode { get; set; }
        [XmlAttribute(AttributeName = "Amount")]
        public string Amount { get; set; }
        [XmlAttribute(AttributeName = "FareDirectionality")]
        public string FareDirectionality { get; set; }
        [XmlAttribute(AttributeName = "FareComponentNumber")]
        public string FareComponentNumber { get; set; }
    }

    [XmlRoot(ElementName = "PTC_FareBreakdown", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PTC_FareBreakdown
    {
        [XmlElement(ElementName = "Endorsements", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Endorsements Endorsements { get; set; }
        [XmlElement(ElementName = "FareBasis", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public FareBasis FareBasis { get; set; }
        [XmlElement(ElementName = "FareCalculation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public FareCalculation FareCalculation { get; set; }
        [XmlElement(ElementName = "FlightSegment", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<FlightSegment> FlightSegment { get; set; }
        [XmlElement(ElementName = "FareComponent", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<FareComponent> FareComponent { get; set; }
        [XmlElement(ElementName = "ResTicketingRestrictions", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<string> ResTicketingRestrictions { get; set; }
    }

    [XmlRoot(ElementName = "AirItineraryPricingInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class AirItineraryPricingInfo
    {
        [XmlElement(ElementName = "ItinTotalFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ItinTotalFare ItinTotalFare { get; set; }
        [XmlElement(ElementName = "PassengerTypeQuantity", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PassengerTypeQuantity PassengerTypeQuantity { get; set; }
        [XmlElement(ElementName = "PTC_FareBreakdown", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PTC_FareBreakdown PTC_FareBreakdown { get; set; }
    }

    [XmlRoot(ElementName = "PricedItinerary", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PricedItinerary
    {
        [XmlElement(ElementName = "AirItineraryPricingInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public AirItineraryPricingInfo AirItineraryPricingInfo { get; set; }
        [XmlAttribute(AttributeName = "TaxExempt")]
        public string TaxExempt { get; set; }
        [XmlAttribute(AttributeName = "ValidatingCarrier")]
        public string ValidatingCarrier { get; set; }
        [XmlAttribute(AttributeName = "DisplayOnly")]
        public string DisplayOnly { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlAttribute(AttributeName = "InputMessage")]
        public string InputMessage { get; set; }
        [XmlAttribute(AttributeName = "StatusCode")]
        public string StatusCode { get; set; }
        [XmlAttribute(AttributeName = "StoredDateTime")]
        public string StoredDateTime { get; set; }
    }

    [XmlRoot(ElementName = "ResponseHeader", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ResponseHeader
    {
        [XmlElement(ElementName = "Text", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<string> Text { get; set; }
    }

    [XmlRoot(ElementName = "PassengerData", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PassengerData
    {
        [XmlAttribute(AttributeName = "NameNumber")]
        public string NameNumber { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PassengerInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PassengerInfo
    {
        [XmlElement(ElementName = "PassengerData", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<PassengerData> PassengerData { get; set; }
        [XmlAttribute(AttributeName = "PassengerType")]
        public string PassengerType { get; set; }
    }

    [XmlRoot(ElementName = "PriceQuotePlus", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PriceQuotePlus
    {
        [XmlElement(ElementName = "PassengerInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PassengerInfo PassengerInfo { get; set; }
        [XmlElement(ElementName = "TicketingInstructionsInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string TicketingInstructionsInfo { get; set; }
        [XmlAttribute(AttributeName = "PricingStatus")]
        public string PricingStatus { get; set; }
        [XmlAttribute(AttributeName = "VerifyFareCalc")]
        public string VerifyFareCalc { get; set; }
        [XmlAttribute(AttributeName = "NUCSuppresion")]
        public string NUCSuppresion { get; set; }
        [XmlAttribute(AttributeName = "SystemIndicator")]
        public string SystemIndicator { get; set; }
        [XmlAttribute(AttributeName = "IT_BT_Fare")]
        public string IT_BT_Fare { get; set; }
        [XmlAttribute(AttributeName = "NegotiatedFare")]
        public string NegotiatedFare { get; set; }
        [XmlAttribute(AttributeName = "DomesticIntlInd")]
        public string DomesticIntlInd { get; set; }
        [XmlAttribute(AttributeName = "DisplayOnly")]
        public string DisplayOnly { get; set; }
        [XmlAttribute(AttributeName = "ManualFare")]
        public string ManualFare { get; set; }
        [XmlAttribute(AttributeName = "SubjToGovtApproval")]
        public string SubjToGovtApproval { get; set; }
        [XmlAttribute(AttributeName = "DiscountAmount")]
        public string DiscountAmount { get; set; }
        [XmlAttribute(AttributeName = "ItineraryChanged")]
        public string ItineraryChanged { get; set; }
    }

    [XmlRoot(ElementName = "PriceQuote", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PriceQuote
    {
        [XmlElement(ElementName = "MiscInformation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public MiscInformation MiscInformation { get; set; }
        [XmlElement(ElementName = "PricedItinerary", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PricedItinerary PricedItinerary { get; set; }
        [XmlElement(ElementName = "ResponseHeader", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ResponseHeader ResponseHeader { get; set; }
        [XmlElement(ElementName = "PriceQuotePlus", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PriceQuotePlus PriceQuotePlus { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
    }

    [XmlRoot(ElementName = "PriceQuoteTotals", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PriceQuoteTotals
    {
        [XmlElement(ElementName = "BaseFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public BaseFare BaseFare { get; set; }
        [XmlElement(ElementName = "Taxes", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Taxes Taxes { get; set; }
        [XmlElement(ElementName = "TotalFare", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public TotalFare TotalFare { get; set; }
    }

    [XmlRoot(ElementName = "ItineraryPricing", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ItineraryPricing
    {
        [XmlElement(ElementName = "PriceQuote", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<PriceQuote> PriceQuote { get; set; }
        [XmlElement(ElementName = "PriceQuoteTotals", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PriceQuoteTotals PriceQuoteTotals { get; set; }
    }

    [XmlRoot(ElementName = "DestinationLocation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class DestinationLocation
    {
        [XmlAttribute(AttributeName = "TerminalCode")]
        public string TerminalCode { get; set; }
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
        [XmlAttribute(AttributeName = "Terminal")]
        public string Terminal { get; set; }
    }

    [XmlRoot(ElementName = "Equipment", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Equipment
    {
        [XmlAttribute(AttributeName = "AirEquipType")]
        public string AirEquipType { get; set; }
    }

    [XmlRoot(ElementName = "Meal", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Meal
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "OperatingAirline", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class OperatingAirline
    {
        [XmlElement(ElementName = "Banner", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Banner { get; set; }
        [XmlAttribute(AttributeName = "FlightNumber")]
        public string FlightNumber { get; set; }
        [XmlAttribute(AttributeName = "ResBookDesigCode")]
        public string ResBookDesigCode { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "OperatingAirlinePricing", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class OperatingAirlinePricing
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "DisclosureCarrier", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class DisclosureCarrier
    {
        [XmlElement(ElementName = "Banner", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Banner { get; set; }
        [XmlAttribute(AttributeName = "DOT")]
        public string DOT { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "Cabin", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Cabin
    {
        [XmlElement(ElementName = "SabreCode")]
        public List<string> SabreCode { get; set; }
        [XmlElement(ElementName = "ShortName")]
        public List<string> ShortName { get; set; }
        [XmlElement(ElementName = "Lang")]
        public List<string> Lang { get; set; }
        [XmlElement(ElementName = "Code")]
        public List<string> Code { get; set; }
        [XmlElement(ElementName = "Name")]
        public List<string> Name { get; set; }
    }

    [XmlRoot(ElementName = "ProductName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ProductName
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "Air", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Air
    {
        [XmlElement(ElementName = "DepartureAirport", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string DepartureAirport { get; set; }
        [XmlElement(ElementName = "DepartureTerminalName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string DepartureTerminalName { get; set; }
        [XmlElement(ElementName = "DepartureTerminalCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string DepartureTerminalCode { get; set; }
        [XmlElement(ElementName = "ArrivalAirport", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ArrivalAirport { get; set; }
        [XmlElement(ElementName = "ArrivalTerminalName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ArrivalTerminalName { get; set; }
        [XmlElement(ElementName = "ArrivalTerminalCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ArrivalTerminalCode { get; set; }
        [XmlElement(ElementName = "OperatingAirlineCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string OperatingAirlineCode { get; set; }
        [XmlElement(ElementName = "EquipmentType", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string EquipmentType { get; set; }
        [XmlElement(ElementName = "MarketingAirlineCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string MarketingAirlineCode { get; set; }
        [XmlElement(ElementName = "MarketingFlightNumber", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string MarketingFlightNumber { get; set; }
        [XmlElement(ElementName = "MarketingClassOfService", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string MarketingClassOfService { get; set; }
        [XmlElement(ElementName = "Cabin", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Cabin Cabin { get; set; }
        [XmlElement(ElementName = "MealCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string MealCode { get; set; }
        [XmlElement(ElementName = "ElapsedTime", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ElapsedTime { get; set; }
        [XmlElement(ElementName = "AirMilesFlown", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string AirMilesFlown { get; set; }
        [XmlElement(ElementName = "FunnelFlight", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string FunnelFlight { get; set; }
        [XmlElement(ElementName = "ChangeOfGauge", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ChangeOfGauge { get; set; }
        [XmlElement(ElementName = "DisclosureCarrier", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public DisclosureCarrier DisclosureCarrier { get; set; }
        [XmlElement(ElementName = "Eticket", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Eticket { get; set; }
        [XmlElement(ElementName = "DepartureDateTime", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string DepartureDateTime { get; set; }
        [XmlElement(ElementName = "ArrivalDateTime", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ArrivalDateTime { get; set; }
        [XmlElement(ElementName = "FlightNumber", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string FlightNumber { get; set; }
        [XmlElement(ElementName = "ClassOfService", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ClassOfService { get; set; }
        [XmlElement(ElementName = "ActionCode", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ActionCode { get; set; }
        [XmlElement(ElementName = "NumberInParty", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string NumberInParty { get; set; }
        [XmlElement(ElementName = "inboundConnection", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string InboundConnection { get; set; }
        [XmlElement(ElementName = "outboundConnection", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string OutboundConnection { get; set; }
        [XmlElement(ElementName = "ScheduleChangeIndicator", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ScheduleChangeIndicator { get; set; }
        [XmlElement(ElementName = "SegmentBookedDate", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string SegmentBookedDate { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
        [XmlAttribute(AttributeName = "segmentAssociationId")]
        public string SegmentAssociationId { get; set; }
    }

    [XmlRoot(ElementName = "ProductDetails", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ProductDetails
    {
        [XmlElement(ElementName = "ProductName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ProductName ProductName { get; set; }
        [XmlElement(ElementName = "Air", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Air Air { get; set; }
        [XmlAttribute(AttributeName = "productCategory")]
        public string ProductCategory { get; set; }
    }

    [XmlRoot(ElementName = "Product", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Product
    {
        [XmlElement(ElementName = "ProductDetails", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ProductDetails ProductDetails { get; set; }
    }

    [XmlRoot(ElementName = "Item", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Item
    {
        [XmlElement(ElementName = "FlightSegment", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public FlightSegment FlightSegment { get; set; }
        [XmlElement(ElementName = "Product", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Product Product { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
    }

    [XmlRoot(ElementName = "ReservationItems", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ReservationItems
    {
        [XmlElement(ElementName = "Item", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<Item> Item { get; set; }
    }

    [XmlRoot(ElementName = "Ticketing", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Ticketing
    {
        [XmlAttribute(AttributeName = "TicketTimeLimit")]
        public string TicketTimeLimit { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
    }

    [XmlRoot(ElementName = "ItineraryInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ItineraryInfo
    {
        [XmlElement(ElementName = "ItineraryPricing", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ItineraryPricing ItineraryPricing { get; set; }
        [XmlElement(ElementName = "ReservationItems", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ReservationItems ReservationItems { get; set; }
        [XmlElement(ElementName = "Ticketing", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Ticketing Ticketing { get; set; }
    }

    [XmlRoot(ElementName = "Source", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Source
    {
        [XmlAttribute(AttributeName = "HomePseudoCityCode")]
        public string HomePseudoCityCode { get; set; }
        [XmlAttribute(AttributeName = "LastUpdateDateTime")]
        public string LastUpdateDateTime { get; set; }
        [XmlAttribute(AttributeName = "ReceivedFrom")]
        public string ReceivedFrom { get; set; }
        [XmlAttribute(AttributeName = "CreateDateTime")]
        public string CreateDateTime { get; set; }
        [XmlAttribute(AttributeName = "SequenceNumber")]
        public string SequenceNumber { get; set; }
        [XmlAttribute(AttributeName = "AAA_PseudoCityCode")]
        public string AAA_PseudoCityCode { get; set; }
        [XmlAttribute(AttributeName = "CreationAgent")]
        public string CreationAgent { get; set; }
        [XmlAttribute(AttributeName = "PseudoCityCode")]
        public string PseudoCityCode { get; set; }
    }

    [XmlRoot(ElementName = "Service", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class Service2
    {
        [XmlElement(ElementName = "PersonName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PersonName PersonName { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "SSR_Type")]
        public string SSR_Type { get; set; }
        [XmlAttribute(AttributeName = "SSR_Code")]
        public string SSR_Code { get; set; }
    }

    [XmlRoot(ElementName = "SpecialServiceInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class SpecialServiceInfo
    {
        [XmlElement(ElementName = "Service", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Service2 Service2 { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "ServiceRequest", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class ServiceRequest
    {
        [XmlElement(ElementName = "FreeText", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string FreeText { get; set; }
        [XmlElement(ElementName = "FullText", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string FullText { get; set; }
        [XmlAttribute(AttributeName = "serviceType")]
        public string ServiceType { get; set; }
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "serviceCount")]
        public string ServiceCount { get; set; }
        [XmlAttribute(AttributeName = "ssrType")]
        public string SsrType { get; set; }
        [XmlAttribute(AttributeName = "airlineCode")]
        public string AirlineCode { get; set; }
        [XmlAttribute(AttributeName = "actionCode")]
        public string ActionCode { get; set; }
    }

    [XmlRoot(ElementName = "NameAssociation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class NameAssociation
    {
        [XmlElement(ElementName = "LastName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "FirstName", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "ReferenceId", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string ReferenceId { get; set; }
        [XmlElement(ElementName = "NameRefNumber", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public string NameRefNumber { get; set; }
    }

    [XmlRoot(ElementName = "OpenReservationElement", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class OpenReservationElement
    {
        [XmlElement(ElementName = "ServiceRequest", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ServiceRequest ServiceRequest { get; set; }
        [XmlElement(ElementName = "NameAssociation", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public NameAssociation NameAssociation { get; set; }
        [XmlAttribute(AttributeName = "elementId")]
        public string ElementId { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "Email", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public Email Email { get; set; }
    }

    [XmlRoot(ElementName = "OpenReservationElements", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class OpenReservationElements
    {
        [XmlElement(ElementName = "OpenReservationElement", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<OpenReservationElement> OpenReservationElement { get; set; }
    }

    [XmlRoot(ElementName = "TravelItinerary", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class TravelItinerary
    {
        [XmlElement(ElementName = "CustomerInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public CustomerInfo CustomerInfo { get; set; }
        [XmlElement(ElementName = "ItineraryInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ItineraryInfo ItineraryInfo { get; set; }
        [XmlElement(ElementName = "ItineraryRef", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ItineraryRef ItineraryRef { get; set; }
        [XmlElement(ElementName = "SpecialServiceInfo", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public List<SpecialServiceInfo> SpecialServiceInfo { get; set; }
        [XmlElement(ElementName = "OpenReservationElements", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public OpenReservationElements OpenReservationElements { get; set; }
    }

    [XmlRoot(ElementName = "TravelItineraryReadRS", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class TravelItineraryReadRS
    {
        [XmlElement(ElementName = "TravelItinerary", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public TravelItinerary TravelItinerary { get; set; }
    }

    [XmlRoot(ElementName = "PassengerDetailsRS", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
    public class PassengerDetailsRS
    {
        [XmlElement(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL_Payload/v02_01")]
        public ApplicationResults ApplicationResults { get; set; }
        [XmlElement(ElementName = "ItineraryRef", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public ItineraryRef ItineraryRef { get; set; }
        [XmlElement(ElementName = "TravelItineraryReadRS", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public TravelItineraryReadRS TravelItineraryReadRS { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(ElementName = "PassengerDetailsRS", Namespace = "http://services.sabre.com/sp/pd/v3_4")]
        public PassengerDetailsRS PassengerDetailsRS { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Header Header { get; set; }
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body { get; set; }
        [XmlAttribute(AttributeName = "soap-env", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Soapenv { get; set; }
    }

}
