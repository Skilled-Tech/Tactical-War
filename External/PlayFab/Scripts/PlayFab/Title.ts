namespace PlayFab
{
    export namespace Title
    {
        export namespace Data
        {
            export function RetrieveAll(keys: string[]): PlayFabServerModels.GetTitleDataResult
            {
                let result = server.GetTitleData({
                    Keys: keys,
                })

                return result;
            }

            export function Retrieve(key: string): string | null
            {
                var result = RetrieveAll([key]);

                if (result.Data == null) return null;

                if (result.Data[key] == null) return null;

                return result.Data[key];
            }
        }

        export namespace Catalog
        {
            export const Default = "Default";

            export function Retrieve(version: string): Data
            {
                let result = server.GetCatalogItems(
                    {
                        CatalogVersion: version,
                    }
                );

                return new Data(result.Catalog);
            }

            export class Data
            {
                items: PlayFabServerModels.CatalogItem[];

                public FindWithID(itemID: string): PlayFabServerModels.CatalogItem
                {
                    for (let i = 0; i < this.items.length; i++)
                        if (this.items[i].ItemId == itemID)
                            return this.items[i];

                    return null;
                }

                constructor(Items: PlayFabServerModels.CatalogItem[])
                {
                    this.items = Items;
                }
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

                    return result.ItemGrantResults;
                }
            }

            export namespace Tables
            {
                export function Evaluate(tableID: string): string
                {
                    let result = server.EvaluateRandomResultTable({
                        CatalogVersion: Catalog.Default,
                        TableId: tableID
                    });

                    return result.ResultItemId;
                }

                export function Process(table: API.DropTable): Array<string>
                {
                    let items = Array<string>();

                    for (let i = 0; i < table.iterations; i++)
                    {
                        let item = Evaluate(table.ID);

                        items.push(item);
                    }

                    return items;
                }
            }
        }
    }
}