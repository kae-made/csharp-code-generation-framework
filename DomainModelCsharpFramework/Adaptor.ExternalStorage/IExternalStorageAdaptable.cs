using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage
{
    public interface IExternalStorageAdaptable
    {
        void AddCInstanceChangedStateNotifyHandler(CInstanceChangedStateNotifyHandler handler);
        void AddCLinkChangedStateNotifyHandler(CLinkChangedStateNotifyHandler handler);
        void AddCEventChangedStateNotifyHandler(CEventChangedStateNotifyHandler handler);
        void ClearAllInstances(string domainName);
    }
}
