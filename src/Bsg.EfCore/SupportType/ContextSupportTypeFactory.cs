namespace Bsg.EfCore.SupportType
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Context;
    using Domain;
    using Dtos;
    using Microsoft.EntityFrameworkCore;
    using Utils;

    public class ContextSupportTypeFactory : IContextSupportTypeFactory
    {
        private readonly IGenericReflectorService genericReflectorService;

        public ContextSupportTypeFactory(IGenericReflectorService genericReflectorService)
        {
            this.genericReflectorService = genericReflectorService;
        }

        public ContextSupportTypeDto BuildContextTypes<TContext>()
            where TContext : IDbContext
        {
            var allTypesInContextAssembly = this.GetTypesInContextImplementationAssembly<TContext>();
            var entityInterfaceForContextType = this.GetEntityInterfaceForContextType<TContext>();

            var supportTypes = new ContextSupportTypeDto
            {
                ConfigTypes = this.GetConfigTypes<TContext>(allTypesInContextAssembly, entityInterfaceForContextType),
            };

            return supportTypes;
        }

        private IList<ConfigSupportDto> GetConfigTypes<TContext>(IList<Type> typesToCheck, Type entityInterfaceForContextType)
            where TContext : IDbContext
        {
            var configSupportTypes = new List<ConfigSupportDto>();

            var openEntityConfigType = typeof(IEntityTypeConfiguration<>);
            var modelApplyMethod = this.GetModelApplyEntityConfigMethod(openEntityConfigType);

            var entityConfigurationsForContext =
                typesToCheck
                .Where(t => this.IsValidEntityConfigurationForContext(t, openEntityConfigType, entityInterfaceForContextType))
                .ToList();

            foreach (var entityConfigurationType in entityConfigurationsForContext)
            {
                var actualEntityType = this.ExtractEntityTypeFromEntityConfig(entityConfigurationType, openEntityConfigType);
                var generifiedApplyMethod = this.genericReflectorService.GetGenericMethodFromMethod(modelApplyMethod, new[] { actualEntityType });

                configSupportTypes.Add(
                    new ConfigSupportDto
                    {
                        EntityConfigurationType = entityConfigurationType,
                        GenerifiedModelApplyMethod = generifiedApplyMethod
                    });
            }

            return configSupportTypes;
        }
        
        private Type GetEntityInterfaceForContextType<TContext>()
            where TContext : IDbContext
        {
            return this.genericReflectorService.GetGenericTypeFromType(typeof(IEntity<>), new[] { typeof(TContext) });
        }

        private MethodInfo GetModelApplyEntityConfigMethod(Type openEntityConfigType)
        {
            var modelApplyConfigName = nameof(ModelBuilder.ApplyConfiguration);
            var modelBuilderType = typeof(ModelBuilder);

            return modelBuilderType
                 .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                 .First(m =>
                 m.Name == modelApplyConfigName &&
                 m.GetParameters().Count() == 1 &&
                 m.GetParameters().First().ParameterType.GetGenericTypeDefinition() == openEntityConfigType);
        }

        private IList<Type> GetTypesInContextImplementationAssembly<TContext>()
            where TContext : IDbContext
        {
            return Assembly.GetAssembly(typeof(TContext)).GetTypes().ToList();
        }

        private bool IsValidEntityConfigurationForContext(Type currentType, Type openEntityConfigType, Type entityInterfaceForContextType)
        {
            if (!currentType.IsClass || currentType.IsAbstract)
            {
                return false;
            }

            var validConfigInterface = currentType
                .GetInterfaces()
                .Where(i =>
                i.IsConstructedGenericType &&
                i.GetGenericTypeDefinition() == openEntityConfigType &&
                i.GetGenericArguments().Count() == 1)
                .FirstOrDefault();

            if (validConfigInterface == null)
            {
                return false;
            }

            return validConfigInterface
                .GetGenericArguments()
                .First()
                .GetInterfaces()
                .Where(i => i == entityInterfaceForContextType)
                .Count() == 1;
        }

        private Type ExtractEntityTypeFromEntityConfig(Type entityConfigType, Type openEntityConfigType)
        {
            return entityConfigType
                .GetInterfaces()
                .Where(i =>
                i.IsConstructedGenericType &&
                i.GetGenericTypeDefinition() == openEntityConfigType &&
                i.GetGenericArguments().Count() == 1)
                .First()
                .GetGenericArguments()
                .First();
        }
    }
}
