using System.Collections.Specialized;
using System.Web;
using System;

namespace KOK.ApiHandler.Utilities
{
    public static class QueryHelper
    {
        public static string BuildUrl(string baseUrl, NameValueCollection queryParams)
        {
            var builder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (string key in queryParams)
            {
                query[key] = queryParams[key];
            }

            builder.Query = query.ToString();
            return builder.ToString();
        }
    }
}
