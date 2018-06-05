using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml.XPath;

namespace MS.IoT.Simulator.Installer.SetConfigXML
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult ReadSetConfigXML(Session session)
        {
            session.Log("Begin ReadSetConfigXML");

            try
            {
                string directory = session["CURRENTDIRECTORY"];
                session.Log("Current MSI Local directory: " + directory);


                if (File.Exists(@directory + @"\Config.xml"))
                {
                    var doc = new XPathDocument(@directory + @"\Config.xml");
                    var navigator = doc.CreateNavigator();
                    var node = navigator.SelectSingleNode("appsettings");
                    node.MoveToFirstChild();
                    do
                    {
                        Console.WriteLine(node.Name + "  " + node.Value);
                        session.Log("AppSettings actual {0}: Value {1} ", node.Name, node.Value);
                        session[node.Name] = node.Value;

                    } while (node.MoveToNext());                   
                }               
            }
            catch (Exception ex)
            {
                session.Log("ERROR in custom action ReadSetConfigXML {0}", ex.ToString());
                return ActionResult.Failure;
            }
            session.Log("End ReadSetConfigXML");
            return ActionResult.Success;
        }
    }
}
