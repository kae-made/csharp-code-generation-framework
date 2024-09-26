// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.RND
{
    public class RNDImpl : RNDWrapper
    {
        protected Random random;
        public RNDImpl()
        {
            random = new Random();
        }

        public override void seed(int seed)
        {
            random = new Random(seed);
        }
        public override int nextinteger()
        {
            throw new NotImplementedException();
        }

        public override double nextreal()
        {
            throw new NotImplementedException();
        }
    }
}
