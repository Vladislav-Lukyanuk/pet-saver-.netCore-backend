using System.Collections.Generic;
using Mapster;

namespace animalFinder.Mapper
{
    public class BaseMapper<TA, TS, TD>
    {
        /// <summary>
        /// Map from api to service layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public TS AtoS(TA source)
        {
            return source.Adapt<TA, TS>();
        }

        /// <summary>
        /// Map from service to database layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public TD StoD(TS source)
        {
            return source.Adapt<TS, TD>();
        }

        /// <summary>
        /// Map from database to service layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public TS DtoS(TD source)
        {
            return source.Adapt<TD, TS>();
        }

        /// <summary>
        /// Map from service to api layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public TA StoA(TS source)
        {
            return source.Adapt<TS, TA>();
        }

        /// <summary>
        /// Map from api to service layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<TS> AtoS(IEnumerable<TA> source)
        {
            return source.Adapt<IEnumerable<TA>, IEnumerable<TS>>();
        }

        /// <summary>
        /// Map from service to database layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<TD> StoD(IEnumerable<TS> source)
        {
            return source.Adapt<IEnumerable<TS>, IEnumerable<TD>>();
        }

        /// <summary>
        /// Map from api to service layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<TS> DtoS(IEnumerable<TD> source)
        {
            return source.Adapt<IEnumerable<TD>, IEnumerable<TS>>();
        }

        /// <summary>
        /// Map from service to api layer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<TA> StoA(IEnumerable<TS> source)
        {
            return source.Adapt<IEnumerable<TS>, IEnumerable<TA>>();
        }
    }
}
