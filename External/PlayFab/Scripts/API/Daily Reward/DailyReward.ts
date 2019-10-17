namespace API
{
    export namespace DailyReward
    {
        export const ID = "daily-rewards";

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