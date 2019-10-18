using System;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

namespace SpotifyToAppleMusic
{
    class Program
    {
        const string SpotifyClientID = "";
        const string SpotifyClientSecret = "";

        private static readonly HttpClient client = new HttpClient();


        static void Main(string[] args)
        {
            var newToken = AuthenticateSpotify().Result;

            if (newToken != null)
            {
                //here we should go on to retrieve the playlist

                //once thats sucessful, search each track using apple music api and get an id for best match

                //once we have a list of tracks with apple music id's, add them to a playlist.
            }

        }

        /* Method that uses client id and secret and returns a token object if successful.
         * Created using api info from https://developer.spotify.com/documentation/general/guides/authorization-guide/#client-credentials-flow
         * If this breaks, you dont have clientID and secret set, the api has probably changed, or spotify is down.
         */
        private static async Task<SpotifyToken> AuthenticateSpotify()
        {
            Console.WriteLine("Getting Spotfiy Token...");
            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {   //this is the kvp's to be encoded and posted
                    { "grant_type", "client_credentials" }
                }))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(SpotifyClientID + ":" + SpotifyClientSecret);
                    var encodedAuth = System.Convert.ToBase64String(plainTextBytes);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuth);

                    HttpResponseMessage response = await httpClient.PostAsync("https://accounts.spotify.com/api/token", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(
                            "Spotify Auth returned an error, the status code was: {0}. \n" +
                            "Please ensure that Spotify ClientID and Secret are set.",
                            response.StatusCode
                        );
                        return null;
                    }

                    var responseMessage = await response.Content.ReadAsStreamAsync();
                    var serializer = new DataContractJsonSerializer(typeof(SpotifyToken));

                    return serializer.ReadObject(responseMessage) as SpotifyToken;
                }
            }
        }
    }

}
