//
// ================
// Shared Resources
// ================
//
// This class is a container for all shared resources that may be needed
// by the drivers served by the Local Server. 
//
// NOTES:
//
//	* ALL DECLARATIONS MUST BE STATIC HERE!! INSTANCES OF THIS CLASS MUST NEVER BE CREATED!

using ASCOM.Utilities;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace ASCOM.LocalServer
{
    /// <summary>
    /// Add and manage resources that are shared by all drivers served by this local server here.
    /// In this example it's a serial port with a shared SendMessage method an idea for locking the message and handling connecting is given.
    /// In reality extensive changes will probably be needed. 
    /// Multiple drivers means that several drivers connect to the same hardware device, aka a hub.
    /// Multiple devices means that there are more than one instance of the hardware, such as two focusers. In this case there needs to be multiple instances
    /// of the hardware connector, each with it's own connection count.
    /// </summary>
    [HardwareClass]
    public static class SharedResources
    {
        
        private static readonly object lockObject = new object();   // Object used for locking to prevent multiple drivers accessing common code at the same time
        private static Utils.WebClient webclient = new Utils.WebClient();   // Shared web client


        // Public access to shared resources

        #region Dispose method to clean up resources before close
        /// <summary>
        /// Deterministically release both managed and unmanaged resources that are used by this class.
        /// </summary>
        /// <remarks>
        /// TODO: Release any managed or unmanaged resources that are used in this class.
        /// 
        /// Do not call this method from the DomeHardware.Dispose() method in your hardware class.
        ///
        /// This is because this shared resources class is decorated with the <see cref="HardwareClassAttribute"/> attribute and this Dispose() method will be called 
        /// automatically by the local server executable when it is irretrievably shutting down. This gives you the opportunity to release managed and unmanaged resources
        /// in a timely fashion and avoid any time delay between local server close down and garbage collection by the .NET runtime.
        ///
        /// </remarks>
        public static void Dispose()
        {
            try
            {
                if (webclient != null)
                {
                    webclient.Dispose();
                }
            }
            catch
            {
            }

        }
        #endregion

        #region WebClient Commands

            /// <summary>
            /// Shared Web Client
            /// </summary>
            public static Utils.WebClient WebClient
            {
                get
                {
                    return webclient;
                }
            }


        /// <summary>
        ///     getVariable method allows clients to submit a URL and variable name
        ///     and retrieve current value of that variable from the API
        /// </summary>
        /// <param name="URL"> The root URL of the API </param>
        /// <param name="variableName"> The variable to retrieve from the API </param>
        /// <returns>String : Current value of the variable</returns>
        /// <remarks>
        /// The lock prevents different drivers tripping over one another. It needs error handling and assumes that the message will be sent unchanged and that the reply will always be terminated by a "#" character.
        /// TODO : Not sure I need this
        /// </remarks>
        public static string getVariable(string URL, string variableName)
        {
            lock (lockObject)
            {
                Utils.ObservatoryControl observatoryControl = new Utils.ObservatoryControl();
                string response = WebClient.DownloadString(URL);
                observatoryControl = JsonConvert.DeserializeObject<Utils.ObservatoryControl>(response);

                // Directly access the Variables property and its properties
                var variables = observatoryControl.Variables;

                switch (variableName)
                {
                    case "shutterState":
                        return variables.ShutterState.ToString();
                    case "requestTimeUTC":
                        return variables.RequestTimeUTC.ToString();
                    case "roofStatusTimeUTC":
                        return variables.RoofStatusTimeUTC.ToString();
                    default:
                        return "Invalid variableName";
                }
            }
        }


        public static int roofCommand(string URL, string parameter)
        {
            lock (lockObject)
            {
                Utils.RoofCommand roofcommand = new Utils.RoofCommand();
                string response = WebClient.DownloadString(URL + "/roof_command?params=" + parameter);
                roofcommand = JsonConvert.DeserializeObject<Utils.RoofCommand>(response);

                // Directly access the Variables property and its properties
                var returnvalue = int.Parse(roofcommand.Return_Value);

                return returnvalue;
            }
        }
        #endregion

    }

}
