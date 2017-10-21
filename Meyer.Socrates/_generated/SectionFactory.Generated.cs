/*
	This code was generated 10/07/2017 02:31:34
*/
using Meyer.Socrates.Data;
using Meyer.Socrates.Data.Sections;

namespace Meyer.Socrates.Services
{
	internal static partial class SectionFactory
	{
		internal static Section CreateSection(string key)
        {
			switch(key)
			{
				case "ACTN": return new ACTN();
				case "BMDL": return new BMDL();
				case "CMTL": return new CMTL();
				case "FILL": return new FILL();
				case "GGAE": return new GGAE();
				case "GGCL": return new GGCL();
				case "GGCM": return new GGCM();
				case "GLBS": return new GLBS();
				case "GLCR": return new GLCR();
				case "GLDC": return new GLDC();
				case "GLLT": return new GLLT();
				case "GLPI": return new GLPI();
				case "GLXF": return new GLXF();
				case "GOKD": return new GOKD();
				case "HTOP": return new HTOP();
				case "MASK": return new MASK();
				case "MBMP": return new MBMP();
				case "MTRL": return new MTRL();
				case "PATH": return new PATH();
				case "SMTH": return new SMTH();
				case "SVTH": return new SVTH();
				case "SFTH": return new SFTH();
				case "BKTH": return new BKTH();
				case "CATH": return new CATH();
				case "MTTH": return new MTTH();
				case "PRTH": return new PRTH();
				case "TMTH": return new TMTH();
				case "TCTH": return new TCTH();
				case "TBTH": return new TBTH();
				case "TZTH": return new TZTH();
				case "TSTH": return new TSTH();
				case "TFTH": return new TFTH();
				case "THUM": return new THUM();
				case "TMAP": return new TMAP();
				case "TMPL": return new TMPL();
				case "TXXF": return new TXXF();
				case "ZBMP": return new ZBMP();
				default: return new Section();
			}
        }
	}
}
