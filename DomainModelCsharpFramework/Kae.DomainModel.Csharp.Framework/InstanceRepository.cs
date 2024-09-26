// Copyright (c) Knowledge & Experience. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Kae.DomainModel.Csharp.Framework.Adaptor.ExternalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework
{
    public abstract class InstanceRepository : INotifyInstancesSstateChanged, IExternalStorageAdaptable
    {
        protected Dictionary<string, List<DomainClassDef>> domainInstances = new Dictionary<string, List<DomainClassDef>>();
        protected Dictionary<string, ExternalEntityDef> externalEntities = new Dictionary<string, ExternalEntityDef>();

        public abstract event ClassPropertiesUpdateHandler ClassPropertiesUpdated;
        public abstract event RelationshipUpdateHandler RelationshipUpdated;

        protected CInstanceChangedStateNotifyHandler cinstanceChangedStateNotifyHandler;
        protected CLinkChangedStateNotifyHandler clinkChangedStateNotifyHandler;
        protected CEventChangedStateNotifyHandler eventChangedStateNotifyHandler;
      

        public void AddCInstanceChangedStateNotifyHandler(CInstanceChangedStateNotifyHandler handler)
        {
            cinstanceChangedStateNotifyHandler = handler;
        }
        public void AddCLinkChangedStateNotifyHandler(CLinkChangedStateNotifyHandler handler)
        {
            clinkChangedStateNotifyHandler = handler;
        }
        public void AddCEventChangedStateNotifyHandler(CEventChangedStateNotifyHandler handler)
        {
            eventChangedStateNotifyHandler = handler;
        }

        public IEnumerable<string> GetDomainNames()
        {
            return domainInstances.Keys;
        }

        public void Add(DomainClassDef instance)
        {
            lock (domainInstances)
            {
                if (!domainInstances.ContainsKey(instance.ClassName))
                {
                    domainInstances.Add(instance.ClassName, new List<DomainClassDef>());
                }
                domainInstances[instance.ClassName].Add(instance);
            }
        }

        public bool Delete(DomainClassDef instance)
        {
            bool result = false;

            lock (domainInstances)
            {
                if (domainInstances.ContainsKey(instance.ClassName))
                {
                    if (domainInstances[instance.ClassName].Contains(instance))
                    {
                        domainInstances[instance.ClassName].Remove(instance);
                        if (domainInstances[instance.ClassName].Count == 0)
                        {
                            domainInstances.Remove(instance.ClassName);
                        }
                        result = true;
                    }
                }
            }

            return result;
        }

        public IEnumerable<DomainClassDef> GetDomainInstances(string domainName)
        {
            List<DomainClassDef> result = new List<DomainClassDef>();

            lock (domainInstances)
            {
                if (domainInstances.ContainsKey(domainName))
                {
                    var instances = domainInstances[domainName];
                    foreach (var instance in instances)
                    {
                        result.Add(instance);
                    }
                }
            }

            return result;
        }

        public delegate void CheckDomainInstanceStateDelegator(InstanceRepository instanceRepository, string domainName, Func<DomainClassDef, string> query, string classKeyLetter);

        public IEnumerable<DomainClassDef> GetDomainInstances(string domainName, Func<DomainClassDef, bool> predicate)
        {
            List<DomainClassDef> result = new List<DomainClassDef>();
            lock (domainInstances)
            {
                if (domainInstances.ContainsKey(domainName))
                {
                    var instances = domainInstances[domainName];
                    result.AddRange(instances.Where(predicate));
                }
            }
            return result;
        }

        public void ClearAllInstances(string domainName)
        {
            lock (domainInstances)
            {
                if (domainInstances.ContainsKey(domainName))
                {
                    foreach (var ci in domainInstances[domainName])
                    {
                        Delete(ci);
                    }
                }
            }
        }

        public IList<ChangedState> CreateChangedStates()
        {
            return new List<ChangedState>();
        }

        public void SyncChangedStates(IList<ChangedState> changedStates)
        {
            lock (domainInstances)
            {
                var createdInstances = new List<DomainClassDef>();
                var changedAll = new List<ChangedState>();
                foreach (var changedState in changedStates)
                {
                    if (changedState is CInstanceChagedState)
                    {
                        var cinstChangedState = (CInstanceChagedState)changedState;
                        if (changedState.OP == ChangedState.Operation.Create)
                        {
                            createdInstances.Add(cinstChangedState.Target);
                            cinstChangedState.ChangedProperties = cinstChangedState.Target.ChangedProperties();
                        }
                        UpdateCInstance((CInstanceChagedState)changedState);
                        if (cinstanceChangedStateNotifyHandler != null)
                        {
                            cinstanceChangedStateNotifyHandler((CInstanceChagedState)changedState);
                        }
                    }
                    else if (changedState is CLinkChangedState)
                    {
                        UpdateCLink((CLinkChangedState)changedState);
                        if (clinkChangedStateNotifyHandler != null)
                        {
                            clinkChangedStateNotifyHandler((CLinkChangedState)changedState);
                        }
                    }
                }
                foreach (var className in domainInstances.Keys)
                {
                    foreach (var instance in domainInstances[className])
                    {
                        var updatedState = instance.ChangedProperties();
                        if (updatedState.Count > 0)
                        {
                            if (!createdInstances.Contains(instance))
                            {
                                var cinstChangedState = new CInstanceChagedState() { OP = ChangedState.Operation.Update, Target = instance, ChangedProperties = updatedState };
                                UpdateCInstance(cinstChangedState);
                                if (cinstanceChangedStateNotifyHandler != null)
                                {
                                    cinstanceChangedStateNotifyHandler(cinstChangedState);
                                }
                            }
                        }
                    }
                }
            }
        }

#if false
        public void UpdateState()
        {
            foreach (var className in domainInstances.Keys)
            {
                foreach (var instance in domainInstances[className])
                {
                    var changedStates = instance.ChangedProperties();
                    UpdateState(instance, changedStates);
                }
            }
        }
#endif
        ///
        /// Update stored state of the instance by changed argument.
        /// changed.key is name of property of the instance.
        /// changed.value is value of the property that the name of it  is changed.key
        ///
        // public abstract void UpdateState(DomainClassDef instance, IDictionary<string, object> chnaged);

        ///
        /// Construct state of the instances by instances argument.
        /// instances.key is domain class name.
        /// instances.value is instances states of the domain class.
        /// each item of the instances.value is property name and value pairs.
        ///
        public abstract void LoadState(string domainName, IDictionary<string, IList<IDictionary<string, object>>> instances);

        public abstract void UpdateCInstance(CInstanceChagedState instanceState);
        public abstract void UpdateCLink(CLinkChangedState linkState);

        public abstract IEnumerable<T> SelectInstances<T>(string className, IDictionary<string, object> conditionPropertyValues, Func<T, IDictionary<string, object>, bool> compare) where T : DomainClassDef;

        public void Add(ExternalEntityDef externalEntity)
        {
            lock (externalEntities)
            {
                externalEntities.Add(externalEntity.EEKey, externalEntity);
            }
        }

        public ExternalEntityDef GetExternalEntity(string eeKey)
        {
            ExternalEntityDef eeDef = null;
            lock (externalEntities)
            {
                if (externalEntities.ContainsKey(eeKey))
                {
                    eeDef = externalEntities[eeKey];
                }
            }
            return eeDef;
        }

        public IList<ExternalEntityDef> GetExternalEntities()
        {
            var resultSet = new List<ExternalEntityDef>();
            lock (externalEntities)
            {
                foreach (var eeKey in externalEntities.Keys)
                {
                    resultSet.Add(GetExternalEntity(eeKey));
                }
            }
            return resultSet;
        }

        protected IExternalStorageAdaptor externalStorageAdaptor;
        public IExternalStorageAdaptor ExternalStorageAdaptor { get { return externalStorageAdaptor; } set { externalStorageAdaptor = value; } }
    }

}
