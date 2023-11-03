using StocksApp.ServiceContracts;
using System.Collections.Specialized;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace StocksApp.Services
{
	public class FinnhubService:IFinnhubService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		public FinnhubService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
		}
		public Dictionary<string, object>? GetCompanyProfile(string stockSymbol)
		{
			using (HttpClient httpClient = _httpClientFactory.CreateClient())
			{
				HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
				{
					
					RequestUri = new Uri($"https://finnhub.io/api/v1/stock/profile?symbol={stockSymbol}&token={_configuration["Token"]}"),
					Method = HttpMethod.Get
				};
				HttpResponseMessage httpResponseMessage = httpClient.Send(httpRequestMessage);
				Stream stream = httpResponseMessage.Content.ReadAsStream();
				StreamReader streamReader = new StreamReader(stream);
				string response = streamReader.ReadToEnd();
				Dictionary<string, object>? responseDictionary
					= JsonSerializer.Deserialize<Dictionary<string, object>>(response);
				if (responseDictionary == null)
					throw new InvalidOperationException("No response from server");

				if (responseDictionary.ContainsKey("error"))
					throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));
				return responseDictionary;
			}
		}
		public Dictionary<string, object>? GetStockPriceQuote(string stockSymbol)
		{
			using (HttpClient httpClient = _httpClientFactory.CreateClient())
			{
				HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
				{
					RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={_configuration["Token"]}"),
					Method = HttpMethod.Get
				};
				HttpResponseMessage httpResponseMessage = httpClient.Send(httpRequestMessage);
				Stream stream = httpResponseMessage.Content.ReadAsStream();
				StreamReader streamReader = new StreamReader(stream);
				string response = streamReader.ReadToEnd();
				Dictionary<string, object>? responseDictionary =
					JsonSerializer.Deserialize<Dictionary<string, object>>(response);
				if (responseDictionary == null)
					throw new InvalidOperationException("No response from server");

				if (responseDictionary.ContainsKey("error"))
					throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));
				return responseDictionary;
			}
		}
	}
}
