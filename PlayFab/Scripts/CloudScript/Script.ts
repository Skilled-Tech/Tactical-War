// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)

handlers.OnLoggedIn = function (context, args)
{

}

function GrantItem(playerID: string, annotation: string, itemID: string, ammount: number)
{
    var inventory = Inventory.Retrieve(currentPlayerId);

    var itemInstance = inventory.FindWithID(itemID);

    if (itemInstance == null)
    {
        var result = server.GrantItemsToUser({
            PlayFabId: playerID,
            Annotation: "Loggin Bonus",
            CatalogVersion: Catalog.Default,
            ItemIds: [itemID],
        });

        itemInstance = result.ItemGrantResults[0];
    }

    server.ModifyItemUses({
        PlayFabId: playerID,
        ItemInstanceId: itemInstance.ItemInstanceId,
        UsesToAdd: itemInstance.RemainingUses + ammount,
    });
}

handlers.UpgradeItem = function (args)
{
    var argggggs = {
        itemInstanceID: args.itemInstanceId,
        upgradeType: args.upgradeType,
    }

    log.info(currentPlayerId);
    log.info(argggggs.itemInstanceID);
    log.info(argggggs.upgradeType);

    var inventory = Inventory.Retrieve(currentPlayerId);
    var itemInstance = inventory.FindWithInstanceID(argggggs.itemInstanceID);

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

    if (template.Find(argggggs.upgradeType) == null)
        return FormatError(argggggs.upgradeType + " Upgrade Type Not Defined");

    var data = Upgrades.Data.Load(itemInstance);
    if (data.Contains(argggggs.upgradeType) == false) data.Add(argggggs.upgradeType);

    if (data.Find(argggggs.upgradeType).value >= template.Find(argggggs.upgradeType).ranks.length)
        return FormatError("Maximum Upgrade Level Achieved");

    var rank = template.Match(argggggs.upgradeType, data);

    if (rank.requirements != null)
    {
        if (inventory.CompliesWithRequirements(rank.requirements) == false)
            return FormatError("Player Doesn't The Required Items For the Upgrade");
    }

    if (inventory.virtualCurrency[Upgrades.Currency] < rank.cost)
        return FormatError("Insufficient Funds");

    //Validation Completed, Start Processing Request
    {
        SubtractCurrency(currentPlayerId, Upgrades.Currency, rank.cost);

        ItemRequirement.ConsumeAll(inventory, rank.requirements);

        data.Find(argggggs.upgradeType).value++;

        UpdateUserInventoryItemData(currentPlayerId, itemInstance.ItemInstanceId, Upgrades.Name, data.ToJson());
    }

    return { message: "Success" }
}

namespace Upgrades
{
    export const Name = "upgrades";

    export const Currency = "JL";

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
            cost: number;
            percentage: number;
            requirements: ItemRequirement.Data[]
        }
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

function SubtractCurrency(playerID, currency, ammout)
{
    var request = server.SubtractUserVirtualCurrency({
        PlayFabId: playerID,
        VirtualCurrency: currency,
        Amount: ammout
    });
}

function UpdateUserInventoryItemData(playerID, itemInstanceID, key, value)
{
    var data = {};

    data[key] = value;

    var request = server.UpdateUserInventoryItemCustomData({
        PlayFabId: playerID,
        ItemInstanceId: itemInstanceID,
        Data: data
    });
}

function FormatError(message)
{
    return message;
}