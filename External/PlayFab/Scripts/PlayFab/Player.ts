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
                    if (CompareIgnoreCase(this.items[i].ItemId, itemID))
                        return this.items[i];

                return null;
            }
            public FindWithInstanceID(itemInstanceID: string): PlayFabServerModels.ItemInstance | null
            {
                for (let i = 0; i < this.items.length; i++)
                    if (CompareIgnoreCase(this.items[i].ItemInstanceId, itemInstanceID))
                        return this.items[i];

                return null;
            }

            public Contains(itemID: string): boolean
            {
                if (this.FindWithID(itemID) == null) return false;

                return true;
            }

            public CompliesWith(requirement: API.ItemStack): boolean
            {
                if (requirement == null) return true;

                let instance = this.FindWithID(requirement.item);

                if (instance == null) return false;

                if (instance.RemainingUses == null)
                {
                    if (requirement.count > 1)
                        return false;
                    else
                        return true;
                }

                return instance.RemainingUses >= requirement.count;
            }
            public CompliesWithAll(requirements: API.ItemStack[]): boolean
            {
                if (requirements == null || requirements.length == 0) return true;

                for (let i = 0; i < requirements.length; i++)
                    if (this.CompliesWith(requirements[i]) == false)
                        return false;

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
                        log.info("item with ID " + stacks[i].item + " not found in inventory, cannot consume, skipping");
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

                export function Read(playerID: string, key: string): string | null
                {
                    let result = ReadAll(playerID, [key]);

                    if (result.Data == null) return null;

                    if (result.Data[key] == null) return null;

                    let value = result.Data[key].Value;
                    if (value == null) return null;

                    return value;
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