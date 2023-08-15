using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Serialization.Serializers;
using WaterS.Application.Interfaces.Services.Storage;
using WaterS.Application.Interfaces.Services.Storage.Provider;
using WaterS.Application.Serialization.JsonConverters;
using WaterS.Application.Serialization.Options;
using WaterS.Application.Serialization.Serializers;
using WaterS.Infrastructure.Repositories;
using WaterS.Infrastructure.Services.Storage;
using WaterS.Infrastructure.Services.Storage.Provider;

namespace WaterS.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<IBrandRepository, BrandRepository>()
                .AddTransient<IBottleTypeRepository, BottleTypeRepository>()
                .AddTransient<ICompanyRepository, CompanyRepository>()
                 .AddTransient<IAccTransRepository, AccTransRepository>()
                                  .AddTransient<IAccountMovmentRepository, AccountMovmentRepository>()
                                                                    .AddTransient<ICustomersRepository, CustomersRepository>()
                                                                    .AddTransient<ICustomerPhoneRepository, CustomerPhoneRepository>()

                 .AddTransient<ITalapRepository, TalapRepository>()

                .AddTransient<IDocumentRepository, DocumentRepository>()
                .AddTransient<IDocumentTypeRepository, DocumentTypeRepository>()
                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }

        public static IServiceCollection AddExtendedAttributesUnitOfWork(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IExtendedAttributeUnitOfWork<,,>), typeof(ExtendedAttributeUnitOfWork<,,>));
        }

        public static IServiceCollection AddServerStorage(this IServiceCollection services)
            => AddServerStorage(services, null);

        public static IServiceCollection AddServerStorage(this IServiceCollection services, Action<SystemTextJsonOptions> configure)
        {
            return services
                .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
                .AddScoped<IStorageProvider, ServerStorageProvider>()
                .AddScoped<IServerStorageService, ServerStorageService>()
                .AddScoped<ISyncServerStorageService, ServerStorageService>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
        }
    }
}