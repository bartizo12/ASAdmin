using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
    public static class TypeConfigurationExtensions
    {
        /// <summary>
        /// Creates unique index on table when mapping entities.
        /// Taken From : http://stackoverflow.com/a/25779348/6117242
        /// </summary>
        /// <param name="property"></param>
        /// <param name="indexName">Name of the index</param>
        /// <param name="columnOrder">A number which will be used to determine column ordering for multi-column indexes.</param>
        /// <returns></returns>
        public static PrimitivePropertyConfiguration HasUniqueIndexAnnotation(
            this PrimitivePropertyConfiguration property,
            string indexName,
            int columnOrder)
        {
            var indexAttribute = new IndexAttribute(indexName, columnOrder) { IsUnique = true };
            var indexAnnotation = new IndexAnnotation(indexAttribute);

            return property.HasColumnAnnotation(IndexAnnotation.AnnotationName, indexAnnotation);
        }
    }
}