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
        Dictionary<string, string> dataList;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var request = new RestRequest("/api/v1/companylist", Method.GET);

            request.AddQueryParameter("t_key", Program.key);

            //request.AddHeader("Accept", "application/json;charset=UTF-8");

            var response = restClient.Execute(request);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                label1.Text = response.Content;
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content);

            dataList = doc["Companies"].ChildNodes.Cast<XmlNode>().Select(s => new KeyValuePair<string, string>(s["Name"].InnerText, s["Code"].InnerText)).ToDictionary(s=>s.Key, s=>s.Value);
            comboBox1.Items.AddRange(dataList.Select(x => x.Key).ToArray());

            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var request = new RestRequest("/api/v1/trackingInfo", Method.GET);

            request.AddQueryParameter("t_key", Program.key);
            request.AddQueryParameter("t_code", dataList[comboBox1.Text].ToString());
            request.AddQueryParameter("t_invoice", textBox1.Text);

            var response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                label2.Text = response.Content;
            }
        }
    }
}
