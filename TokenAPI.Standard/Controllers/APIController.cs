// <copyright file="APIController.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace TokenAPI.Standard.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Converters;
    using TokenAPI.Standard;
    using TokenAPI.Standard.Authentication;
    using TokenAPI.Standard.Exceptions;
    using TokenAPI.Standard.Http.Client;
    using TokenAPI.Standard.Http.Request;
    using TokenAPI.Standard.Http.Request.Configuration;
    using TokenAPI.Standard.Http.Response;
    using TokenAPI.Standard.Utilities;

    /// <summary>
    /// APIController.
    /// </summary>
    public class APIController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="APIController"/> class.
        /// </summary>
        /// <param name="config"> config instance. </param>
        /// <param name="httpClient"> httpClient. </param>
        /// <param name="authManagers"> authManager. </param>
        /// <param name="httpCallBack"> httpCallBack. </param>
        internal APIController(IConfiguration config, IHttpClient httpClient, IDictionary<string, IAuthManager> authManagers, HttpCallBack httpCallBack = null)
            : base(config, httpClient, authManagers, httpCallBack)
        {
        }

        /// <summary>
        /// Get JWT access token from AAD to pass in auth header.
        /// </summary>
        /// <param name="grantType">Optional parameter: This should always be "client_credentials".</param>
        /// <param name="clientId">Optional parameter: This is the client/appID listed in your AD app registration.</param>
        /// <param name="clientSecret">Optional parameter: This is the client secret you will create and copy in from your AD app registration.</param>
        /// <param name="scope">Optional parameter: This is the resource URI found in the app manifest of your AD app registration with a "/.default" suffix (ie. api://xxxxxxx/.default).</param>
        /// <returns>Returns the object response from the API call.</returns>
        public object GetJwt(
                string grantType = null,
                string clientId = null,
                string clientSecret = null,
                string scope = null)
        {
            Task<object> t = this.GetJwtAsync(grantType, clientId, clientSecret, scope);
            ApiHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Get JWT access token from AAD to pass in auth header.
        /// </summary>
        /// <param name="grantType">Optional parameter: This should always be "client_credentials".</param>
        /// <param name="clientId">Optional parameter: This is the client/appID listed in your AD app registration.</param>
        /// <param name="clientSecret">Optional parameter: This is the client secret you will create and copy in from your AD app registration.</param>
        /// <param name="scope">Optional parameter: This is the resource URI found in the app manifest of your AD app registration with a "/.default" suffix (ie. api://xxxxxxx/.default).</param>
        /// <param name="cancellationToken"> cancellationToken. </param>
        /// <returns>Returns the object response from the API call.</returns>
        public async Task<object> GetJwtAsync(
                string grantType = null,
                string clientId = null,
                string clientSecret = null,
                string scope = null,
                CancellationToken cancellationToken = default)
        {
            // the base uri for api requests.
            string baseUri = this.Config.GetBaseUri();

            // prepare query string for API call.
            StringBuilder queryBuilder = new StringBuilder(baseUri);
            queryBuilder.Append("/");

            // append request with appropriate headers and parameters
            var headers = new Dictionary<string, string>()
            {
                { "user-agent", this.UserAgent },
                { "accept", "application/json" },
                { "Content-Type", "application/x-www-form-urlencoded" },
            };

            // append form/field parameters.
            var fields = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("grant_type", grantType),
                new KeyValuePair<string, object>("client_id", clientId),
                new KeyValuePair<string, object>("client_secret", clientSecret),
                new KeyValuePair<string, object>("scope", scope),
            };

            // remove null parameters.
            fields = fields.Where(kvp => kvp.Value != null).ToList();

            // prepare the API call request to fetch the response.
            HttpRequest httpRequest = this.GetClientInstance().Post(queryBuilder.ToString(), headers, fields);

            if (this.HttpCallBack != null)
            {
                this.HttpCallBack.OnBeforeHttpRequestEventHandler(this.GetClientInstance(), httpRequest);
            }

            // invoke request and get response.
            HttpStringResponse response = await this.GetClientInstance().ExecuteAsStringAsync(httpRequest, cancellationToken: cancellationToken).ConfigureAwait(false);
            HttpContext context = new HttpContext(httpRequest, response);
            if (this.HttpCallBack != null)
            {
                this.HttpCallBack.OnAfterHttpResponseEventHandler(this.GetClientInstance(), response);
            }

            // [200,208] = HTTP OK
            if ((response.StatusCode < 200) || (response.StatusCode > 208))
            {
                throw new ErrorException("unexpected error", context);
            }

            // handle errors defined at the API level.
            this.ValidateResponse(response, context);

            return response.Body;
        }
    }
}