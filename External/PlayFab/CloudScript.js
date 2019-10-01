"use strict";
handlers.LoginReward = function (args, context) {
    let template = API.Rewards.Template.Retrieve();
    let playerData = API.Rewards.PlayerData.Retrieve(currentPlayerId);
    let itemIDs = new Array();
    if (playerData == null) //Signup
     {
        playerData = API.Rewards.PlayerData.Create();
        var signupItems = API.Rewards.Grant(currentPlayerId, template.signup, "Signup Reward");
        itemIDs = itemIDs.concat(signupItems);
    }
    else //Recurring
     {
        let daysFromLastReward = Utility.Dates.DaysFrom(Date.parse(playerData.daily.timestamp));
        if (daysFromLastReward < 1) {
            return;
        }
        if (daysFromLastReward >= 2) {
            playerData.daily.progress = 0;
        }
        else {
        }
    }
    let dailyItems = API.Rewards.Grant(currentPlayerId, template.daily[playerData.daily.progress], "Daily Reward");
    itemIDs = itemIDs.concat(dailyItems);
    var result = new API.Rewards.Result(playerData.daily.progress, itemIDs);
    API.Rewards.PlayerData.Daily.Incremenet(playerData, template);
    API.Rewards.PlayerData.Update(currentPlayerId, playerData);
    return result;
};
handlers.FinishLevel = function (args) {
    try {
        var world = new API.World.Template.Instance(args.region, args.level);
    }
    catch (error) {
        log.error(error);
        return;
    }
    try {
        var playerData = new API.World.PlayerData.Instance(currentPlayerId, world, args.region);
    }
    catch (error) {
        log.error(error);
        return;
    }
    if (args.level >= playerData.progress) //cheating... probably
     {
        log.error("trying to finish level " + args.level + " without finishing the previous level ");
        return;
    }
    let itemIDs = new Array();
    if (args.level == playerData.progress - 1) //Initial
     {
        log.info("Initial Completion");
        playerData.Increment();
        API.World.PlayerData.Save(currentPlayerId, playerData.data);
        let rewardItemIDs = API.Rewards.Grant(currentPlayerId, world.level.reward.initial, "Level Completion Award");
        itemIDs = itemIDs.concat(rewardItemIDs);
    }
    else //Recurring
     {
        log.info("Recurring Completion");
        let rewardItemIDs = API.Rewards.Grant(currentPlayerId, world.level.reward.constant, "Level Completion Award");
        itemIDs = itemIDs.concat(rewardItemIDs);
    }
    return itemIDs;
};
handlers.UpgradeItem = function (args) {
    let inventory = PlayFab.Player.Inventory.Retrieve(currentPlayerId);
    let itemInstance = inventory.FindWithInstanceID(args.itemInstanceId);
    if (itemInstance == null) {
        log.error(args.itemInstanceId + " is an Invalid Instance ID");
        return;
    }
    if (itemInstance.CatalogVersion == null) {
        log.error("itemInstance has no catalog version defined");
        return;
    }
    if (itemInstance.ItemId == null) {
        log.error("itemInstance has no itemID value defined");
        return;
    }
    let catalog = PlayFab.Title.Catalog.Retrieve(itemInstance.CatalogVersion);
    let catalogItem = catalog.FindWithID(itemInstance.ItemId);
    if (catalogItem == null) {
        log.error("no catalog item relating to " + itemInstance.ItemId + " was found in catalog version " + itemInstance.CatalogVersion);
        return;
    }
};
var Utility;
(function (Utility) {
    let Dates;
    (function (Dates) {
        function DaysFrom(date) {
            return DaysBetween(date, new Date());
        }
        Dates.DaysFrom = DaysFrom;
        function DaysBetween(date1, date2) {
            return Math.round((date2.valueOf() - date1.valueOf()) / (86400000));
        }
        Dates.DaysBetween = DaysBetween;
    })(Dates = Utility.Dates || (Utility.Dates = {}));
    let Class;
    (function (Class) {
        function Assign(ctor, props) {
            return Object.assign(new ctor(), props);
        }
        Class.Assign = Assign;
        function WriteProperty(target, name, value) {
            let descriptor = {
                value: value,
                enumerable: true,
                writable: false,
            };
            Object.defineProperty(target, name, descriptor);
        }
        Class.WriteProperty = WriteProperty;
    })(Class = Utility.Class || (Utility.Class = {}));
})(Utility || (Utility = {}));
var API;
(function (API) {
    class DropTable {
        constructor(ID, iterations) {
            this.ID = ID;
            this.iterations = iterations;
        }
    }
    API.DropTable = DropTable;
    class Cost {
        constructor(type, value) {
            this.type = type;
            this.value = value;
        }
    }
    API.Cost = Cost;
    class ItemStack {
        constructor(item, count) {
            this.item = item;
            this.count = count;
        }
    }
    API.ItemStack = ItemStack;
})(API || (API = {}));
var API;
(function (API) {
    let Rewards;
    (function (Rewards) {
        Rewards.ID = "rewards";
        function Grant(playerID, data, annotation) {
            let IDs = Array();
            if (data.items == null) {
            }
            else {
                IDs = IDs.concat(data.items);
            }
            if (data.droptable == null) {
            }
            else {
                let result = PlayFab.Title.Catalog.Tables.Process(data.droptable);
                if (result != null)
                    IDs = IDs.concat(result);
            }
            PlayFab.Title.Catalog.Item.GrantAll(playerID, IDs, annotation);
            return IDs;
        }
        Rewards.Grant = Grant;
        class Template {
            constructor(object) {
                this.signup = object.signup;
                this.daily = object.daily;
            }
        }
        Rewards.Template = Template;
        (function (Template) {
            function Retrieve() {
                var json = PlayFab.Title.Data.Retrieve(Rewards.ID);
                if (json == null)
                    throw "no rewards template defined";
                var object = JSON.parse(json);
                var instance = new Template(object);
                return instance;
            }
            Template.Retrieve = Retrieve;
        })(Template = Rewards.Template || (Rewards.Template = {}));
        class PlayerData {
            constructor(object) {
                this.daily = object.daily;
            }
        }
        Rewards.PlayerData = PlayerData;
        (function (PlayerData) {
            function Retrieve(playerID) {
                let result = PlayFab.Player.Data.ReadOnly.Read(playerID, Rewards.ID);
                if (result == null)
                    return null;
                let object = JSON.parse(result);
                let instance = new PlayerData(object);
                return instance;
            }
            PlayerData.Retrieve = Retrieve;
            function Create() {
                var data = new PlayerData({ daily: Daily.Create() });
                return data;
            }
            PlayerData.Create = Create;
            function Save(playerID, data) {
                PlayFab.Player.Data.ReadOnly.Write(playerID, Rewards.ID, JSON.stringify(data));
            }
            PlayerData.Save = Save;
            function Update(playerID, data) {
                data.daily.timestamp = new Date().toJSON();
                Save(playerID, data);
            }
            PlayerData.Update = Update;
            class Daily {
                constructor(timestamp, progress) {
                    this.timestamp = timestamp;
                    this.progress = progress;
                }
            }
            PlayerData.Daily = Daily;
            (function (Daily) {
                function Incremenet(data, templates) {
                    data.daily.progress++;
                    if (data.daily.progress >= templates.daily.length)
                        data.daily.progress = 0;
                }
                Daily.Incremenet = Incremenet;
                function Create() {
                    var instance = new Daily(new Date().toJSON(), 0);
                    return instance;
                }
                Daily.Create = Create;
            })(Daily = PlayerData.Daily || (PlayerData.Daily = {}));
        })(PlayerData = Rewards.PlayerData || (Rewards.PlayerData = {}));
        class Result {
            constructor(progress, items) {
                this.progress = progress;
                this.items = items;
            }
        }
        Rewards.Result = Result;
        class Type {
            constructor(items, droptable) {
                this.items = items;
                this.droptable = droptable;
            }
        }
        Rewards.Type = Type;
    })(Rewards = API.Rewards || (API.Rewards = {}));
})(API || (API = {}));
var API;
(function (API) {
    let Upgrades;
    (function (Upgrades) {
        Upgrades.ID = "upgrades";
        class InstanceData {
            constructor(list) {
                this.list = list;
            }
            Add(type) {
                var element = new InstanceData.Element(type, 0);
                this.list.push(element);
                return element;
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
            Increment(type) {
                let element = this.Find(type);
                if (element == null)
                    return null;
                else {
                    element.value++;
                    return element;
                }
            }
            ToJson() {
                return JSON.stringify(this.list);
            }
        }
        Upgrades.InstanceData = InstanceData;
        (function (InstanceData) {
            function Load(itemInstance, args) {
                let data = null;
                if (itemInstance.CustomData == null) {
                }
                else {
                    var json = itemInstance.CustomData[Upgrades.ID];
                    if (json == null) {
                    }
                    else {
                        var object = JSON.parse(json);
                        data = new InstanceData(object);
                    }
                }
                if (data == null)
                    data = new InstanceData([]);
                if (data.Contains(args.upgradeType) == false)
                    data.Add(args.upgradeType);
                return data;
            }
            InstanceData.Load = Load;
            function Save(playerID, itemInstance, data) {
                var itemInstanceID = itemInstance.ItemInstanceId;
                if (itemInstanceID == null) {
                    log.debug("No Instance ID defined for item instance");
                    return;
                }
                PlayFab.Player.Inventory.UpdateItemData(playerID, itemInstanceID, API.Upgrades.ID, data.ToJson());
            }
            InstanceData.Save = Save;
            class Element {
                constructor(name, value) {
                    this.type = name;
                    this.value = value;
                }
            }
            InstanceData.Element = Element;
        })(InstanceData = Upgrades.InstanceData || (Upgrades.InstanceData = {}));
        class ItemData {
            constructor(object) {
                this.template = object.template;
                this.applicable = object.applicable;
            }
        }
        Upgrades.ItemData = ItemData;
        (function (ItemData) {
            ItemData.Default = "Default";
            function Load(catalogItem) {
                if (catalogItem == null)
                    return null;
                if (catalogItem.CustomData == null)
                    return null;
                let object = JSON.parse(catalogItem.CustomData);
                var element = object[Upgrades.ID];
                if (element == null)
                    return null;
                let data = new ItemData(element);
                return data;
            }
            ItemData.Load = Load;
        })(ItemData = Upgrades.ItemData || (Upgrades.ItemData = {}));
        class Template {
            constructor(object) {
                this.name = object.name;
                this.elements = object.elements;
            }
            Find(name) {
                for (let i = 0; i < this.elements.length; i++)
                    if (this.elements[i].type == name)
                        return this.elements[i];
                return null;
            }
            Match(name, data) {
                let element = this.Find(name);
                if (element == null)
                    return null;
                let dataElement = data.Find(name);
                if (dataElement == null)
                    return null;
                return element.ranks[dataElement.value];
            }
        }
        Upgrades.Template = Template;
        (function (Template) {
            function Find(name) {
                let list = GetAll();
                if (list == null)
                    return null;
                for (let i = 0; i < list.length; i++)
                    if (list[i].name == name)
                        return new Template(list[i]);
                return null;
            }
            Template.Find = Find;
            function GetAll() {
                let json = PlayFab.Title.Data.Retrieve(API.Upgrades.ID);
                if (json == null)
                    return null;
                var object = JSON.parse(json);
                return object;
            }
            Template.GetAll = GetAll;
            class Element {
                constructor(type, ranks) {
                    this.type = type;
                    this.ranks = ranks;
                }
            }
            Template.Element = Element;
            class Rank {
                constructor(cost, percentage, requirements) {
                    this.cost = cost;
                    this.percentage = percentage;
                    this.requirements = requirements;
                }
            }
            Template.Rank = Rank;
        })(Template = Upgrades.Template || (Upgrades.Template = {}));
    })(Upgrades = API.Upgrades || (API.Upgrades = {}));
})(API || (API = {}));
var API;
(function (API) {
    let World;
    (function (World) {
        World.Name = "world";
        class PlayerData {
            constructor(object) {
                this.regions = object.regions;
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
        World.PlayerData = PlayerData;
        (function (PlayerData) {
            function Retrieve(playerID) {
                let json = PlayFab.Player.Data.ReadOnly.Read(playerID, World.Name);
                let object;
                if (json == null)
                    object = { regions: [] };
                else
                    object = JSON.parse(json);
                let instance = new PlayerData(object);
                return instance;
            }
            PlayerData.Retrieve = Retrieve;
            function Save(playerID, data) {
                let json = JSON.stringify(data);
                PlayFab.Player.Data.ReadOnly.Write(playerID, World.Name, json);
            }
            PlayerData.Save = Save;
            class Instance {
                constructor(playerID, instance, region) {
                    this.data = Retrieve(playerID);
                    this.region = this.GetRegion(instance, region);
                }
                GetRegion(world, name) {
                    if (this.data.Contains(name)) {
                    }
                    else {
                        if (this.HasAccessToRegion(world, name)) {
                            let instance = new Region(name, 1);
                            this.data.Add(instance);
                        }
                        else {
                            throw "Trying to index " + name + " region without having access to that region";
                        }
                    }
                    var result = this.data.Find(name);
                    if (result == null)
                        throw "No " + name + " region found in player data";
                    return result;
                }
                HasAccessToRegion(world, currentRegion) {
                    var index = world.template.IndexOf(currentRegion);
                    if (index == null)
                        throw "No region index for " + name + " was found in world template";
                    if (index == 0)
                        return true;
                    var previousRegion = world.template.regions[index - 1];
                    var previousRegionData = this.data.Find(previousRegion.name);
                    if (previousRegionData == null)
                        return false;
                    return previousRegionData.progress > previousRegion.levels.length;
                }
                get progress() { return this.region.progress; }
                set progress(value) { this.region.progress = value; }
                Increment() {
                    this.progress += 1;
                }
            }
            PlayerData.Instance = Instance;
            class Region {
                constructor(name, progress) {
                    this.name = name;
                    this.progress = progress;
                }
            }
            PlayerData.Region = Region;
        })(PlayerData = World.PlayerData || (World.PlayerData = {}));
        class Template {
            constructor(object) {
                this.regions = object.regions;
            }
            get Last() {
                return this.regions[this.regions.length - 1];
            }
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
            IndexOf(name) {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return i;
                return null;
            }
        }
        World.Template = Template;
        (function (Template) {
            function Retrieve() {
                let json = PlayFab.Title.Data.Retrieve(World.Name);
                if (json == null)
                    throw "No world template defined";
                let object = JSON.parse(json);
                var data = new Template(object);
                return data;
            }
            Template.Retrieve = Retrieve;
            function Validate(data, args) {
                let region = data.Find(args.region);
                if (region == null) {
                    throw args.region + " region doesn't exist";
                }
                else {
                    if (args.level >= 0 && args.level < region.levels.length) {
                        return;
                    }
                    else {
                        throw "Level " + args.level + " on " + args.region + " region doesn't exist";
                    }
                }
            }
            Template.Validate = Validate;
            class Instance {
                constructor(region, level) {
                    this.template = API.World.Template.Retrieve();
                    this.region = this.GetRegion(region);
                    this.level = this.GetLevel(level);
                }
                GetRegion(name) {
                    let result = this.template.Find(name);
                    if (result == null)
                        throw "No region named " + name + " found in world template";
                    return result;
                }
                GetLevel(index) {
                    let result = this.region.levels[index];
                    if (result == null)
                        throw "No level indexed " + index + " found in " + this.region.name + " world region template";
                    return result;
                }
            }
            Template.Instance = Instance;
            class Region {
                constructor(name, levels) {
                    this.name = name;
                    this.levels = levels;
                }
            }
            Template.Region = Region;
            class Level {
                constructor(reward) {
                    this.reward = reward;
                }
            }
            Template.Level = Level;
            class Rewards {
                constructor(initial, constant) {
                    this.initial = initial;
                    this.constant = constant;
                }
            }
            Template.Rewards = Rewards;
        })(Template = World.Template || (World.Template = {}));
    })(World = API.World || (API.World = {}));
})(API || (API = {}));
var PlayFab;
(function (PlayFab) {
    let Player;
    (function (Player) {
        class Inventory {
            constructor(items, virtualCurrency) {
                if (items == null)
                    this.items = [];
                else
                    this.items = items;
                this.virtualCurrency = virtualCurrency;
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
            CompliesWith(requirements) {
                for (let i = 0; i < requirements.length; i++) {
                    let instance = this.FindWithID(requirements[i].item);
                    if (instance == null)
                        return false;
                    if (instance.RemainingUses == null) {
                        if (requirements[i].count > 1)
                            return false;
                        else
                            continue;
                    }
                    if (instance.RemainingUses < requirements[i].count)
                        return false;
                }
                return true;
            }
        }
        Player.Inventory = Inventory;
        (function (Inventory) {
            function Retrieve(playerID) {
                let result = server.GetUserInventory({
                    PlayFabId: playerID,
                });
                return new Inventory(result.Inventory, result.VirtualCurrency);
            }
            Inventory.Retrieve = Retrieve;
            function Consume(playerID, itemInstanceID, count) {
                let result = server.ConsumeItem({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    ConsumeCount: count,
                });
                return result;
            }
            Inventory.Consume = Consume;
            function ConsumeAll(inventory, stacks) {
                let results = Array();
                if (stacks == null)
                    return results;
                for (let i = 0; i < stacks.length; i++) {
                    let itemInstance = inventory.FindWithID(stacks[i].item);
                    if (itemInstance == null) {
                        log.info("item with ID " + stacks[i].item + " not found in inventory");
                        continue;
                    }
                    var itemInstanceID = itemInstance.ItemInstanceId;
                    if (itemInstanceID == null) {
                        log.info("itemInstance doesn't have an itemInstanceID, what the heck ?");
                        continue;
                    }
                    let result = PlayFab.Player.Inventory.Consume(currentPlayerId, itemInstanceID, stacks[i].count);
                    results.push(result);
                }
                return results;
            }
            Inventory.ConsumeAll = ConsumeAll;
            function UpdateItemData(playerID, itemInstanceID, key, value) {
                let data = {};
                Utility.Class.WriteProperty(data, key, value);
                let response = server.UpdateUserInventoryItemCustomData({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    Data: data
                });
            }
            Inventory.UpdateItemData = UpdateItemData;
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
                function ReadAll(playerID, keys) {
                    let result = server.GetUserReadOnlyData({
                        PlayFabId: playerID,
                        Keys: keys
                    });
                    return result;
                }
                ReadOnly.ReadAll = ReadAll;
                function Read(playerID, key) {
                    let result = ReadAll(playerID, [key]);
                    if (result.Data == null)
                        return null;
                    if (result.Data[key] == null)
                        return null;
                    let value = result.Data[key].Value;
                    if (value == null)
                        return null;
                    return value;
                }
                ReadOnly.Read = Read;
                function Write(playerID, key, value) {
                    let data = {};
                    Utility.Class.WriteProperty(data, key, value);
                    log.info(JSON.stringify(data));
                    var result = server.UpdateUserReadOnlyData({
                        PlayFabId: playerID,
                        Data: data,
                    });
                    return result;
                }
                ReadOnly.Write = Write;
            })(ReadOnly = Data.ReadOnly || (Data.ReadOnly = {}));
        })(Data = Player.Data || (Player.Data = {}));
    })(Player = PlayFab.Player || (PlayFab.Player = {}));
})(PlayFab || (PlayFab = {}));
var PlayFab;
(function (PlayFab) {
    let Title;
    (function (Title) {
        let Data;
        (function (Data) {
            function RetrieveAll(keys) {
                let result = server.GetTitleData({
                    Keys: keys,
                });
                return result;
            }
            Data.RetrieveAll = RetrieveAll;
            function Retrieve(key) {
                var result = RetrieveAll([key]);
                if (result.Data == null)
                    return null;
                if (result.Data[key] == null)
                    return null;
                return result.Data[key];
            }
            Data.Retrieve = Retrieve;
        })(Data = Title.Data || (Title.Data = {}));
        class Catalog {
            constructor(items) {
                if (items == null)
                    this.items = [];
                else
                    this.items = items;
            }
            FindWithID(itemID) {
                for (let i = 0; i < this.items.length; i++)
                    if (this.items[i].ItemId == itemID)
                        return this.items[i];
                return null;
            }
        }
        Title.Catalog = Catalog;
        (function (Catalog) {
            Catalog.Default = "Default";
            function Retrieve(version) {
                let result = server.GetCatalogItems({
                    CatalogVersion: version,
                });
                return new Catalog(result.Catalog);
            }
            Catalog.Retrieve = Retrieve;
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
                    if (result.ItemGrantResults == null)
                        return [];
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
                    if (result.ResultItemId == null)
                        return null;
                    return result.ResultItemId;
                }
                Tables.Evaluate = Evaluate;
                function Process(table) {
                    let items = Array();
                    for (let i = 0; i < table.iterations; i++) {
                        let item = Evaluate(table.ID);
                        if (item == null)
                            continue;
                        items.push(item);
                    }
                    return items;
                }
                Tables.Process = Process;
            })(Tables = Catalog.Tables || (Catalog.Tables = {}));
        })(Catalog = Title.Catalog || (Title.Catalog = {}));
    })(Title = PlayFab.Title || (PlayFab.Title = {}));
})(PlayFab || (PlayFab = {}));
