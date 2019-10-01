namespace PlayFab
{
    export namespace Player
    {
        export class Inventory
        {
            items: PlayFabServerModels.ItemInstance[];
            virtualCurrency: { [key: string]: number } | undefined;

            public FindWithID(itemID: string): PlayFabServerModels.ItemInstance | null
            {
                for (let i = 0; i < this.items.length; i++)
                    if (this.items[i].ItemId == itemID)
                        return this.items[i];

                return null;
            }
            public FindWithInstanceID(itemInstanceID: string): PlayFabServerModels.ItemInstance | null
            {
                for (let i = 0; i < this.items.length; i++)
                    if (this.items[i].ItemInstanceId == itemInstanceID)
                        return this.items[i];

                return null;
            }

            public CompliesWith(requirements: API.ItemStack[]): boolean
            {
                for (let i = 0; i < requirements.length; i++)
                {
                    let instance = this.FindWithID(requirements[i].item);

                    if (instance == null) return false;

                    if (instance.RemainingUses == null)
                    {
                        if (requirements[i].count > 1)
                            return false;
                        else
                            continue;
                    }

                    if (instance.RemainingUses < requirements[i].count) return false;
                }

                return true;
            }

            constructor(items?: PlayFabServerModels.ItemInstance[], virtualCurrency?: { [key: string]: number })
            {
                if (items == null)
                    this.items = [];
                else
                    this.items = items;

                this.virtualCurrency = virtualCurrency;
            }
        }
        export namespace Inventory
        {
            export function Retrieve(playerID: string): Inventory
            {
                let result = server.GetUserInventory(
                    {
                        PlayFabId: playerID,
                    }
                );

                return new Inventory(result.Inventory, result.VirtualCurrency);
            }

            export function Consume(playerID: string, itemInstanceID: string, count: number): PlayFabServerModels.ConsumeItemResult
            {
                let result = server.ConsumeItem({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    ConsumeCount: count,
                });

                return result;
            }

            export function ConsumeAll(inventory: PlayFab.Player.Inventory, stacks: API.ItemStack[]): Array<PlayFabServerModels.ConsumeItemResult>
            {
                let results = Array<PlayFabServerModels.ConsumeItemResult>();

                if (stacks == null) return results;

                for (let i = 0; i < stacks.length; i++)
                {
                    let itemInstance = inventory.FindWithID(stacks[i].item);

                    if (itemInstance == null)
                    {
                        log.info("item with ID " + stacks[i].item + " not found in inventory");
                        continue;
                    }

                    var itemInstanceID = itemInstance.ItemInstanceId;

                    if (itemInstanceID == null)
                    {
                        log.info("itemInstance doesn't have an itemInstanceID, what the heck ?");
                        continue;
                    }

                    let result = PlayFab.Player.Inventory.Consume(currentPlayerId, itemInstanceID, stacks[i].count);

                    results.push(result);
                }

                return results;
            }

            export function UpdateItemData(playerID: string, itemInstanceID: string, key: string, value: string)
            {
                let data = {};

                Utility.Class.WriteProperty(data, key, value);

                let response = server.UpdateUserInventoryItemCustomData({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    Data: data
                });
            }
        }

        export namespace Currency
        {
            export function Subtract(playerID: string, currency: string, ammout: number)
            {
                let request = server.SubtractUserVirtualCurrency({
                    PlayFabId: playerID,
                    VirtualCurrency: currency,
                    Amount: ammout
                });
            }
        }

        export namespace Data
        {
            export namespace ReadOnly
            {
                export function ReadAll(playerID: string, keys: string[]): PlayFabServerModels.GetUserDataResult
                {
                    let result = server.GetUserReadOnlyData({
                        PlayFabId: playerID,
                        Keys: keys
                    });

                    return result;
                }

                export function Read(playerID: string, key: string): PlayFabServerModels.GetUserDataResult
                {
                    var result = ReadAll(playerID, [key]);

                    return result;
                }

                export function Write(playerID: string, key: string, value: string): PlayFabServerModels.UpdateUserDataResult
                {
                    let data = {};

                    Utility.Class.WriteProperty(data, key, value);

                    var result = server.UpdateUserReadOnlyData({
                        PlayFabId: playerID,
                        Data: data,
                    });

                    return result;
                }
            }
        }
    }
}