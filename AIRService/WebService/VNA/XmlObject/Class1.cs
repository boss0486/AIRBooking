using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WebService.VNA.XmlObject
{
	/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
	using System;
	using System.Xml.Serialization;
	using System.Collections.Generic;
	namespace Xml2CSharp
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

		[XmlRoot(ElementName = "FlightsRange", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class FlightsRange
		{
			[XmlAttribute(AttributeName = "Start")]
			public string Start { get; set; }
			[XmlAttribute(AttributeName = "End")]
			public string End { get; set; }
		}

		[XmlRoot(ElementName = "BookingDetails", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class BookingDetails
		{
			[XmlElement(ElementName = "RecordLocator", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string RecordLocator { get; set; }
			[XmlElement(ElementName = "CreationTimestamp", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string CreationTimestamp { get; set; }
			[XmlElement(ElementName = "SystemCreationTimestamp", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string SystemCreationTimestamp { get; set; }
			[XmlElement(ElementName = "CreationAgentID", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string CreationAgentID { get; set; }
			[XmlElement(ElementName = "UpdateTimestamp", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string UpdateTimestamp { get; set; }
			[XmlElement(ElementName = "PNRSequence", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string PNRSequence { get; set; }
			[XmlElement(ElementName = "FlightsRange", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public FlightsRange FlightsRange { get; set; }
			[XmlElement(ElementName = "DivideSplitDetails", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DivideSplitDetails { get; set; }
			[XmlElement(ElementName = "EstimatedPurgeTimestamp", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string EstimatedPurgeTimestamp { get; set; }
			[XmlElement(ElementName = "UpdateToken", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string UpdateToken { get; set; }
		}

		[XmlRoot(ElementName = "OAC", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class OAC
		{
			[XmlElement(ElementName = "PartitionId", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string PartitionId { get; set; }
			[XmlElement(ElementName = "AccountingCityCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string AccountingCityCode { get; set; }
			[XmlElement(ElementName = "AccountingCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string AccountingCode { get; set; }
			[XmlElement(ElementName = "AccountingOfficeStationCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string AccountingOfficeStationCode { get; set; }
		}

		[XmlRoot(ElementName = "Source", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Source
		{
			[XmlElement(ElementName = "OAC", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public OAC OAC { get; set; }
			[XmlAttribute(AttributeName = "BookingSource")]
			public string BookingSource { get; set; }
			[XmlAttribute(AttributeName = "AgentSine")]
			public string AgentSine { get; set; }
			[XmlAttribute(AttributeName = "PseudoCityCode")]
			public string PseudoCityCode { get; set; }
			[XmlAttribute(AttributeName = "ISOCountry")]
			public string ISOCountry { get; set; }
			[XmlAttribute(AttributeName = "AgentDutyCode")]
			public string AgentDutyCode { get; set; }
			[XmlAttribute(AttributeName = "AirlineVendorID")]
			public string AirlineVendorID { get; set; }
			[XmlAttribute(AttributeName = "HomePseudoCityCode")]
			public string HomePseudoCityCode { get; set; }
			[XmlAttribute(AttributeName = "PrimeHostID")]
			public string PrimeHostID { get; set; }
		}

		[XmlRoot(ElementName = "POS", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class POS
		{
			[XmlElement(ElementName = "Source", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Source Source { get; set; }
			[XmlAttribute(AttributeName = "AirExtras")]
			public string AirExtras { get; set; }
			[XmlAttribute(AttributeName = "InhibitCode")]
			public string InhibitCode { get; set; }
		}

		[XmlRoot(ElementName = "EmailAddress", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class EmailAddress
		{
			[XmlElement(ElementName = "Address", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Address { get; set; }
			[XmlElement(ElementName = "Comment", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Comment { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
		}

		[XmlRoot(ElementName = "Passenger", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Passenger
		{
			[XmlElement(ElementName = "LastName", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string LastName { get; set; }
			[XmlElement(ElementName = "FirstName", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string FirstName { get; set; }
			[XmlElement(ElementName = "EmailAddress", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public EmailAddress EmailAddress { get; set; }
			[XmlElement(ElementName = "Seats", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Seats { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "nameType")]
			public string NameType { get; set; }
			[XmlAttribute(AttributeName = "nameId")]
			public string NameId { get; set; }
			[XmlAttribute(AttributeName = "nameAssocId")]
			public string NameAssocId { get; set; }
			[XmlAttribute(AttributeName = "elementId")]
			public string ElementId { get; set; }
			[XmlElement(ElementName = "SpecialRequests", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public SpecialRequests SpecialRequests { get; set; }
			[XmlAttribute(AttributeName = "passengerType")]
			public string PassengerType { get; set; }
		}

		[XmlRoot(ElementName = "ChildRequest", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class ChildRequest
		{
			[XmlElement(ElementName = "DateOfBirth", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DateOfBirth { get; set; }
			[XmlElement(ElementName = "VendorCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string VendorCode { get; set; }
			[XmlElement(ElementName = "ActionCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ActionCode { get; set; }
			[XmlElement(ElementName = "NumberInParty", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string NumberInParty { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "type")]
			public string Type { get; set; }
		}

		[XmlRoot(ElementName = "SpecialRequests", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class SpecialRequests
		{
			[XmlElement(ElementName = "ChildRequest", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public List<ChildRequest> ChildRequest { get; set; }
		}

		[XmlRoot(ElementName = "Passengers", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Passengers
		{
			[XmlElement(ElementName = "Passenger", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public List<Passenger> Passenger { get; set; }
		}

		[XmlRoot(ElementName = "Poc", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Poc
		{
			[XmlElement(ElementName = "Airport", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Airport { get; set; }
			[XmlElement(ElementName = "Departure", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Departure { get; set; }
		}

		[XmlRoot(ElementName = "DisclosureCarrier", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class DisclosureCarrier
		{
			[XmlElement(ElementName = "Banner", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Banner { get; set; }
			[XmlAttribute(AttributeName = "Code")]
			public string Code { get; set; }
			[XmlAttribute(AttributeName = "DOT")]
			public string DOT { get; set; }
		}

		[XmlRoot(ElementName = "MarriageGrp", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class MarriageGrp
		{
			[XmlElement(ElementName = "Ind", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Ind { get; set; }
			[XmlElement(ElementName = "Group", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Group { get; set; }
			[XmlElement(ElementName = "Sequence", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Sequence { get; set; }
		}

		[XmlRoot(ElementName = "Meal", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Meal
		{
			[XmlAttribute(AttributeName = "Code")]
			public string Code { get; set; }
		}

		[XmlRoot(ElementName = "Pos", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Pos
		{
			[XmlElement(ElementName = "IataNumber", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string IataNumber { get; set; }
			[XmlElement(ElementName = "AgencyCityCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string AgencyCityCode { get; set; }
			[XmlElement(ElementName = "CountryCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string CountryCode { get; set; }
			[XmlElement(ElementName = "DutyCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DutyCode { get; set; }
			[XmlElement(ElementName = "OACAccountingCityCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string OACAccountingCityCode { get; set; }
			[XmlElement(ElementName = "OACAccountingCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string OACAccountingCode { get; set; }
		}

		[XmlRoot(ElementName = "Cabin", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Cabin
		{
			[XmlAttribute(AttributeName = "Code")]
			public string Code { get; set; }
			[XmlAttribute(AttributeName = "SabreCode")]
			public string SabreCode { get; set; }
			[XmlAttribute(AttributeName = "Name")]
			public string Name { get; set; }
			[XmlAttribute(AttributeName = "ShortName")]
			public string ShortName { get; set; }
			[XmlAttribute(AttributeName = "Lang")]
			public string Lang { get; set; }
		}

		[XmlRoot(ElementName = "Air", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Air
		{
			[XmlElement(ElementName = "DepartureAirport", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DepartureAirport { get; set; }
			[XmlElement(ElementName = "DepartureAirportCodeContext", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DepartureAirportCodeContext { get; set; }
			[XmlElement(ElementName = "DepartureTerminalName", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DepartureTerminalName { get; set; }
			[XmlElement(ElementName = "DepartureTerminalCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DepartureTerminalCode { get; set; }
			[XmlElement(ElementName = "ArrivalAirport", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ArrivalAirport { get; set; }
			[XmlElement(ElementName = "ArrivalAirportCodeContext", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ArrivalAirportCodeContext { get; set; }
			[XmlElement(ElementName = "ArrivalTerminalName", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ArrivalTerminalName { get; set; }
			[XmlElement(ElementName = "ArrivalTerminalCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ArrivalTerminalCode { get; set; }
			[XmlElement(ElementName = "OperatingAirlineCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string OperatingAirlineCode { get; set; }
			[XmlElement(ElementName = "OperatingAirlineShortName", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string OperatingAirlineShortName { get; set; }
			[XmlElement(ElementName = "OperatingFlightNumber", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string OperatingFlightNumber { get; set; }
			[XmlElement(ElementName = "EquipmentType", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string EquipmentType { get; set; }
			[XmlElement(ElementName = "MarketingAirlineCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string MarketingAirlineCode { get; set; }
			[XmlElement(ElementName = "MarketingFlightNumber", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string MarketingFlightNumber { get; set; }
			[XmlElement(ElementName = "OperatingClassOfService", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string OperatingClassOfService { get; set; }
			[XmlElement(ElementName = "MarketingClassOfService", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string MarketingClassOfService { get; set; }
			[XmlElement(ElementName = "DisclosureCarrier", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public DisclosureCarrier DisclosureCarrier { get; set; }
			[XmlElement(ElementName = "MarriageGrp", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public MarriageGrp MarriageGrp { get; set; }
			[XmlElement(ElementName = "Meal", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Meal Meal { get; set; }
			[XmlElement(ElementName = "Seats", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Seats { get; set; }
			[XmlElement(ElementName = "Eticket", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Eticket { get; set; }
			[XmlElement(ElementName = "DepartureDateTime", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string DepartureDateTime { get; set; }
			[XmlElement(ElementName = "ArrivalDateTime", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ArrivalDateTime { get; set; }
			[XmlElement(ElementName = "FlightNumber", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string FlightNumber { get; set; }
			[XmlElement(ElementName = "ClassOfService", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ClassOfService { get; set; }
			[XmlElement(ElementName = "ActionCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ActionCode { get; set; }
			[XmlElement(ElementName = "NumberInParty", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string NumberInParty { get; set; }
			[XmlElement(ElementName = "SegmentSpecialRequests", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string SegmentSpecialRequests { get; set; }
			[XmlElement(ElementName = "inboundConnection", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string InboundConnection { get; set; }
			[XmlElement(ElementName = "outboundConnection", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string OutboundConnection { get; set; }
			[XmlElement(ElementName = "ScheduleChangeIndicator", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ScheduleChangeIndicator { get; set; }
			[XmlElement(ElementName = "SegmentBookedDate", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string SegmentBookedDate { get; set; }
			[XmlElement(ElementName = "ElapsedTime", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ElapsedTime { get; set; }
			[XmlElement(ElementName = "AirMilesFlown", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string AirMilesFlown { get; set; }
			[XmlElement(ElementName = "FunnelFlight", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string FunnelFlight { get; set; }
			[XmlElement(ElementName = "ChangeOfGauge", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ChangeOfGauge { get; set; }
			[XmlElement(ElementName = "Pos", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Pos Pos { get; set; }
			[XmlElement(ElementName = "Cabin", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Cabin Cabin { get; set; }
			[XmlElement(ElementName = "Banner", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Banner { get; set; }
			[XmlElement(ElementName = "Informational", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Informational { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "sequence")]
			public string Sequence { get; set; }
			[XmlAttribute(AttributeName = "segmentAssociationId")]
			public string SegmentAssociationId { get; set; }
			[XmlAttribute(AttributeName = "isPast")]
			public string IsPast { get; set; }
			[XmlAttribute(AttributeName = "DayOfWeekInd")]
			public string DayOfWeekInd { get; set; }
			[XmlAttribute(AttributeName = "CodeShare")]
			public string CodeShare { get; set; }
			[XmlAttribute(AttributeName = "SpecialMeal")]
			public string SpecialMeal { get; set; }
			[XmlAttribute(AttributeName = "StopQuantity")]
			public string StopQuantity { get; set; }
			[XmlAttribute(AttributeName = "SmokingAllowed")]
			public string SmokingAllowed { get; set; }
			[XmlAttribute(AttributeName = "ResBookDesigCode")]
			public string ResBookDesigCode { get; set; }
			[XmlAttribute(AttributeName = "Code")]
			public string Code { get; set; }
		}

		[XmlRoot(ElementName = "ProductName", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class ProductName
		{
			[XmlAttribute(AttributeName = "type")]
			public string Type { get; set; }
		}

		[XmlRoot(ElementName = "Cabin", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class Cabin2
		{
			[XmlAttribute(AttributeName = "code")]
			public string Code { get; set; }
			[XmlAttribute(AttributeName = "sabreCode")]
			public string SabreCode { get; set; }
			[XmlAttribute(AttributeName = "name")]
			public string Name { get; set; }
			[XmlAttribute(AttributeName = "shortName")]
			public string ShortName { get; set; }
			[XmlAttribute(AttributeName = "lang")]
			public string Lang { get; set; }
		}

		[XmlRoot(ElementName = "DisclosureCarrier", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class DisclosureCarrier2
		{
			[XmlElement(ElementName = "Banner", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string Banner { get; set; }
			[XmlAttribute(AttributeName = "Code")]
			public string Code { get; set; }
			[XmlAttribute(AttributeName = "DOT")]
			public string DOT { get; set; }
		}

		[XmlRoot(ElementName = "Air", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class Air2
		{
			[XmlElement(ElementName = "DepartureAirport", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string DepartureAirport { get; set; }
			[XmlElement(ElementName = "DepartureTerminalName", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string DepartureTerminalName { get; set; }
			[XmlElement(ElementName = "DepartureTerminalCode", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string DepartureTerminalCode { get; set; }
			[XmlElement(ElementName = "ArrivalAirport", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ArrivalAirport { get; set; }
			[XmlElement(ElementName = "ArrivalTerminalName", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ArrivalTerminalName { get; set; }
			[XmlElement(ElementName = "ArrivalTerminalCode", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ArrivalTerminalCode { get; set; }
			[XmlElement(ElementName = "OperatingAirlineCode", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string OperatingAirlineCode { get; set; }
			[XmlElement(ElementName = "EquipmentType", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string EquipmentType { get; set; }
			[XmlElement(ElementName = "MarketingAirlineCode", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string MarketingAirlineCode { get; set; }
			[XmlElement(ElementName = "MarketingFlightNumber", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string MarketingFlightNumber { get; set; }
			[XmlElement(ElementName = "MarketingClassOfService", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string MarketingClassOfService { get; set; }
			[XmlElement(ElementName = "Cabin", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public Cabin2 Cabin2 { get; set; }
			[XmlElement(ElementName = "MealCode", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string MealCode { get; set; }
			[XmlElement(ElementName = "ElapsedTime", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ElapsedTime { get; set; }
			[XmlElement(ElementName = "AirMilesFlown", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string AirMilesFlown { get; set; }
			[XmlElement(ElementName = "FunnelFlight", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string FunnelFlight { get; set; }
			[XmlElement(ElementName = "ChangeOfGauge", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ChangeOfGauge { get; set; }
			[XmlElement(ElementName = "DisclosureCarrier", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public DisclosureCarrier2 DisclosureCarrier2 { get; set; }
			[XmlElement(ElementName = "Eticket", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string Eticket { get; set; }
			[XmlElement(ElementName = "DepartureDateTime", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string DepartureDateTime { get; set; }
			[XmlElement(ElementName = "ArrivalDateTime", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ArrivalDateTime { get; set; }
			[XmlElement(ElementName = "FlightNumber", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string FlightNumber { get; set; }
			[XmlElement(ElementName = "ClassOfService", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ClassOfService { get; set; }
			[XmlElement(ElementName = "ActionCode", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ActionCode { get; set; }
			[XmlElement(ElementName = "NumberInParty", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string NumberInParty { get; set; }
			[XmlElement(ElementName = "inboundConnection", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string InboundConnection { get; set; }
			[XmlElement(ElementName = "outboundConnection", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string OutboundConnection { get; set; }
			[XmlElement(ElementName = "ScheduleChangeIndicator", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ScheduleChangeIndicator { get; set; }
			[XmlElement(ElementName = "SegmentBookedDate", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string SegmentBookedDate { get; set; }
			[XmlAttribute(AttributeName = "sequence")]
			public string Sequence { get; set; }
			[XmlAttribute(AttributeName = "segmentAssociationId")]
			public string SegmentAssociationId { get; set; }
		}

		[XmlRoot(ElementName = "ProductDetails", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class ProductDetails
		{
			[XmlElement(ElementName = "ProductName", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public ProductName ProductName { get; set; }
			[XmlElement(ElementName = "Air", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public Air2 Air2 { get; set; }
			[XmlAttribute(AttributeName = "productCategory")]
			public string ProductCategory { get; set; }
		}

		[XmlRoot(ElementName = "Product", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Product
		{
			[XmlElement(ElementName = "ProductDetails", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public ProductDetails ProductDetails { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
		}

		[XmlRoot(ElementName = "Segment", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Segment
		{
			[XmlElement(ElementName = "Air", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Air Air { get; set; }
			[XmlElement(ElementName = "Product", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Product Product { get; set; }
			[XmlAttribute(AttributeName = "sequence")]
			public string Sequence { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
		}

		[XmlRoot(ElementName = "Segments", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Segments
		{
			[XmlElement(ElementName = "Poc", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Poc Poc { get; set; }
			[XmlElement(ElementName = "Segment", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public List<Segment> Segment { get; set; }
		}

		[XmlRoot(ElementName = "TicketingTimeLimit", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class TicketingTimeLimit
		{
			[XmlElement(ElementName = "Time", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Time { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "index")]
			public string Index { get; set; }
			[XmlAttribute(AttributeName = "elementId")]
			public string ElementId { get; set; }
		}

		[XmlRoot(ElementName = "TicketingInfo", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class TicketingInfo
		{
			[XmlElement(ElementName = "TicketingTimeLimit", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public TicketingTimeLimit TicketingTimeLimit { get; set; }
		}

		[XmlRoot(ElementName = "PassengerReservation", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class PassengerReservation
		{
			[XmlElement(ElementName = "Passengers", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Passengers Passengers { get; set; }
			[XmlElement(ElementName = "Segments", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Segments Segments { get; set; }
			[XmlElement(ElementName = "TicketingInfo", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public TicketingInfo TicketingInfo { get; set; }
			[XmlElement(ElementName = "ItineraryPricing", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string ItineraryPricing { get; set; }
		}

		[XmlRoot(ElementName = "ReceivedFrom", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class ReceivedFrom
		{
			[XmlElement(ElementName = "Name", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Name { get; set; }
		}

		[XmlRoot(ElementName = "PhoneNumber", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class PhoneNumber
		{
			[XmlElement(ElementName = "CityCode", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string CityCode { get; set; }
			[XmlElement(ElementName = "Number", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string Number { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "index")]
			public string Index { get; set; }
			[XmlAttribute(AttributeName = "elementId")]
			public string ElementId { get; set; }
		}

		[XmlRoot(ElementName = "PhoneNumbers", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class PhoneNumbers
		{
			[XmlElement(ElementName = "PhoneNumber", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public PhoneNumber PhoneNumber { get; set; }
		}

		[XmlRoot(ElementName = "ServiceRequest", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class ServiceRequest
		{
			[XmlElement(ElementName = "FreeText", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string FreeText { get; set; }
			[XmlElement(ElementName = "FullText", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string FullText { get; set; }
			[XmlAttribute(AttributeName = "actionCode")]
			public string ActionCode { get; set; }
			[XmlAttribute(AttributeName = "airlineCode")]
			public string AirlineCode { get; set; }
			[XmlAttribute(AttributeName = "code")]
			public string Code { get; set; }
			[XmlAttribute(AttributeName = "serviceCount")]
			public string ServiceCount { get; set; }
			[XmlAttribute(AttributeName = "serviceType")]
			public string ServiceType { get; set; }
			[XmlAttribute(AttributeName = "ssrType")]
			public string SsrType { get; set; }
		}

		[XmlRoot(ElementName = "NameAssociation", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class NameAssociation
		{
			[XmlElement(ElementName = "LastName", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string LastName { get; set; }
			[XmlElement(ElementName = "FirstName", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string FirstName { get; set; }
			[XmlElement(ElementName = "ReferenceId", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string ReferenceId { get; set; }
			[XmlElement(ElementName = "NameRefNumber", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string NameRefNumber { get; set; }
		}

		[XmlRoot(ElementName = "OpenReservationElement", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class OpenReservationElement
		{
			[XmlElement(ElementName = "ServiceRequest", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public ServiceRequest ServiceRequest { get; set; }
			[XmlElement(ElementName = "NameAssociation", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public NameAssociation NameAssociation { get; set; }
			[XmlAttribute(AttributeName = "id")]
			public string Id { get; set; }
			[XmlAttribute(AttributeName = "type")]
			public string Type { get; set; }
			[XmlAttribute(AttributeName = "elementId")]
			public string ElementId { get; set; }
			[XmlElement(ElementName = "Email", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public Email Email { get; set; }
		}

		[XmlRoot(ElementName = "Email", Namespace = "http://services.sabre.com/res/or/v1_14")]
		public class Email
		{
			[XmlElement(ElementName = "Address", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public string Address { get; set; }
			[XmlAttribute(AttributeName = "comment")]
			public string Comment { get; set; }
		}

		[XmlRoot(ElementName = "OpenReservationElements", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class OpenReservationElements
		{
			[XmlElement(ElementName = "OpenReservationElement", Namespace = "http://services.sabre.com/res/or/v1_14")]
			public List<OpenReservationElement> OpenReservationElement { get; set; }
		}

		[XmlRoot(ElementName = "Reservation", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class Reservation
		{
			[XmlElement(ElementName = "BookingDetails", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public BookingDetails BookingDetails { get; set; }
			[XmlElement(ElementName = "POS", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public POS POS { get; set; }
			[XmlElement(ElementName = "PassengerReservation", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public PassengerReservation PassengerReservation { get; set; }
			[XmlElement(ElementName = "ReceivedFrom", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public ReceivedFrom ReceivedFrom { get; set; }
			[XmlElement(ElementName = "PhoneNumbers", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public PhoneNumbers PhoneNumbers { get; set; }
			[XmlElement(ElementName = "EmailAddresses", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public string EmailAddresses { get; set; }
			[XmlElement(ElementName = "OpenReservationElements", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public OpenReservationElements OpenReservationElements { get; set; }
			[XmlAttribute(AttributeName = "numberInParty")]
			public string NumberInParty { get; set; }
			[XmlAttribute(AttributeName = "numberOfInfants")]
			public string NumberOfInfants { get; set; }
			[XmlAttribute(AttributeName = "NumberInSegment")]
			public string NumberInSegment { get; set; }
			[XmlAttribute(AttributeName = "isDirectConnectPlatformBooking")]
			public string IsDirectConnectPlatformBooking { get; set; }
		}

		[XmlRoot(ElementName = "GetReservationRS", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
		public class GetReservationRS
		{
			[XmlElement(ElementName = "Reservation", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public Reservation Reservation { get; set; }
			[XmlAttribute(AttributeName = "stl19", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Stl19 { get; set; }
			[XmlAttribute(AttributeName = "ns6", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Ns6 { get; set; }
			[XmlAttribute(AttributeName = "or114", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Or114 { get; set; }
			[XmlAttribute(AttributeName = "raw", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Raw { get; set; }
			[XmlAttribute(AttributeName = "ns4", Namespace = "http://www.w3.org/2000/xmlns/")]
			public string Ns4 { get; set; }
			[XmlAttribute(AttributeName = "Version")]
			public string Version { get; set; }
		}

		[XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
		public class Body
		{
			[XmlElement(ElementName = "GetReservationRS", Namespace = "http://webservices.sabre.com/pnrbuilder/v1_19")]
			public GetReservationRS GetReservationRS { get; set; }
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

}
