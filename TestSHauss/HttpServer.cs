// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)

using System.Net;
using System.Text;
using TestSHauss.Controller;
using TestSHauss.Model;
using TestSHauss.Service;

namespace HttpListenerExample
{
	class HttpServer
	{
		public static HttpListener listener;
		public static string url = "http://localhost:8000/";
		public static int pageViews = 0;
		public static int requestCount = 0;
		public static string feedbackString = string.Empty;
		public static string pageData = File.ReadAllText("index.html");
		private const string displayHtmlString = "inline-block";
		private const string hideHtmlString = "none";
		private const string TOKEN_REPLACEMENT_STRING = "%AUTH_TOKEN%";
		private const string AUTHENTIFIED_REPLACEMENT_STRING = "%AUTHENTIFIED_DISPLAY%";
		private const string USER_REPLACEMENT_STRING = "%USER_INFO%";
		private const string SHOP_REPLACEMENT_STRING = "%ITEMS_SHOP_LIST%";
		private const string SEARCH_RESULT_REPLACEMENT_STRING = "%SEARCH_RESULT%";

		private static bool displaySearchResult = false;
		private static string searchResultHtmlString = "";


		public static bool runServer = true;

		public static async Task HandleIncomingConnections()
		{
			;
			// While a user hasn't visited the `shutdown` url, keep on handling requests
			while (runServer)
			{
				// Will wait here until we hear from a connection
				HttpListenerContext ctx = await listener.GetContextAsync();

				// Peel out the requests and response objects
				HttpListenerRequest req = ctx.Request;
				HttpListenerResponse resp = ctx.Response;
				try
				{
					PrintRequest(req);

					User currentRequestUser = null;
					string authenticationToken = null;
					if (CheckRequestValidityForAction(req))
					{
						Dictionary<string, string> formData = FormDataExtractor.OfHttpListenerRequest(req);
						formData.TryGetValue("authToken", out authenticationToken);
						currentRequestUser = AppController.DispatchRequestAction(req.Url.AbsolutePath, formData, authenticationToken);
					}

					// Make sure we don't increment the page views counter if `favicon.ico` is requested
					if (req.Url?.AbsolutePath != "/favicon.ico")
						pageViews += 1;

					await WriteResponse(resp, currentRequestUser, authenticationToken);
				}
				finally
				{
					displaySearchResult = false;
					resp.Close();
				}
			}
		}

		private static async Task WriteResponse(HttpListenerResponse resp, User user, string requestAuthToken)
		{
			string disableSubmit = !runServer ? "disabled" : "";
			string displayLoginForm = user == null ? displayHtmlString : hideHtmlString;
			string displayLoggedInInfo = user == null ? hideHtmlString : displayHtmlString;
			string loggedInInfoNickname = user == null ? "" : user.nickname;
			string authToken = user == null ? requestAuthToken : AuthenticationService.LoadToken(user);


			string newPageData = pageData.Replace(TOKEN_REPLACEMENT_STRING, authToken);
			newPageData = newPageData.Replace(AUTHENTIFIED_REPLACEMENT_STRING, displayLoggedInInfo);
			newPageData = newPageData.Replace(SHOP_REPLACEMENT_STRING, HtmlStrings.ShopHtmlString());
			if (displaySearchResult)
			{
				newPageData = newPageData.Replace(SEARCH_RESULT_REPLACEMENT_STRING, searchResultHtmlString);
			}
			else
			{
				newPageData = newPageData.Replace(SEARCH_RESULT_REPLACEMENT_STRING, "");
			}

			if (user != null)
			{
				newPageData = newPageData.Replace(USER_REPLACEMENT_STRING, HtmlStrings.UserHtmlString(user));
			}

			byte[] data = Encoding.UTF8.GetBytes(String.Format(newPageData,
				pageViews,
				feedbackString,
				disableSubmit,
				displayLoginForm,
				loggedInInfoNickname));
			resp.ContentType = "text/html";
			resp.ContentEncoding = Encoding.UTF8;
			resp.ContentLength64 = data.LongLength;

			// Write out to the response stream (asynchronously), then close it
			await resp.OutputStream.WriteAsync(data, 0, data.Length);
		}

		public static void DisplayResult(List<User> users, User authentifiedUser)
		{
			displaySearchResult = true;
			searchResultHtmlString = HtmlStrings.SearchResultHtmlString(users, authentifiedUser);
		}

		private static void PrintRequest(HttpListenerRequest req)
		{
			// Print out some info about the request
			Console.WriteLine("Request #: {0}", ++requestCount);
			Console.WriteLine(req.Url?.ToString());
			Console.WriteLine(req.HttpMethod);
			Console.WriteLine(req.UserHostName);
			Console.WriteLine(req.UserAgent);
			Console.WriteLine();
		}

		private static bool CheckRequestValidityForAction(HttpListenerRequest req)
		{
			// Only Allow POST and form data
			if (req.HttpMethod != HttpMethod.Post.Method || req.ContentType == null || req.ContentType != "application/x-www-form-urlencoded")
			{
				return false;
			}

			return true;
		}

		public static void Main(string[] args)
		{
			// Create a Http server and start listening for incoming connections
			listener = new HttpListener();
			listener.Prefixes.Add(url);
			listener.Start();
			Console.WriteLine("Listening for connections on {0}", url);

			// Handle requests
			Task listenTask = HandleIncomingConnections();
			listenTask.GetAwaiter().GetResult();

			// Close the listener
			listener.Close();
		}
	}
}