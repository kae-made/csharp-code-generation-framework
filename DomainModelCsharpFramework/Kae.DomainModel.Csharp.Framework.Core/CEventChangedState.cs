using Kae.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public class CEventChangedState : ChangedState
    {
        public DomainClassDef Target { get; set; }

        public EventData Event { get; set; }
    }
    public delegate void CEventChangedStateNotifyHandler(CEventChangedState changedState);
}
