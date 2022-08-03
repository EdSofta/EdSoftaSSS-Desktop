using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;
using Newtonsoft.Json;
using System.Net;
using EdSofta.Constants;

namespace EdSofta.Services
{
    class WebClientService<T, TResponse> : IWebClientService<T, TResponse> where TResponse : new()
    {
        public async Task<TResponse> getDataAsync(string endPoint)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.BaseAddress = Urls.EndPoint;
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    var json = await webClient.DownloadStringTaskAsync(endPoint);
                    var result = JsonConvert.DeserializeObject<TResponse>(json);
                    return result;
                }
            }
            catch (WebException ex)
            {
               return new TResponse();
            }
        }

        public async Task<TResponse> postDataAsync(string endPoint, T data)
        {
            var result = new TResponse();
            
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.BaseAddress = Urls.EndPoint;
                    var url = endPoint;
                    webClient.Headers.Add("user-agent",
                        "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    webClient.Encoding = Encoding.UTF8;
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    var body = JsonConvert.SerializeObject(data);
                    var response = await webClient.UploadStringTaskAsync(url, body);
                    result = JsonConvert.DeserializeObject<TResponse>(response);
                    return result;
                }
            }
            catch
            {
                return result;
            }
        }

        public async Task postDataSingleAsync(string endPoint, T data)
        {
            //var result = new TResponse();

            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.BaseAddress = Urls.EndPoint;
                    var url = endPoint;
                    webClient.Headers.Add("user-agent",
                        "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    webClient.Encoding = Encoding.UTF8;
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    var body = JsonConvert.SerializeObject(data);
                    var response = await webClient.UploadStringTaskAsync(url, body);
                    //result = JsonConvert.DeserializeObject<TResponse>(response);
                    //return result;
                }
            }
            catch (Exception ex)
            {
                //return result;
            }
        }

    }
}
