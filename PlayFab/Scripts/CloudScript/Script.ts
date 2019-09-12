// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)

handlers.UpgradeItem = function ($args) {
    var args = {
        itemInstanceID: $args.ItemInstanceId,
        upgradeType: $args.UpgradeType,
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

    if (rank.requirements != null) {
        if (inventory.CompliesWithRequirements(rank.requirements) == false)
            return FormatError("Player Doesn't The Required Items For the Upgrade");
    }

    if (inventory.virtualCurrency[Upgrades.Currency] < rank.cost)
        return FormatError("Insufficient Funds");

    //Validation Completed, Start Processing Request
    {
        SubtractCurrency(currentPlayerId, Upgrades.Currency, rank.cost);

        if (rank.requirements != null) {
            for (let i = 0; i < rank.requirements.length; i++) {
                var itemInstance = inventory.FindWithID(rank.requirements[i].item);

                Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, rank.requirements[i].count);
            }
        }

        data.Find(args.upgradeType).value++;

        UpdateUserInventoryItemData(currentPlayerId, itemInstance.ItemInstanceId, Upgrades.Name, data.ToJson());
    }

    return { message: "Success" }
}

namespace Upgrades {
    export const Name = "upgrades";

    export const Currency = "JL";

    export namespace Data {
        export function Load(itemInstance: PlayFabServerModels.ItemInstance): Instance {
            if (itemInstance.CustomData == null) {
                return new Instance();
            }
            else {
                if (itemInstance.CustomData[Upgrades.Name] == null) {
                    return new Instance();
                }
                else {
                    var object = JSON.parse(itemInstance.CustomData[Upgrades.Name]);

                    var instance = new Instance();

                    instance.Load(object);

                    return instance;
                }
            }
        }

        export class Instance {
            list: Element[];

            public Add(type: string) {
                this.list.push(new Element(type, 0));
            }

            public Contains(type: string): boolean {
                for (var i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return true;

                return false;
            }

            public Find(type: string): Element {
                for (var i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return this.list[i];

                return null;
            }

            public Load(object: object) {
                this.list = Object.assign([], object);
            }

            public ToJson(): string {
                return JSON.stringify(this.list);
            }

            constructor() {
                this.list = [];
            }
        }

        class Element {
            type: string;
            value: number;

            public Increament(): void {
                this.value++;
            }

            constructor(name: string, value: number) {
                this.type = name;
                this.value = value;
            }
        }
    }

    export namespace Arguments {
        export const Default = "Default";

        export function Load(catalogItem: PlayFabServerModels.CatalogItem): Instance {
            if (catalogItem == null) return null;

            if (catalogItem.CustomData == null) return null;

            var object = JSON.parse(catalogItem.CustomData);

            if (object[Name] == null) {

            }

            var data = Object.assign(new Instance(), object[Name]) as Instance;

            if (data.template == null) data.template = Default;

            return data;
        }

        export class Instance {
            template: string;
            applicable: string[];
        }
    }

    export namespace Template {
        export function Find(json: string, name: string): Instance {
            if (json == null) return null;

            if (name == null) return null;

            var object = JSON.parse(json);

            var target = object.find(x => x.Name == name);

            var template = Object.assign(new Instance(), target);

            return template;
        }
        export function Parse(json: string): Instance {
            var object = JSON.parse(json);

            var instance = Object.assign(new Instance(), object);

            return instance;
        }

        export class Instance {
            name: string;
            elements: Element[];

            Find(name: string): Element {
                for (var i = 0; i < this.elements.length; i++)
                    if (this.elements[i].type == name)
                        return this.elements[i];

                return null;
            }

            Match(name: string, data: Upgrades.Data.Instance): Rank {
                return this.Find(name).ranks[data.Find(name).value];
            }
        }

        export class Element {
            type: string;
            ranks: Rank[];
        }

        export class Rank {
            cost: number;
            percentage: number;
            requirements: ItemRequirementData[]
        }
    }
}

class ItemRequirementData {
    item: string;
    count: number;
}

namespace Inventory {
    export function Retrieve(playerID: string): Data {
        var result = server.GetUserInventory(
            {
                PlayFabId: playerID,
            }
        );

        return new Data(result.Inventory, result.VirtualCurrency);
    }

    export function Consume(playerID: string, itemInstanceID, count: number) {
        var result = server.ConsumeItem({
            PlayFabId: playerID,
            ItemInstanceId: itemInstanceID,
            ConsumeCount: count,
        });
    }

    export class Data {
        items: PlayFabServerModels.ItemInstance[];
        virtualCurrency: { [key: string]: number };

        public FindWithID(itemID: string): PlayFabServerModels.ItemInstance {
            for (let i = 0; i < this.items.length; i++)
                if (this.items[i].ItemId == itemID)
                    return this.items[i];

            return null;
        }
        public FindWithInstanceID(itemInstanceID: string): PlayFabServerModels.ItemInstance {
            for (let i = 0; i < this.items.length; i++)
                if (this.items[i].ItemInstanceId == itemInstanceID)
                    return this.items[i];

            return null;
        }

        public CompliesWithRequirements(requirements: ItemRequirementData[]): boolean {
            for (let i = 0; i < requirements.length; i++) {
                var instance = this.FindWithID(requirements[i].item);

                if (instance == null) return false;

                if (instance.RemainingUses < requirements[i].count) return false;
            }

            return true;
        }

        constructor(Items: PlayFabServerModels.ItemInstance[], VirtualCurrency: { [key: string]: number }) {
            this.items = Items;
            this.virtualCurrency = VirtualCurrency;
        }
    }
}

namespace Catalog {
    export function Retrieve(version: string): Data {
        var result = server.GetCatalogItems(
            {
                CatalogVersion: version,
            }
        );

        return new Data(result.Catalog);
    }

    export class Data {
        items: PlayFabServerModels.CatalogItem[];

        public FindWithID(itemID: string): PlayFabServerModels.CatalogItem {
            for (let i = 0; i < this.items.length; i++)
                if (this.items[i].ItemId == itemID)
                    return this.items[i];

            return null;
        }

        constructor(Items: PlayFabServerModels.CatalogItem[]) {
            this.items = Items;
        }
    }
}

function GetTitleData() {
    var result = server.GetTitleData({
        Keys: [Upgrades.Name],
    })

    return result.Data;
}

function SubtractCurrency(playerID, currency, ammout) {
    var request = server.SubtractUserVirtualCurrency({
        PlayFabId: playerID,
        VirtualCurrency: currency,
        Amount: ammout
    });
}

function UpdateUserInventoryItemData(playerID, itemInstanceID, key, value) {
    var data = {};

    data[key] = value;

    var request = server.UpdateUserInventoryItemCustomData({
        PlayFabId: playerID,
        ItemInstanceId: itemInstanceID,
        Data: data
    });
}

function FormatError(message) {
    return message;
}