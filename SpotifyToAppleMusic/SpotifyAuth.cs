using System;
using System.Runtime.Serialization;
namespace SpotifyToAppleMusic
{
    [DataContract(Name = "SpotifyToken")]
    public class SpotifyToken
    {
        [DataMember(Name = "access_token")]
        public string Token { get; set; }
        [DataMember(Name = "token_type")]
        public string Type { get; set; }
        [DataMember(Name = "expires_in")]
        public int TTL { get; set; }
    }
}
