// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public delegate void ClassPropertiesUpdateHandler(object sender, ClassPropertiesUpdatedEventArgs e);
    public delegate void RelationshipUpdateHandler(object sender, RelationshipUpdatedEventArgs e);
    

    public interface INotifyInstancesSstateChanged
    {
        public event ClassPropertiesUpdateHandler? ClassPropertiesUpdated;
        public event RelationshipUpdateHandler? RelationshipUpdated;
    }
    public class ClassPropertiesUpdatedEventArgs : EventArgs
    {
        public string Operation { get; set; }
        public string ClassKeyLetter { get; set; }
        public DomainClassDef Instance { get; set; }
        public string Identities { get; set; }
        public IDictionary<string, object> Properties { get; set; }
    }

    public class RelationshipUpdatedEventArgs : EventArgs
    {
        public string Operation { get; set; }
        public string RelationshipId { get; set; }
        public string Phrase { get; set; }
        public string SourceClassKeyLetter { get; set; }
        public DomainClassDef SourceInstance { get; set; }
        public string SourceIdentities { get; set; }
        public string DestinationClassKeyLetter { get; set; }
        public DomainClassDef DestinationInstance { get; set; }
        public string DestinationIdentities { get; set; }
    }

}
