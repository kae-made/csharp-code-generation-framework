// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.TIM.impl
{
    internal class TimerImpl : Timer
    {
        private Task timerAction;
        private CancellationToken cancelToken;
        private CancellationTokenSource cancelTokenSource;
        public delegate void RemoveSelf(Timer timer);
        private RemoveSelf remove;

        public TimerImpl(EventData eventInst, long microseconds, RemoveSelf remove) : base(eventInst, microseconds)
        {
            this.remove = remove;
            firingTime = DateTime.Now + TimeSpan.FromMilliseconds(microseconds / 1000);
            cancelTokenSource = new CancellationTokenSource();
            cancelToken = cancelTokenSource.Token;
            timerAction = new Task(async () =>
            {
                while (true)
                {
                    await Task.Delay((int)(microseconds / 1000), cancelToken);
                    lock (this)
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        if (DateTime.Now > firingTime)
                        {
                            eventInst.Send();
                            waiting = false;
                            break;
                        }
                    }
                }
                remove(this);
            });
        }
        public override bool AddTime(long microseconds)
        {
            bool waited = false;
            lock (this)
            {
                if (waiting)
                {
                    waited = waiting;
                    firingTime.Add(TimeSpan.FromMilliseconds(microseconds / 1000));
                }
            }
            return waited;
        }

        public override bool Cancel()
        {
            bool waited = false;
            lock (this)
            {
                if (waiting)
                {
                    waited = waiting;
                    cancelTokenSource.Cancel();
                }
            }
            return waited;
        }

        public override bool ResetTime(DateTime time)
        {
            bool waited = true;
            lock (this)
            {
                if (waiting)
                {
                    waited = waiting;
                    firingTime = time;
                }
            }
            return waited;
        }

        public override void Start()
        {
            timerAction.Start();
        }
    }
}
