using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// MetadataProvider to integrate our String Resource Manager to  application
    /// </summary>
    public class ASMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            if (containerType == null || propertyName == null)
                return metadata;
            string resourceName = containerType.Name + "_" + propertyName;

            metadata.DisplayName = ResMan.GetString(resourceName);

            if (!ResMan.Exists(resourceName))
            {
                metadata.DisplayName = ResMan.GetString(propertyName);
            }
            string hintName = resourceName + "_Hint";
            string hintValue = ResMan.GetString(hintName);

            if (hintValue != hintName)
            {
                metadata.Description = hintValue;
            }

            return metadata;
        }
    }
}