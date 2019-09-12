// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)
handlers.UpgradeItem = function ($args) {
    var args = {
        itemInstanceID: $args.ItemInstanceId,
        upgradeType: $args.UpgradeType,
    };
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
    if (data.Contains(args.upgradeType) == false)
        data.Add(args.upgradeType);
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
        ItemRequirement.ConsumeAll(inventory, rank.requirements);
        data.Find(args.upgradeType).value++;
        UpdateUserInventoryItemData(currentPlayerId, itemInstance.ItemInstanceId, Upgrades.Name, data.ToJson());
    }
    return { message: "Success" };
};
var Upgrades;
(function (Upgrades) {
    Upgrades.Name = "upgrades";
    Upgrades.Currency = "JL";
    let Data;
    (function (Data) {
        function Load(itemInstance) {
            var instance = new Instance;
            if (itemInstance.CustomData == null) {
            }
            else {
                if (itemInstance.CustomData[Upgrades.Name] == null) {
                }
                else {
                    var object = JSON.parse(itemInstance.CustomData[Upgrades.Name]);
                    instance.Load(object);
                }
            }
            return instance;
        }
        Data.Load = Load;
        class Instance {
            constructor() {
                this.list = [];
            }
            Add(type) {
                this.list.push(new Element(type, 0));
            }
            Contains(type) {
                for (var i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return true;
                return false;
            }
            Find(type) {
                for (var i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return this.list[i];
                return null;
            }
            Load(object) {
                this.list = Object.assign([], object);
            }
            ToJson() {
                return JSON.stringify(this.list);
            }
        }
        Data.Instance = Instance;
        class Element {
            constructor(name, value) {
                this.type = name;
                this.value = value;
            }
        }
    })(Data = Upgrades.Data || (Upgrades.Data = {}));
    let Arguments;
    (function (Arguments) {
        Arguments.Default = "Default";
        function Load(catalogItem) {
            if (catalogItem == null)
                return null;
            if (catalogItem.CustomData == null)
                return null;
            var object = JSON.parse(catalogItem.CustomData);
            if (object[Upgrades.Name] == null) {
            }
            var data = Object.assign(new Instance(), object[Upgrades.Name]);
            if (data.template == null)
                data.template = Arguments.Default;
            return data;
        }
        Arguments.Load = Load;
        class Instance {
        }
        Arguments.Instance = Instance;
    })(Arguments = Upgrades.Arguments || (Upgrades.Arguments = {}));
    let Template;
    (function (Template) {
        function Find(json, name) {
            if (json == null)
                return null;
            if (name == null)
                return null;
            var object = JSON.parse(json);
            var target = object.find(x => x.name == name);
            var template = Object.assign(new Instance(), target);
            return template;
        }
        Template.Find = Find;
        function Parse(json) {
            var object = JSON.parse(json);
            var instance = Object.assign(new Instance(), object);
            return instance;
        }
        Template.Parse = Parse;
        class Instance {
            Find(name) {
                for (var i = 0; i < this.elements.length; i++)
                    if (this.elements[i].type == name)
                        return this.elements[i];
                return null;
            }
            Match(name, data) {
                return this.Find(name).ranks[data.Find(name).value];
            }
        }
        Template.Instance = Instance;
        class Element {
        }
        Template.Element = Element;
        class Rank {
        }
        Template.Rank = Rank;
    })(Template = Upgrades.Template || (Upgrades.Template = {}));
})(Upgrades || (Upgrades = {}));
var ItemRequirement;
(function (ItemRequirement) {
    function ConsumeAll(inventory, requirements) {
        if (requirements == null)
            return;
        for (let i = 0; i < requirements.length; i++) {
            var itemInstance = inventory.FindWithID(requirements[i].item);
            Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, requirements[i].count);
        }
    }
    ItemRequirement.ConsumeAll = ConsumeAll;
    class Data {
    }
    ItemRequirement.Data = Data;
})(ItemRequirement || (ItemRequirement = {}));
var Inventory;
(function (Inventory) {
    function Retrieve(playerID) {
        var result = server.GetUserInventory({
            PlayFabId: playerID,
        });
        return new Data(result.Inventory, result.VirtualCurrency);
    }
    Inventory.Retrieve = Retrieve;
    function Consume(playerID, itemInstanceID, count) {
        var result = server.ConsumeItem({
            PlayFabId: playerID,
            ItemInstanceId: itemInstanceID,
            ConsumeCount: count,
        });
    }
    Inventory.Consume = Consume;
    class Data {
        constructor(Items, VirtualCurrency) {
            this.items = Items;
            this.virtualCurrency = VirtualCurrency;
        }
        FindWithID(itemID) {
            for (let i = 0; i < this.items.length; i++)
                if (this.items[i].ItemId == itemID)
                    return this.items[i];
            return null;
        }
        FindWithInstanceID(itemInstanceID) {
            for (let i = 0; i < this.items.length; i++)
                if (this.items[i].ItemInstanceId == itemInstanceID)
                    return this.items[i];
            return null;
        }
        CompliesWithRequirements(requirements) {
            for (let i = 0; i < requirements.length; i++) {
                var instance = this.FindWithID(requirements[i].item);
                if (instance == null)
                    return false;
                if (instance.RemainingUses < requirements[i].count)
                    return false;
            }
            return true;
        }
    }
    Inventory.Data = Data;
})(Inventory || (Inventory = {}));
var Catalog;
(function (Catalog) {
    function Retrieve(version) {
        var result = server.GetCatalogItems({
            CatalogVersion: version,
        });
        return new Data(result.Catalog);
    }
    Catalog.Retrieve = Retrieve;
    class Data {
        constructor(Items) {
            this.items = Items;
        }
        FindWithID(itemID) {
            for (let i = 0; i < this.items.length; i++)
                if (this.items[i].ItemId == itemID)
                    return this.items[i];
            return null;
        }
    }
    Catalog.Data = Data;
})(Catalog || (Catalog = {}));
function GetTitleData() {
    var result = server.GetTitleData({
        Keys: [Upgrades.Name],
    });
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
//# sourceMappingURL=Script.js.map