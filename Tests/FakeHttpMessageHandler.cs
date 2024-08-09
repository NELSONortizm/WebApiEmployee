using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public  class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Simula una respuesta HTTP exitosa con datos ficticios.
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"rates\": {\"USD\": 1.0, \"EUR\": 0.85}}") // Mocked response content
            };
            return Task.FromResult(response);
        }
    }//
}
