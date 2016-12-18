using System.Diagnostics.Contracts;
using System;
using AS.Domain.Entities;
using System.Collections.Generic;

namespace AS.Services.Interfaces
{
    /// <summary>
    /// Contract class for application parameter service
    /// </summary>
    [ContractClassFor(typeof(IParameterService))]
    internal abstract class ParameterServiceContract : IParameterService
    {
        public void Reload(string parameterName)
        {
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(parameterName), "parameterName cannot be null");
        }
        public void ReloadAll()
        {

        }
        public void AddParameterDefinition(ParameterDefinition parameterDefinition)
        {
            Contract.Requires<ArgumentNullException>(parameterDefinition != null, "parameterDefinition cannot be null");
        }
        public void AddParameterValue(ParameterValue parameterValue)
        {
            Contract.Requires<ArgumentNullException>(parameterValue != null, "parameterValue cannot be null");
        }
        public void UpdateParameterDefinition(ParameterDefinition parameterDefinition)
        {
            Contract.Requires<ArgumentNullException>(parameterDefinition != null, "parameterDefinition cannot be null");
        }
        public void UpdateParameterValue(ParameterValue parameterValue)
        {
            Contract.Requires<ArgumentNullException>(parameterValue != null, "parameterValue cannot be null");
        }
        public ParameterDefinition GetParameterDefinitionById(int id)
        {
            Contract.Requires(id > 0, "id must be bigger than 0");
            Contract.Ensures(Contract.Result<ParameterDefinition>() != null);

            return null;
        }
        public ParameterDefinition GetParameterDefinitionByName(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Ensures(Contract.Result<ParameterDefinition>() != null);

            return null;
        }
        public void DeleteParameterDefinitionById(int id)
        {
            Contract.Requires(id > 0, "id must be bigger than 0");
        }
        public void DeleteParameterValueById(int id)
        {
            Contract.Requires(id > 0, "id must be bigger than 0");
        }
        public ParameterValue GetParameterValueById(int id)
        {
            Contract.Requires(id > 0, "id must be bigger than 0");
            Contract.Ensures(Contract.Result<ParameterValue>() != null);

            return new ParameterValue();
        }
        public IList<ParameterDefinition> SelectAllParameterDefinitions()
        {
            Contract.Ensures(Contract.Result<IEnumerable<ParameterDefinition>>() != null);
            return null;
        }
        public IList<ParameterValue> SelectParameterValues(string ordering, int parameterDefinitionId)
        {
            Contract.Requires(parameterDefinitionId > 0, "id must be bigger than 0");
            Contract.Ensures(Contract.Result<IEnumerable<ParameterValue>>() != null);
            return null;
        }
        public void DeleteParameterDefinitionByName(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
        }
    }
}
