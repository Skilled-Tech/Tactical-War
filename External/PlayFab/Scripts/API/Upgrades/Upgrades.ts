namespace API
{
    export namespace Upgrades
    {
        export const ID = "upgrades";

        export const Max = 5;

        export class Result
        {
            success: boolean;

            constructor(success: boolean)
            {
                this.success = success;
            }
        }
    }
}