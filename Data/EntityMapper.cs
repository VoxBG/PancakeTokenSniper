using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AutoMapper;
using BscTokenSniper.Data.Entities;
using BscTokenSniper.Models;
using Fractions;

namespace BscTokenSniper.Data
{
    public class EntityMapper
    {
        public EntityMapper()
        {
            var configuration = new MapperConfiguration(cfg => 
            {
                // cfg.CreateMap<TokensOwnedEntity, TokensOwned>()
                //     .ForMember(source => source.Amount, options => 
                //         options.MapFrom((src, dest, destMember, context) => BigInteger.Parse(src.Amount)))
                //     .ForMember(source => source.BnbAmount, options => 
                //         options.MapFrom((src, dest, destMember, context) => BigInteger.Parse(src.BnbAmount)))
                //     .ForMember(source => source.SinglePrice, options => 
                //         options.MapFrom((src, dest, destMember, context) => Fraction.FromString(src.SinglePrice)));
                // cfg.CreateMap<TokensOwned, TokensOwnedEntity>()
                //     .ForMember(source => source.Amount, options => 
                //         options.MapFrom((src, dest, destMember, context) => src.Amount.ToString()))
                //     .ForMember(source => source.BnbAmount, options => 
                //         options.MapFrom((src, dest, destMember, context) => src.BnbAmount.ToString()))
                //     .ForMember(source => source.SinglePrice, options => 
                //         options.MapFrom((src, dest, destMember, context) => src.SinglePrice.ToString()));
            });
            // only during development, validate your mappings; remove it before release
            configuration.AssertConfigurationIsValid();
            // use DI (http://docs.automapper.org/en/latest/Dependency-injection.html) or create the mapper yourself
            mapper = configuration.CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        => mapper.Map(source, destination);
        public TDestination Map<TSource, TDestination>(TSource source)
        => mapper.Map<TDestination>(source);

        private IMapper mapper;
    }
}