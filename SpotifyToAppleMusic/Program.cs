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

        //private static readonly HttpClient client = new HttpClient();


        static void Main(string[] args)
        {
            var newToken = AuthenticateSpotify().Result;

            if (newToken != null)
            {
                Console.WriteLine("Please enter the Spotify PlaylistID: ");
                string spotiPlaylistID = Console.ReadLine();
                GetSpotifyPlaylist(spotiPlaylistID, newToken).Wait();
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
        /* This method returns the playlist as a list of tracks.
         * API information taken from https://developer.spotify.com/documentation/web-api/reference/playlists/get-playlists-tracks/
         * If returns an error, its likely the playlist is private, or just outight doesnt exist.
         */
        private static async Task GetSpotifyPlaylist(string playlistID, SpotifyToken spotifyToken)
        {

            string url = String.Format("https://api.spotify.com/v1/playlists/{0}/tracks", playlistID);
            Console.WriteLine("Getting Spotfiy Playlist...");
            using (var httpClient = new HttpClient())
            {
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(spotifyToken.Type, spotifyToken.Token);

                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(
                            "Spotify Returned an error while retrieving the playlist, the status code was: {0}. \n" +
                            "Please ensure that you are using the Spotify Playlist ID and not the URI.\n" +
                            "Please also ensure the playlist exists and is public.",
                            response.StatusCode
                        );
                       
                    }

                    var responseMessage = await response.Content.ReadAsStringAsync();
                    // var serializer = new DataContractJsonSerializer(typeof(SpotifyToken));
                    Console.WriteLine(responseMessage);
                    // return serializer.ReadObject(responseMessage) as SpotifyToken;
                }
            }
        }
    }

}
;