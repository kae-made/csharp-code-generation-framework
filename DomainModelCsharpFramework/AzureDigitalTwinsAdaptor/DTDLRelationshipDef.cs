// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage.AzureDigitalTwins
{
    public class DTDLRelationshipDef
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SourceTwinModelId { get; set; }
        public string FormClassKeyLetter { get; set; }
        public string TargetTwinModelId { get; set; }
        public string PartClassKeyLetter { get; set; }
    }
}
