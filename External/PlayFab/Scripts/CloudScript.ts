// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)

handlers.OnLoggedIn = function (args, context: IPlayFabContext)
{
    return;

    API.Item.Grant(context.playerProfile.PlayerId, "Wood_Sword", 5, "Login Bonus");
    API.Item.Grant(context.playerProfile.PlayerId, "Wood_Shield", 5, "Login Bonus");
}

handlers.FinishLevel = function ($args)
{
    var args = {
        region: $args.region as string,
        level: $args.level as number,
    }

    var world = API.World.Retrieve();

    var region = world.FindRegion(args.region);

    if (region == null)
        return FormatError(args.region + " Region Doesn't Exist");

    var level = region.levels[args.level];

    if (level == null)
        return FormatError("Level Doesn't Exist");

    var IDs = Reward.Grant(currentPlayerId, level.reward, "Level Award");

    return IDs;
}

handlers.UpgradeItem = function ($args)
{
    var args = {
        itemInstanceID: $args.itemInstanceId as string,
        upgradeType: $args.upgradeType as string,
    }

    var inventory = API.Inventory.Retrieve(currentPlayerId);
    var itemInstance = inventory.FindWithInstanceID(args.itemInstanceID);

    if (itemInstance == null)
        return FormatError("Invalid Instance ID");

    var catalog = API.Catalog.Retrieve(itemInstance.CatalogVersion);
    var catalogItem = catalog.FindWithID(itemInstance.ItemId);

    var arguments = API.Upgrades.Arguments.Load(catalogItem);

    if (arguments == null)
        return FormatError("Current Item Can't Be Upgraded");

    var titleData = API.Title.Data.Retrieve([API.Upgrades.Name]);

    var template = API.Upgrades.Template.Find(titleData[API.Upgrades.Name], arguments.template);

    if (template == null)
        return FormatError(arguments.template + " Upgrades Template Not Defined");

    if (template.Find(args.upgradeType) == null)
        return FormatError(args.upgradeType + " Upgrade Type Not Defined");

    var data = API.Upgrades.Data.Load(itemInstance);
    if (data.Contains(args.upgradeType) == false) data.Add(args.upgradeType);

    if (data.Find(args.upgradeType).value >= template.Find(args.upgradeType).ranks.length)
        return FormatError("Maximum Upgrade Level Achieved");

    var rank = template.Match(args.upgradeType, data);

    if (rank.requirements != null)
    {
        if (inventory.CompliesWithRequirements(rank.requirements) == false)
            return FormatError("Player Doesn't The Required Items For the Upgrade");
    }

    if (inventory.virtualCurrency[rank.cost.type] < rank.cost.value)
        return FormatError("Insufficient Funds");

    //Validation Completed, Start Processing Request
    {
        API.Currency.Subtract(currentPlayerId, rank.cost.type, rank.cost.value);

        ItemRequirement.ConsumeAll(inventory, rank.requirements);

        data.Find(args.upgradeType).value++;

        API.Inventory.UpdateItemData(currentPlayerId, itemInstance.ItemInstanceId, API.Upgrades.Name, data.ToJson());
    }

    return "Success";
}

namespace API
{
    export namespace World
    {
        export const Name = "world";

        export function Retrieve(): Data
        {
            var titleData = API.Title.Data.Retrieve([Name]);

            var json = titleData[Name];

            var object = JSON.parse(json);

            var data = Object.assign(new Data(), object);

            return data;
        }

        export class Data
        {
            regions: Region.Data[];

            public FindRegion(name: string)
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return this.regions[i];

                return null;
            }
        }

        export namespace Region
        {
            export class Data
            {
                name: string;
                levels: Level.Data[];
            }
        }

        export namespace Level
        {
            export class Data
            {
                reward: Reward.Data;
            }
        }
    }

    export namespace Upgrades
    {
        export const Name = "upgrades";

        export namespace Data
        {
            export function Load(itemInstance: PlayFabServerModels.ItemInstance): Instance
            {
                var instance = new Instance;
                if (itemInstance.CustomData == null)
                {

                }
                else
                {
                    if (itemInstance.CustomData[Upgrades.Name] == null)
                    {

                    }
                    else
                    {
                        var object = JSON.parse(itemInstance.CustomData[Upgrades.Name]);

                        instance.Load(object);
                    }
                }

                return instance;
            }

            export class Instance
            {
                list: Element[];

                public Add(type: string)
                {
                    this.list.push(new Element(type, 0));
                }

                public Contains(type: string): boolean
                {
                    for (var i = 0; i < this.list.length; i++)
                        if (this.list[i].type == type)
                            return true;

                    return false;
                }

                public Find(type: string): Element
                {
                    for (var i = 0; i < this.list.length; i++)
                        if (this.list[i].type == type)
                            return this.list[i];

                    return null;
                }

                public Load(object: object)
                {
                    this.list = Object.assign([], object);
                }

                public ToJson(): string
                {
                    return JSON.stringify(this.list);
                }

                constructor()
                {
                    this.list = [];
                }
            }

            class Element
            {
                type: string;
                value: number;

                constructor(name: string, value: number)
                {
                    this.type = name;
                    this.value = value;
                }
            }
        }

        export namespace Arguments
        {
            export const Default = "Default";

            export function Load(catalogItem: PlayFabServerModels.CatalogItem): Instance
            {
                if (catalogItem == null) return null;

                if (catalogItem.CustomData == null) return null;

                var object = JSON.parse(catalogItem.CustomData);

                if (object[Name] == null)
                {

                }

                var data = Object.assign(new Instance(), object[Name]) as Instance;

                if (data.template == null) data.template = Default;

                return data;
            }

            export class Instance
            {
                template: string;
                applicable: string[];
            }
        }

        export namespace Template
        {
            export function Find(json: string, name: string): Instance
            {
                if (json == null) return null;

                if (name == null) return null;

                var object = JSON.parse(json);

                var target = object.find(x => x.name == name);

                var template = Object.assign(new Instance(), target);

                return template;
            }
            export function Parse(json: string): Instance
            {
                var object = JSON.parse(json);

                var instance = Object.assign(new Instance(), object);

                return instance;
            }

            export class Instance
            {
                name: string;
                elements: Element[];

                Find(name: string): Element
                {
                    for (var i = 0; i < this.elements.length; i++)
                        if (this.elements[i].type == name)
                            return this.elements[i];

                    return null;
                }

                Match(name: string, data: Upgrades.Data.Instance): Rank
                {
                    return this.Find(name).ranks[data.Find(name).value];
                }
            }

            export class Element
            {
                type: string;
                ranks: Rank[];
            }

            export class Rank
            {
                cost: Cost.Data;
                percentage: number;
                requirements: ItemRequirement.Data[]
            }
        }
    }

    export namespace Inventory
    {
        export function Retrieve(playerID: string): Data
        {
            var result = server.GetUserInventory(
                {
                    PlayFabId: playerID,
                }
            );

            return new Data(result.Inventory, result.VirtualCurrency);
        }

        export function Consume(playerID: string, itemInstanceID, count: number)
        {
            var result = server.ConsumeItem({
                PlayFabId: playerID,
                ItemInstanceId: itemInstanceID,
                ConsumeCount: count,
            });
        }

        export function UpdateItemData(playerID, itemInstanceID, key, value)
        {
            var data = {};

            data[key] = value;

            var request = server.UpdateUserInventoryItemCustomData({
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

            public CompliesWithRequirements(requirements: ItemRequirement.Data[]): boolean
            {
                for (let i = 0; i < requirements.length; i++)
                {
                    var instance = this.FindWithID(requirements[i].item);

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

    export namespace Catalog
    {
        export const Default = "Default";

        export function Retrieve(version: string): Data
        {
            var result = server.GetCatalogItems(
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
    }

    export namespace Currency
    {
        export function Subtract(playerID, currency: string, ammout: number)
        {
            var request = server.SubtractUserVirtualCurrency({
                PlayFabId: playerID,
                VirtualCurrency: currency,
                Amount: ammout
            });
        }
    }

    export namespace Title
    {
        export namespace Data
        {
            export function Retrieve(keys: string[])
            {
                var result = server.GetTitleData({
                    Keys: keys,
                })

                return result.Data;
            }
        }
    }

    export namespace Item
    {
        export function Grant(playerID: string, itemID, ammount: number, annotation: string, ): Array<PlayFabServerModels.ItemInstance>
        {
            var items = [];

            for (let i = 0; i < ammount; i++)
                items.push(itemID);

            return GrantAll(playerID, items, annotation);
        }

        export function GrantAll(playerID: string, itemIDs: string[], annotation: string): Array<PlayFabServerModels.ItemInstance>
        {
            if (itemIDs == null || itemIDs.length == 0) return [];

            var result = server.GrantItemsToUser({
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
            var result = server.EvaluateRandomResultTable({
                CatalogVersion: Catalog.Default,
                TableId: tableID
            });

            return result.ResultItemId;
        }

        export function Process(table: Reward.DropTable): Array<string>
        {
            var items = Array<string>();

            for (let i = 0; i < table.iterations; i++)
            {
                var item = Evaluate(table.ID);

                items.push(item);
            }

            return items;
        }
    }
}

//#region Types
namespace Reward
{
    export function Grant(playerID: string, data: Data, annotation: string): string[]
    {
        var IDs = Array<string>();

        IDs = IDs.concat(data.items);

        var result = API.Tables.Process(data.droptable);
        if (result != null)
            IDs = IDs.concat(result);

        API.Item.GrantAll(playerID, IDs, annotation);

        return IDs;
    }

    export class Data
    {
        items: [string];
        droptable: DropTable;
    }

    export class DropTable
    {
        ID: string;
        iterations: number;
    }
}

namespace Cost
{
    export class Data
    {
        type: string;
        value: number;
    }
}

namespace ItemRequirement
{
    export function ConsumeAll(inventory: API.Inventory.Data, requirements: Data[])
    {
        if (requirements == null) return;

        for (let i = 0; i < requirements.length; i++)
        {
            var itemInstance = inventory.FindWithID(requirements[i].item);

            API.Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, requirements[i].count);
        }
    }

    export class Data
    {
        item: string;
        count: number;
    }
}
//#endregion

function FormatError(message)
{
    return new Error(message);
}