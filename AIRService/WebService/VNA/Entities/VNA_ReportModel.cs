using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class VNA_ReportModel
    {
        public DateTime ReportDate { get; set; }
        public string EmpNumber { get; set; }
    }

    public class EmpReportDate
    {
        public string ReportDate { get; set; }
    }

    public class VNA_EmpReportModel : TokenModel
    {
        public DateTime ReportDate { get; set; }
    }

    public class VNA_ReportDetailsModel : TokenModel
    {
        public DateTime ReportDate { get; set; }
        public List<string> DocumentNumbers { get; set; }
        public string EmpNumber { get; set; }
    }

    public class ReportSaleSummaryTransaction
    {
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string PassengerName { get; set; }
        public string PnrLocator { get; set; }
        public string TicketPrinterLniata { get; set; }
        public string TransactionTime { get; set; }
        public bool ExceptionItem { get; set; }
        public bool DecoupleItem { get; set; }
        public string TicketStatusCode { get; set; }
        public bool IsElectronicTicket { get; set; }
        public ReportSaleSummaryTransactionSSFop SaleSummaryTransactionSSFop { get; set; }
    }

    public class ReportSaleSummaryTransactionSSFop
    {
        public string FopCode { get; set; }
        public string CurrencyCode { get; set; }
        public double FareAmount { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
    }
    public class ReportSummaryDetail
    {
        public string DocumentNumber { get; set; }
        public string AssociatedDocument { get; set; }
        public string ReasonForIssuanceCode { get; set; }
        public string ReasonForIssuanceDesc { get; set; }
        public int CouponNumber { get; set; }
        public string TicketingProvider { get; set; }
        public int FlightNumber { get; set; }
        public string ClassOfService { get; set; }
        public string DepartureDtm { get; set; }
        public string ArrivalDtm { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string CouponStatus { get; set; }
        public string FareBasis { get; set; }
        public string BaggageAllowance { get; set; }
    }
    //
    public class VNA_ReportSaleSummaryResult
    {
        public string EmpNumber { get; set; }
        public bool Status { get; set; }
        public List<ReportSaleSummaryTransaction> SaleSummaryTransaction { get; set; }
    }


    public class VNA_ReportSaleSummaryDetailResult
    {
        public string DocumentNumber { get; set; }
        public bool Status { get; set; }
        public bool StatusGetDetail { get; set; }
        public ReportSummaryDetail SaleSummaryDetail { get; set; }
    }


    // ****************************************************************************************************************************

    public class VNA_ReportSaleSummaryTicketing
    {
        public List<VNA_ReportSaleSummaryTicketingDocument> SaleSummaryTicketingDocument { get; set; }
        public VNA_ReportSaleSummaryTicketingDocumentAmount SaleSummaryTicketingDocumentAmount { get; set; }
        public List<VNA_ReportSaleSummaryTicketingDocumentTaxes> SaleSummaryTicketingDocumentTaxes { get; set; }
    }

    public class VNA_ReportSaleSummaryTicketingDocument
    {
        public string DocumentNumber { get; set; }
        public string MarketingFlightNumber { get; set; }
        public string ClassOfService { get; set; }
        public string FareBasis { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string BookingStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string SystemDateTime { get; set; }
        public string FlownCoupon_DepartureDateTime { get; set; }
    }
    public class VNA_ReportSaleSummaryTicketingDocumentAmount
    {
        public string DocumentNumber { get; set; }
        public double BaseAmount { get; set; }
        public double TotalTax { get; set; }
        public double Total { get; set; }
        public double NonRefundable { get; set; }
        public string Unit { get; set; }
    }
    public class VNA_ReportSaleSummaryTicketingDocumentTaxes
    {
        public string DocumentNumber { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }

    // ****************************************************************************************************************************












}



namespace XMLObject.ReportSaleSummay
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
        public string Service { get; set; }
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

    [XmlRoot(ElementName = "OrchestrationID", Namespace = "http://services.sabre.com/STL/v01")]
    public class OrchestrationID
    {
        [XmlAttribute(AttributeName = "seq")]
        public string Seq { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DiagnosticData", Namespace = "http://services.sabre.com/STL/v01")]
    public class DiagnosticData
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Success", Namespace = "http://services.sabre.com/STL/v01")]
    public class Success
    {
        [XmlElement(ElementName = "System", Namespace = "http://services.sabre.com/STL/v01")]
        public string System { get; set; }
        [XmlElement(ElementName = "Source", Namespace = "http://services.sabre.com/STL/v01")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
    }

    [XmlRoot(ElementName = "Results", Namespace = "http://services.sabre.com/STL/v01")]
    public class Results
    {
        [XmlElement(ElementName = "Success", Namespace = "http://services.sabre.com/STL/v01")]
        public Success Success { get; set; }
    }

    [XmlRoot(ElementName = "STL_Header.RS", Namespace = "http://services.sabre.com/STL/v01")]
    public class STL_HeaderRS
    {
        [XmlElement(ElementName = "OrchestrationID", Namespace = "http://services.sabre.com/STL/v01")]
        public OrchestrationID OrchestrationID { get; set; }
        [XmlElement(ElementName = "DiagnosticData", Namespace = "http://services.sabre.com/STL/v01")]
        public DiagnosticData DiagnosticData { get; set; }
        [XmlElement(ElementName = "Results", Namespace = "http://services.sabre.com/STL/v01")]
        public Results Results { get; set; }
        [XmlAttribute(AttributeName = "messageID")]
        public string MessageID { get; set; }
        [XmlAttribute(AttributeName = "timeStamp")]
        public string TimeStamp { get; set; }
    }

    [XmlRoot(ElementName = "TicketingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class TicketingProvider
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlAttribute(AttributeName = "checkDigit")]
        public string CheckDigit { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "number")]
        public string Number { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "StationLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class StationLocation
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "StationNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class StationNumber
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "HomeLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class HomeLocation
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Lniata", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Lniata
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "IsoCountryCode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class IsoCountryCode
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "EmployeeNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class EmployeeNumber
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ShiftStartDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ShiftStartDateTime
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AgentOpenType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class AgentOpenType
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AgentCloseType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class AgentCloseType
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "StationCloseType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class StationCloseType
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Session", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Session
    {
        [XmlElement(ElementName = "AgentOpenType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public AgentOpenType AgentOpenType { get; set; }
        [XmlElement(ElementName = "AgentCloseType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public AgentCloseType AgentCloseType { get; set; }
        [XmlElement(ElementName = "StationCloseType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public StationCloseType StationCloseType { get; set; }
    }

    [XmlRoot(ElementName = "Agent", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Agent
    {
        [XmlElement(ElementName = "TicketingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TicketingProvider TicketingProvider { get; set; }
        [XmlElement(ElementName = "StationLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public StationLocation StationLocation { get; set; }
        [XmlElement(ElementName = "StationNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public StationNumber StationNumber { get; set; }
        [XmlElement(ElementName = "WorkLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string WorkLocation { get; set; }
        [XmlElement(ElementName = "HomeLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public HomeLocation HomeLocation { get; set; }
        [XmlElement(ElementName = "Lniata", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Lniata Lniata { get; set; }
        [XmlElement(ElementName = "IsoCountryCode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public IsoCountryCode IsoCountryCode { get; set; }
        [XmlElement(ElementName = "CompanyName", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string CompanyName { get; set; }
        [XmlElement(ElementName = "EmployeeNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public EmployeeNumber EmployeeNumber { get; set; }
        [XmlElement(ElementName = "ShiftStartDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ShiftStartDateTime ShiftStartDateTime { get; set; }
        [XmlElement(ElementName = "Session", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Session Session { get; set; }
        [XmlAttribute(AttributeName = "duty")]
        public string Duty { get; set; }
        [XmlAttribute(AttributeName = "sine")]
        public string Sine { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "TCN", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class TCN
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "LocalDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class LocalDateTime
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SystemDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class SystemDateTime
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SystemProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class SystemProvider
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "InputEntry", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class InputEntry
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "TransactionInfo", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class TransactionInfo
    {
        [XmlElement(ElementName = "TCN", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TCN TCN { get; set; }
        [XmlElement(ElementName = "LocalDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public LocalDateTime LocalDateTime { get; set; }
        [XmlElement(ElementName = "SystemDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public SystemDateTime SystemDateTime { get; set; }
        [XmlElement(ElementName = "SystemProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public SystemProvider SystemProvider { get; set; }
        [XmlElement(ElementName = "InputEntry", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public InputEntry InputEntry { get; set; }
        [XmlAttribute(AttributeName = "actionType")]
        public string ActionType { get; set; }
        [XmlAttribute(AttributeName = "category")]
        public string Category { get; set; }
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "InputMessage", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public InputMessage InputMessage { get; set; }
    }

    [XmlRoot(ElementName = "Indicators", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Indicators
    {
        [XmlAttribute(AttributeName = "electronic")]
        public string Electronic { get; set; }
        [XmlAttribute(AttributeName = "endorsable")]
        public string Endorsable { get; set; }
        [XmlAttribute(AttributeName = "exchangeable")]
        public string Exchangeable { get; set; }
        [XmlAttribute(AttributeName = "historical")]
        public string Historical { get; set; }
        [XmlAttribute(AttributeName = "manualAdd")]
        public string ManualAdd { get; set; }
        [XmlAttribute(AttributeName = "manualUpdate")]
        public string ManualUpdate { get; set; }
        [XmlAttribute(AttributeName = "netReporting")]
        public string NetReporting { get; set; }
        [XmlAttribute(AttributeName = "penaltyRestriction")]
        public string PenaltyRestriction { get; set; }
        [XmlAttribute(AttributeName = "presentCreditCard")]
        public string PresentCreditCard { get; set; }
        [XmlAttribute(AttributeName = "refundCalculation")]
        public string RefundCalculation { get; set; }
        [XmlAttribute(AttributeName = "reservarionPurge")]
        public string ReservarionPurge { get; set; }
        [XmlAttribute(AttributeName = "retransmit")]
        public string Retransmit { get; set; }
        [XmlAttribute(AttributeName = "reverseVoid")]
        public string ReverseVoid { get; set; }
        [XmlAttribute(AttributeName = "selfSale")]
        public string SelfSale { get; set; }
        [XmlAttribute(AttributeName = "fareBreak")]
        public string FareBreak { get; set; }
        [XmlAttribute(AttributeName = "sideTripEnd")]
        public string SideTripEnd { get; set; }
        [XmlAttribute(AttributeName = "sideTripStart")]
        public string SideTripStart { get; set; }
        [XmlAttribute(AttributeName = "unchargeableSurface")]
        public string UnchargeableSurface { get; set; }
        [XmlAttribute(AttributeName = "zeroFareAmount")]
        public string ZeroFareAmount { get; set; }
    }

    [XmlRoot(ElementName = "Sabre", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Sabre
    {
        [XmlAttribute(AttributeName = "createDate")]
        public string CreateDate { get; set; }
        [XmlAttribute(AttributeName = "provider")]
        public string Provider { get; set; }
        [XmlAttribute(AttributeName = "purgeDate")]
        public string PurgeDate { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Reservation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Reservation
    {
        [XmlElement(ElementName = "Sabre", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Sabre Sabre { get; set; }
    }

    [XmlRoot(ElementName = "SystemCreateDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class SystemCreateDateTime
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "LocalIssueDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class LocalIssueDateTime
    {
        [XmlAttribute(AttributeName = "useTimeForPricing")]
        public string UseTimeForPricing { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "LastUpdate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class LastUpdate
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ValidatingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ValidatingProvider
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlAttribute(AttributeName = "checkDigit")]
        public string CheckDigit { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "number")]
        public string Number { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ExchCategory", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ExchCategory
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "FareCalculationMode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FareCalculationMode
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "TicketingMode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class TicketingMode
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CouponText", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class CouponText
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "BaggageDisclosure", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class BaggageDisclosure
    {
        [XmlAttribute(AttributeName = "disclosureIndicator")]
        public string DisclosureIndicator { get; set; }
    }

    [XmlRoot(ElementName = "Details", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Details
    {
        [XmlElement(ElementName = "Reservation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Reservation Reservation { get; set; }
        [XmlElement(ElementName = "SystemCreateDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public SystemCreateDateTime SystemCreateDateTime { get; set; }
        [XmlElement(ElementName = "LocalIssueDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public LocalIssueDateTime LocalIssueDateTime { get; set; }
        [XmlElement(ElementName = "LastUpdate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public LastUpdate LastUpdate { get; set; }
        [XmlElement(ElementName = "ValidatingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ValidatingProvider ValidatingProvider { get; set; }
        [XmlElement(ElementName = "ExchCategory", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ExchCategory ExchCategory { get; set; }
        [XmlElement(ElementName = "FareCalculationMode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FareCalculationMode FareCalculationMode { get; set; }
        [XmlElement(ElementName = "TicketingMode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TicketingMode TicketingMode { get; set; }
        [XmlElement(ElementName = "CouponText", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public CouponText CouponText { get; set; }
        [XmlElement(ElementName = "BaggageDisclosure", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public BaggageDisclosure BaggageDisclosure { get; set; }
        [XmlElement(ElementName = "SupportingDocument", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public SupportingDocument SupportingDocument { get; set; }
        [XmlElement(ElementName = "OldReservation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public OldReservation OldReservation { get; set; }
        [XmlElement(ElementName = "DocumentPurgeTypeCode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public DocumentPurgeTypeCode DocumentPurgeTypeCode { get; set; }
        [XmlElement(ElementName = "AffectedCoupons", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public AffectedCoupons AffectedCoupons { get; set; }
        [XmlElement(ElementName = "Agent", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Agent Agent { get; set; }
        [XmlElement(ElementName = "TransactionInfo", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TransactionInfo TransactionInfo { get; set; }
        [XmlElement(ElementName = "Ticket", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Ticket Ticket { get; set; }
    }

    [XmlRoot(ElementName = "FirstName", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FirstName
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "LastName", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class LastName
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Contact", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Contact
    {
        [XmlElement(ElementName = "Phone", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string Phone { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
    }

    [XmlRoot(ElementName = "Traveler", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Traveler
    {
        [XmlElement(ElementName = "Name", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string Name { get; set; }
        [XmlElement(ElementName = "FirstName", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FirstName FirstName { get; set; }
        [XmlElement(ElementName = "LastName", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public LastName LastName { get; set; }
        [XmlElement(ElementName = "PassengerType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string PassengerType { get; set; }
        [XmlElement(ElementName = "Contact", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Contact Contact { get; set; }
        [XmlAttribute(AttributeName = "nameId")]
        public string NameId { get; set; }
        [XmlAttribute(AttributeName = "nameNumber")]
        public string NameNumber { get; set; }
    }

    [XmlRoot(ElementName = "Customer", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Customer
    {
        [XmlElement(ElementName = "Traveler", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Traveler Traveler { get; set; }
    }

    [XmlRoot(ElementName = "MarketingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class MarketingProvider
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlAttribute(AttributeName = "checkDigit")]
        public string CheckDigit { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "number")]
        public string Number { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "MarketingFlightNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class MarketingFlightNumber
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ClassOfService", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ClassOfService
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "FareBasis", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FareBasis
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "StartLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class StartLocation
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "StartDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class StartDateTime
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "EndLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class EndLocation
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "EndDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class EndDateTime
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "NotValidBeforeDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class NotValidBeforeDate
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "NotValidAfterDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class NotValidAfterDate
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "BookingStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class BookingStatus
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CurrentStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class CurrentStatus
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PreviousStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class PreviousStatus
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "OperatingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class OperatingProvider
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "OperatingFlightNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class OperatingFlightNumber
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DepartureCity", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class DepartureCity
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DepartureDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class DepartureDateTime
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ArrivalCity", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ArrivalCity
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "FlightOriginalDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FlightOriginalDate
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "FlownCoupon", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FlownCoupon
    {
        [XmlElement(ElementName = "MarketingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public MarketingProvider MarketingProvider { get; set; }
        [XmlElement(ElementName = "OperatingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public OperatingProvider OperatingProvider { get; set; }
        [XmlElement(ElementName = "OperatingFlightNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public OperatingFlightNumber OperatingFlightNumber { get; set; }
        [XmlElement(ElementName = "ClassOfService", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ClassOfService ClassOfService { get; set; }
        [XmlElement(ElementName = "DepartureCity", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public DepartureCity DepartureCity { get; set; }
        [XmlElement(ElementName = "DepartureDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public DepartureDateTime DepartureDateTime { get; set; }
        [XmlElement(ElementName = "ArrivalCity", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ArrivalCity ArrivalCity { get; set; }
        [XmlElement(ElementName = "FlightOriginalDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FlightOriginalDate FlightOriginalDate { get; set; }
        [XmlAttribute(AttributeName = "coupon")]
        public string Coupon { get; set; }
    }

    [XmlRoot(ElementName = "BagAllowance", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class BagAllowance
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CouponUse", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class CouponUse
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "StartTimeText", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class StartTimeText
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Amount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Amount
    {
        [XmlAttribute(AttributeName = "currencyCode")]
        public string CurrencyCode { get; set; }
        [XmlAttribute(AttributeName = "decimalPlace")]
        public string DecimalPlace { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
    }

    [XmlRoot(ElementName = "FareBreakAmount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FareBreakAmount
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Amount Amount { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
    }

    [XmlRoot(ElementName = "PricingRecordFareType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class PricingRecordFareType
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AssociatedFareBasis", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class AssociatedFareBasis
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Tariff", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Tariff
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "FareRule", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FareRule
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PricingVendor", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class PricingVendor
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "FareComponent", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FareComponent
    {
        [XmlElement(ElementName = "AssociatedFareBasis", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public AssociatedFareBasis AssociatedFareBasis { get; set; }
        [XmlElement(ElementName = "FareProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string FareProvider { get; set; }
        [XmlElement(ElementName = "Tariff", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Tariff Tariff { get; set; }
        [XmlElement(ElementName = "FareRule", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FareRule FareRule { get; set; }
        [XmlElement(ElementName = "PricingVendor", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public PricingVendor PricingVendor { get; set; }
        [XmlAttribute(AttributeName = "IATAAuthorisedIndicator")]
        public string IATAAuthorisedIndicator { get; set; }
        [XmlAttribute(AttributeName = "appendNonrefundableIndicator")]
        public string AppendNonrefundableIndicator { get; set; }
        [XmlAttribute(AttributeName = "flownIndicator")]
        public string FlownIndicator { get; set; }
        [XmlAttribute(AttributeName = "inboundIndicator")]
        public string InboundIndicator { get; set; }
        [XmlAttribute(AttributeName = "oneWayDirectionalIndicator")]
        public string OneWayDirectionalIndicator { get; set; }
        [XmlAttribute(AttributeName = "oneWayIndicator")]
        public string OneWayIndicator { get; set; }
        [XmlAttribute(AttributeName = "roundTripIndicator")]
        public string RoundTripIndicator { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
    }

    [XmlRoot(ElementName = "ServiceCoupon", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ServiceCoupon
    {
        [XmlElement(ElementName = "MarketingProvider", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public MarketingProvider MarketingProvider { get; set; }
        [XmlElement(ElementName = "MarketingFlightNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public MarketingFlightNumber MarketingFlightNumber { get; set; }
        [XmlElement(ElementName = "ClassOfService", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ClassOfService ClassOfService { get; set; }
        [XmlElement(ElementName = "FareBasis", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FareBasis FareBasis { get; set; }
        [XmlElement(ElementName = "StartLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public StartLocation StartLocation { get; set; }
        [XmlElement(ElementName = "StartDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public StartDateTime StartDateTime { get; set; }
        [XmlElement(ElementName = "EndLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public EndLocation EndLocation { get; set; }
        [XmlElement(ElementName = "EndDateTime", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public EndDateTime EndDateTime { get; set; }
        [XmlElement(ElementName = "NotValidBeforeDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public NotValidBeforeDate NotValidBeforeDate { get; set; }
        [XmlElement(ElementName = "NotValidAfterDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public NotValidAfterDate NotValidAfterDate { get; set; }
        [XmlElement(ElementName = "BookingStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public BookingStatus BookingStatus { get; set; }
        [XmlElement(ElementName = "CurrentStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public CurrentStatus CurrentStatus { get; set; }
        [XmlElement(ElementName = "PreviousStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public PreviousStatus PreviousStatus { get; set; }
        [XmlElement(ElementName = "FlownCoupon", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FlownCoupon FlownCoupon { get; set; }
        [XmlElement(ElementName = "BagAllowance", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public BagAllowance BagAllowance { get; set; }
        [XmlElement(ElementName = "CouponUse", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public CouponUse CouponUse { get; set; }
        [XmlElement(ElementName = "Indicators", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Indicators Indicators { get; set; }
        [XmlElement(ElementName = "StartTimeText", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public StartTimeText StartTimeText { get; set; }
        [XmlElement(ElementName = "FareBreakAmount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FareBreakAmount FareBreakAmount { get; set; }
        [XmlElement(ElementName = "PricingRecordFareType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public PricingRecordFareType PricingRecordFareType { get; set; }
        [XmlElement(ElementName = "FareType", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string FareType { get; set; }
        [XmlElement(ElementName = "FareComponent", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FareComponent FareComponent { get; set; }
        [XmlAttribute(AttributeName = "coupon")]
        public string Coupon { get; set; }
        [XmlAttribute(AttributeName = "entitlement")]
        public string Entitlement { get; set; }
    }

    [XmlRoot(ElementName = "Base", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Base
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Amount Amount { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlElement(ElementName = "ApplyCreditInd", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ApplyCreditInd ApplyCreditInd { get; set; }
    }

    [XmlRoot(ElementName = "TotalTax", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class TotalTax
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Amount Amount { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
    }

    [XmlRoot(ElementName = "Text", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class TextTx
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Total", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Total
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Amount Amount { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TextTx Text { get; set; }
        [XmlElement(ElementName = "ApplyCreditInd", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ApplyCreditInd ApplyCreditInd { get; set; }
    }

    [XmlRoot(ElementName = "Tax", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Tax
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Amount Amount { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "exempt")]
        public string Exempt { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TextTx Text { get; set; }
        [XmlElement(ElementName = "ApplyCreditInd", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ApplyCreditInd ApplyCreditInd { get; set; }
    }

    [XmlRoot(ElementName = "New", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class New
    {
        [XmlElement(ElementName = "Base", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Base Base { get; set; }
        [XmlElement(ElementName = "TotalTax", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TotalTax TotalTax { get; set; }
        [XmlElement(ElementName = "Total", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Total Total { get; set; }
        [XmlElement(ElementName = "Tax", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public List<Tax> Tax { get; set; }
    }

    [XmlRoot(ElementName = "NonRefundable", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class NonRefundable
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Amount Amount { get; set; }
        [XmlElement(ElementName = "Text", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
    }

    [XmlRoot(ElementName = "Other", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Other
    {
        [XmlElement(ElementName = "NonRefundable", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public NonRefundable NonRefundable { get; set; }
    }

    [XmlRoot(ElementName = "Amounts", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Amounts
    {
        [XmlElement(ElementName = "New", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public New New { get; set; }
        [XmlElement(ElementName = "Other", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Other Other { get; set; }
    }

    [XmlRoot(ElementName = "Taxes", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Taxes
    {
        [XmlElement(ElementName = "New", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public New New { get; set; }
    }

    [XmlRoot(ElementName = "Endorsements", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Endorsements
    {
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Payment", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Payment
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlElement(ElementName = "Base", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Base Base { get; set; }
        [XmlElement(ElementName = "Tax", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Tax Tax { get; set; }
        [XmlElement(ElementName = "Total", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Total Total { get; set; }
        [XmlElement(ElementName = "Remarks", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string Remarks { get; set; }
        [XmlElement(ElementName = "Card", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Card Card { get; set; }
        [XmlAttribute(AttributeName = "paymentConfirmation")]
        public string PaymentConfirmation { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "Remark", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Remark
    {
        [XmlElement(ElementName = "Endorsements", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Endorsements Endorsements { get; set; }
        [XmlElement(ElementName = "Payment", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Payment Payment { get; set; }
    }

    [XmlRoot(ElementName = "IataNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class IataNumber
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Booking", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Booking
    {
        [XmlElement(ElementName = "WorkLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string WorkLocation { get; set; }
        [XmlElement(ElementName = "IataNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public IataNumber IataNumber { get; set; }
        [XmlElement(ElementName = "HomeLocation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public HomeLocation HomeLocation { get; set; }
        [XmlAttribute(AttributeName = "duty")]
        public string Duty { get; set; }
        [XmlAttribute(AttributeName = "sine")]
        public string Sine { get; set; }
    }

    [XmlRoot(ElementName = "AffiliatedAgent", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class AffiliatedAgent
    {
        [XmlElement(ElementName = "Booking", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Booking Booking { get; set; }
    }

    [XmlRoot(ElementName = "FareCalculation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class FareCalculation
    {
        [XmlElement(ElementName = "New", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string New { get; set; }
    }

    [XmlRoot(ElementName = "PrintCoupon", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class PrintCoupon
    {
        [XmlElement(ElementName = "Lniata", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Lniata Lniata { get; set; }
        [XmlAttribute(AttributeName = "couponType")]
        public string CouponType { get; set; }
        [XmlAttribute(AttributeName = "printerType")]
        public string PrinterType { get; set; }
        [XmlAttribute(AttributeName = "quantity")]
        public string Quantity { get; set; }
        [XmlAttribute(AttributeName = "stockType")]
        public string StockType { get; set; }
    }

    [XmlRoot(ElementName = "ApplyCreditInd", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ApplyCreditInd
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "MaskedCardNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class MaskedCardNumber
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ExpireDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ExpireDate
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ApprovalCode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ApprovalCode
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "CardBinNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class CardBinNumber
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "TransactionId", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class TransactionId
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Card", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Card
    {
        [XmlElement(ElementName = "MaskedCardNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public MaskedCardNumber MaskedCardNumber { get; set; }
        [XmlElement(ElementName = "ExpireDate", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ExpireDate ExpireDate { get; set; }
        [XmlElement(ElementName = "ApprovalCode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ApprovalCode ApprovalCode { get; set; }
        [XmlElement(ElementName = "CardBinNumber", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public CardBinNumber CardBinNumber { get; set; }
        [XmlElement(ElementName = "TransactionId", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TransactionId TransactionId { get; set; }
        [XmlAttribute(AttributeName = "cardType")]
        public string CardType { get; set; }
    }

    [XmlRoot(ElementName = "InputMessage", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class InputMessage
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SupportingDocument", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class SupportingDocument
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "OldReservation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class OldReservation
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DocumentPurgeTypeCode", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class DocumentPurgeTypeCode
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ServiceCouponHistory", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class ServiceCouponHistory
    {
        [XmlElement(ElementName = "CurrentStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string CurrentStatus { get; set; }
        [XmlElement(ElementName = "PreviousStatus", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public string PreviousStatus { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlAttribute(AttributeName = "coupon")]
        public string Coupon { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
    }

    [XmlRoot(ElementName = "History", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class History
    {
        [XmlElement(ElementName = "Agent", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Agent Agent { get; set; }
        [XmlElement(ElementName = "TransactionInfo", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public TransactionInfo TransactionInfo { get; set; }
        [XmlElement(ElementName = "Details", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Details Details { get; set; }
        [XmlElement(ElementName = "ServiceCouponHistory", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ServiceCouponHistory ServiceCouponHistory { get; set; }
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "number")]
        public string Number { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
    }

    [XmlRoot(ElementName = "AffectedCoupons", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class AffectedCoupons
    {
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Ticket", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class Ticket
    {
        [XmlElement(ElementName = "Indicators", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Indicators Indicators { get; set; }
        [XmlElement(ElementName = "Details", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Details Details { get; set; }
        [XmlElement(ElementName = "Customer", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Customer Customer { get; set; }
        [XmlElement(ElementName = "ServiceCoupon", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public ServiceCoupon ServiceCoupon { get; set; }
        [XmlElement(ElementName = "Amounts", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Amounts Amounts { get; set; }
        [XmlElement(ElementName = "Taxes", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Taxes Taxes { get; set; }
        [XmlElement(ElementName = "Remark", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Remark Remark { get; set; }
        [XmlElement(ElementName = "AffiliatedAgent", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public AffiliatedAgent AffiliatedAgent { get; set; }
        [XmlElement(ElementName = "FareCalculation", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public FareCalculation FareCalculation { get; set; }
        [XmlElement(ElementName = "PrintCoupon", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public PrintCoupon PrintCoupon { get; set; }
        [XmlElement(ElementName = "Payment", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public Payment Payment { get; set; }
        [XmlElement(ElementName = "History", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public List<History> History { get; set; }
        [XmlAttribute(AttributeName = "accountingCode")]
        public string AccountingCode { get; set; }
        [XmlAttribute(AttributeName = "checkDigit")]
        public string CheckDigit { get; set; }
        [XmlAttribute(AttributeName = "formNumber")]
        public string FormNumber { get; set; }
        [XmlAttribute(AttributeName = "number")]
        public string Number { get; set; }
        [XmlAttribute(AttributeName = "serialNumber")]
        public string SerialNumber { get; set; }
        [XmlAttribute(AttributeName = "service")]
        public string Service { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "GetTicketingDocumentRS", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
    public class GetTicketingDocumentRS
    {
        [XmlElement(ElementName = "STL_Header.RS", Namespace = "http://services.sabre.com/STL/v01")]
        public STL_HeaderRS STL_HeaderRS { get; set; }
        [XmlElement(ElementName = "Details", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public List<Details> Details { get; set; }
        [XmlAttribute(AttributeName = "TT", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string TT { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "STL", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string STL { get; set; }
        [XmlAttribute(AttributeName = "fn", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Fn { get; set; }
        [XmlAttribute(AttributeName = "str", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Str { get; set; }
        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xs { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(ElementName = "GetTicketingDocumentRS", Namespace = "http://www.sabre.com/ns/Ticketing/DC")]
        public GetTicketingDocumentRS GetTicketingDocumentRS { get; set; }
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
