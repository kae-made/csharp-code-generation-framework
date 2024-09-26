// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public  class LinkedInstance
    {
        public string RelationshipID { get; set; }
        public string Phrase { get; set; }
        public DomainClassDef Source { get; set; }
        public DomainClassDef Destination { get; set; }
        public bool Changed { get; set; } = true;
        public T GetDestination<T>() where T : DomainClassDef { return (T)Destination; }
    }
}
