using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;

namespace M4.ClientData
{
    public class JSONHelper
    {
        public static string Serialize<T>(T obj)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            ms.Dispose();
            return obj;
        }

        public static bool SendAnSMSMessage(string message)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://api.pennysms.com/jsonrpc");
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{ \"method\": \"send\", " +
                                  "  \"params\": [ " +
                                  "             \"IPutAGuidHere\", " +
                                  "             \"msg@MyCompany.com\", " +
                                  "             \"MyTenDigitNumberWasHere\", " +
                                  "             \"" + message + "\" " +
                                  "             ] " +
                                  "}";

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                //Now you have your response.
                //or false depending on information in the response
                return true;
            }
        }
    }

}
