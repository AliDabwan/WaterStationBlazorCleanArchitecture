using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using WaterS.Application.Serialization.Options;
using WaterS.Application.Serialization.Serializers;
using WaterS.Domain.Contracts;
using WaterS.Infrastructure.Extensions;

namespace WaterS.Infrastructure.Configurations
{
    public class EntityExtendedAttributeConfiguration : IEntityTypeConfiguration<IEntityExtendedAttribute>
    {
        public void Configure(EntityTypeBuilder<IEntityExtendedAttribute> builder)
        {
            // This Converter will perform the conversion to and from Json to the desired type
            builder
                .Property(e => e.Json)
                .HasJsonConversion(
                    new SystemTextJsonSerializer(
                        new OptionsWrapper<SystemTextJsonOptions>(new SystemTextJsonOptions())));
        }
    }
}