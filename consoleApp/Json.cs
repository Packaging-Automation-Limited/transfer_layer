using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace ConsoleRecipe00
{
    class Json
    {
        CookieContainer cookieContainer = new CookieContainer();
        public bool JsonPost(string Tag, int Value, string IP)
        {
            string result = "OK";
            string Url = @"http://" + IP + ":" + "80" + "/TagBatch";
            string json = "{ \"setTags\":[{\"name\":\"" + Tag + "\",\"value\":" + Value + "}]}";
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(Url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.KeepAlive = false;
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = false;
            request.Timeout = 5000;
            try
            {
                using (var stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(json);
                    stream.Flush();
                    stream.Close();
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Error setting Tag @ {IP}!\n\r{ex.Message}");
                return false;
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    Console.WriteLine(streamReader.ReadToEnd());
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Error setting Tag @ {IP}!\n\r{ex.Message}");
                result = ex.Message;
            }
            request.Abort();
            return true;
        }

        /*
        public string JsonRead(string Tag, Line line)
        {
            string result;
            string dataReceived = "";
            string Url = @"http://" + line.IP + ":" + line.HttpPort + "/TagBatch";
            string json = "{ \"getTags\":[\"" + Tag + "\"]}";
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(Url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.KeepAlive = false;
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = false;
            request.Timeout = 5000;
            try
            {
                using (var stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(json);
                    stream.Flush();
                    stream.Close();
                }
            }
            catch (WebException ex)
            {
                result = ex.Message;
                return result;
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    dataReceived = streamReader.ReadToEnd();
                    response.Close();
                    Console.WriteLine(dataReceived);
                    int indexStart = dataReceived.IndexOf("value") + 7;
                    int indexEnd = dataReceived.LastIndexOf("}]}");
                    string TagValue;
                    TagValue = dataReceived.Substring(indexStart, indexEnd - indexStart);
                    result = TagValue;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show($"Error Reading Tag @ {line.Name}!\n\r{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
                result = ex.Message;
            }
            request.Abort();


            return result;
        }*/
    }

    }
