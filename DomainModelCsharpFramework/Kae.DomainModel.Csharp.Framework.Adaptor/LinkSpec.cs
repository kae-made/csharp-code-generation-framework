// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class LinkSpec
    {
        public string Name { get; set; }
        public string RelID { get; set; }
        public string Phrase { get; set; }
        public bool Set { get; set; }
        public bool Condition { get; set; }
        public string DstKeyLett { get; set; }
    }
}
