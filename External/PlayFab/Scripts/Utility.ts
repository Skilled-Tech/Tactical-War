namespace Utility
{
    export namespace Dates
    {
        export function DaysFrom(date: any): number
        {
            return DaysBetween(date, new Date());
        }

        export function DaysBetween(date1: Date, date2: Date): number
        {
            return Math.round((date2.valueOf() - date1.valueOf()) / (86400000));
        }
    }
}