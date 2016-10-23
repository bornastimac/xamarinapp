using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CollectingMobile
{
    class ConfigHelper
    {
        public static string GetServerDomain()
        {
            using (var streamReader = new StreamReader(GetConfigPath()))
            {
                string content = streamReader.ReadToEnd();
                return (string)JObject.Parse(content)["server"];
            }
        }

        private static string GetConfigPath()
        {
            Java.IO.File configDirectory = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/CollectingMobile");
            if (!configDirectory.Exists())
            {
                configDirectory.Mkdir();

                using (var writer = new StreamWriter(configDirectory.AbsolutePath + "/config.json"))
                {
                    writer.WriteLine("{\"server\": \"yourServerDomain\"}");
                }
            }

            return configDirectory.AbsolutePath + "/config.json";
        }
    }
}