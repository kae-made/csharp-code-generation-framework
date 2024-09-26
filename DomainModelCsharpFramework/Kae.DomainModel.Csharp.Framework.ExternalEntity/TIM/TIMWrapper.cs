// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.StateMachine;
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM
{
    public abstract class TIMWrapper : ExternalEntityDef
    {
        protected static readonly string eeKeyLetter = "TIM";
        public string EEKey { get { return eeKeyLetter; } }

        protected Logger logger;
        public Logger Logger { get { return logger; } set { logger = value; } }
        public IList<string> ConfigurationKeys { get { return new List<string>(); } }

        public void Initialize(IDictionary<string, object> configuration)
        {
            ;
        }

        public abstract DateTime create_date(int year, int month, int day, int hour, int minute, int second);
        public abstract DateTime current_clock();
        public abstract DateTime current_date();
        public abstract int get_year(DateTime date);
        public abstract int get_month(DateTime date);
        public abstract int get_day(DateTime date);
        public abstract int get_hour(DateTime date);
        public abstract int get_minute(DateTime date);
        public abstract int get_second(DateTime date);
        public abstract DateTime time_add(DateTime time, int days, int hours, int minutes, int seconds);
        public abstract long time_duration(DateTime timestamp1, DateTime timestamp2);
        public abstract bool timer_add_time(Timer timer_inst_ref, long microseconds);
        public abstract bool timer_cancel(Timer timer_inst_ref);
        public abstract long timer_remaining_time(Timer timer_inst_ref);
        public abstract bool timer_reset_time(Timer timer_inst_ref, long microseconds);
        public abstract Timer timer_start(EventData event_inst, long microseconds);
        public abstract Timer timer_start_recurring(EventData event_inst, long microseconds);
    }
}
