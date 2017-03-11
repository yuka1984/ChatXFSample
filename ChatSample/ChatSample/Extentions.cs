using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;


    public static class HttpClientExtentions
    {
        private static Lazy<HttpClient> httpclientInitializer = new Lazy<HttpClient>(() => new HttpClient());

        /// <summary>
        /// Create HttpClient
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static HttpClient CreateHttpClient() => httpclientInitializer.Value;
    }

