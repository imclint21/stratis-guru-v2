using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.Guru.Services
{
    public class ServiceBase
    {
        private readonly RestClient _client;

        public ServiceBase(string baseUrl)
        {
            // RestClient is suppose to be able to be re-used. If not, we should move this into the Execute method.
            _client = new RestClient();
           _client.BaseUrl = new System.Uri(baseUrl);
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var response = _client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var blockIndexServiceException = new ApplicationException(message, response.ErrorException);
                throw blockIndexServiceException;
            }

            return response.Data;
        }

        public string Execute(RestRequest request)
        {
            var response = _client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                var blockIndexServiceException = new ApplicationException(message, response.ErrorException);
                throw blockIndexServiceException;
            }

            return response.Content;
        }

        protected RestRequest GetRequest(string resource)
        {
            var request = new RestRequest();
            request.Method = Method.GET;
            request.Resource = resource;
            return request;
        }

    }
}
