namespace CurrencyScraper.CurrencyScraper.Tests;

// [Fact]
// public async Task FetchRatesAsync_ParsesCbrXml_Success()
// {
//     var handler = new MockHttpMessageHandler(); // Moq.Contrib.HttpClient
//     handler.When("http://www.cbr.ru/scripts/XML_daily.asp")
//         .Respond("application/xml", SampleXml); // SampleXml — строка с тестовым XML
//     
//     var parser = new CbrXmlParser(handler.CreateClient());
//     var rates = await parser.FetchRatesAsync(CancellationToken.None);
//     
//     rates.Should().Contain(r => r.CharCode == "USD");
//     rates.Should().Contain(r => r.Rate > 0);
// }