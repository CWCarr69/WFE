using System.Xml.Linq;
using System.Xml.Serialization;

namespace Timesheet.FDPDataIntegrator.Services
{
    public class NodeReader : INodeReader
    {
        public T Read<T>(string response)
        {
			var doc = XDocument.Load(new StringReader(response));
			var xRecords = doc.Descendants().FirstOrDefault(d => d.Name.LocalName.Equals("OutboundData"));

			var serializer = new XmlSerializer(typeof(T));
			using var reader = xRecords.CreateReader();
			var result = (T)serializer.Deserialize(reader);

			return result;
		}
    }
}