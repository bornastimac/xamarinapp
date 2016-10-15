using System.Collections.Generic;
using System.Linq;
using Android.Content;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace CollectingMobile
{
    class SerializationHelper
    {
        public static List<Request> DeserializeRequests(ContextWrapper cw, string username)
        {
            var requestsFilename = "requests_" + username;

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

            else//requests file for user not found
            {
                //TODO: create file for if it doesnt exist(first time he logs in)
                return new List<Request>();
            }
        }

        public static void SerializeRequests(ContextWrapper cw, List<Request> requests, string username)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, requests);

                using(Stream fos = cw.OpenFileOutput("requests_" + username, FileCreationMode.Private))
                {
                    fos.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    fos.Close();
                }
            }         
        }

        public static List<User> DeserializeUsers(ContextWrapper cw)
        {
            var usersFilename = "users";

            if (cw.FileList().Contains(usersFilename))
            {
                using (Stream fis = cw.OpenFileInput(usersFilename))
                using (MemoryStream ms1 = new MemoryStream())
                {
                    fis.CopyTo(ms1);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        ms.Write(ms1.ToArray(), 0, ms1.ToArray().Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        return (List<User>)bf.Deserialize(ms);
                    }
                }
            }

            else//users file not found
            {
                SerializeUsers(cw,new List<User>());
                return new List<User>();
            }
        }

        public static void SerializeUsers(ContextWrapper cw, List<User> users) 
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, users);

                using (Stream fos = cw.OpenFileOutput("users", FileCreationMode.Private))
                {
                    fos.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    fos.Close();
                }
            }
        }
    }
}
