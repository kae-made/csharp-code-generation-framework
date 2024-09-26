// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public interface ExternalEntityDef
    {
        string EEKey { get; }
       
        Logger Logger { get; set; }

        IList<string> ConfigurationKeys { get; }
        void Initialize(IDictionary<string, object> configuration);

    }
}
