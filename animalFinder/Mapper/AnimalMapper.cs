using System;
using System.Collections.Generic;
using AAnimal = animalFinder.DTO.API.Animal;
using SAnimal = animalFinder.DTO.Service.Animal;
using DAnimal = DAL.Entity.Animal;
using Mapster;

namespace animalFinder.Mapper
{
    public class AnimalMapper
    {
        private static readonly BaseMapper<AAnimal, SAnimal, DAnimal> BaseMapper;

        static AnimalMapper()
        {
            BaseMapper = new BaseMapper<AAnimal, SAnimal, DAnimal>();

            TypeAdapterConfig<AAnimal, SAnimal>.NewConfig()
                .Map(d => d.Id, s => !string.IsNullOrEmpty(s.Id) ? new Guid(s.Id) : Guid.Empty)
                .Map(d => d.Title, s => s.Title)
                .Map(d => d.Description, s => s.Description)
                .Map(d => d.Coordinates, s => CoordinateMapper.AtoS(s.Coordinates));

            TypeAdapterConfig<SAnimal, AAnimal>.NewConfig()
                .Map(d => d.Id, s => s.Id.ToString())
                .Map(d => d.Title, s => s.Title)
                .Map(d => d.Description, s => s.Description)
                .Map(d => d.PhoneNumber, s => s.PhoneNumber)
                .Map(d => d.Coordinates, s => CoordinateMapper.StoA(s.Coordinates));

            TypeAdapterConfig<DAnimal, SAnimal>.NewConfig()
                .Map(d => d.Id, s => s.Id.ToString())
                .Map(d => d.Title, s => s.Title)
                .Map(d => d.Description, s => s.Description)
                .Map(d => d.Image, s => s.Image)
                .Map(d => d.PhoneNumber, s => s.User.PhoneNumber)
                .Map(d => d.Status, s => s.Status)
                .Map(d => d.UploadDate, s => s.UploadDate)
                .Map(d => d.Coordinates, s => s.Coordinates);
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
