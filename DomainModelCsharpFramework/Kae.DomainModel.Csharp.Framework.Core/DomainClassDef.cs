// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Kae.DomainModel.Csharp.Framework
{
    public interface DomainClassDef
    {
        public string DomainName { get; }
        public string ClassName { get; }

        void DeleteInstance(IList<ChangedState> changedStates = null);

        /// <summary>
        /// Check attributes and links are valid or not.
        /// </summary>
        /// <returns></returns>
        bool Validate();

        // methods for storage
        void Restore(IDictionary<string, object> propertyValues);
        IDictionary<string, object> ChangedProperties();
        IDictionary<string, object> GetProperties(bool onlyIdentity);
        // IList<ChangedState> ChangedStates();
        string GetIdentities();

        string GetIdForExternalStorage();
    }
}
