using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookList.DataAccess.Repository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        // return single values as count or first value - using integer or boolean value
        T Single<T>(string procedureName, DynamicParameters param = null);

        // scenario editng, deleting, execute some action and not retrieve anything back
        void Execute(string procedureName, DynamicParameters param = null);

        // retrun one complete row or record
        T OneRecord<T>(string procedureName, DynamicParameters param = null);

        // get all rows
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);

        // return two tables
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
    }
}
