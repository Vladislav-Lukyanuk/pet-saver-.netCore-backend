using System;
using System.Collections.Generic;
using AAnimal = animalFinder.DTO.API.RegisteredAnimal;
using SAnimal = animalFinder.DTO.Service.RegisteredAnimal;
using DAnimal = DAL.Entity.RegisteredAnimal;
using Mapster;

namespace animalFinder.Mapper
{
    public class RegisteredAnimalMapper
    {
        private static readonly BaseMapper<AAnimal, SAnimal, DAnimal> BaseMapper;

        static RegisteredAnimalMapper()
        {
            BaseMapper = new BaseMapper<AAnimal, SAnimal, DAnimal>();

            TypeAdapterConfig<AAnimal, SAnimal>.NewConfig()
                .Map(d => d.Id, s => !string.IsNullOrEmpty(s.Id) ? new Guid(s.Id) : Guid.Empty)
                .Map(d => d.Image, s => s.Image)
                .Map(d => d.QR, s => s.QR)
                .Map(d => d.Name, s => s.Name)
                .Map(d => d.Animal, s => AnimalMapper.AtoS(s.Animal));

            TypeAdapterConfig<SAnimal, AAnimal>.NewConfig()
                .Map(d => d.Id, s => s.Id.ToString())
                .Map(d => d.Image, s => s.Image)
                .Map(d => d.QR, s => s.QR)
                .Map(d => d.Name, s => s.Name)
                .Map(d => d.Animal, s => AnimalMapper.StoA(s.Animal));
        }

        public static SAnimal AtoS(AAnimal source)
        {
            return BaseMapper.AtoS(source);
        }

        public static DAnimal StoD(SAnimal source)
        {
            return BaseMapper.StoD(source);
        }

        public static SAnimal DtoS(DAnimal source)
        {
            return BaseMapper.DtoS(source);
        }

        public static AAnimal StoA(SAnimal source)
        {
            return BaseMapper.StoA(source);
        }

        public static IEnumerable<SAnimal> AtoS(IEnumerable<AAnimal> source)
        {
            return BaseMapper.AtoS(source);
        }

        public static IEnumerable<DAnimal> StoD(IEnumerable<SAnimal> source)
        {
            return BaseMapper.StoD(source);
        }

        public static IEnumerable<SAnimal> DtoS(IEnumerable<DAnimal> source)
        {
            return BaseMapper.DtoS(source);
        }

        public static IEnumerable<AAnimal> StoA(IEnumerable<SAnimal> source)
        {
            return BaseMapper.StoA(source);
        }
    }
}
