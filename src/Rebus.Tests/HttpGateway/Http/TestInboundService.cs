﻿using System;
using System.Net;
using NUnit.Framework;
using Rebus.Gateway.Http.Inbound;
using Shouldly;

namespace Rebus.Tests.Gateway.Http
{
    [TestFixture]
    public class TestInboundService : FixtureBase
    {
        const string DestinationQueueName = "test.inbound.destination";
        const string ListenUri = "http://+:8080";
        const string InboundServiceUri = "http://127.0.0.1:8080";
        
        InboundService service;

        protected override void DoSetUp()
        {
            Console.WriteLine("Creating new service");
            service = new InboundService(ListenUri, DestinationQueueName);
            service.Start();
        }

        protected override void DoTearDown()
        {
            Console.WriteLine("Tearing down the service");
            service.Stop();
        }

        [TestCase("GET")]
        [TestCase("HEAD")]
        [TestCase("DELETE")]
        [TestCase("PUT")]
        public void GivesCorrectReplyWhenRequestUsesWrongMethod(string method)
        {
            // arrange
            var request = CreateRequest(InboundServiceUri, method);

            // act
            var response = GetResponse(request);

            // assert
            response.StatusCode.ShouldBe(HttpStatusCode.MethodNotAllowed);
        }

        HttpWebResponse GetResponse(HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse) request.GetResponse();
            }
            catch(WebException webException)
            {
                return (HttpWebResponse) webException.Response;
            }
        }

        HttpWebRequest CreateRequest(string uri, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.ContentLength = 0;
            return request;
        }
    }
}