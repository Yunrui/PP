using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace PP
{
    enum EventId
    {
        None = 0,
        UnhandledException = 1,
        ComponentUsed = 2,
        SessionDuration = 3,
    }

    [DataContract]
    class Record
    {
        public Record()
        {
            this.LogTime = DateTime.Now;
        }

        [DataMember]
        public DateTime LogTime
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
        public EventId Event
        {
            get;
            set;
        }

        /// <summary>
        /// $TODO: get version from app configuration
        /// </summary>
        [DataMember]
        public Version Version
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

        private async Task Log(Record record)
        {
            // $TODO: seems that we need to distinguish enterprise account and individual account
            // And for enterprise account, NameAccessAllowed might be false unless we enable it explicitly in Settings
            // $TODO: cache for long run operation
            bool allowed = Windows.System.UserProfile.UserInformation.NameAccessAllowed;
            string pName = await Windows.System.UserProfile.UserInformation.GetPrincipalNameAsync();
            record.UserId = pName;

            this.records.Add(record);
        }

        public async Task PersistSettings()
        {
            System.Diagnostics.Debug.Assert(this.watcher.IsRunning, "Stopwatch is not in running state.");
            this.watcher.Stop();

            await this.Log(new Record()
            {
                Event = EventId.SessionDuration,
                CustomA = (this.watcher.ElapsedMilliseconds / 1000).ToString(),
            });

            DataContractSerializer serializer = new DataContractSerializer(typeof(IList<Record>));
            using(MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this.records);
                byte[] bytes = stream.ToArray();
                string content = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["Instrumentation"] = content;
            }
        }

        public void RestoreSettings()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Instrumentation"))
            {
                string logs = ApplicationData.Current.LocalSettings.Values["Instrumentation"] as string;

                using (Stream stream = new MemoryStream())
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(logs);
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                    DataContractSerializer deserializer = new DataContractSerializer(typeof(IList<Record>));
                    IList<Record> records = deserializer.ReadObject(stream) as IList<Record>;
                    if (records != null)
                    {
                        // Must cast to List otherwise it's fixed size array
                        this.records = records.ToList();
                    }
                }
            }
        }

        public string GetRecords()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Record record in this.records)
            {
                sb.AppendLine(string.Format(@"{0} - {1} - {2} - {3} - {4} - {5}", record.LogTime, record.UserId, record.Event, record.CustomA, record.CustomB, record.CustomC));
            }

            return sb.ToString();
        }

        public void SessionStart()
        {
            this.watcher.Restart();
        }
    }
}
