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
using System.Runtime.Serialization.Formatters.Binary;

namespace CollectingMobile
{
    class RequestsSerialization
    {
        public static List<Request> DeserializeAll(ContextWrapper cw)
        {
            var requestsFilename = "requests_" + ActiveUser.Username;

            if (cw.FileList().Contains(requestsFilename))
            {
                using (Stream fis = cw.OpenFileInput(requestsFilename))
                using (MemoryStream ms1 = new MemoryStream())
                {
                    fis.CopyTo(ms1);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        ms.Write(ms1.ToArray(), 0, ms1.ToArray().Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        return (List<Request>)bf.Deserialize(ms);
                    }
                }
            }
            else
            {
                //TODO: create file if it doesnt exist
                return new List<Request>();
            }
        }

        public static void SerializeAll(ContextWrapper cw)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, ActiveRequests.Requests);

                using(Stream fos = cw.OpenFileOutput("requests_" + ActiveUser.Username, FileCreationMode.Private))
                {
                    fos.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    fos.Close();
                }
            }         
        }

    }
}
