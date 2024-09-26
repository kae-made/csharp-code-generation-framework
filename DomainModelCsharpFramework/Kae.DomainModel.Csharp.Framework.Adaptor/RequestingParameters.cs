// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class RequestingParameters
    {
        public string OpType { get; set; }
        public string Name { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public IDictionary<string,string> Identities { get; set; }

    }

}
