using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace XMLObject.AirARUNKLLSRQ
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

	[XmlRoot(ElementName = "ARUNK_RS", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
	public class ARUNK_RS
	{
		[XmlElement(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL/v01")]
		public ApplicationResults ApplicationResults { get; set; }
		[XmlElement(ElementName = "Text", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
		public string Text { get; set; }
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
