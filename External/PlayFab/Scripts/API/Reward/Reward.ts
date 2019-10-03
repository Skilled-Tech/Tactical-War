namespace API
{
    export class Reward
    {
        items: [string];
        droptable: DropTable;

        constructor(items: [string], droptable: DropTable)
        {
            this.items = items;
            this.droptable = droptable;
        }

        //Static
        static Grant(playerID: string, data: Reward, annotation: string): string[]
        {
            let IDs = Array<string>();

            if (data.items == null)
            {

            }
            else
            {
                IDs = IDs.concat(data.items);
            }

            if (data.droptable == null)
            {

            }
            else
            {
                let result = PlayFab.Title.Catalog.Tables.Process(data.droptable);
                if (result != null)
                    IDs = IDs.concat(result);
            }

            PlayFab.Title.Catalog.Item.GrantAll(playerID, IDs, annotation);

            return IDs;
        }
    }
    export namespace Reward
    {
        export const ID = "rewards";

        export namespace Daily
        {
            export class Template
            {

            }
        }
    }
}