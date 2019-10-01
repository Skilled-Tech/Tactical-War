namespace PlayFab
{
    export namespace Player
    {
        export namespace Inventory
        {
            export function Retrieve(playerID: string): Data
            {
                let result = server.GetUserInventory(
                    {
                        PlayFabId: playerID,
                    }
                );

                return new Data(result.Inventory, result.VirtualCurrency);
            }

            export function Consume(playerID: string, itemInstanceID, count: number)
            {
                let result = server.ConsumeItem({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    ConsumeCount: count,
                });
            }

            export function ConsumeAll(inventory: PlayFab.Player.Inventory.Data, stacks: API.ItemStack[])
            {
                if (stacks == null) return;

                for (let i = 0; i < stacks.length; i++)
                {
                    let itemInstance = inventory.FindWithID(stacks[i].item);

                    PlayFab.Player.Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, stacks[i].count);
                }
            }

            export function UpdateItemData(playerID, itemInstanceID, key, value)
            {
                let data = {};

                data[key] = value;

                let request = server.UpdateUserInventoryItemCustomData({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    Data: data
                });
            }

            export class Data
            {
                items: PlayFabServerModels.ItemInstance[];
                virtualCurrency: { [key: string]: number };

                public FindWithID(itemID: string): PlayFabServerModels.ItemInstance
                {
                    for (let i = 0; i < this.items.length; i++)
                        if (this.items[i].ItemId == itemID)
                            return this.items[i];

                    return null;
                }
                public FindWithInstanceID(itemInstanceID: string): PlayFabServerModels.ItemInstance
                {
                    for (let i = 0; i < this.items.length; i++)
                        if (this.items[i].ItemInstanceId == itemInstanceID)
                            return this.items[i];

                    return null;
                }

                public CompliesWithRequirements(requirements: API.ItemStack[]): boolean
                {
                    for (let i = 0; i < requirements.length; i++)
                    {
                        let instance = this.FindWithID(requirements[i].item);

                        if (instance == null) return false;

                        if (instance.RemainingUses < requirements[i].count) return false;
                    }

                    return true;
                }

                constructor(Items: PlayFabServerModels.ItemInstance[], VirtualCurrency: { [key: string]: number })
                {
                    this.items = Items;
                    this.virtualCurrency = VirtualCurrency;
                }
            }
        }

        export namespace Currency
        {
            export function Subtract(playerID, currency: string, ammout: number)
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

                export function Write(playerID: string, key: string, value: string)
                {
                    let data = {};

                    data[key] = value;

                    server.UpdateUserReadOnlyData({
                        PlayFabId: playerID,
                        Data: data,
                    })
                }
            }
        }
    }
}