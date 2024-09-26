// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class ClassSpec
    {
        public string Name { get; set; }
        public string KeyLetter { get; set; }
        public IDictionary<string, PropSpec> Properties { get; set; }
        public IDictionary<string, OperationSpec> Operations { get; set; }
        public IDictionary<string, LinkSpec> Links { get; set; }
        public IDictionary<string, OperationSpec> Events { get; set; }
    }
}
