namespace API
{
    export namespace DailyReward
    {
        export const ID = "daily reward";

        export class Result
        {
            progress: number;
            items: string[];

            constructor(progress: number, items: string[])
            {
                this.progress = progress;
                this.items = items;
            }
        }
    }
}