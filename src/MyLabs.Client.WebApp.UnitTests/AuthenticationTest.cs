using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MyLabs.Client.WebApp.UnitTests
{
    public class AuthenticationTest
    {
        [Fact]
        public async Task TestAuthentication()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            
            var tokenClient1 = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse1 = await tokenClient1.RequestClientCredentialsAsync("api1");

            Assert.False(tokenResponse.IsError,tokenResponse.Error);

            Assert.Equal(tokenResponse.AccessToken, tokenResponse1.AccessToken);

            Console.WriteLine(tokenResponse.Json);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            
            var response = await client.GetAsync("http://localhost:5001/identity");

            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
    }
}
