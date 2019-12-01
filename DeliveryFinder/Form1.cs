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
                SearchResult searchResult = new SearchResult();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response.Content);
                var subDoc = doc["tracking_info"];
                searchResult.itemName = subDoc["item_name"].InnerText;
                searchResult.reciverAddr = subDoc["reciver_addr"].InnerText;
                searchResult.reciverName = subDoc["reciver_name"].InnerText;
                searchResult.senderName = subDoc["sender_name"].InnerText;

                XmlNodeList trackingDetails = subDoc.GetElementsByTagName("tracking_details");
                foreach(XmlNode i in trackingDetails)
                {
                    searchResult.trackingDetails.Add(new trackingDetail()
                    {
                        transKind = i["trans_kind"].InnerText,
                        transTelno = i["trans_telno"].InnerText,
                        transTime = i["trans_time"].InnerText,
                        transWhere = i["trans_where"].InnerText
                    });
                }
                textBox2.Text += "ItemName: " + searchResult.itemName + "\r\n" + "\r\n";
                textBox2.Text += "ReceiverAddress: " + searchResult.reciverAddr + "\r\n" + "\r\n";
                textBox2.Text += "ReceiverNmae: " + searchResult.reciverName + "\r\n" + "\r\n";
                textBox2.Text += "SenderName: " + searchResult.senderName + "\r\n" + "\r\n";

                foreach(var i in searchResult.trackingDetails)
                {
                    textBox2.Text += "TransKind: " + i.transKind + "\r\n";
                    textBox2.Text += "TransTelno: " + i.transTelno + "\r\n";
                    textBox2.Text += "TransTime: " + i.transTime + "\r\n";
                    textBox2.Text += "TransWhere: " + i.transWhere + "\r\n" + "\r\n";
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
    class SearchResult
    {
        //private string invoiceNum;
        public string itemName;
        public string reciverName;
        public string reciverAddr;
        public string senderName;
        public List<trackingDetail> trackingDetails = new List<trackingDetail>();
    }
    public class trackingDetail
    {
        public string transKind;
        public string transTelno;
        public string transTime;
        public string transWhere;
    }
}
