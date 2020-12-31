
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Dalisama.ResilientMicroserviceArchitecture.Common
{
    public class Message
    {
        public Dictionary<string, string> MetaData { get; set; }
        public string Body { get; set; }
        public List<ApplicationMetaData> applicationMetaData { get; set; } = new List<ApplicationMetaData>();
        public void SetStartTime()
        {
            applicationMetaData.Add(new ApplicationMetaData
            {
                ApplicationName = Environment.GetEnvironmentVariable("application name"),
                StartTime = DateTime.UtcNow,
                EndTime = default(DateTime),
                TimeSpan = new TimeSpan(0, 0, 0)

            });

        }
        public void SetEndTime()
        {
            var tmp = applicationMetaData.FirstOrDefault(x => x.ApplicationName == Environment.GetEnvironmentVariable("application name"));
            tmp.EndTime = DateTime.UtcNow;
            tmp.TimeSpan = tmp.EndTime - tmp.StartTime;
        }
        public static explicit operator Message(byte[] bytes)
        {
            var content = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<Message>(content);
        }
        public static explicit operator byte[](Message message)
        {
            string msgJson = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(msgJson);
        }
        public Dictionary<string, object> GetMetaData()
        {
            var result = MetaData.Keys.ToDictionary(x => x, x => MetaData[x] as object);
            foreach (var app in applicationMetaData)
            {
                result.Add($"{app.ApplicationName}.start", app.StartTime);
                if (default(DateTime) != app.EndTime)
                {
                    result.Add($"{app.ApplicationName}.end", app.EndTime);
                    result.Add($"{app.ApplicationName}.elapsed", app.TimeSpan);
                }
            }
            return result;
        }

    }
    public class ApplicationMetaData
    {
        public string ApplicationName { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}