using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"\\STORAGE3\projects2\relight_my_fire_beats_unmod_original.txt");
            Console.ReadLine();
            ConvertAuToXML(@"\\STORAGE3\projects2\relight_my_fire_beats_unmod_original.txt");
            Console.ReadLine();
        }

        static void ConvertAuToXML(string path)
        {
            StreamReader sr;
            string s;
            List<float> floatList = new List<float>();


            sr = File.OpenText(path);
            s = sr.ReadLine();
            while (s != null)
            {
                s = s.Replace('.', ',');
                string[] temp = s.Split("\t"[0]);
                float value = (float)System.Convert.ToDouble(temp[0]);
                string name = temp[temp.Length - 1];
                floatList.Add(value);
                s = sr.ReadLine();
            }
            sr.Close();

            readXMLfromFloatlist(makeXML(floatList));
        }

        static void readXMLfromFloatlist(XmlDocument myDoc)
        {
            XmlNodeList nodelist = myDoc.GetElementsByTagName("beat");

            List<float> floatlist = new List<float>();

            foreach (XmlNode item in nodelist)
            {
                floatlist.Add(float.Parse(item.InnerText));
            }



            float temp = 0.0f;
            float averageTime = 0.0f;
            List<float> newFloatList = new List<float>();

            foreach (float item in floatlist)
            {
                averageTime += item - temp;
                temp = item;
            }
            averageTime /= floatlist.Count;
            Console.WriteLine(averageTime);

            temp = 0.0f;
            foreach (float item in floatlist)
            {
                if (item - temp > averageTime)
                {
                    newFloatList.Add(item);
                    temp = item;
                }
            }

            StreamWriter SW;
            SW = File.CreateText("c:\\XMLfile.xml");
            SW.WriteLine(makeXML(newFloatList).OuterXml.Replace("</beat>", "</beat>\n"));
            SW.Close();
            SW = File.CreateText("c:\\AudacityFile.txt");
            foreach (float item in newFloatList)
            {
                SW.WriteLine(item.ToString() + "\t" + item.ToString() + "\tB");
            }
            SW.Close();
        }

        static XmlDocument makeXML(List<float> floatlist)
        {
            XmlDocument myDoc = new XmlDocument();
            myDoc.LoadXml("<music>\n</music>");

            XmlElement root = myDoc.DocumentElement;
            XmlNode myBaseNode;
            myBaseNode = myDoc.CreateNode(XmlNodeType.Element, "beats", "");
            root.AppendChild(myBaseNode);
            foreach (float item in floatlist)
            {
                XmlNode myNode;
                myNode = myDoc.CreateNode(XmlNodeType.Element, "beat", "");
                myNode.InnerText = item.ToString();
                myBaseNode.AppendChild(myNode);
            }

            Console.WriteLine(myDoc.OuterXml);

            return myDoc;
        }
    }
}
