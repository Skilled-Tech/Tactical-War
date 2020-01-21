namespace API
{
    export namespace NewPlayerReward
    {
        export const ID = "new-player-reward";

        export class Result
        {
            items: string[];

            constructor(progress: number, items: string[])
            {
                this.items = items;
            }
        }
    }
}