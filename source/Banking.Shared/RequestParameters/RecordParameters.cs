

namespace Banking.Shared.RequestParameters
{
    public class RecordParameters : RequestParameters
    {
        public RecordParameters()
        {
            OrderBy = "IsPending";
        }

        public DateTime EndingDate {get; set;} = DateTime.UtcNow;

        public DateTime BeginningDate {get; set;} = DateTime.MinValue;

        public bool IsDateValid() => EndingDate > BeginningDate;

        public bool? IsPending {get; set; } = null;

        public int? UserId {get; set;} = null;


    }
}