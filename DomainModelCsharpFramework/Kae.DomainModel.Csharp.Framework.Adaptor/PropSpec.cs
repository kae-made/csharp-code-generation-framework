// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class PropSpec
    {
        public string Name { get; set; }
        public int Identity { get; set; }
        public bool Writable { get; set; }
        public ParamSpec.DataType DataType { get; set; }
        public bool Reference { get; set; }
        public bool Mathematical { get; set; }
        public bool StateMachineState { get; set; }
    }
}
