// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class ParamSpec
    {
        public enum DataType
        {
            String,
            Integer,
            Real,
            Boolean,
            DateTime,
            Void,
            Enum,
            Complex
        }
        public string Name { get; set; }
        public DataType TypeKind { get; set; }
        public bool IsArray { get; set; }
    }
}
