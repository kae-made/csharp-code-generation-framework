// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM
{
    public abstract class Timer
    {
        protected string timerId;
        protected EventData eventInstance;
        protected DateTime firingTime;
        protected bool waiting = true;

        public string TimerId { get { return timerId; } }
        public bool Waiting { get { return waiting; } }
        public EventData EventInstance { get { return eventInstance; } }

        public Timer(EventData eventInst, long microseconds)
        {
            this.eventInstance = eventInst;
            this.firingTime = DateTime.Now + TimeSpan.FromMilliseconds(1000 * microseconds);
            this.timerId = Guid.NewGuid().ToString();
        }
        public TimeSpan RemainingTime()
        {
            return firingTime - DateTime.Now;
        }
        public abstract void Start();
        public abstract bool Cancel();

        public abstract bool ResetTime(DateTime time);
        public abstract bool AddTime(long microseconds);
        
    }
}
