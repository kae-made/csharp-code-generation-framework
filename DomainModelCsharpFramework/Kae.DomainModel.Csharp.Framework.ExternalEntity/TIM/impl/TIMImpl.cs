// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.impl
{
    public class TIMImpl : TIMWrapper
    {
        private List<Timer> timers = new List<Timer>();
        public override DateTime create_date(int year, int month, int day, int hour, int minute, int second)
        {
            return new DateTime(year, month, day, hour, minute, second);
        }

        public override DateTime current_clock()
        {
            return DateTime.Now;
        }

        public override DateTime current_date()
        {
            return current_clock();
        }

        public override int get_day(DateTime date)
        {
            return date.Day;
        }

        public override int get_hour(DateTime date)
        {
            return date.Hour;
        }

        public override int get_minute(DateTime date)
        {
            return date.Minute;
        }

        public override int get_month(DateTime date)
        {
            return date.Month;
        }

        public override int get_second(DateTime date)
        {
            return date.Second;
        }

        public override int get_year(DateTime date)
        {
            return date.Year;
        }

        public override bool timer_add_time(Timer timer_inst_ref, long microseconds)
        {
            bool existed = false;
            lock (timers)
            {
                var candidates = timers.Where(t => (t.TimerId == timer_inst_ref.TimerId));
                if (candidates.Count() > 0)
                {
                    existed = true; ;
                }
                if (existed)
                {
                    lock (timer_inst_ref)
                    {
                        timer_inst_ref.AddTime(microseconds);
                    }
                }
            }
            return existed;
        }

        public override bool timer_cancel(Timer timer_inst_ref)
        {
            bool existed = false;
            lock (timers)
            {
                var candidates = timers.Where(t => (t.TimerId == timer_inst_ref.TimerId));
                if (candidates.Count() > 0)
                {
                    existed = true;
                    existed = timer_inst_ref.Cancel();
                    timers.Remove(timer_inst_ref);
                }
            }
            return existed;
        }

        public override long timer_remaining_time(Timer timer_inst_ref)
        {
            long remaining = 0;
            lock (timers)
            {
                var candidates = timers.Where(t => (t.TimerId == timer_inst_ref.TimerId));
                if (candidates.Count() > 0)
                {
                    remaining = (long)(timer_inst_ref.RemainingTime().TotalSeconds * 1000000);
                }
            }
            return remaining;
        }

        public override bool timer_reset_time(Timer timer_inst_ref, long microseconds)
        {
            bool existed = false;
            lock (timers)
            {
                var candidates = timers.Where(t => (t.TimerId == timer_inst_ref.TimerId));
                if (candidates.Count() > 0)
                {
                    lock (timer_inst_ref)
                    {
                        existed = timer_inst_ref.ResetTime(DateTime.Now + TimeSpan.FromMilliseconds(microseconds / 1000));
                    }
                }
            }
            return existed;
        }

        public void RemoveTimer(Timer timer_inst_ref)
        {
            lock (timers)
            {
                timers.Remove(timer_inst_ref);
            }
        }
        public override Timer timer_start(EventData event_inst, long microseconds)
        {
            Timer timer = null;
            lock (timers)
            {
                timer = new TimerImpl(event_inst, microseconds, RemoveTimer);
                timers.Add(timer);
                lock (timer)
                {
                    timer.Start();
                }
            }
            return timer;
        }

        public override Timer timer_start_recurring(EventData event_inst, long microseconds)
        {
            Timer timer = null;
            lock (timers)
            {
                var candidates = timers.Where(t => t.EventInstance == event_inst);
                if (candidates.Count() > 0)
                {
                    var oldTimer = candidates.First();
                    lock (oldTimer)
                    {
                        oldTimer.Cancel();
                        timers.Remove(oldTimer);
                    }
                }
                timer = new TimerImpl(event_inst, microseconds, RemoveTimer);
                timers.Add(timer);
            }
            return timer;
        }

        public override DateTime time_add(DateTime time, int days, int hours, int minutes, int seconds)
        {
            return time.Add(TimeSpan.FromSeconds(seconds)).Add(TimeSpan.FromMinutes(minutes)).Add(TimeSpan.FromHours(hours)).Add(TimeSpan.FromDays(days));
        }

        public override long time_duration(DateTime timestamp1, DateTime timestamp2)
        {
            var duration = timestamp2 - timestamp1;
            return (int)((duration.Ticks / TimeSpan.TicksPerMillisecond) * 1000);
        }
    }
}

