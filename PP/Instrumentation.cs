using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Popups;

namespace PP
{
    enum EventId
    {
        None = 0,
        UnhandledException = 1,
        ComponentUsed = 2,
        SessionDuration = 3,
        Action = 4,
    }

    [DataContract]
    class Record
    {
        public Record()
        {
            this.LogTime = DateTime.Now.ToString("yyyy-M-d H:m:s");
            var ver = Windows.ApplicationModel.Package.Current.Id.Version;
            this.Version = string.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
        }

        [DataMember]
        public string LogTime
        {
            get;
            private set;
        }

        [DataMember]
        public string UserId
        {
            get;
            set;
        }

        [DataMember]
        public string HardwareId
        {
            get;
            set;
        }

        [DataMember]
        public EventId Event
        {
            get;
            set;
        }

        [DataMember]
        public string Version
        {
            get;
            set;
        }

        [DataMember]
        public string CustomA
        {
            get;
            set;
        }

        [DataMember]
        public string CustomB
        {
            get;
            set;
        }

        [DataMember]
        public string CustomC
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Singleton Class for Instrumentation
    /// </summary>
    class Instrumentation
    {
        private static object lockobject = new object();
        private static Instrumentation instance;
        private IList<Record> records = new List<Record>();
        private Stopwatch watcher = new Stopwatch();
        private string hardwareId = string.Empty;
        private string userId = string.Empty;

        protected Instrumentation()
        {
        }

        public static Instrumentation Current
        {
            get
            {
                if (Instrumentation.instance != null)
                {
                    return Instrumentation.instance;
                }

                lock (Instrumentation.lockobject)
                {
                    if (Instrumentation.instance != null)
                    {
                        return Instrumentation.instance;
                    }

                    Instrumentation.instance = new Instrumentation();
                    return Instrumentation.instance;
                }
            }
        }

        public Task Log(Exception exception, string message)
        {
            // $TODO: do we really need log call stack? It's huge!
            // At least, we should make it smart
            Record record = new Record()
            {
                Event = EventId.UnhandledException,
                CustomA = exception.GetType().FullName,
                CustomB = exception.Message,
                CustomC = message,
            };

            return this.Log(record);
        }

        public Task Log(string componentName)
        {
            Record record = new Record()
            {
                Event = EventId.ComponentUsed,
                CustomA = componentName,
            };

            return this.Log(record);
        }

        private string HardwareId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.hardwareId))
                {
                    var token = HardwareIdentification.GetPackageSpecificToken(null);
                    var hardwareId = token.Id;
                    var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

                    byte[] bytes = new byte[hardwareId.Length];
                    dataReader.ReadBytes(bytes);

                    this.hardwareId = ComputeMD5(BitConverter.ToString(bytes));
                }

                return this.hardwareId;
            }
        }

        private static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

        private async Task<string> GetUserId()
        {
            if (string.IsNullOrWhiteSpace(this.userId))
            {
                // $TODO: seems that we need to distinguish enterprise account and individual account
                // And for enterprise account, NameAccessAllowed might be false unless we enable it explicitly in Settings
                // $TODO: cache for long run operation
                bool allowed = Windows.System.UserProfile.UserInformation.NameAccessAllowed;
                string pName = string.Empty;
                if (allowed)
                {
                    var first = await Windows.System.UserProfile.UserInformation.GetFirstNameAsync();
                    var last = await Windows.System.UserProfile.UserInformation.GetLastNameAsync();
                    pName = string.Format("{0} {1}", first, last);
                }

                this.userId = pName;
            }

            return this.userId;
        }

        public async Task Log(Record record)
        {
            record.UserId = await this.GetUserId();
            record.HardwareId = this.HardwareId;
            this.records.Add(record);
        }

        public async Task PersistSettings()
        {
            System.Diagnostics.Debug.Assert(this.watcher.IsRunning, "Stopwatch is not in running state.");
            this.watcher.Stop();

            await this.Log(new Record()
            {
                Event = EventId.SessionDuration,
                // Duration for an active session
                CustomA = (this.watcher.ElapsedMilliseconds / 1000).ToString(),
                // Region of this device
                CustomB = (new Windows.Globalization.GeographicRegion()).CodeTwoLetter,
                // Touch Enabled?
                CustomC = (new Windows.Devices.Input.TouchCapabilities()).TouchPresent.ToString(),
            });

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IList<Record>));
            using(MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this.records);
                await stream.FlushAsync();

                stream.Seek(0, SeekOrigin.Begin);
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Instrumentation", CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    await stream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
        }

        public async Task<string> LoadPersistData()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("Instrumentation");
                return await FileIO.ReadTextAsync(file);
            }
            catch (FileNotFoundException)
            {
            }
            catch (XmlException)
            {
            }

            return string.Empty;
        }

        public void RestoreSettings(string content)
        {
            var ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IList<Record>));

            this.records = ((IList<Record>)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(content)))).ToList();
        }

        public string GetRecords(int count)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Record record in this.records.Reverse().Take(count))
            {
                sb.AppendLine(string.Format(@"{0} - {1} - {2} - {3} - {4} - {5} - {6}", record.LogTime, record.HardwareId, record.UserId, record.Event, record.CustomA, record.CustomB, record.CustomC));
            }

            return sb.ToString();
        }

        public void SessionStart()
        {
            this.watcher.Restart();
        }
    }
}
