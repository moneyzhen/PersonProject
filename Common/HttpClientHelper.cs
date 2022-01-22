using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZHONGJIAN_API.Common
{
    public class HttpClientHelper
    {
        private static readonly JsonSerializerSettings serializerSettings;
        static HttpClientHelper()
        {
            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            serializerSettings.Converters.Add(new StringEnumConverter());
        }

        /// <summary>
        ///     GET请求
        /// </summary>
        public static string Get(string url)
        {
            try
            {
                var httpclientHandler = new HttpClientHandler();
                httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                using (var httpClient = new HttpClient(httpclientHandler))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = httpClient.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResult = response.Content.ReadAsStringAsync().Result;
                        return apiResult;
                    }
                    string exMsg = $"远程接口调用错误,返回了失败的状态值。response={response}";
                    throw new Exception(exMsg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                throw ex;
            }
        }

        /// <summary>
        ///     GET请求(返回Stream)
        /// </summary>
        public static Stream GetStream(string url)
        {
            try
            {
                var httpclientHandler = new HttpClientHandler();
                httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                using (var httpClient = new HttpClient(httpclientHandler))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = httpClient.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResult = response.Content.ReadAsStreamAsync().Result;
                        return apiResult;
                    }
                    string exMsg = $"远程接口调用错误,返回了失败的状态值。response={response}";
                    throw new Exception(exMsg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="requestParameters">请求参数</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>
        /// <param name="responseInfo">返回请求结果</param>
        /// <returns></returns>
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> requestParameters, int timeout, string userAgent, Encoding requestEncoding, ref string responseInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                if (requestEncoding == null)
                {
                    throw new ArgumentNullException("requestEncoding");
                }
                HttpWebRequest request = null;
                //如果是发送HTTPS请求
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = timeout;
                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }
                //如果需要POST数据
                List<string> list = new List<string>();
                list.AddRange(requestParameters.Keys);
                foreach (string t in list)
                {
                    if (t == "fileName")
                    {
                        requestParameters[t] = HttpUtility.UrlEncode(requestParameters[t]);
                    }
                }
                if (!(requestParameters == null || requestParameters.Count == 0))
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in requestParameters.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, requestParameters[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, requestParameters[key]);
                        }
                        i++;
                    }
                    byte[] data = requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                HttpWebResponse webresponse = request.GetResponse() as HttpWebResponse;
                using (StreamReader reader = new StreamReader(webresponse.GetResponseStream(), requestEncoding))
                {
                    responseInfo = reader.ReadToEnd();
                }
                return webresponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public static string Post(string url, string requestJson)
        {
            try
            {
                string result = string.Empty;
                Uri postUrl = new Uri(url);

                using (HttpContent httpContent = new StringContent(requestJson))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var httpclientHandler = new HttpClientHandler();
                    httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                    using (var httpClient = new HttpClient(httpclientHandler))
                    {
                        //Http2ErrorCode
                        httpClient.Timeout = new TimeSpan(0, 0, 60);
                        result = httpClient.PostAsync(postUrl, httpContent).Result.Content.ReadAsStringAsync().Result;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                throw ex;
            }
        }

        /// <summary>
        /// POST请求（RequestHeaders添加Authorization）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestJson"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static string PostToken(string url, string requestJson,string Token)
        {
            try
            {
                string result = string.Empty;
                Uri postUrl = new Uri(url);

                using (HttpContent httpContent = new StringContent(requestJson))
                {

                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var httpclientHandler = new HttpClientHandler();
                    httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                    using (var httpClient = new HttpClient(httpclientHandler))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", Token);
                        //Http2ErrorCode
                        httpClient.Timeout = new TimeSpan(0, 0, 60);
                        result = httpClient.PostAsync(postUrl, httpContent).Result.Content.ReadAsStringAsync().Result;

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                throw ex;
            }
        }

        /// <summary>
        /// PUT请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public static string Put(string url, string requestJson)
        {
            try
            {
                string result = string.Empty;
                Uri postUrl = new Uri(url);

                using (HttpContent httpContent = new StringContent(requestJson))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var httpclientHandler = new HttpClientHandler();
                    httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                    using (var httpClient = new HttpClient(httpclientHandler))
                    {
                        //Http2ErrorCode
                        httpClient.Timeout = new TimeSpan(0, 0, 60);
                        result = httpClient.PutAsync(postUrl, httpContent).Result.Content.ReadAsStringAsync().Result;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                throw ex;
            }
        }

        public static TResult Post<TRequest, TResult>(string url, TRequest data, string token = "")
        {
            try
            {
                Uri postUrl = new Uri(url);
                string requestJson = JsonConvert.SerializeObject(data, serializerSettings);
                using (HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    //忽略[System.Security.Authentication.AuthenticationException：根据验证过程，远程证书无效。]
                    var httpclientHandler = new HttpClientHandler();
                    httpclientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                    using (var httpClient = new HttpClient(httpclientHandler))
                    {
                        var response = httpClient.PostAsync(postUrl, httpContent).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            var content = response.Content.ReadAsStringAsync().Result;

                            if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                throw new Exception(content);
                            }
                            throw new HttpRequestException(content);
                        }
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<TResult>(responseData, serializerSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                throw ex;
            }
        }
    }
}
