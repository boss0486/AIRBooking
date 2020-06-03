//using System;
//using System.Collections.Generic;

//using System.Globalization;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

//namespace ApiPortalBooking.Models.VNA_WS_Model
//{
//    public partial class Welcome
//    {
//        [JsonProperty("PriceQuoteInfo")]
//        public PriceQuoteInfo PriceQuoteInfo { get; set; }
//    }

//    public partial class PriceQuoteInfo
//    {
//        [JsonProperty("Reservation")]
//        public Reservation Reservation { get; set; }

//        [JsonProperty("Details")]
//        public List<Detail> Details { get; set; }
//    }

//    public partial class Detail
//    {
//        [JsonProperty("number")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long Number { get; set; }

//        [JsonProperty("passengerType")]
//        public string PassengerType { get; set; }

//        [JsonProperty("pricingType")]
//        public string PricingType { get; set; }

//        [JsonProperty("status")]
//        public string Status { get; set; }

//        [JsonProperty("type")]
//        public string Type { get; set; }

//        [JsonProperty("AgentInfo")]
//        public AgentInfo AgentInfo { get; set; }

//        [JsonProperty("TransactionInfo")]
//        public TransactionInfo TransactionInfo { get; set; }

//        [JsonProperty("NameAssociationInfo")]
//        public NameAssociationInfo NameAssociationInfo { get; set; }

//        [JsonProperty("SegmentInfo")]
//        public List<SegmentInfo> SegmentInfo { get; set; }

//        [JsonProperty("FareInfo")]
//        public FareInfo FareInfo { get; set; }

//        [JsonProperty("MiscellaneousInfo")]
//        public MiscellaneousInfo MiscellaneousInfo { get; set; }

//        [JsonProperty("MessageInfo")]
//        public MessageInfo MessageInfo { get; set; }
//    }

//    public partial class AgentInfo
//    {
//        [JsonProperty("duty")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long Duty { get; set; }

//        [JsonProperty("sine")]
//        public string Sine { get; set; }

//        [JsonProperty("HomeLocation")]
//        public string HomeLocation { get; set; }

//        [JsonProperty("WorkLocation")]
//        public string WorkLocation { get; set; }
//    }

//    public partial class FareInfo
//    {
//        [JsonProperty("FareIndicators")]
//        public object FareIndicators { get; set; }

//        [JsonProperty("BaseFare")]
//        public BaseFare BaseFare { get; set; }

//        [JsonProperty("TotalTax")]
//        public BaseFare TotalTax { get; set; }

//        [JsonProperty("TotalFare")]
//        public BaseFare TotalFare { get; set; }

//        [JsonProperty("TaxInfo")]
//        public TaxInfo TaxInfo { get; set; }

//        [JsonProperty("FareCalculation")]
//        public string FareCalculation { get; set; }

//        [JsonProperty("FareComponent")]
//        public List<FareComponent> FareComponent { get; set; }
//    }

//    public partial class BaseFare
//    {
//        [JsonProperty("currencyCode")]
//        public CurrencyCode CurrencyCode { get; set; }

//        [JsonProperty("text")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long Text { get; set; }
//    }

//    public partial class FareComponent
//    {
//        [JsonProperty("fareBasisCode")]
//        public string FareBasisCode { get; set; }

//        [JsonProperty("number")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long Number { get; set; }

//        [JsonProperty("FlightSegmentNumbers")]
//        public FlightSegmentNumbers FlightSegmentNumbers { get; set; }

//        [JsonProperty("FareDirectionality")]
//        public FareDirectionality FareDirectionality { get; set; }

//        [JsonProperty("Departure")]
//        public Arrival Departure { get; set; }

//        [JsonProperty("Arrival")]
//        public Arrival Arrival { get; set; }

//        [JsonProperty("Amount")]
//        public Amount Amount { get; set; }

//        [JsonProperty("GoverningCarrier")]
//        public string GoverningCarrier { get; set; }

//        [JsonProperty("TicketDesignator", NullValueHandling = NullValueHandling.Ignore)]
//        public string TicketDesignator { get; set; }
//    }

//    public partial class Amount
//    {
//        [JsonProperty("currencyCode")]
//        public CurrencyCode CurrencyCode { get; set; }

//        [JsonProperty("decimalPlace")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long DecimalPlace { get; set; }

//        [JsonProperty("text")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long Text { get; set; }
//    }

//    public partial class Arrival
//    {
//        [JsonProperty("DateTime")]
//        public DateTimeOffset DateTime { get; set; }

//        [JsonProperty("CityCode")]
//        public CityCode CityCode { get; set; }
//    }

//    public partial class CityCode
//    {
//        [JsonProperty("name")]
//        public Name Name { get; set; }

//        [JsonProperty("text")]
//        public Text Text { get; set; }
//    }

//    public partial class FareDirectionality
//    {
//        [JsonProperty("roundTrip")]
//        [JsonConverter(typeof(FluffyParseStringConverter))]
//        public bool RoundTrip { get; set; }

//        [JsonProperty("inbound", NullValueHandling = NullValueHandling.Ignore)]
//        [JsonConverter(typeof(FluffyParseStringConverter))]
//        public bool? Inbound { get; set; }
//    }

//    public partial class FlightSegmentNumbers
//    {
//        [JsonProperty("SegmentNumber")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long SegmentNumber { get; set; }
//    }

//    public partial class TaxInfo
//    {
//        [JsonProperty("CombinedTax")]
//        public CombinedTaxUnion CombinedTax { get; set; }

//        [JsonProperty("Tax")]
//        public CombinedTaxUnion Tax { get; set; }
//    }

//    public partial class CombinedTaxElement
//    {
//        [JsonProperty("code")]
//        public string Code { get; set; }

//        [JsonProperty("Amount")]
//        public BaseFare Amount { get; set; }
//    }

//    public partial class MessageInfo
//    {
//        [JsonProperty("Message")]
//        public List<Message> Message { get; set; }

//        [JsonProperty("Remarks")]
//        public Remarks Remarks { get; set; }

//        [JsonProperty("PricingParameters")]
//        public string PricingParameters { get; set; }
//    }

//    public partial class Message
//    {
//        [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long? Number { get; set; }

//        [JsonProperty("type")]
//        public TypeEnum Type { get; set; }

//        [JsonProperty("text")]
//        public string Text { get; set; }
//    }

//    public partial class Remarks
//    {
//        [JsonProperty("type")]
//        public string Type { get; set; }

//        [JsonProperty("text")]
//        public string Text { get; set; }
//    }

//    public partial class MiscellaneousInfo
//    {
//        [JsonProperty("ValidatingCarrier")]
//        public string ValidatingCarrier { get; set; }

//        [JsonProperty("ItineraryType")]
//        public string ItineraryType { get; set; }
//    }

//    public partial class NameAssociationInfo
//    {
//        [JsonProperty("firstName")]
//        public string FirstName { get; set; }

//        [JsonProperty("lastName")]
//        public string LastName { get; set; }

//        [JsonProperty("nameId")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long NameId { get; set; }

//        [JsonProperty("nameNumber")]
//        public string NameNumber { get; set; }
//    }

//    public partial class SegmentInfo
//    {
//        [JsonProperty("number")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long Number { get; set; }

//        [JsonProperty("segmentStatus")]
//        public string SegmentStatus { get; set; }

//        [JsonProperty("Flight")]
//        public Flight Flight { get; set; }

//        [JsonProperty("FareBasis")]
//        public string FareBasis { get; set; }

//        [JsonProperty("NotValidBefore")]
//        public DateTimeOffset NotValidBefore { get; set; }

//        [JsonProperty("NotValidAfter")]
//        public DateTimeOffset NotValidAfter { get; set; }

//        [JsonProperty("Baggage")]
//        public Baggage Baggage { get; set; }
//    }

//    public partial class Baggage
//    {
//        [JsonProperty("allowance")]
//        public string Allowance { get; set; }

//        [JsonProperty("type")]
//        public string Type { get; set; }
//    }

//    public partial class Flight
//    {
//        [JsonProperty("connectionIndicator")]
//        public string ConnectionIndicator { get; set; }

//        [JsonProperty("MarketingFlight")]
//        public MarketingFlight MarketingFlight { get; set; }

//        [JsonProperty("ClassOfService")]
//        public string ClassOfService { get; set; }

//        [JsonProperty("Departure")]
//        public Arrival Departure { get; set; }

//        [JsonProperty("Arrival")]
//        public Arrival Arrival { get; set; }
//    }

//    public partial class MarketingFlight
//    {
//        [JsonProperty("number")]
//        [JsonConverter(typeof(PurpleParseStringConverter))]
//        public long Number { get; set; }

//        [JsonProperty("text")]
//        public string Text { get; set; }
//    }

//    public partial class TransactionInfo
//    {
//        [JsonProperty("CreateDateTime")]
//        public DateTimeOffset CreateDateTime { get; set; }

//        [JsonProperty("LastDateToPurchase")]
//        public DateTimeOffset LastDateToPurchase { get; set; }

//        [JsonProperty("LocalCreateDateTime")]
//        public DateTimeOffset LocalCreateDateTime { get; set; }

//        [JsonProperty("InputEntry")]
//        public string InputEntry { get; set; }
//    }

//    public partial class Reservation
//    {
//        [JsonProperty("updateToken")]
//        public string UpdateToken { get; set; }

//        [JsonProperty("text")]
//        public string Text { get; set; }
//    }

//    public enum CurrencyCode { Vnd };

//    public enum Name { Hanoi, HoChiMinhCity };

//    public enum Text { Han, Sgn };

//    public enum TypeEnum { Warning };

//    public partial struct CombinedTaxUnion
//    {
//        public CombinedTaxElement CombinedTaxElement;
//        public List<CombinedTaxElement> CombinedTaxElementArray;

//        public static implicit operator CombinedTaxUnion(CombinedTaxElement CombinedTaxElement) => new CombinedTaxUnion { CombinedTaxElement = CombinedTaxElement };
//        public static implicit operator CombinedTaxUnion(List<CombinedTaxElement> CombinedTaxElementArray) => new CombinedTaxUnion { CombinedTaxElementArray = CombinedTaxElementArray };
//    }

//    public partial class Welcome
//    {
//        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, ApiPortalBooking.Models.VNA_WS_Model.Converter.Settings);
//    }

//    public static class Serialize
//    {
//        public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, ApiPortalBooking.Models.VNA_WS_Model.Converter.Settings);
//    }

//    internal static class Converter
//    {
//        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
//        {
//            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
//            DateParseHandling = DateParseHandling.None,
//            Converters =
//            {
//                CurrencyCodeConverter.Singleton,
//                NameConverter.Singleton,
//                TextConverter.Singleton,
//                CombinedTaxUnionConverter.Singleton,
//                TypeEnumConverter.Singleton,
//                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
//            },
//        };
//    }

//    internal class PurpleParseStringConverter : JsonConverter
//    {
//        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

//        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType == JsonToken.Null) return null;
//            var value = serializer.Deserialize<string>(reader);
//            long l;
//            if (Int64.TryParse(value, out l))
//            {
//                return l;
//            }
//            throw new Exception("Cannot unmarshal type long");
//        }

//        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//        {
//            if (untypedValue == null)
//            {
//                serializer.Serialize(writer, null);
//                return;
//            }
//            var value = (long)untypedValue;
//            serializer.Serialize(writer, value.ToString());
//            return;
//        }

//        public static readonly PurpleParseStringConverter Singleton = new PurpleParseStringConverter();
//    }

//    internal class CurrencyCodeConverter : JsonConverter
//    {
//        public override bool CanConvert(Type t) => t == typeof(CurrencyCode) || t == typeof(CurrencyCode?);

//        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType == JsonToken.Null) return null;
//            var value = serializer.Deserialize<string>(reader);
//            if (value == "VND")
//            {
//                return CurrencyCode.Vnd;
//            }
//            throw new Exception("Cannot unmarshal type CurrencyCode");
//        }

//        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//        {
//            if (untypedValue == null)
//            {
//                serializer.Serialize(writer, null);
//                return;
//            }
//            var value = (CurrencyCode)untypedValue;
//            if (value == CurrencyCode.Vnd)
//            {
//                serializer.Serialize(writer, "VND");
//                return;
//            }
//            throw new Exception("Cannot marshal type CurrencyCode");
//        }

//        public static readonly CurrencyCodeConverter Singleton = new CurrencyCodeConverter();
//    }

//    internal class NameConverter : JsonConverter
//    {
//        public override bool CanConvert(Type t) => t == typeof(Name) || t == typeof(Name?);

//        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType == JsonToken.Null) return null;
//            var value = serializer.Deserialize<string>(reader);
//            switch (value)
//            {
//                case "HANOI":
//                    return Name.Hanoi;
//                case "HO CHI MINH CITY":
//                    return Name.HoChiMinhCity;
//            }
//            throw new Exception("Cannot unmarshal type Name");
//        }

//        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//        {
//            if (untypedValue == null)
//            {
//                serializer.Serialize(writer, null);
//                return;
//            }
//            var value = (Name)untypedValue;
//            switch (value)
//            {
//                case Name.Hanoi:
//                    serializer.Serialize(writer, "HANOI");
//                    return;
//                case Name.HoChiMinhCity:
//                    serializer.Serialize(writer, "HO CHI MINH CITY");
//                    return;
//            }
//            throw new Exception("Cannot marshal type Name");
//        }

//        public static readonly NameConverter Singleton = new NameConverter();
//    }

//    internal class TextConverter : JsonConverter
//    {
//        public override bool CanConvert(Type t) => t == typeof(Text) || t == typeof(Text?);

//        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType == JsonToken.Null) return null;
//            var value = serializer.Deserialize<string>(reader);
//            switch (value)
//            {
//                case "HAN":
//                    return Text.Han;
//                case "SGN":
//                    return Text.Sgn;
//            }
//            throw new Exception("Cannot unmarshal type Text");
//        }

//        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//        {
//            if (untypedValue == null)
//            {
//                serializer.Serialize(writer, null);
//                return;
//            }
//            var value = (Text)untypedValue;
//            switch (value)
//            {
//                case Text.Han:
//                    serializer.Serialize(writer, "HAN");
//                    return;
//                case Text.Sgn:
//                    serializer.Serialize(writer, "SGN");
//                    return;
//            }
//            throw new Exception("Cannot marshal type Text");
//        }

//        public static readonly TextConverter Singleton = new TextConverter();
//    }

//    internal class FluffyParseStringConverter : JsonConverter
//    {
//        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

//        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType == JsonToken.Null) return null;
//            var value = serializer.Deserialize<string>(reader);
//            bool b;
//            if (Boolean.TryParse(value, out b))
//            {
//                return b;
//            }
//            throw new Exception("Cannot unmarshal type bool");
//        }

//        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//        {
//            if (untypedValue == null)
//            {
//                serializer.Serialize(writer, null);
//                return;
//            }
//            var value = (bool)untypedValue;
//            var boolString = value ? "true" : "false";
//            serializer.Serialize(writer, boolString);
//            return;
//        }

//        public static readonly FluffyParseStringConverter Singleton = new FluffyParseStringConverter();
//    }

//    internal class CombinedTaxUnionConverter : JsonConverter
//    {
//        public override bool CanConvert(Type t) => t == typeof(CombinedTaxUnion) || t == typeof(CombinedTaxUnion?);

//        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//        {
//            switch (reader.TokenType)
//            {
//                case JsonToken.StartObject:
//                    var objectValue = serializer.Deserialize<CombinedTaxElement>(reader);
//                    return new CombinedTaxUnion { CombinedTaxElement = objectValue };
//                case JsonToken.StartArray:
//                    var arrayValue = serializer.Deserialize<List<CombinedTaxElement>>(reader);
//                    return new CombinedTaxUnion { CombinedTaxElementArray = arrayValue };
//            }
//            throw new Exception("Cannot unmarshal type CombinedTaxUnion");
//        }

//        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//        {
//            var value = (CombinedTaxUnion)untypedValue;
//            if (value.CombinedTaxElementArray != null)
//            {
//                serializer.Serialize(writer, value.CombinedTaxElementArray);
//                return;
//            }
//            if (value.CombinedTaxElement != null)
//            {
//                serializer.Serialize(writer, value.CombinedTaxElement);
//                return;
//            }
//            throw new Exception("Cannot marshal type CombinedTaxUnion");
//        }

//        public static readonly CombinedTaxUnionConverter Singleton = new CombinedTaxUnionConverter();
//    }

//    internal class TypeEnumConverter : JsonConverter
//    {
//        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

//        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType == JsonToken.Null) return null;
//            var value = serializer.Deserialize<string>(reader);
//            if (value == "WARNING")
//            {
//                return TypeEnum.Warning;
//            }
//            throw new Exception("Cannot unmarshal type TypeEnum");
//        }

//        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//        {
//            if (untypedValue == null)
//            {
//                serializer.Serialize(writer, null);
//                return;
//            }
//            var value = (TypeEnum)untypedValue;
//            if (value == TypeEnum.Warning)
//            {
//                serializer.Serialize(writer, "WARNING");
//                return;
//            }
//            throw new Exception("Cannot marshal type TypeEnum");
//        }

//        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
//    }
//}
