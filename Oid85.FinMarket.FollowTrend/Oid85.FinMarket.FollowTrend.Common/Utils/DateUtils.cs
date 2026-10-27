using System.Globalization;

namespace Oid85.FinMarket.FollowTrend.Common.Utils
{
    public static class DateUtils
    {
        public static List<DateOnly> GetDates(DateOnly from, DateOnly to)
        {
            var curDate = from;
            var dates = new List<DateOnly>();

            while (curDate <= to)
            {
                dates.Add(curDate);
                curDate = curDate.AddDays(1);
            }

            return dates;
        }

        public static List<DateOnly> GetMonthDates(DateOnly from, DateOnly to)
        {
            var curDate = new DateOnly(from.Year, from.Month, 1);
            var dates = new List<DateOnly>();

            while (curDate <= to)
            {
                dates.Add(curDate);
                curDate = curDate.AddMonths(1);
            }

            return dates;
        }

        public static List<(DateOnly StartDate, DateOnly EndDate)> GetWeeks(DateOnly from, DateOnly to)
        {
            // Для формирования полных недель
            var shiftDays = to.DayOfWeek.GetHashCode();
            to = to.AddDays(-1 * shiftDays);
            
            var result = new List<(DateOnly StartDate, DateOnly EndDate)>();

            var startDate = from.ToDateTime(TimeOnly.MinValue);
            var endDate = to.ToDateTime(TimeOnly.MinValue);

            // Обходим каждую неделю начиная с начальной даты
            while (startDate <= endDate)
            {
                // Определяем первый и последний дни недели
                DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                var monday = startDate.AddDays((firstDayOfWeek - startDate.DayOfWeek + 7) % 7);
                var sunday = monday.AddDays(6);

                result.Add(new(DateOnly.FromDateTime(monday), DateOnly.FromDateTime(sunday)));

                // Проверяем, входит ли воскресенье в диапазон дат
                if (sunday > endDate)
                    break;

                // Переход к следующей неделе
                startDate = sunday.AddDays(1);
            }

            return result;
        }
    }
}
