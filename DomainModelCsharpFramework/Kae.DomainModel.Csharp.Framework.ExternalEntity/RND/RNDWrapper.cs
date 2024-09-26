// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.ExternalEntities.RND
{
    public abstract class RNDWrapper : ExternalEntityDef
    {
        protected Logger logger;
        protected static readonly string eeKeyLetter = "RND";
        public string EEKey { get { return eeKeyLetter; } }

        public Logger Logger { get { return logger; } set { logger = value; } }
        public IList<string> ConfigurationKeys { get { return new List<string>(); } }

        public abstract void seed(int seed);

        public abstract int nextinteger();
        public abstract double nextreal();

        public void Initialize(IDictionary<string, object> configuration)
        {
            ;
        }
    }
}