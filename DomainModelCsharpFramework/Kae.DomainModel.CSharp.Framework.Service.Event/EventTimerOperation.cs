using Newtonsoft.Json.Converters;

namespace Kae.DomainModel.CSharp.Framework.Service.Event
{
    public class EventTimerOperation
    {
        public enum OperationType
        {
            start,
            start_recurring,
            reset_time,
            cancel,
            remaining_time
        }
        public OperationType Operation { get; set; }
        public string TimerId { get; set; }

        public DateTime FireTime { get; set; }
        public string DestinationIdentities { get; set; }
        public string EventLabel { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new StringEnumConverter());
        }

        public static EventTimerOperation Deserialize(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<EventTimerOperation>(json);
        }
    }
}