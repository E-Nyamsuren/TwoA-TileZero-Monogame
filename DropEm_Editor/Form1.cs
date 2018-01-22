using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using AssetPackage;

namespace DropEm_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //ISettings settings;

        private void button1_Click(object sender, EventArgs e)
        {
            //foreach (String key in Environment.GetEnvironmentVariables().Keys)
            //{
            //    Debug.Print("{0}={1}", key, Environment.GetEnvironmentVariable(key));
            //}

            //string solutionDirectory = ((EnvDTE.DTE)System.Runtime
            //                                  .InteropServices5
            //                                  .Marshal
            //                                  .GetActiveObject("VisualStudio.DTE.10.0"))
            //                       .Solution
            //                       .FullName;
            //solutionDirectory = System.IO.Path.GetDirectoryName(solutionDirectory);
            // 
            Directory.SetCurrentDirectory(@"..\..\..\DropEm_MonoGame\bin\WindowsGL\Debug");

            // HATAssetSettings
            string xml = File.ReadAllText(@"DataStorage\HATAssetAppSettings.xml");

            Assembly asm = Assembly.LoadFrom(@"HATAsset.dll");

            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    // See http://stackoverflow.com/questions/4963160/how-to-determine-if-a-type-implements-an-interface-with-c-sharp-reflection
                    if (typeof(ISettings).IsAssignableFrom(type))
                    {
                        Debug.Print("Bingo {0}", type.Name);

                        XmlSerializer ser = new XmlSerializer(type);

                        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                        {
                            var test = ser.Deserialize(ms);
                            propertyGrid1.SelectedObject = test;
                            //    //return (ISettings)ser.Deserialize(ms);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // http://stackoverflow.com/questions/4667078/how-to-retrieve-the-loaderexception-property  
                var typeLoadException = ex as ReflectionTypeLoadException;
                var loaderExceptions = typeLoadException.LoaderExceptions;
            }
            //XmlSerializer ser = new XmlSerializer(typeof(HatAssetSettings));

            //using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            //{
            //    //! Use DataContractSerializer or DataContractJsonSerializer?
            //    //
            //    var test = ser.Deserialize(ms);
            //    propertyGrid1.SelectedObject = test;
            //    //return (ISettings)ser.Deserialize(ms);
            //}
        }
    }
}
