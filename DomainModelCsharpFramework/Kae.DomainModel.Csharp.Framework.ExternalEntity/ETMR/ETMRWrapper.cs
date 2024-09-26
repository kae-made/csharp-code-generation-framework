using Kae.StateMachine;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.ETMR
{
    public abstract class ETMRWrapper : ExternalEntityDef
    {
        protected Logger logger;
        protected static readonly string eeKeyLetter = "ETMR";
        protected List<string> configurationKeys = new List<string>();
        protected IDictionary<string, object> configurations;
        public string EEKey { get { return eeKeyLetter; } }

        public Logger Logger { get { return logger; } set { logger = value; } }

        public IList<string> ConfigurationKeys { get { return configurationKeys; } }

        public void Initialize(IDictionary<string, object> configuration)
        {
            this.configurations = configuration;
            InitializeImple();
        }

        protected abstract void InitializeImple();

        public DateTime datetime_add(DateTime datetime, DateTime delta)
        {
            var resultOfDatetime = datetime.AddYears(delta.Year);
            resultOfDatetime = resultOfDatetime.AddMonths(delta.Month);
            resultOfDatetime = resultOfDatetime.AddDays(delta.Day);
            resultOfDatetime = resultOfDatetime.AddHours(delta.Hour);
            resultOfDatetime = resultOfDatetime.AddMinutes(delta.Minute);
            resultOfDatetime = resultOfDatetime.AddSeconds(delta.Second);

            return resultOfDatetime;
        }
        public DateTime datetime_sub(DateTime datetime, DateTime delta)
        {
            var resultOfDatetime = datetime.AddYears(-delta.Year);
            resultOfDatetime = resultOfDatetime.AddMonths(-delta.Month);
            resultOfDatetime = resultOfDatetime.AddDays(-delta.Day);
            resultOfDatetime = resultOfDatetime.AddHours(-delta.Hour);
            resultOfDatetime = resultOfDatetime.AddMinutes(-delta.Minute);
            resultOfDatetime = resultOfDatetime.AddSeconds(-delta.Second);

            return resultOfDatetime;
        }
        public  DateTime datetime_duration(DateTime datetime1, DateTime datetime2)
        {
            var duration = datetime2.AddYears(-datetime1.Year);
            duration = duration.AddMonths(-datetime1.Month);
            duration = duration.AddDays(-datetime1.Day);
            duration=duration.AddHours(-datetime1.Hour);
            duration = duration.AddMinutes(-datetime1.Minute);
            duration = duration.AddSeconds(-datetime1.Second);

            return duration;
        }

        public abstract Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer start(DateTime datetime, EventData event_inst);
        public abstract Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer start_recuring(DateTime datetime, EventData event_inst);
        public abstract bool reset_time(Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer timer_inst_ref, DateTime datetime);
        public abstract bool cancel(Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer timer_inst_ref);
        public abstract DateTime remaining_time(Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.Timer timer_inst_ref);
    }
}
