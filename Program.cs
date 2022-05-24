using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdInMoTechTest
{
    class Program
    {
        private class StoreLookUpData //The class for the JSON data to be placed into
        {
            public string resultCount { get; set; }
            public ResultData[] results { get; set; }
            public class ResultData
            {
                public string trackName;
                public string bundleId;
            }
        }

        #region Global variables
        private static Regex _selectNonNumRgx = new Regex(@"[^\d]+");
        private static readonly HttpClient _client = new HttpClient();
        private const string _baseUrl = @"https://itunes.apple.com/lookup?id=";
        #endregion

        private static async Task Main(string[] args)
        {
            await BundleIDRetriever();
        }

        private static async Task BundleIDRetriever()
        {
            //Accept the input from the user
            Console.WriteLine("Hi AdInMo! Please type in an AppStoreID to retrieve a bundleID:");
            string appStoreID = "0";
            try { appStoreID = _selectNonNumRgx.Replace(Console.ReadLine(), ""); }
            catch { }
            if (appStoreID.Length == 0) { appStoreID = "0"; } //Ensure that something will be given to the request
            Console.WriteLine("\nLooking for " + appStoreID + "...\n");

            //Retrieve the bundleID if possible via http request
            try
            {
                HttpResponseMessage response = await _client.GetAsync(_baseUrl + appStoreID);
                string responseBody = await response.Content.ReadAsStringAsync();
                StoreLookUpData storeData = JsonConvert.DeserializeObject<StoreLookUpData>(responseBody);
                if (storeData.resultCount == "0") //If there are no results, do not return anything
                    Console.WriteLine("No Results found. Please check your AppStoreID.\n");
                else //If there are results, then show them
                    Console.WriteLine("The bundle ID for " + storeData.results[0].trackName + " is: " + storeData.results[0].bundleId + "\n");
            }
            catch (HttpRequestException error) { Console.WriteLine(error); }

            //Restart via recursion if you want to try again
            Console.WriteLine("Do you want to look for another bundleID?");
            string answer = Console.ReadLine().ToLower();
            if (answer == "y" || answer == "yes")
            {
                Console.WriteLine("");
                await BundleIDRetriever();
            }
        }
    }
}
