using FluentValidation;
using System;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// ValidatorFactory for FluentValidation. This class is required to integrate
    /// FluentValidation to our IOC container for DI.
    /// </summary>
    public class ValidatorFactory : IValidatorFactory
    {
        public IValidator GetValidator(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return DependencyResolver.Current.GetService(typeof(IValidator<>).MakeGenericType(type)) as IValidator;
        }

        public IValidator<T> GetValidator<T>()
        {
            return DependencyResolver.Current.GetService<IValidator<T>>();
        }
    }
}