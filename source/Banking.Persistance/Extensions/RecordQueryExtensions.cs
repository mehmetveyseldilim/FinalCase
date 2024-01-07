
using System.Linq.Dynamic.Core;
using Banking.Persistance.Entities;
using Banking.Persistance.Helper;
using Banking.Shared.RequestParameters;

namespace Banking.Persistance.Extensions
{
    public static class RecordQueryExtensions
    {
        public static IQueryable<Record> FilterRecords(this IQueryable<Record> records, RecordParameters parameters) 
        {
            var filteredByUserId = FilterRecordsByUserId(records, parameters.UserId);
            var filteredByPending = FilterRecordsByPending(filteredByUserId, parameters.IsPending);
            var filteredByDate = FilterRecordsByDate(filteredByPending, parameters.BeginningDate, parameters.EndingDate);

            return filteredByDate;

        }

        public static IQueryable<Record> Sort(this IQueryable<Record> records, string orderByQueryString) 
        {
            if(string.IsNullOrWhiteSpace(orderByQueryString)) 
            {
                return records.OrderBy(x => x.Id);
            }

            var orderQuery = GenericQueryParameterProcessor.CreateOrderQuery<Record>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return records.OrderBy(e => e.Id);

            return records.OrderBy(orderQuery);


        }

        private static IQueryable<Record> FilterRecordsByPending(IQueryable<Record> records, bool? isPending)
        {
            if(!isPending.HasValue) return records;

            var pendingRecords = records.Where(r => r.IsPending == isPending);
            return pendingRecords;

        }

        private static IQueryable<Record> FilterRecordsByUserId(IQueryable<Record> records, int? userId)
        {
            if(!userId.HasValue) return records;

            var userIdRecords = records.Where(r => r.UserId == userId);
            return userIdRecords;

        }

        private static IQueryable<Record> FilterRecordsByDate(IQueryable<Record> records, DateTime? begin, DateTime? end)
        {
            switch ((begin.HasValue, end.HasValue))
            {
                case (true, true):
                    return records.Where(r => r.TimeStamp >= begin && r.TimeStamp <= end);
                case (true, false):
                    return records.Where(r => r.TimeStamp >= begin);
                case (false, true):
                    return records.Where(r => r.TimeStamp <= end);
                default:
                    return records;
            }
        }

    }
}