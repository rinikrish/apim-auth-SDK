// <copyright file="APIControllerTest.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace TokenAPI.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Converters;
    using NUnit.Framework;
    using TokenAPI.Standard;
    using TokenAPI.Standard.Controllers;
    using TokenAPI.Standard.Exceptions;
    using TokenAPI.Standard.Http.Client;
    using TokenAPI.Standard.Http.Response;
    using TokenAPI.Standard.Utilities;
    using TokenAPI.Tests.Helpers;

    /// <summary>
    /// APIControllerTest.
    /// </summary>
    [TestFixture]
    public class APIControllerTest : ControllerTestBase
    {
        /// <summary>
        /// Controller instance (for all tests).
        /// </summary>
        private APIController controller;

        /// <summary>
        /// Setup test class.
        /// </summary>
        [OneTimeSetUp]
        public void SetUpDerived()
        {
            this.controller = this.Client.APIController;
        }

        /// <summary>
        /// Get JWT access token from AAD to pass in auth header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task TestTestGetJwt()
        {
            // Parameters for the API call
            string grantType = null;
            string clientId = null;
            string clientSecret = null;
            string scope = null;

            // Perform API call
            object result = null;
            try
            {
                result = await this.controller.GetJwtAsync(grantType, clientId, clientSecret, scope);
            }
            catch (ApiException)
            {
            }

            // Test response code
            Assert.AreEqual(200, this.HttpCallBackHandler.Response.StatusCode, "Status should be 200");

            // Test headers
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");

            Assert.IsTrue(
                    TestHelper.AreHeadersProperSubsetOf (
                    headers,
                    this.HttpCallBackHandler.Response.Headers),
                    "Headers should match");
        }
    }
}