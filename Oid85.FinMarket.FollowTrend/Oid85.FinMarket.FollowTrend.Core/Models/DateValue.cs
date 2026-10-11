namespace Oid85.FinMarket.FollowTrend.Core.Models
{
    /// <summary>
    /// Дата - значение
    /// </summary>
    public class DateValue<T>
    {
        public DateOnly Date { get; set; }
        public T Value { get; set; }
    }
}
