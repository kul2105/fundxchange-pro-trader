using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using FundXchange.Model.AuthenticationService;

namespace FundXchange.Model.Gateways
{
    public class AuthenticationGateway
    {
        public static UserResult VerifyUser(string username, string password)
        {
            UserResult userResult = new UserResult();

            try
            {
                using (AuthenticationServiceClient service = new AuthenticationServiceClient())
                {
                    userResult = service.GetUser(username, password);
                }
            }
            catch (Exception ex)
            {
                userResult.ErrorMessage = ex.Message;
                userResult.Result = ResultTypes.Failure;
            }

            return userResult;
        }

        //Namo's Authentication Service

        public static bool AuthenticateUser(string username, string password)
        {
            try
            {
                Credential data = new Credential()
                {
                    UserId = username,
                    Password = password
                };
                var url = @"http://197.242.148.230:99/HistoricalDataService.svc/AuthenticateUser";


                MemoryStream ms = new MemoryStream();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Credential));
                serializer.WriteObject(ms, data);

                var syncClient = new WebClient();
                syncClient.Headers["Content-type"] = "application/json";
                var content = syncClient.UploadData(url, "POST", ms.ToArray());

                Stream stream = new MemoryStream(content);
                serializer = new DataContractJsonSerializer(typeof(AuthenticationResponse));
                var resultResponse = serializer.ReadObject(stream) as AuthenticationResponse;
                return resultResponse.IsAuthenticated;
                //if (resultResponse.IsAuthenticated)
                //{
                //    //Console.WriteLine("Authentication Successful");
                //}
                //else
                //{
                //    //Console.WriteLine("Authentication Failed, " + resultResponse.message);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Authentication : Exception : " + ex.Message);
                return false;
            }
        }
    }

   public class Credential
    {
        public string UserId;
        public string Password;
    }
   
    public class AuthenticationResponse
    {
        //public Result result;                                // Response Result
        public string message;                               // Error message
        public bool IsAuthenticated;                         // Authentication Result
        public string UserId;
        public string Password;
    }
}
