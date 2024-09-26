using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.CSharp.Framework.Service.Event
{
    public class EventTimerResponse
    {
        public string TimerId { get; set; }
        public DateTime RemainingTime { get; set; }
        public bool WaitForFire { get; set; }
    }
}
