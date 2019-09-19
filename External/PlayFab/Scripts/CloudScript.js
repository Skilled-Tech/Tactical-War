// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)
handlers.OnLoggedIn = function (args, context) {
    return;
    PlayFab.Title.Catalog.Item.Grant(context.playerProfile.PlayerId, "Wood_Sword", 5, "Login Bonus");
    PlayFab.Title.Catalog.Item.Grant(context.playerProfile.PlayerId, "Wood_Shield", 5, "Login Bonus");
};
handlers.FinishLevel = function (args) {
    let world = API.World.World.Retrieve();
    try {
        API.World.World.Validate(world, args.region, args.level);
    }
    catch (error) {
        log.error(error);
        return;
    }
    let data = API.World.Data.Retrieve(currentPlayerId);
    try {
        API.World.Data.Validate(data, world, args.region, args.level);
    }
    catch (error) {
        log.error(error);
        return;
    }
    let progress = data.Find(args.region).progress;
    log.info(progress.toString());
    if (args.level >= progress) //cheating
     {
        log.error("trying to finish level " + args.level + " without finishing the previous level ");
        return;
    }
    let IDs = [];
    var level = world.Find(args.region).levels[args.level];
    if (args.level == progress - 1) //Initial
     {
        var text = "initial/n";
        text += JSON.stringify(level.reward.initial);
        log.info(text);
        data.Find(args.region).progress++;
        PlayFab.Player.Data.ReadOnly.Write(currentPlayerId, API.World.Name, JSON.stringify(data));
        let items = API.Reward.Grant(currentPlayerId, level.reward.initial, "Level Completing Award");
        IDs = IDs.concat(items);
    }
    if (args.level < progress - 1) //Recurring
     {
        var text = "recurring/n";
        text += JSON.stringify(level.reward.constant);
        log.info(text);
        let items = API.Reward.Grant(currentPlayerId, level.reward.constant, "Level Completing Award");
        IDs = IDs.concat(items);
    }
    return IDs;
};
handlers.UpgradeItem = function (args) {
    let inventory = PlayFab.Player.Inventory.Retrieve(currentPlayerId);
    let itemInstance = inventory.FindWithInstanceID(args.itemInstanceId);
    if (itemInstance == null) {
        log.error(args.itemInstanceId + " is an Invalid Instance ID");
        return;
    }
    let catalog = PlayFab.Title.Catalog.Retrieve(itemInstance.CatalogVersion);
    let catalogItem = catalog.FindWithID(itemInstance.ItemId);
    let arguments = API.Upgrades.Arguments.Load(catalogItem);
    if (arguments == null) {
        log.error("Current Item Can't Be Upgraded");
        return;
    }
    let titleData = PlayFab.Title.Data.Retrieve([API.Upgrades.Name]);
    let template = API.Upgrades.Template.Find(titleData[API.Upgrades.Name], arguments.template);
    if (template == null) {
        log.error(arguments.template + " Upgrades Template Not Defined");
        return;
    }
    if (template.Find(args.upgradeType) == null) {
        log.error(args.upgradeType + " Upgrade Type Not Defined");
        return;
    }
    let data = API.Upgrades.Data.Load(itemInstance);
    if (data.Contains(args.upgradeType) == false)
        data.Add(args.upgradeType);
    if (data.Find(args.upgradeType).value >= template.Find(args.upgradeType).ranks.length) {
        log.error("Maximum Upgrade Level Achieved");
        return;
    }
    let rank = template.Match(args.upgradeType, data);
    if (rank.requirements != null) {
        if (inventory.CompliesWithRequirements(rank.requirements) == false) {
            log.error("Player Doesn't The Required Items For the Upgrade");
            return;
        }
    }
    if (inventory.virtualCurrency[rank.cost.type] < rank.cost.value) {
        log.error("Insufficient Funds");
        return;
    }
    //Validation Completed, Start Processing Request
    {
        PlayFab.Player.Currency.Subtract(currentPlayerId, rank.cost.type, rank.cost.value);
        API.ItemRequirement.ConsumeAll(inventory, rank.requirements);
        data.Find(args.upgradeType).value++;
        PlayFab.Player.Inventory.UpdateItemData(currentPlayerId, itemInstance.ItemInstanceId, API.Upgrades.Name, data.ToJson());
    }
    return "Success";
};
var API;
(function (API) {
    let World;
    (function (World_1) {
        World_1.Name = "world";
        let Data;
        (function (Data) {
            function Retrieve(playerID) {
                let playerData = PlayFab.Player.Data.ReadOnly.Read(playerID, [World_1.Name]);
                if (playerData.Data[World_1.Name] == null)
                    return new Instance();
                let json = playerData.Data[World_1.Name].Value;
                let object = JSON.parse(json);
                let instance = Object.assign(new Instance(), object);
                return instance;
            }
            Data.Retrieve = Retrieve;
            function Validate(data, template, region, level) {
                if (data.Contains(region)) {
                }
                else {
                    if (region == template.regions[0].name) {
                        let instance = new Region(region, 1);
                        data.Add(instance);
                    }
                    else {
                        throw "Can't begin data indexing from " + region + " region";
                    }
                }
            }
            Data.Validate = Validate;
            class Instance {
                constructor() {
                    this.regions = [];
                }
                Add(region) {
                    this.regions.push(region);
                }
                Contains(name) {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return true;
                    return false;
                }
                Find(name) {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return this.regions[i];
                    return null;
                }
            }
            Data.Instance = Instance;
            class Region {
                constructor(name, progress) {
                    this.name = name;
                    this.progress = progress;
                }
            }
            Data.Region = Region;
        })(Data = World_1.Data || (World_1.Data = {}));
        let World;
        (function (World) {
            function Retrieve() {
                let titleData = PlayFab.Title.Data.Retrieve([World_1.Name]);
                let json = titleData[World_1.Name];
                let object = JSON.parse(json);
                let data = Object.assign(new Data(), object);
                return data;
            }
            World.Retrieve = Retrieve;
            function Validate(data, region, level) {
                if (data.Contains(region)) {
                    if (level >= 0 && level < data.Find(region).levels.length) {
                        return;
                    }
                    else {
                        throw "Level " + level + " on " + region + " region doesn't exist";
                    }
                }
                else {
                    throw region + " region doesn't exist";
                }
            }
            World.Validate = Validate;
            class Data {
                Find(name) {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return this.regions[i];
                    return null;
                }
                Contains(name) {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return true;
                    return false;
                }
            }
            World.Data = Data;
            class Region {
            }
            World.Region = Region;
            class Level {
            }
            World.Level = Level;
            class Rewards {
            }
            World.Rewards = Rewards;
        })(World = World_1.World || (World_1.World = {}));
    })(World = API.World || (API.World = {}));
    let Upgrades;
    (function (Upgrades) {
        Upgrades.Name = "upgrades";
        let Data;
        (function (Data) {
            function Load(itemInstance) {
                let instance = new Instance;
                if (itemInstance.CustomData == null) {
                }
                else {
                    if (itemInstance.CustomData[Upgrades.Name] == null) {
                    }
                    else {
                        let object = JSON.parse(itemInstance.CustomData[Upgrades.Name]);
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
                    for (let i = 0; i < this.list.length; i++)
                        if (this.list[i].type == type)
                            return true;
                    return false;
                }
                Find(type) {
                    for (let i = 0; i < this.list.length; i++)
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
                let object = JSON.parse(catalogItem.CustomData);
                if (object[Upgrades.Name] == null) {
                }
                let data = Object.assign(new Instance(), object[Upgrades.Name]);
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
                let object = JSON.parse(json);
                let target = object.find(x => x.name == name);
                let template = Object.assign(new Instance(), target);
                return template;
            }
            Template.Find = Find;
            function Parse(json) {
                let object = JSON.parse(json);
                let instance = Object.assign(new Instance(), object);
                return instance;
            }
            Template.Parse = Parse;
            class Instance {
                Find(name) {
                    for (let i = 0; i < this.elements.length; i++)
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
    let Reward;
    (function (Reward) {
        function Grant(playerID, data, annotation) {
            let IDs = Array();
            IDs = IDs.concat(data.items);
            let result = PlayFab.Title.Catalog.Tables.Process(data.droptable);
            if (result != null)
                IDs = IDs.concat(result);
            PlayFab.Title.Catalog.Item.GrantAll(playerID, IDs, annotation);
            return IDs;
        }
        Reward.Grant = Grant;
        class Data {
        }
        Reward.Data = Data;
        class DropTable {
        }
        Reward.DropTable = DropTable;
    })(Reward = API.Reward || (API.Reward = {}));
    let Cost;
    (function (Cost) {
        class Data {
        }
        Cost.Data = Data;
    })(Cost = API.Cost || (API.Cost = {}));
    let ItemRequirement;
    (function (ItemRequirement) {
        function ConsumeAll(inventory, requirements) {
            if (requirements == null)
                return;
            for (let i = 0; i < requirements.length; i++) {
                let itemInstance = inventory.FindWithID(requirements[i].item);
                PlayFab.Player.Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, requirements[i].count);
            }
        }
        ItemRequirement.ConsumeAll = ConsumeAll;
        class Data {
        }
        ItemRequirement.Data = Data;
    })(ItemRequirement = API.ItemRequirement || (API.ItemRequirement = {}));
})(API || (API = {}));
var PlayFab;
(function (PlayFab) {
    let Player;
    (function (Player) {
        let Inventory;
        (function (Inventory) {
            function Retrieve(playerID) {
                let result = server.GetUserInventory({
                    PlayFabId: playerID,
                });
                return new Data(result.Inventory, result.VirtualCurrency);
            }
            Inventory.Retrieve = Retrieve;
            function Consume(playerID, itemInstanceID, count) {
                let result = server.ConsumeItem({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    ConsumeCount: count,
                });
            }
            Inventory.Consume = Consume;
            function UpdateItemData(playerID, itemInstanceID, key, value) {
                let data = {};
                data[key] = value;
                let request = server.UpdateUserInventoryItemCustomData({
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
                        let instance = this.FindWithID(requirements[i].item);
                        if (instance == null)
                            return false;
                        if (instance.RemainingUses < requirements[i].count)
                            return false;
                    }
                    return true;
                }
            }
            Inventory.Data = Data;
        })(Inventory = Player.Inventory || (Player.Inventory = {}));
        let Currency;
        (function (Currency) {
            function Subtract(playerID, currency, ammout) {
                let request = server.SubtractUserVirtualCurrency({
                    PlayFabId: playerID,
                    VirtualCurrency: currency,
                    Amount: ammout
                });
            }
            Currency.Subtract = Subtract;
        })(Currency = Player.Currency || (Player.Currency = {}));
        let Data;
        (function (Data) {
            let ReadOnly;
            (function (ReadOnly) {
                function Read(playerID, keys) {
                    let result = server.GetUserReadOnlyData({
                        PlayFabId: playerID,
                        Keys: keys
                    });
                    return result;
                }
                ReadOnly.Read = Read;
                function Write(playerID, key, value) {
                    let data = {};
                    data[key] = value;
                    server.UpdateUserReadOnlyData({
                        PlayFabId: playerID,
                        Data: data,
                    });
                }
                ReadOnly.Write = Write;
            })(ReadOnly = Data.ReadOnly || (Data.ReadOnly = {}));
        })(Data = Player.Data || (Player.Data = {}));
    })(Player = PlayFab.Player || (PlayFab.Player = {}));
    let Title;
    (function (Title) {
        let Data;
        (function (Data) {
            function Retrieve(keys) {
                let result = server.GetTitleData({
                    Keys: keys,
                });
                return result.Data;
            }
            Data.Retrieve = Retrieve;
        })(Data = Title.Data || (Title.Data = {}));
        let Catalog;
        (function (Catalog) {
            Catalog.Default = "Default";
            function Retrieve(version) {
                let result = server.GetCatalogItems({
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
            let Item;
            (function (Item) {
                function Grant(playerID, itemID, ammount, annotation) {
                    let items = [];
                    for (let i = 0; i < ammount; i++)
                        items.push(itemID);
                    return GrantAll(playerID, items, annotation);
                }
                Item.Grant = Grant;
                function GrantAll(playerID, itemIDs, annotation) {
                    if (itemIDs == null || itemIDs.length == 0)
                        return [];
                    let result = server.GrantItemsToUser({
                        PlayFabId: playerID,
                        Annotation: annotation,
                        CatalogVersion: Catalog.Default,
                        ItemIds: itemIDs
                    });
                    return result.ItemGrantResults;
                }
                Item.GrantAll = GrantAll;
            })(Item = Catalog.Item || (Catalog.Item = {}));
            let Tables;
            (function (Tables) {
                function Evaluate(tableID) {
                    let result = server.EvaluateRandomResultTable({
                        CatalogVersion: Catalog.Default,
                        TableId: tableID
                    });
                    return result.ResultItemId;
                }
                Tables.Evaluate = Evaluate;
                function Process(table) {
                    let items = Array();
                    for (let i = 0; i < table.iterations; i++) {
                        let item = Evaluate(table.ID);
                        items.push(item);
                    }
                    return items;
                }
                Tables.Process = Process;
            })(Tables = Catalog.Tables || (Catalog.Tables = {}));
        })(Catalog = Title.Catalog || (Title.Catalog = {}));
    })(Title = PlayFab.Title || (PlayFab.Title = {}));
})(PlayFab || (PlayFab = {}));
