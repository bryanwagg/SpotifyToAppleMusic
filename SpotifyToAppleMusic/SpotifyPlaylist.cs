using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace SpotifyToAppleMusic
{
    [DataContract(Name = "SpotifyPlaylistInfo")]
    public class SpotifyPlaylistInfo
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "tracks")]
        public SpotifyPlaylistTotalTracks Tracks { get; set; }
    }

    [DataContract(Name = "tracks")]
    public class SpotifyPlaylistTotalTracks
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }
    }

}
