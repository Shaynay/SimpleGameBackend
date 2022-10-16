using System.Net;

namespace TestSHauss.Controller
{
	public static class FormDataExtractor
	{
		public static Dictionary<string, string> OfHttpListenerRequest(HttpListenerRequest request)
		{
			string requestBodyAsString = RequestContentAsString(request);
			if (string.IsNullOrEmpty(requestBodyAsString))
			{
				return null;
			}

			return ParseFormData(requestBodyAsString);
		}

		public static string RequestContentAsString(HttpListenerRequest request)
		{
			if (!request.HasEntityBody)
			{
				return string.Empty;
			}

			using(Stream body = request.InputStream)
			{
				using(StreamReader reader = new StreamReader(body, request.ContentEncoding))
				{
					return reader.ReadToEnd();
				}
			}
		}

		private static Dictionary<string, string> ParseFormData(string requestBodyAsString)
		{
			string[] parameters = requestBodyAsString.Split("&", StringSplitOptions.RemoveEmptyEntries);
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach (string keyValuePair in parameters)
			{
				string[] keyAndValue = keyValuePair.Split("=");
				result.Add(keyAndValue[0], keyAndValue[1]);
			}
			return result;
		}
	}
}
