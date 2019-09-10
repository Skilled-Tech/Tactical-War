// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)
handlers.UpgradeItem = function (args) {
    var arg = {
        itemInstanceID: args.ItemInstanceId,
        upgradeType: args.UpgradeType,
    };
    var inventory = GetInventory(currentPlayerId);
    var itemInstance = inventory.Items.find(x => x.ItemInstanceId === arg.itemInstanceID);
    if (itemInstance == null)
        return FormatError("Invalid Instance ID");
    var catalog = GetCatalog(itemInstance.CatalogVersion);
    var catalogItem = catalog.Items.find(x => x.ItemId == itemInstance.ItemId);
    var arguments = Upgrades.Arguments.Load(catalogItem);
    if (arguments == null)
        return FormatError("Current Item Can't Be Upgraded");
    var titleData = GetTitleData();
    var template = Upgrades.Template.Find(titleData[Upgrades.Name], arguments.Template);
    if (template == null)
        return FormatError(arguments.Template + " Upgrades Template Not Defined");
    if (template.Find(arg.upgradeType) == null)
        return FormatError(arg.upgradeType + " Upgrade Type Not Defined");
    var data = Upgrades.Data.Load(itemInstance);
    if (data.Contains(arg.upgradeType) == false)
        data.Add(arg.upgradeType);
    if (data.Find(arg.upgradeType).Value >= template.Find(arg.upgradeType).Ranks.length)
        return FormatError("Maximum Upgrade Level Achieved");
    var rank = template.Match(arg.upgradeType, data);
    if (inventory.VirtualCurrency[Upgrades.Currency] < rank.Cost)
        return FormatError("Insufficient Funds");
    SubtractCurrency(currentPlayerId, Upgrades.Currency, rank.Cost);
    data.Find(arg.upgradeType).Value++;
    UpdateUserInventoryItemData(currentPlayerId, itemInstance.ItemInstanceId, Upgrades.Name, data.ToJson());
    return { message: "Success" };
};
var Upgrades;
(function (Upgrades) {
    Upgrades.Name = "Upgrades";
    Upgrades.Currency = "JL";
    let Data;
    (function (Data) {
        function Load(itemInstance) {
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
        Data.Load = Load;
        class Instance {
            constructor() {
                this.List = [];
            }
            Add(type) {
                this.List.push(new Element(type, 0));
            }
            Contains(type) {
                for (var i = 0; i < this.List.length; i++)
                    if (this.List[i].Type == type)
                        return true;
                return false;
            }
            Find(type) {
                for (var i = 0; i < this.List.length; i++)
                    if (this.List[i].Type == type)
                        return this.List[i];
                return null;
            }
            Load(object) {
                this.List = Object.assign([], object);
            }
            ToJson() {
                return JSON.stringify(this.List);
            }
        }
        Data.Instance = Instance;
        class Element {
            constructor(name, value) {
                this.Type = name;
                this.Value = value;
            }
            Increament() {
                this.Value++;
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
            if (data.Template == null)
                data.Template = Arguments.Default;
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
            var target = object.find(x => x.Name == name);
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
                for (var i = 0; i < this.Elements.length; i++)
                    if (this.Elements[i].Type == name)
                        return this.Elements[i];
                return null;
            }
            Match(name, data) {
                return this.Find(name).Ranks[data.Find(name).Value];
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
function GetInventory(playerID) {
    var result = server.GetUserInventory({
        PlayFabId: playerID,
    });
    return {
        Items: result.Inventory,
        VirtualCurrency: result.VirtualCurrency
    };
}
function GetCatalog(version) {
    var result = server.GetCatalogItems({
        CatalogVersion: version,
    });
    return {
        Items: result.Catalog,
    };
}
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