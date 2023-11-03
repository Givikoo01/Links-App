using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StocksApp.Models;
using StocksApp.ServiceContracts;

namespace StocksApp.Controllers
{
	public class TradeController : Controller
	{
			private readonly TradingOptions _tradingOptions;
			private readonly IFinnhubService _finnhubService;
			private readonly IConfiguration _configuration;
			
			public TradeController(IOptions<TradingOptions> tradingOptions, IFinnhubService finnhubService, IConfiguration configuration)
			{
				_tradingOptions = tradingOptions.Value;
				_finnhubService = finnhubService;
				_configuration = configuration;
			}


			[Route("/")]
			[Route("[action]")]
			[Route("~/[controller]")]
			public IActionResult Index()
			{
				//reset stock symbol if not exists
				if (string.IsNullOrEmpty(_tradingOptions.DefaultStockSymbol))
					_tradingOptions.DefaultStockSymbol = "AAPL";
				//get company profile from API server
				Dictionary<string, object>? companyProfileDictionary = _finnhubService.GetCompanyProfile(_tradingOptions.DefaultStockSymbol);

				//get stock price quotes from API server
				Dictionary<string, object>? stockQuoteDictionary = _finnhubService.GetStockPriceQuote(_tradingOptions.DefaultStockSymbol);


				//create model object
				StockTrade stockTrade = new StockTrade() { StockSymbol = _tradingOptions.DefaultStockSymbol };

				//load data from finnHubService into model object
				if (companyProfileDictionary != null && stockQuoteDictionary != null)
				{
					stockTrade = new StockTrade() { StockSymbol = Convert.ToString(companyProfileDictionary["ticker"]), StockName = Convert.ToString(companyProfileDictionary["name"]), Price = Convert.ToDouble(stockQuoteDictionary["c"].ToString()) };
				}

				//Send Finnhub token to view
				ViewBag.FinnhubToken = _configuration["FinnhubToken"];

				return View(stockTrade);
			}
		}
	}
