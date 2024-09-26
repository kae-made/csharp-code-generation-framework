using Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM;
using Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.impl;
using Kae.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.ETMR.impl
{
    public class ETMRImpl : ETMRWrapper
    {
        TIMWrapper defaultTIM = new TIMImpl();
        public override bool cancel(ExternalEntities.TIM.Timer timer_inst_ref)
        {
            return defaultTIM.timer_cancel(timer_inst_ref);
        }

        public override DateTime remaining_time(ExternalEntities.TIM.Timer timer_inst_ref)
        {
            var span = timer_inst_ref.RemainingTime();
            return new DateTime(span.Ticks);
        }

        public override bool reset_time(ExternalEntities.TIM.Timer timer_inst_ref, DateTime datetime)
        {
            return timer_inst_ref.ResetTime(datetime);
        }

        public override ExternalEntities.TIM.Timer start(DateTime datetime, EventData event_inst)
        {
            long msec = datetime.Ticks / TimeSpan.TicksPerMillisecond;
            return defaultTIM.timer_start(event_inst, msec * 1000);
        }

        public override ExternalEntities.TIM.Timer start_recuring(DateTime datetime, EventData event_inst)
        {
            long msec = datetime.Ticks / TimeSpan.TicksPerMillisecond;
            return defaultTIM.timer_start_recurring(event_inst, msec * 1000);
        }

        protected override void InitializeImple()
        {
            //
        }
    }
}
