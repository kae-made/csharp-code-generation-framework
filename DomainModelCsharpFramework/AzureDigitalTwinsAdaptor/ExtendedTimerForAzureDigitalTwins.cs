using Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM;
using Kae.DomainModel.Csharp.Framework.ExternalEntities.ETMR;
using Kae.DomainModel.CSharp.Framework.Service.Event;
using Kae.StateMachine;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer;

namespace Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage.AzureDigitalTwins
{
    public class ExtendedTimerForAzureDigitalTwins : ETMRWrapper
    {
        private static readonly string timerServiceUrlKey = "timer-service-url";
        private static readonly string loggerKey = "logger";
        private static HttpClient httpClient = null;
        private string timerServiceUrl;
        private Logger logger;

        public ExtendedTimerForAzureDigitalTwins()
        {
            configurationKeys.Add(timerServiceUrlKey);
            configurationKeys.Add(loggerKey);
        }
        public override bool cancel(ExternalEntities.TIM.Timer timer_inst_ref)
        {
            bool existed = false;
            string cancelUrl = $"{timerServiceUrl}/CancelTimer?timerid={timer_inst_ref.TimerId}";
            var response = httpClient.GetAsync(timerServiceUrl).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string responseJson = reader.ReadToEnd();
                    var timerServiceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventTimerResponse>(responseJson);
                    if (timerServiceResponse != null)
                    {
                        existed = timerServiceResponse.WaitForFire;
                                            }
                }
                if (existed)
                {
                    logger.LogInfo($"Timer[{timer_inst_ref.TimerId}] has been caneled - {existed}");
                }
                else
                {
                    logger.LogInfo($"Timer[{timer_inst_ref.TimerId}] has been expired - {existed}");
                }
            }
            else
            {
                logger.LogWarning($"\"Timer[{timer_inst_ref.TimerId}] cancel failed - {response.StatusCode.ToString()}");
            }
            return existed;
        }

        public override DateTime remaining_time(ExternalEntities.TIM.Timer timer_inst_ref)
        {
            DateTime datetime = DateTime.Now;
            bool existed = false;
            string cancelUrl = $"{timerServiceUrl}/TimerStatus?timerid={timer_inst_ref.TimerId}";
            var response = httpClient.GetAsync(timerServiceUrl).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string responseJson = reader.ReadToEnd();
                    var timerServiceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EventTimerResponse>(responseJson);
                    if (timerServiceResponse != null)
                    {
                        existed = timerServiceResponse.WaitForFire;
                        if (existed)
                        {
                            datetime = timerServiceResponse.RemainingTime;
                            logger.LogInfo($"Timer[{timer_inst_ref.TimerId}] remaining_time is {datetime.ToString("yyyyMMddTHHmmss")}");
                        }
                        else
                        {
                            logger.LogInfo($"Timer[{timer_inst_ref.TimerId}] has been expired!");
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Timer[{timer_inst_ref.TimerId}] has no information!");
                    }
                }
            }
            else
            {
                logger.LogWarning($"Timer[{timer_inst_ref.TimerId}] remaining_time failed - {response.StatusCode.ToString()}");
            }
            return datetime;
        }

        public override bool reset_time(ExternalEntities.TIM.Timer timer_inst_ref, DateTime datetime)
        {
            logger.LogError("reset_time is not supported!");
            throw new NotImplementedException("reset_time is not supported!");
        }

        public override ExternalEntities.TIM.Timer start(DateTime datetime, EventData event_inst)
        {
            var newTimer = new TimerImpl(event_inst, datetime);
            var operation = new EventTimerOperation()
            {
                TimerId = newTimer.TimerId,
                Operation = EventTimerOperation.OperationType.start,
                FireTime = datetime,
                EventLabel = event_inst.EventName,
                DestinationIdentities = event_inst.GetReceiverIdentities(),
                Parameters = event_inst.GetSupplementalData()
            };
            var content = new StringContent(operation.Serialize());
            string timerServiceStartUrl = $"{timerServiceUrl}/TimerService_HttpStart";
            var response = httpClient.PostAsync(timerServiceUrl, content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string instanceId = "";
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string responseContentJson = reader.ReadToEnd();
                    dynamic responseContent = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContentJson);
                    instanceId = responseContent.id;
                }
                logger.LogInfo($"Timer[{newTimer.TimerId}] has been started.");
                if (newTimer.TimerId != instanceId)
                {
                    logger.LogError($"orchestration id is different - {instanceId}");
                }
            }
            else
            {
                logger.LogWarning($"Timer[{newTimer.TimerId}] start failed - {response.StatusCode.ToString()}");
                using (var reader = new StreamReader(response.Content.ReadAsStream()))
                {
                    string responseContentJson = reader.ReadToEnd();
                    logger.LogWarning($"Response - {responseContentJson}");
                }
            }
            return newTimer;
        }

        public override ExternalEntities.TIM.Timer start_recuring(DateTime datetime, EventData event_inst)
        {
            logger.LogError("start_recuring is not supported!");
            throw new NotImplementedException("start_recuring is not supported!");
        }

        protected override void InitializeImple()
        {
            httpClient = new HttpClient();
            timerServiceUrl = (string)this.configurations[timerServiceUrlKey];
            if (this.configurations.ContainsKey(loggerKey))
            {
                logger = (Logger)this.configurations[loggerKey];
            }
        }
    }

    public class TimerImpl : Timer
    {
        public string TimerIdOnService { get; set; }
        public TimerImpl() : base(null, 0)
        {

        }
        public TimerImpl(EventData eventData, DateTime fireTime) : base(eventData, 0)
        {
            this.firingTime = fireTime;
            TimerIdOnService = this.TimerId;
        }

        public override bool AddTime(long microseconds)
        {
            throw new NotImplementedException();
        }

        public override bool Cancel()
        {
            throw new NotImplementedException();
        }

        public override bool ResetTime(DateTime time)
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public void SetTime(DateTime fireTime)
        {
            this.firingTime = fireTime;
        }
    }
}
