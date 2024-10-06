using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WireMock.Server;
using WireMock.Settings;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

[Binding]
public class CreditCardPaymentSteps
{
    private HttpResponseMessage _response;
    private HttpClient _client = new HttpClient();
    private WireMockServer _server;

    [Given(@"I am at the credit card payment page")]
    public void GivenIAmAtTheCreditCardPaymentPage()
    {
        TestContext.WriteLine("Navigating to the credit card payment page...");
        // This step just initializes the WireMock server
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = 3030
        });
        TestContext.WriteLine("Setting up WireMock server...");

        // Setup the WireMock server to respond with a successful transaction
        _server
            .Given(Request.Create().WithPath("/payment").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithBody("Successful transaction")
            );
        TestContext.WriteLine("WireMock server is running on port 3030.");
    }

    [When(@"I click to pay with valid info")]
    public async Task WhenIClickToPay()
    {
        // Create a sample payment request body
        var requestBody = new
        {
            name = "George Papadopoulos",
            cardNumber = "1234567812345678",
            expiryDate = "12/25",
            cvv = "123",
            amount = 100.0
        };

        var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // Send a POST request to the mock server
        _response = await _client.PostAsync("http://localhost:3030/payment", jsonContent);
        TestContext.WriteLine("POST request sent. Awaiting response...");
    }

    [Then(@"my transaction is successful")]
    public void ThenMyTransactionIsSuccessful()
    {
        // Verify the HTTP response status code and content
        Assert.IsNotNull(_response, "Response should not be null.");
        Assert.AreEqual(201, (int)_response.StatusCode, "Expected status code 201.");

        var content = _response.Content.ReadAsStringAsync().Result;
        Assert.IsTrue(content.Contains("Successful transaction"), "Expected response body to contain 'Successful transaction'.");

        // Output message after all assertions pass
        TestContext.WriteLine("Transaction was successful!");

        //Console.ReadKey();
    }
}
