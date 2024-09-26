// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public class ChangedState
    {
        public enum Operation
        {
            Create,
            Update,
            Delete
        }
        public Operation OP { get; set; }
    }
}
