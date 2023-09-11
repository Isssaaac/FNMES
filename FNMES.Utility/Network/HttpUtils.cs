using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace FNMES.Utility.Network
{
    public class HttpUtils
    {
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str, Encoding.UTF8);
        }

        public static string UrlDecode(string str, Encoding encoding)
        {
            return HttpUtility.UrlDecode(str, encoding);
        }

        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str, Encoding.UTF8);
        }
        public static string UrlEncode(string str, Encoding encoding)
        {
            return HttpUtility.UrlEncode(str, encoding);
        }


        private static CookieContainer m_Cookie = new CookieContainer();
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = m_Cookie;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
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
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.CookieContainer = m_Cookie;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }


        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreatePostHttpResponse(string url, string body, string contentType, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
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
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.CookieContainer = m_Cookie;
            request.Method = "POST";
            request.ContentType = contentType;

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据
            byte[] data = requestEncoding.GetBytes(body);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }


        /// <summary>
        /// get方法请求服务器
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parametersDict"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string DoGet(string url, Dictionary<string, string> parametersDict, int? timeout)
        {
            if (parametersDict != null && parametersDict.Count != 0)
            {
                if (!url.Contains("?"))
                    url = url + "?";
                foreach (string key in parametersDict.Keys)
                {
                    url = url + key + "=" + parametersDict[key] + "&";
                }
                url = url.Substring(0, url.Length - 1);
            }
            try
            {
                HttpWebResponse response = CreateGetHttpResponse(url, timeout, null, null);
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
                    {
                        return sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public static string PostFile(string url, Dictionary<string, string> parms, Dictionary<string, string> fileParms, int? timeout)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                HttpWebRequest request = null;
                //如果是发送HTTPS请求  
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.CookieContainer = m_Cookie;
                request.Method = "POST";
                request.UserAgent = DefaultUserAgent;
                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }

                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                Stream postStream = request.GetRequestStream();
                if (fileParms != null)
                {
                    foreach (KeyValuePair<string, string> keyValue in fileParms)
                    {
                        string filePath = keyValue.Value;
                        if (!File.Exists(filePath))
                        {
                            continue;
                        }
                        string fileName = Path.GetFileName(filePath);
                        byte[] headBytes = Encoding.UTF8.GetBytes($"Content-Disposition:form-data;name=\"{keyValue.Key}\";filename=\"{fileName}\"\r\nContent-Type:application/octet-stream\r\n\r\n");
                        byte[] fileDataBytes = null;
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            fileDataBytes = new byte[fs.Length];
                            fs.Read(fileDataBytes, 0, fileDataBytes.Length);
                        }
                        postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                        postStream.Write(headBytes, 0, headBytes.Length);
                        postStream.Write(fileDataBytes, 0, fileDataBytes.Length);
                        postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                    }
                }

                if (parms != null)
                {
                    foreach (KeyValuePair<string, string> keyValue in parms)
                    {
                        postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                        byte[] parmBytes = Encoding.UTF8.GetBytes($"Content-Disposition:form-data;name=\"{keyValue.Key}\"\r\n{keyValue.Value}\r\n");
                        postStream.Write(parmBytes, 0, parmBytes.Length);
                        postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                    }
                }
                postStream.Close();
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Post方法请求服务器
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parametersDict"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string DoPost(string url, Dictionary<string, string> parametersDict, int? timeout)
        {
            try
            {
                HttpWebResponse response = CreatePostHttpResponse(url, parametersDict, timeout, null, Encoding.UTF8, null);
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
                    {
                        return sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception exp)
            {
                return null;
            }
        }


        public static string DoPostData(string url, string data, string contentType, int? timeout)
        {
            try
            {
                HttpWebResponse response = CreatePostHttpResponse(url, data, contentType, timeout, null, Encoding.UTF8, null);
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
                    {
                        return sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public static string DoPostData(string url, string data, int? timeout)
        {
            return DoPostData(url, data, "application/json", timeout);
        }

        #region HttpDownloadFile 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">下载文件url路径</param>
        /// <param name="cookie">cookie</param>
        public static MemoryStream HttpDownloadFile(string url, CookieContainer cookie = null, WebHeaderCollection headers = null)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.CookieContainer = cookie;

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();

                //创建写入流
                MemoryStream stream = new MemoryStream();

                byte[] bArr = new byte[10240];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Seek(0, SeekOrigin.Begin);
                responseStream.Close();

                return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpUtil.HttpDownloadFile 下载文件 错误 URL：" + url);
                throw ex;
            }
        }
        #endregion

        #region HttpUploadFile 上传文件
        /// <summary>
        /// Http上传文件
        /// </summary>
        /// <param name="url">上传文件url路径</param>
        /// <param name="bArr">byte数据上传文件</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="cookie">cookie</param>
        public static string HttpUploadFile(string url, byte[] bArr, string fileName, CookieContainer cookie = null, WebHeaderCollection headers = null)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                request.ContentType = "text/plain;charset=utf-8";
                request.CookieContainer = cookie;

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                Stream postStream = request.GetRequestStream();
                postStream.Write(bArr, 0, bArr.Length);
                postStream.Close();

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                instream.Close();

                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpUtil.HttpUploadFile 上传文件 错误 URL：" + url);
                throw ex;
            }
        }
        #endregion

        #region HttpPost
        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url">url路径名称</param>
        /// <param name="data">需要传输的数据</param>
        /// <param name="cookie">cookie</param>
        public static string HttpPost(string url, string data, CookieContainer cookie = null, WebHeaderCollection headers = null)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = cookie;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                if (data != null)
                {
                    byte[] bArr = ASCIIEncoding.UTF8.GetBytes(data);

                    request.ContentLength = bArr.Length;

                    Stream postStream = request.GetRequestStream();
                    postStream.Write(bArr, 0, bArr.Length);
                    postStream.Close();
                }

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                instream.Close();

                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpUtil.HttpPost POST请求 错误 URL：" + url + " DATA：" + data);
                throw ex;
            }
        }
        #endregion

        #region HttpPost
        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url">url路径名称</param>
        /// <param name="cookie">cookie</param>
        public static string HttpPost(string url, CookieContainer cookie = null, WebHeaderCollection headers = null)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = cookie;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                instream.Close();

                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpUtil.HttpPost POST请求 错误 URL：" + url);
                throw ex;
            }
        }
        #endregion

        #region HttpGet
        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url">url路径名称</param>
        /// <param name="cookie">cookie</param>
        public static string HttpGet(string url, CookieContainer cookie = null, WebHeaderCollection headers = null)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = cookie;
                request.Method = "GET";
                request.ContentType = "text/plain;charset=utf-8";

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                instream.Close();
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpUtil.HttpGet GET请求 错误 URL：" + url);
                throw ex;
            }
        }
        #endregion

        #region HttpGet
        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url">url路径名称</param>
        /// <param name="cookieOut">传出的cookie</param>
        /// <param name="cookie">cookie</param>
        public static string HttpGet(string url, out Cookie cookieOut, CookieContainer cookie = null, WebHeaderCollection headers = null)
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.CookieContainer = cookie;
                request.Method = "GET";
                request.ContentType = "text/plain;charset=utf-8";

                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream instream = response.GetResponseStream();
                StreamReader sr = new StreamReader(instream, Encoding.UTF8);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                instream.Close();

                //获取Cookie
                CookieCollection cookieCollection = request.CookieContainer.GetCookies(request.RequestUri);
                if (cookieCollection != null && cookieCollection.Count > 0)
                {
                    cookieOut = cookieCollection[0];
                }
                else
                {
                    cookieOut = null;
                }

                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HttpUtil.HttpGet GET请求 错误 URL：" + url);
                cookieOut = null;
                throw ex;
            }
        }
        #endregion

        #region Put
        /// <summary>
        /// HTTP Put方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpPut(string url, string param = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "PUT";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }
        #endregion

        #region Delete
        /// <summary>
        /// HTTP Delete方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpDelete(string url, string param = null)
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "Delete";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }
        #endregion
    }
}
