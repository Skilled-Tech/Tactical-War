// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)
handlers.OnLoggedIn = function (args, context) {
    return;
    API.Item.Grant(context.playerProfile.PlayerId, "Wood_Sword", 5, "Login Bonus");
    API.Item.Grant(context.playerProfile.PlayerId, "Wood_Shield", 5, "Login Bonus");
};
handlers.FinishLevel = function ($args) {
    var args = {
        region: $args.region,
        level: $args.level,
    };
    var world = API.World.Retrieve();
    var region = world.FindRegion(args.region);
    if (region == null)
        return FormatError(args.region + " Region Doesn't Exist");
    var level = region.levels[args.level];
    if (level == null)
        return FormatError("Level Doesn't Exist");
    var IDs = Reward.Grant(currentPlayerId, level.reward, "Level Award");
    return IDs;
};
handlers.UpgradeItem = function ($args) {
    var args = {
        itemInstanceID: $args.itemInstanceId,
        upgradeType: $args.upgradeType,
    };
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
    if (data.Contains(args.upgradeType) == false)
        data.Add(args.upgradeType);
    if (data.Find(args.upgradeType).value >= template.Find(args.upgradeType).ranks.length)
        return FormatError("Maximum Upgrade Level Achieved");
    var rank = template.Match(args.upgradeType, data);
    if (rank.requirements != null) {
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
};
var API;
(function (API) {
    let World;
    (function (World) {
        World.Name = "world";
        function Retrieve() {
            var titleData = API.Title.Data.Retrieve([World.Name]);
            var json = titleData[World.Name];
            var object = JSON.parse(json);
            var data = Object.assign(new Data(), object);
            return data;
        }
        World.Retrieve = Retrieve;
        class Data {
            FindRegion(name) {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return this.regions[i];
                return null;
            }
        }
        World.Data = Data;
        let Region;
        (function (Region) {
            class Data {
            }
            Region.Data = Data;
        })(Region = World.Region || (World.Region = {}));
        let Level;
        (function (Level) {
            class Data {
            }
            Level.Data = Data;
        })(Level = World.Level || (World.Level = {}));
    })(World = API.World || (API.World = {}));
    let Upgrades;
    (function (Upgrades) {
        Upgrades.Name = "upgrades";
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
    })(Upgrades = API.Upgrades || (API.Upgrades = {}));
    let Inventory;
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
        function UpdateItemData(playerID, itemInstanceID, key, value) {
            var data = {};
            data[key] = value;
            var request = server.UpdateUserInventoryItemCustomData({
                PlayFabId: playerID,
                ItemInstanceId: itemInstanceID,
                Data: data
            });
        }
        Inventory.UpdateItemData = UpdateItemData;
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
    })(Inventory = API.Inventory || (API.Inventory = {}));
    let Catalog;
    (function (Catalog) {
        Catalog.Default = "Default";
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
    })(Catalog = API.Catalog || (API.Catalog = {}));
    let Currency;
    (function (Currency) {
        function Subtract(playerID, currency, ammout) {
            var request = server.SubtractUserVirtualCurrency({
                PlayFabId: playerID,
                VirtualCurrency: currency,
                Amount: ammout
            });
        }
        Currency.Subtract = Subtract;
    })(Currency = API.Currency || (API.Currency = {}));
    let Title;
    (function (Title) {
        let Data;
        (function (Data) {
            function Retrieve(keys) {
                var result = server.GetTitleData({
                    Keys: keys,
                });
                return result.Data;
            }
            Data.Retrieve = Retrieve;
        })(Data = Title.Data || (Title.Data = {}));
    })(Title = API.Title || (API.Title = {}));
    let Item;
    (function (Item) {
        function Grant(playerID, itemID, ammount, annotation) {
            var items = [];
            for (let i = 0; i < ammount; i++)
                items.push(itemID);
            return GrantAll(playerID, items, annotation);
        }
        Item.Grant = Grant;
        function GrantAll(playerID, itemIDs, annotation) {
            if (itemIDs == null || itemIDs.length == 0)
                return [];
            var result = server.GrantItemsToUser({
                PlayFabId: playerID,
                Annotation: annotation,
                CatalogVersion: Catalog.Default,
                ItemIds: itemIDs
            });
            return result.ItemGrantResults;
        }
        Item.GrantAll = GrantAll;
    })(Item = API.Item || (API.Item = {}));
    let Tables;
    (function (Tables) {
        function Evaluate(tableID) {
            var result = server.EvaluateRandomResultTable({
                CatalogVersion: Catalog.Default,
                TableId: tableID
            });
            return result.ResultItemId;
        }
        Tables.Evaluate = Evaluate;
        function Process(table) {
            var items = Array();
            for (let i = 0; i < table.iterations; i++) {
                var item = Evaluate(table.ID);
                items.push(item);
            }
            return items;
        }
        Tables.Process = Process;
    })(Tables = API.Tables || (API.Tables = {}));
})(API || (API = {}));
//#region Types
var Reward;
(function (Reward) {
    function Grant(playerID, data, annotation) {
        var IDs = Array();
        IDs = IDs.concat(data.items);
        var result = API.Tables.Process(data.droptable);
        if (result != null)
            IDs = IDs.concat(result);
        API.Item.GrantAll(playerID, IDs, annotation);
        return IDs;
    }
    Reward.Grant = Grant;
    class Data {
    }
    Reward.Data = Data;
    class DropTable {
    }
    Reward.DropTable = DropTable;
})(Reward || (Reward = {}));
var Cost;
(function (Cost) {
    class Data {
    }
    Cost.Data = Data;
})(Cost || (Cost = {}));
var ItemRequirement;
(function (ItemRequirement) {
    function ConsumeAll(inventory, requirements) {
        if (requirements == null)
            return;
        for (let i = 0; i < requirements.length; i++) {
            var itemInstance = inventory.FindWithID(requirements[i].item);
            API.Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, requirements[i].count);
        }
    }
    ItemRequirement.ConsumeAll = ConsumeAll;
    class Data {
    }
    ItemRequirement.Data = Data;
})(ItemRequirement || (ItemRequirement = {}));
//#endregion
function FormatError(message) {
    return new Error(message);
}
