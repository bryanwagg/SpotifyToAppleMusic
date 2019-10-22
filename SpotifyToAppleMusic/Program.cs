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
                Console.WriteLine(newToken.Token);

                // we need to user the spotify uri here instead.
                //TODO: split the incoming string on : and use the last one

                Console.Write("Please enter the Spotify PlaylistID: ");
                string spotiPlaylistID = Console.ReadLine();
                var playlistInfo = GetSpotifyPlaylist(spotiPlaylistID, newToken).Result;
                Console.WriteLine("The playlist name is {0} and has {1} songs.", playlistInfo.Name, playlistInfo.Tracks.Total);

                // At this point we need to creat an array of query strings that the Apple Music Calls will loop through to search and get objects
                while(playlistInfo.Tracks.Total > 0)
                {
                    if (playlistInfo.Tracks.Total < 100)
                    {
                        // if less than 100 tracks, we only have to make one call and only 1 object will be returned
                        GetSpotifyPlaylistTracks(spotiPlaylistID, 0, newToken).Wait();
                    } else
                    {
                        //for loop to make multiple calls, to the spotify endpoints and deals with multiple spotify paging objects.
                        for (int i = 0; i < max; i++)
                        {
                            //code here
                        }
                    }
                }

                
                var SpotifyTracks = GetSpotifyPlaylistTracks(spotiPlaylistID, 0, newToken).Result;

                //GetSpotifyPlaylistTracks().Result();

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
         * API information taken from https://developer.spotify.com/console/get-playlist/
         * If returns an error, its likely the playlist is private, or just outight doesnt exist.
         *
         * we have 2 different urls because spotify is stupid and doesnt allow us to get the name when using /playist/name, only /playlist although both return tracks.
         * https://api.spotify.com/v1/playlists/{0}/?fields=name,tracks.total should be used to first asses how many tracks we have and then use
         * https://api.spotify.com/v1/playlists/{0}/tracks?fields=items(track(name,album(name),artists))&offset={1} to get all the tracks after that.
         */
        private static async Task<SpotifyPlaylistInfo> GetSpotifyPlaylist(string playlistID, SpotifyToken spotifyToken)
        {
            string url = String.Format("https://api.spotify.com/v1/playlists/{0}/?fields=name,tracks.total", "3C69UXWRIO8HwRzwqAGWaz");
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
                            "Please ensure that you are using the Spotify URI.\n",
                            response.StatusCode
                        );

                    }

                    var responseMessage = await response.Content.ReadAsStreamAsync();
                    var serializer = new DataContractJsonSerializer(typeof(SpotifyPlaylistInfo));
                    
                    return serializer.ReadObject(responseMessage) as SpotifyPlaylistInfo;
                }
            }
        }

        /* This method returns the playlist as a list of tracks.
     * API information taken from https://developer.spotify.com/documentation/web-api/reference/playlists/get-playlists-tracks/
     * If returns an error, its likely the playlist is private, or just outight doesnt exist.
     */
        private static async Task<SpotifyPaging> GetSpotifyPlaylistTracks(string playlistID, int offset, SpotifyToken spotifyToken)
        {
            string url = String.Format("https://api.spotify.com/v1/playlists/{0}/tracks?fields=items(track(name,album(name),artists))&offset={1}", playlistID, offset);
            Console.WriteLine("Getting Spotfiy Tracks...");
            using (var httpClient = new HttpClient())
            {
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(spotifyToken.Type, spotifyToken.Token);

                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Something went seriously wrong, it worked 2 seconds ago. Here is the message: ");
                        Console.WriteLine(await response.Content.ReadAsStringAsync());
                        return null;
                    }
                    var responseMessage = await response.Content.ReadAsStreamAsync();
                    var serializer = new DataContractJsonSerializer(typeof(SpotifyPaging));
                    
                    return serializer.ReadObject(responseMessage) as SpotifyPaging;
                }
            }
        }
    }
}


