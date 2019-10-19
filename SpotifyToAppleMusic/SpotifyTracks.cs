using System;
using System.Runtime.Serialization;

namespace SpotifyToAppleMusic
{
    // this Entire class is basically for the JSON response from getting tracks from a playlist.


    // spotify uses objects wrapped in what they call paging objects and one of the objects we're interested in is Items🤷🏻‍♂️
    [DataContract(Name= "items")]
    public class SpotifyPaging
    {
        [DataMember(Name="items")]
        public SpotifyItem[] Items { get; set; }
    }

    //pages contain Items
    [DataContract(Name = "items")]
    public class SpotifyItem
    {
        [DataMember(Name="track")]
        public SpotifyTrack Track { get; set; }
    }

    // Items contain an array of tracks
    [DataContract(Name = "track")]
    public class SpotifyTrack
    {
        [DataMember(Name = "album")]
        public SpotifyAlbum Album { get; set; }

        [DataMember(Name = "artists")]
        public SpotifyArtist[] Artists { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    // Tracks contain an album
    [DataContract(Name = "album")]
    public class SpotifyAlbum
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    // Tracks contain an array of artists.
    [DataContract(Name = "artist")]
    public class SpotifyArtist
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
