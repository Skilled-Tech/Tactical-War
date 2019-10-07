namespace API
{
    export namespace World
    {
        export const ID = "world";

        export namespace Level
        {
            export namespace Finish
            {
                export enum Occurrence
                {
                    Initial = 1, Recurring = 2
                }

                export class Result
                {
                    rewards: Array<string>;

                    constructor(reward: Array<string>)
                    {
                        this.rewards = reward;
                    }
                }
            }
        }
    }
}