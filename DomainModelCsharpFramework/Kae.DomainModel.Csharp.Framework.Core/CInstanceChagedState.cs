// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public class CInstanceChagedState : ChangedState
    {
        public DomainClassDef Target { get; set; }
        public IDictionary<string, object> ChangedProperties { get; set; }
    }

    public delegate void CInstanceChangedStateNotifyHandler(CInstanceChagedState changedState);
}
