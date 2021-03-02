using System;
using System.Collections.Generic;
using ACoordinate = animalFinder.DTO.API.Coordinate;
using SCoordinate = animalFinder.DTO.Service.Coordinate;
using DCoordinate = DAL.Entity.Coordinate;

namespace animalFinder.Mapper
{
    public class CoordinateMapper
    {
        private static readonly BaseMapper<ACoordinate, SCoordinate, DCoordinate> BaseMapper;

        static CoordinateMapper()
        {
            BaseMapper = new BaseMapper<ACoordinate, SCoordinate, DCoordinate>();
        }

        public static SCoordinate AtoS(ACoordinate source)
        {
            return BaseMapper.AtoS(source);
        }

        public static DCoordinate StoD(SCoordinate source)
        {
            return BaseMapper.StoD(source);
        }

        public static SCoordinate DtoS(DCoordinate source)
        {
            return BaseMapper.DtoS(source);
        }

        public static ACoordinate StoA(SCoordinate source)
        {
            return BaseMapper.StoA(source);
        }

        public static IEnumerable<SCoordinate> AtoS(IEnumerable<ACoordinate> source)
        {
            return BaseMapper.AtoS(source);
        }

        public static IEnumerable<DCoordinate> StoD(IEnumerable<SCoordinate> source)
        {
            return BaseMapper.StoD(source);
        }

        public static IEnumerable<SCoordinate> DtoS(IEnumerable<DCoordinate> source)
        {
            return BaseMapper.DtoS(source);
        }

        public static IEnumerable<ACoordinate> StoA(IEnumerable<SCoordinate> source)
        {
            return BaseMapper.StoA(source);
        }
    }
}
