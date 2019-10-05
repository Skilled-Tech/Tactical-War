namespace API
{
    export namespace World
    {
        export const ID = "world";

        export class FinishLevelResult
        {
            rewards: Array<string>;

            constructor(reward: Array<string>)
            {
                this.rewards = reward;
            }
        }
    }
}