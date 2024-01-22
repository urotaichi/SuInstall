using System;
using System.IO;
using System.Xml.Serialization;

namespace SuInstall
{
	[XmlType("SideUpdate")]
	[Serializable]
	public class UpdateData
	{
		public static UpdateData ParseXML(string file)
		{
			UpdateData result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateData));
				using (FileStream fileStream = new FileStream(file, FileMode.Open))
				{
					UpdateData updateData = (UpdateData)xmlSerializer.Deserialize(fileStream);
					result = updateData;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		public string Name = "";

		public string Author = "";

		public string Update = "";

		public double DefVersion;

		public double RequireLower;

		public string Installer = "";
	}
}
