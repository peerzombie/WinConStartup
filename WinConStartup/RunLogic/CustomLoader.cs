using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace WinConStartup.RunLogic
{
	public class CustomLoader
	{
		private readonly XmlDocument _xdoc = new XmlDocument();
		private XmlNode _programs;
		private readonly string _docFile;
		public CustomLoader(string filename)
		{
			_docFile = filename;
			CheckProgramFileEntry();
			_xdoc.Load(filename);
			_programs = _xdoc.SelectSingleNode("root/programs");
		}

		private void GenerateProgramFileEntry()
		{
			XmlElement xel_1 = _xdoc.CreateElement("root");
			XmlElement xel_2 = _xdoc.CreateElement("pass");
			XmlComment xel_3 =
				_xdoc.CreateComment(
					@"<program name='exampleEntry' exec='examplepath\exampleprogram.exe' args='' runInBackground='false' runAsAdmin='true' />");
			xel_1.AppendChild(xel_2);
			xel_2 = _xdoc.CreateElement("programs");
			xel_2.AppendChild(xel_3);
			xel_1.AppendChild(xel_2);
			_xdoc.AppendChild(xel_1);
			_xdoc.Save(_docFile);
		}

		public void CheckProgramFileEntry()
		{
			if (!File.Exists(_docFile))
			{
				GenerateProgramFileEntry();
			}
			else
			{
				// <FIXME> AddCheck
			}
		}
		public RunItem LoadRunItem(string qualifier)
		{
			return new RunItem("test1", "C:\\Windows\\explorer.exe", ".");
		}
	}
}