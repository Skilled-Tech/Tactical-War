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

    export namespace Class
    {
        export function Assign<T>(ctor: new () => T, props: Partial<T>): T
        {
            return Object.assign(new ctor(), props);
        }

        export function WriteProperty(target: any, name: string, value: any)
        {
            let descriptor: PropertyDescriptor = {
                value: value,
                enumerable: true,
                writable: false,
            }

            Object.defineProperty(target, name, descriptor);
        }
    }
}