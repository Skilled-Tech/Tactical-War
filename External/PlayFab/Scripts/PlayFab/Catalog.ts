namespace PlayFab
{
    export class Catalog
    {
        items: PlayFabServerModels.CatalogItem[];

        public FindWithID(itemID: string): PlayFabServerModels.CatalogItem | null
        {
            for (let i = 0; i < this.items.length; i++)
                if (CompareIgnoreCase(this.items[i].ItemId, itemID))
                    return this.items[i];

            return null;
        }

        constructor(items?: PlayFabServerModels.CatalogItem[])
        {
            if (items == null)
                this.items = [];
            else
                this.items = items;
        }
    }
    export namespace Catalog
    {
        export const Default = "Default";

        export function Retrieve(version: string): Catalog
        {
            let result = server.GetCatalogItems(
                {
                    CatalogVersion: version,
                }
            );

            return new Catalog(result.Catalog);
        }

        export namespace Item
        {
            export function Grant(playerID: string, itemID: string, ammount: number, annotation: string, ): Array<PlayFabServerModels.ItemInstance>
            {
                let items = [];

                for (let i = 0; i < ammount; i++)
                    items.push(itemID);

                return GrantAll(playerID, items, annotation);
            }
            export function GrantAll(playerID: string, itemIDs: string[], annotation: string): Array<PlayFabServerModels.ItemInstance>
            {
                if (itemIDs == null || itemIDs.length == 0) return [];

                let result = server.GrantItemsToUser({
                    PlayFabId: playerID,
                    Annotation: annotation,
                    CatalogVersion: Catalog.Default,
                    ItemIds: itemIDs
                });

                if (result.ItemGrantResults == null) return [];

                return result.ItemGrantResults;
            }
        }

        export namespace Tables
        {
            export function Evaluate(tableID: string): string | null
            {
                let result = server.EvaluateRandomResultTable({
                    CatalogVersion: Catalog.Default,
                    TableId: tableID
                });

                if (result.ResultItemId == null) return null;

                return result.ResultItemId;
            }

            export function Process(table: API.DropTable): Array<string>
            {
                let items = Array<string>();

                for (let i = 0; i < table.iterations; i++)
                {
                    let item = Evaluate(table.ID);

                    if (item == null) continue;

                    items.push(item);
                }

                return items;
            }
        }
    }
}