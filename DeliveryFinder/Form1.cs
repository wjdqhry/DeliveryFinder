using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;
using System.Net;
using System.Xml;

namespace DeliveryFinder
{
    public partial class Form1 : Form
    {
        RestClient restClient = new RestClient(Program.baseUrl);
        Dictionary<string, string> DataList;
        List<string> SearchDataList;
        //List<Dictionary<string, string>> SearchDataList;

        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var request = new RestRequest("/api/v1/trackingInfo", Method.GET);

            request.AddQueryParameter("t_key", Program.key);
            request.AddQueryParameter("t_code", DataList[comboBox1.Text].ToString());
            request.AddQueryParameter("t_invoice", textBox1.Text);

            var response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //textBox2.Text = response.Content;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response.Content);

                //XmlNodeList xmlNodeList = doc.GetElementsByTagName("tracking_details");

                //foreach(XmlNode i in xmlNodeList)
                //{

                //}
                XmlElement elements;
                elements = doc["tracking_info"];
                foreach (XmlNode i in elements)
                {
                    textBox2.Text += i.InnerText + "\r\n";
                }
            }
            else
                Console.WriteLine("잘못된 정보");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var request = new RestRequest("/api/v1/companylist", Method.GET);

            request.AddQueryParameter("t_key", Program.key);

            //request.AddHeader("Accept", "application/json;charset=UTF-8");           

            var response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response.Content);

                DataList = doc["Companies"].ChildNodes.Cast<XmlNode>().Select(s => new KeyValuePair<string, string>(s["Name"].InnerText, s["Code"].InnerText)).ToDictionary(s => s.Key, s => s.Value);
                comboBox1.Items.AddRange(DataList.Select(x => x.Key).ToArray());

                comboBox1.SelectedIndex = 0;
            }
        }
    }
}
