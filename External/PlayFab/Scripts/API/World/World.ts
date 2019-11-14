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

                    stars: number;

                    constructor(stars: number, reward: Array<string>)
                    {
                        this.stars = stars;
                        this.rewards = reward;
                    }
                }
            }
        }
    }
}