// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)

handlers.OnLoggedIn = function (args, context: IPlayFabContext)
{
    GrantItem(context.playerProfile.PlayerId, "Wood_Sword", Catalog.Default, 5, "Login Bonus");
    GrantItem(context.playerProfile.PlayerId, "Wood_Shield", Catalog.Default, 5, "Login Bonus");
}

handlers.UpgradeItem = function (_args_)
{
    var args = {
        itemInstanceID: _args_.itemInstanceId,
        upgradeType: _args_.upgradeType,
    }

    var inventory = Inventory.Retrieve(currentPlayerId);
    var itemInstance = inventory.FindWithInstanceID(args.itemInstanceID);

    if (itemInstance == null)
        return FormatError("Invalid Instance ID");

    var catalog = Catalog.Retrieve(itemInstance.CatalogVersion);
    var catalogItem = catalog.FindWithID(itemInstance.ItemId);

    var arguments = Upgrades.Arguments.Load(catalogItem);

    if (arguments == null)
        return FormatError("Current Item Can't Be Upgraded");

    var titleData = GetTitleData();

    var template = Upgrades.Template.Find(titleData[Upgrades.Name], arguments.template);

    if (template == null)
        return FormatError(arguments.template + " Upgrades Template Not Defined");

    if (template.Find(args.upgradeType) == null)
        return FormatError(args.upgradeType + " Upgrade Type Not Defined");

    var data = Upgrades.Data.Load(itemInstance);
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
        SubtractCurrency(currentPlayerId, rank.cost.type, rank.cost.value);

        ItemRequirement.ConsumeAll(inventory, rank.requirements);

        data.Find(args.upgradeType).value++;

        Inventory.UpdateItemData(currentPlayerId, itemInstance.ItemInstanceId, Upgrades.Name, data.ToJson());
    }

    return { message: "Success" }
}

namespace Upgrades
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
    export function ConsumeAll(inventory: Inventory.Data, requirements: Data[])
    {
        if (requirements == null) return;

        for (let i = 0; i < requirements.length; i++)
        {
            var itemInstance = inventory.FindWithID(requirements[i].item);

            Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, requirements[i].count);
        }
    }

    export class Data
    {
        item: string;
        count: number;
    }
}

namespace Inventory
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

namespace Catalog
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

function GetTitleData()
{
    var result = server.GetTitleData({
        Keys: [Upgrades.Name],
    })

    return result.Data;
}

function SubtractCurrency(playerID, currency: string, ammout: number)
{
    var request = server.SubtractUserVirtualCurrency({
        PlayFabId: playerID,
        VirtualCurrency: currency,
        Amount: ammout
    });
}

function GrantItem(playerID: string, itemID, catalogVersion: string, ammount: number, annotation: string, )
{
    var items = [];

    for (let i = 0; i < ammount; i++)
        items.push(itemID);

    server.GrantItemsToUser({
        PlayFabId: playerID,
        Annotation: annotation,
        CatalogVersion: catalogVersion,
        ItemIds: items,
    });
}

function FormatError(message)
{
    return new Error(message);
}