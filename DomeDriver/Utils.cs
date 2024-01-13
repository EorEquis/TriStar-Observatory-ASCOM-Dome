using System;
using Newtonsoft.Json;

namespace Utils
{
    /// <summary>
    /// Subclass of WebClient to provide access to the timeout property
    /// </summary>
    public partial class WebClient : System.Net.WebClient
    {

        private int _TimeoutMS = 1000;
        private bool _Connected = false; 

        public WebClient() : base()
        {
        }
        public WebClient(int TimeoutMS) : base()
        {
            _TimeoutMS = TimeoutMS;
        }
        /// <summary>
        /// Set the web call timeout in Milliseconds
        /// </summary>
        /// <value></value>
        public int setTimeout
        {
            set
            {
                _TimeoutMS = value;
            }
        }

        public bool Connected
        {
            get
            {
                return _Connected;
            }
            set
            {
                _Connected = value;
            }
        }

        protected override System.Net.WebRequest GetWebRequest(Uri address)
        {
            System.Net.WebRequest w = base.GetWebRequest(address);
            if (_TimeoutMS != 0)
            {
                w.Timeout = _TimeoutMS;
            }
            return w;
        }

    }

    public class ObservatoryControl
    {
        [JsonProperty("variables")]
        public Variables Variables { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hardware")]
        public string Hardware { get; set; }

        [JsonProperty("connected")]
        public bool Connected { get; set; }
    }

    public class Variables
    {
        [JsonProperty("shutterState")]
        public int ShutterState { get; set; }

        [JsonProperty("requestTimeUTC")]
        public DateTime RequestTimeUTC { get; set; }

        [JsonProperty("roofStatusTimeUTC")]
        public DateTime RoofStatusTimeUTC { get; set; }
    }

    public class RoofCommand
    {
        [JsonProperty("return_value")]
        public string Return_Value { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hardware")]
        public string Hardware { get; set; }

        [JsonProperty("connected")]
        public bool Connected { get; set; }
    }

}