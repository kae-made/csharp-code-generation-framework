// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage
{
    public interface IExternalStorageAdaptor
    {
        void Initialize();

        bool DoseEventComeFromExternal();

        Task<IEnumerable<DomainClassDef>> CheckInstanceStatus(string domainName, string classKeyLetter, IEnumerable<DomainClassDef> existingInstances, Func<string> query, Func<DomainClassDef> create, string cardinarity);
        Task<IEnumerable<DomainClassDef>> CheckTraverseStatus(string domainName, DomainClassDef spInstance, string epClassKeyLetter, string relationshipName, IEnumerable<DomainClassDef> existingInstances, Func<DomainClassDef> create, string cardinarity);

        void ClassPropertiesUpdater(string classKeyLetter, string operation, DomainClassDef instance, string identities, IDictionary<string, object> properties);
        void RelationshipUpdater(string relationshipId, string operation, string sourceClassKeyLetter, DomainClassDef sourceInstance, string sourceIdentities, string destinationClassKeyLetter, DomainClassDef destinationInstance, string destinationIdenities);
        void EventUpdater(string classKeyLetter, DomainClassDef instance, string eventLabel, IDictionary<string, object> supplimentalData);

        void ClearCache(string domainName);
    }
}
