"use strict";
function ClaimDailyReward(args, context) {
    let template = API.DailyReward.Template.Retrieve();
    let playerData = API.DailyReward.PlayerData.Retrieve(currentPlayerId);
    let reward = null;
    if (playerData == null) //First time claiming daily reward
     {
        playerData = API.DailyReward.PlayerData.Create();
        reward = template.Get(0);
    }
    else {
        var daysFromLastReward = Utility.Dates.DaysFrom(playerData.datestamp);
        if (daysFromLastReward >= 1) {
            if (daysFromLastReward >= 2)
                playerData.progress = 0;
            reward = template.Get(playerData.progress);
        }
    }
    let progress = playerData.progress;
    let itemIDs;
    if (reward == null) {
        itemIDs = [];
    }
    else {
        playerData.Progress(template);
        API.DailyReward.PlayerData.Save(currentPlayerId, playerData);
        itemIDs = API.Reward.Grant(currentPlayerId, reward, "Daily Login Reward");
        let result = new API.DailyReward.Result(progress, itemIDs);
        return result;
    }
    let result = new API.DailyReward.Result(progress, itemIDs);
    return result;
}
function FinishLevel(args) {
    if (args == null) {
        log.error("no arguments specified");
        return;
    }
    if (args.region == null) {
        log.error("no region argument specified");
        return;
    }
    if (args.level == null) {
        log.error("no level argument specified");
        return;
    }
    if (args.difficulty == null) {
        log.error("no difficulty argument specified");
        return;
    }
    if (args.difficulty in API.Difficulty == false) {
        log.error("no difficulty: " + args.difficulty + " defined");
        return;
    }
    let template;
    try {
        template = API.World.Template.Snapshot.Retrieve(args);
    }
    catch (error) {
        log.error(error);
        return;
    }
    let playerData;
    try {
        playerData = API.World.PlayerData.Snapshot.Retrieve(args, template);
    }
    catch (error) {
        log.error(error);
        return;
    }
    if (playerData.occurrence == API.World.Level.Finish.Occurrence.Initial) //First Time Completing Level
     {
        playerData.region.Increment();
        API.World.PlayerData.Save(currentPlayerId, playerData.data);
    }
    let rewards = template.level.GetApplicableRewards(args, playerData);
    let rewardsIDs = Array();
    for (let i = 0; i < rewards.length; i++) {
        let itemIDs = API.Reward.Grant(currentPlayerId, rewards[i].data, "Level Completion " + playerData.occurrence + " Reward");
        rewardsIDs = rewardsIDs.concat(itemIDs);
    }
    let result = {
        rewards: rewardsIDs,
        stars: playerData.star == null ? 0 : playerData.star.rank
    };
    return result;
}
function UpgradeItem(args) {
    if (args == null) {
        log.error("no arguments specified");
        return;
    }
    if (args.upgradeType == null) {
        log.error("no upgrade type argument specified");
        return;
    }
    if (args.itemInstanceId == null) {
        log.error("no itemInstanceID argument specified");
        return;
    }
    let inventory = PlayFab.Player.Inventory.Retrieve(currentPlayerId);
    let itemInstance = inventory.FindWithInstanceID(args.itemInstanceId);
    if (itemInstance == null) {
        log.error("no inventory item found with instanceID: " + args.itemInstanceId);
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
    let catalog = PlayFab.Catalog.Retrieve(itemInstance.CatalogVersion);
    let catalogItem = catalog.FindWithID(itemInstance.ItemId);
    if (catalogItem == null) {
        log.error("no catalog item relating to itemID " + itemInstance.ItemId + " was found in catalog version " + itemInstance.CatalogVersion);
        return;
    }
    let itemData = API.Upgrades.ItemData.Load(catalogItem);
    if (itemData == null) {
        log.error("item: " + catalogItem.ItemId + " Cannot be upgraded");
        return;
    }
    let template;
    try {
        template = API.Upgrades.Template.Snapshot.Retrieve(args, itemData);
    }
    catch (error) {
        log.error(error);
        return;
    }
    let instanceData;
    try {
        instanceData = API.Upgrades.InstanceData.Snapshot.Retrieve(args, itemData, itemInstance, template);
    }
    catch (error) {
        log.error(error);
        return;
    }
    let rank = {
        requirements: template.element.requirements[instanceData.element.value],
        cost: {
            value: template.element.cost.Calculate(instanceData.element.value + 1),
            type: "GD",
        }
    };
    log.info("rank:" + MyJSON.Stringfy(rank));
    if (inventory.CompliesWithAll(rank.requirements) == false) {
        log.error("inventory doesn't comply with upgrade requirements");
        return;
    }
    log.info(currentPlayerId + " - " + rank.cost.type + " - " + rank.cost.value);
    PlayFab.Player.Currency.Subtract(currentPlayerId, rank.cost.type, rank.cost.value);
    PlayFab.Player.Inventory.ConsumeAll(inventory, rank.requirements);
    instanceData.element.Increment();
    API.Upgrades.InstanceData.Save(currentPlayerId, itemInstance, instanceData.data);
    let result = new API.Upgrades.Result(true);
    return result;
}
function Reward(args) {
    if (args == null) {
        log.error("no arguments specified");
        return;
    }
    PlayFab.Catalog.Item.GrantAll(currentPlayerId, args, "Reward");
}
function WelcomeNewPlayer(args) {
    var inventory = PlayFab.Player.Inventory.Retrieve(currentPlayerId);
    var template = API.NewPlayerReward.Template.Retrieve();
    if (inventory.Contains(template.token)) {
        log.error("player " + currentPlayerId + " has already been rewarded a " + template.token);
        return;
    }
    var rewards = template.items.slice();
    rewards.push(template.token);
    PlayFab.Catalog.Item.GrantAll(currentPlayerId, rewards, "New Player Welcome Reward");
    var result = {
        items: rewards
    };
    return result;
}
function Register() {
    handlers[ClaimDailyReward.name] = ClaimDailyReward;
    handlers[FinishLevel.name] = FinishLevel;
    handlers[UpgradeItem.name] = UpgradeItem;
    handlers[Reward.name] = Reward;
    handlers[WelcomeNewPlayer.name] = WelcomeNewPlayer;
}
if (IsOnPlayFab())
    Register();
else
    console.warn("No handlers object found, will not add any handlers");
class Sandbox {
    static Init() {
        if (IsOnPlayFab()) {
        }
        else {
            setTimeout(this.Execute, 50);
        }
    }
    static Execute() {
        var json = "{ \"name\": \"Default\", \"elements\": [ { \"type\": \"Power\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ null, [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] }, { \"type\": \"Defense\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] }, { \"type\": \"Range\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] }, { \"type\": \"Speed\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] } ] }";
        var instance = JSON.parse(json);
        var template = new API.Upgrades.Template(instance);
        console.log(MyJSON.Stringfy(template));
    }
}
Sandbox.Init();
var MyJSON;
(function (MyJSON) {
    MyJSON.IgnoreCharacter = '$';
    function Stringfy(object) {
        function Replacer(key, value) {
            if (key[0] == MyJSON.IgnoreCharacter)
                return undefined;
            return value;
        }
        var json = JSON.stringify(object, Replacer);
        return json;
    }
    MyJSON.Stringfy = Stringfy;
    function Parse(constructor, json) {
        var object = JSON.parse(json);
        var instance = new constructor(object);
        return instance;
    }
    MyJSON.Parse = Parse;
})(MyJSON || (MyJSON = {}));
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
function CompareIgnoreCase(text1, text2) {
    if (text1 == null || text2 == null)
        return text1 == text2;
    return text1.toLowerCase() == text2.toLowerCase();
}
function IsOnPlayFab() {
    return globalThis.handlers != null;
}
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
        static Grant(playerID, stack, annotation) {
            PlayFab.Catalog.Item.Grant(playerID, stack.item, stack.count, annotation);
        }
        static GrantAll(playerID, stacks, annotation) {
            let IDs = [];
            for (let x = 0; x < stacks.length; x++)
                for (let y = 0; y < stacks[x].count; y++)
                    IDs.push(stacks[x].item);
            PlayFab.Catalog.Item.GrantAll(playerID, IDs, annotation);
        }
        static FromText(text) {
            let start = text.indexOf('[');
            if (start < 0)
                return new ItemStack(text, 1);
            let end = text.indexOf(']');
            if (end < 0)
                return new ItemStack(text, 1);
            let count = Number.parseInt(text.substring(start + 1, end));
            let id = text.substring(end + 1, text.length);
            return new ItemStack(id, count);
        }
    }
    API.ItemStack = ItemStack;
    let Difficulty;
    (function (Difficulty) {
        Difficulty[Difficulty["Normal"] = 1] = "Normal";
        Difficulty[Difficulty["Hard"] = 2] = "Hard";
        Difficulty[Difficulty["Skilled"] = 3] = "Skilled";
    })(Difficulty = API.Difficulty || (API.Difficulty = {}));
    class FactorialValue {
        constructor(initial, multiplier) {
            this.initial = initial;
            this.multiplier = multiplier;
        }
        Calculate(value) {
            if (value == 0)
                return 0;
            return this.initial + (this.multiplier * (value - 1));
        }
        static Create(source) {
            return new FactorialValue(source.initial, source.multiplier);
        }
    }
    API.FactorialValue = FactorialValue;
})(API || (API = {}));
var API;
(function (API) {
    let DailyReward;
    (function (DailyReward) {
        DailyReward.ID = "daily-rewards";
        class Result {
            constructor(progress, items) {
                this.progress = progress;
                this.items = items;
            }
        }
        DailyReward.Result = Result;
    })(DailyReward = API.DailyReward || (API.DailyReward = {}));
})(API || (API = {}));
var API;
(function (API) {
    let DailyReward;
    (function (DailyReward) {
        class PlayerData {
            constructor(source) {
                this.timestamp = source.timestamp;
                this.progress = source.progress;
            }
            get datestamp() { return Date.parse(this.timestamp); }
            Progress(template) {
                this.timestamp = new Date().toJSON();
                this.progress += 1;
                if (this.progress >= template.max)
                    this.progress = 0;
            }
            //Static
            static Retrieve(playerID) {
                let json = PlayFab.Player.Data.ReadOnly.Read(playerID, API.DailyReward.ID);
                if (json == null)
                    return null;
                let instance = MyJSON.Parse(PlayerData, json);
                return instance;
            }
            static Create() {
                let source = {
                    progress: 0,
                    timestamp: new Date().toJSON()
                };
                let instance = new PlayerData(source);
                return instance;
            }
            static Save(playerID, data) {
                PlayFab.Player.Data.ReadOnly.Write(playerID, API.DailyReward.ID, JSON.stringify(data));
            }
            static Update(playerID, data) {
                data.timestamp = new Date().toJSON();
                PlayerData.Save(playerID, data);
            }
        }
        DailyReward.PlayerData = PlayerData;
    })(DailyReward = API.DailyReward || (API.DailyReward = {}));
})(API || (API = {}));
var API;
(function (API) {
    let DailyReward;
    (function (DailyReward) {
        class Template {
            constructor(list) {
                this.list = list;
            }
            get max() { return this.list.length; }
            Get(index) {
                if (index < 0 || index + 1 > this.max)
                    throw ("no rewars defined for index: " + index);
                return this.list[index];
            }
            //Static
            static Retrieve() {
                var json = PlayFab.Title.Data.Retrieve(API.DailyReward.ID);
                if (json == null)
                    throw ("no rewards template defined");
                var list = JSON.parse(json);
                var instance = new Template(list);
                return instance;
            }
        }
        DailyReward.Template = Template;
    })(DailyReward = API.DailyReward || (API.DailyReward = {}));
})(API || (API = {}));
var API;
(function (API) {
    let NewPlayerReward;
    (function (NewPlayerReward) {
        NewPlayerReward.ID = "new-player-reward";
        class Result {
            constructor(progress, items) {
                this.items = items;
            }
        }
        NewPlayerReward.Result = Result;
    })(NewPlayerReward = API.NewPlayerReward || (API.NewPlayerReward = {}));
})(API || (API = {}));
var API;
(function (API) {
    let NewPlayerReward;
    (function (NewPlayerReward) {
        class Template {
            constructor(source) {
                this.token = source.token;
                this.items = source.items;
            }
            static Retrieve() {
                var json = PlayFab.Title.Data.Retrieve(API.NewPlayerReward.ID);
                if (json == null)
                    throw ("no new player reward template defined");
                var source = JSON.parse(json);
                var instance = new Template(source);
                return instance;
            }
        }
        NewPlayerReward.Template = Template;
    })(NewPlayerReward = API.NewPlayerReward || (API.NewPlayerReward = {}));
})(API || (API = {}));
var API;
(function (API) {
    class Reward {
        constructor(items, droptable) {
            this.items = items;
            this.droptable = droptable;
        }
        //Static
        static Grant(playerID, data, annotation) {
            let IDs = Array();
            if (data.items == null) {
            }
            else {
                IDs = IDs.concat(data.items);
            }
            if (data.droptable == null) {
            }
            else {
                let result = PlayFab.Catalog.Tables.Process(data.droptable);
                if (result != null)
                    IDs = IDs.concat(result);
            }
            PlayFab.Catalog.Item.GrantAll(playerID, IDs, annotation);
            return IDs;
        }
    }
    API.Reward = Reward;
})(API || (API = {}));
var API;
(function (API) {
    let Upgrades;
    (function (Upgrades) {
        Upgrades.ID = "upgrades";
        Upgrades.Max = 5;
        class Result {
            constructor(success) {
                this.success = success;
            }
        }
        Upgrades.Result = Result;
    })(Upgrades = API.Upgrades || (API.Upgrades = {}));
})(API || (API = {}));
var API;
(function (API) {
    let Upgrades;
    (function (Upgrades) {
        class InstanceData {
            constructor(source) {
                this.list = [];
                for (let i = 0; i < source.length; i++) {
                    let instance = new InstanceData.Element(source[i]);
                    this.list.push(instance);
                }
            }
            Find(name) {
                for (let i = 0; i < this.list.length; i++)
                    if (this.list[i].type == name)
                        return this.list[i];
                return null;
            }
            Add(name) {
                let source = {
                    type: name,
                    value: 0,
                };
                let instance = new InstanceData.Element(source);
                this.list.push(instance);
                return instance;
            }
            toJSON() {
                return MyJSON.Stringfy(this.list);
            }
            static Load(itemInstance) {
                let data = null;
                if (itemInstance == null)
                    throw ("itemInstance is null, can't load upgrade instance data");
                if (itemInstance.CustomData == null)
                    return null;
                let json = itemInstance.CustomData[API.Upgrades.ID];
                if (json == null)
                    return null;
                var list = JSON.parse(json);
                var instance = new InstanceData(list);
                return instance;
            }
            static Create() {
                let source = [];
                let instance = new InstanceData(source);
                return instance;
            }
            static Save(playerID, itemInstance, data) {
                var itemInstanceID = itemInstance.ItemInstanceId;
                if (itemInstanceID == null) {
                    log.debug("No Instance ID defined for item instance");
                    return;
                }
                PlayFab.Player.Inventory.UpdateItemData(playerID, itemInstanceID, API.Upgrades.ID, data.toJSON());
            }
        }
        Upgrades.InstanceData = InstanceData;
        (function (InstanceData) {
            class Element {
                constructor(source) {
                    this.type = source.type;
                    this.value = source.value;
                }
                Increment() {
                    this.value += 1;
                }
            }
            InstanceData.Element = Element;
            class Snapshot {
                constructor(data, element) {
                    this.data = data;
                    this.element = element;
                }
                static Retrieve(args, itemData, itemInstance, template) {
                    let data = API.Upgrades.InstanceData.Load(itemInstance);
                    if (data == null) //First time the player is upgrading this item
                     {
                        data = API.Upgrades.InstanceData.Create();
                    }
                    if (itemData.IsApplicable(args.upgradeType) == false)
                        throw (args.upgradeType + " upgrade is not applicable to " + itemInstance.ItemId);
                    let element = data.Find(args.upgradeType);
                    if (element == null) //First time the player is upgrading this property
                     {
                        element = data.Add(args.upgradeType);
                    }
                    if (element.value >= Upgrades.Max)
                        throw ("can't upgrade " + itemInstance.ItemId + " any further");
                    return {
                        data: data,
                        element: element
                    };
                }
            }
            InstanceData.Snapshot = Snapshot;
        })(InstanceData = Upgrades.InstanceData || (Upgrades.InstanceData = {}));
    })(Upgrades = API.Upgrades || (API.Upgrades = {}));
})(API || (API = {}));
var API;
(function (API) {
    let Upgrades;
    (function (Upgrades) {
        class ItemData {
            constructor(source) {
                this.template = source.template;
                this.applicable = source.applicable;
            }
            IsApplicable(type) {
                for (let i = 0; i < this.applicable.length; i++)
                    if (this.applicable[i] == type)
                        return true;
                return false;
            }
            static Load(catalogItem) {
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
        }
        Upgrades.ItemData = ItemData;
    })(Upgrades = API.Upgrades || (API.Upgrades = {}));
})(API || (API = {}));
var API;
(function (API) {
    let Upgrades;
    (function (Upgrades) {
        class Template {
            constructor(source) {
                this.name = source.name;
                this.elements = [];
                for (let i = 0; i < source.elements.length; i++) {
                    let instance = new Template.Element(this, i, source.elements[i]);
                    this.elements.push(instance);
                }
            }
            Find(name) {
                for (let i = 0; i < this.elements.length; i++)
                    if (this.elements[i].type == name)
                        return this.elements[i];
                return null;
            }
        }
        Upgrades.Template = Template;
        (function (Template) {
            Template.Default = "Default";
            function GetDefault() {
                var result = Find(Template.Default);
                if (result == null)
                    throw ("no " + Template.Default + " upgrade template defined");
                return result;
            }
            Template.GetDefault = GetDefault;
            function Find(name) {
                let list = GetAll();
                for (let i = 0; i < list.length; i++)
                    if (list[i].name == name)
                        return new Template(list[i]);
                return null;
            }
            Template.Find = Find;
            function GetAll() {
                let json = PlayFab.Title.Data.Retrieve(API.Upgrades.ID);
                if (json == null)
                    throw ("no upgrades templates defined");
                var object = JSON.parse(json);
                return object;
            }
            Template.GetAll = GetAll;
            class Element {
                constructor($template, $index, source) {
                    this.$template = $template;
                    this.$index = $index;
                    this.type = source.type;
                    this.cost = API.FactorialValue.Create(source.cost);
                    this.percentage = API.FactorialValue.Create(source.percentage);
                    this.requirements = [];
                    for (let x = 0; x < source.requirements.length; x++) {
                        this.requirements.push([]);
                        if (source.requirements[x] == null)
                            continue;
                        for (let y = 0; y < source.requirements[x].length; y++) {
                            var stack = API.ItemStack.FromText(source.requirements[x][y]);
                            this.requirements[x].push(stack);
                        }
                    }
                }
                get template() { return this.$template; }
                get index() { return this.$index; }
            }
            Template.Element = Element;
            class Snapshot {
                constructor(data, element) {
                    this.data = data;
                    this.element = element;
                }
                static Retrieve(args, itemData) {
                    let data;
                    if (itemData.template == null) {
                        data = API.Upgrades.Template.GetDefault();
                    }
                    else {
                        data = API.Upgrades.Template.Find(itemData.template);
                        if (data == null)
                            throw ("no " + itemData.template + " upgrade template defined");
                    }
                    let element = data.Find(args.upgradeType);
                    if (element == null)
                        throw ("upgrade type " + args.upgradeType + " not defined within " + data.name + " upgrade template");
                    return {
                        data: data,
                        element: element,
                    };
                }
            }
            Template.Snapshot = Snapshot;
        })(Template = Upgrades.Template || (Upgrades.Template = {}));
    })(Upgrades = API.Upgrades || (API.Upgrades = {}));
})(API || (API = {}));
var API;
(function (API) {
    let World;
    (function (World) {
        World.ID = "world";
        let Level;
        (function (Level) {
            let Finish;
            (function (Finish) {
                let Occurrence;
                (function (Occurrence) {
                    Occurrence[Occurrence["Initial"] = 1] = "Initial";
                    Occurrence[Occurrence["Recurring"] = 2] = "Recurring";
                })(Occurrence = Finish.Occurrence || (Finish.Occurrence = {}));
                class Result {
                    constructor(stars, reward) {
                        this.stars = stars;
                        this.rewards = reward;
                    }
                }
                Finish.Result = Result;
            })(Finish = Level.Finish || (Level.Finish = {}));
        })(Level = World.Level || (World.Level = {}));
    })(World = API.World || (API.World = {}));
})(API || (API = {}));
var API;
(function (API) {
    let World;
    (function (World) {
        class PlayerData {
            constructor(source) {
                this.regions = [];
                for (let i = 0; i < source.regions.length; i++) {
                    let copy = new PlayerData.Region(this, i, source.regions[i]);
                    this.regions.push(copy);
                }
            }
            get size() { return this.regions.length; }
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
            Add(name) {
                var region = PlayerData.Region.Create(this, this.size, name, 0);
                this.regions.push(region);
                return region;
            }
            //Static
            static Retrieve(playerID) {
                var json = PlayFab.Player.Data.ReadOnly.Read(playerID, API.World.ID);
                if (json == null)
                    return null;
                var instance = MyJSON.Parse(PlayerData, json);
                return instance;
            }
            static Create() {
                let source = {
                    regions: []
                };
                var instance = new PlayerData(source);
                return instance;
            }
            static Save(playerID, data) {
                let json = MyJSON.Stringfy(data);
                PlayFab.Player.Data.ReadOnly.Write(playerID, API.World.ID, json);
            }
        }
        World.PlayerData = PlayerData;
        (function (PlayerData) {
            class Region {
                constructor($playerData, $index, source) {
                    this.$playerData = $playerData;
                    this.$index = $index;
                    this.name = source.name;
                    this.progress = new Region.Progress(source.progress);
                }
                Increment() {
                    this.progress.count += 1;
                }
                get playerData() { return this.$playerData; }
                get index() { return this.$index; }
                static Create(playerData, index, name, progress) {
                    let source = {
                        name: name,
                        progress: {
                            count: progress,
                            difficulty: API.Difficulty.Normal,
                        }
                    };
                    var instance = new Region(playerData, index, source);
                    return instance;
                }
            }
            PlayerData.Region = Region;
            (function (Region) {
                class Progress {
                    constructor(source) {
                        this.count = source.count;
                        this.difficulty = source.difficulty;
                    }
                    To(difficulty) {
                        this.count = 0;
                        this.difficulty = difficulty;
                    }
                }
                Region.Progress = Progress;
            })(Region = PlayerData.Region || (PlayerData.Region = {}));
            class Snapshot {
                constructor(data, region, occurrence, star) {
                    this.data = data;
                    this.region = region;
                    this.occurrence = occurrence;
                    this.star = star;
                }
                static Retrieve(args, template) {
                    let data = API.World.PlayerData.Retrieve(currentPlayerId);
                    let firstTime = false;
                    if (data == null) //first time for the player finishing any level
                     {
                        data = API.World.PlayerData.Create();
                        firstTime = true;
                    }
                    let region = data.Find(args.region);
                    if (region == null) {
                        if (template.region.previous != null) {
                            let previous = data.Find(template.region.previous.name);
                            if (previous == null)
                                throw ("trying to index region " + args.region + " without unlocking the previous region: " + template.region.previous.name);
                            else {
                                if (previous.progress.count < template.region.previous.size) {
                                    if (previous.progress.difficulty > API.Difficulty.Normal) {
                                    }
                                    else
                                        throw ("trying to index region " + args.region + " without finishing the previous region: " + template.region.previous.name);
                                }
                            }
                        }
                        region = data.Add(args.region);
                    }
                    if (args.difficulty > region.progress.difficulty) //player sending different difficulty than the one we have saved
                     {
                        if (region.progress.difficulty + 1 == args.difficulty) //this is directly the next difficulty
                         {
                            if (region.progress.count < template.region.size)
                                throw ("can't progress difficulty, region not completed at difficulty " + region.progress.difficulty + " yet");
                            region.progress.To(args.difficulty);
                        }
                        else //player trying to jump difficulty
                         {
                            throw ("can't change difficulty from " + region.progress.difficulty + " to " + args.difficulty);
                        }
                    }
                    if (args.level > region.progress.count) {
                        if (args.difficulty >= region.progress.difficulty) {
                            throw ("trying to complete level of index " + args.level + " without completing the previous levels");
                        }
                    }
                    let occurrence;
                    if (region.progress.count == args.level && args.difficulty == region.progress.difficulty)
                        occurrence = World.Level.Finish.Occurrence.Initial;
                    else
                        occurrence = World.Level.Finish.Occurrence.Recurring;
                    let star = null;
                    for (let i = 0; i < template.level.stars.length; i++) {
                        if (args.time <= template.level.stars[i].time)
                            star = template.level.stars[i];
                    }
                    return new Snapshot(data, region, occurrence, star);
                }
            }
            PlayerData.Snapshot = Snapshot;
        })(PlayerData = World.PlayerData || (World.PlayerData = {}));
    })(World = API.World || (API.World = {}));
})(API || (API = {}));
var API;
(function (API) {
    let World;
    (function (World) {
        class Template {
            constructor(instance) {
                this.regions = [];
                for (let i = 0; i < instance.regions.length; i++) {
                    let copy = new Template.Region(this, i, instance.regions[i]);
                    this.regions.push(copy);
                }
            }
            get size() { return this.regions.length; }
            Find(name) {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return this.regions[i];
                return null;
            }
        }
        World.Template = Template;
        (function (Template) {
            function Retrieve() {
                var json = PlayFab.Title.Data.Retrieve(API.World.ID);
                if (json == null)
                    throw ("no World Template data defined within PlayFab Title Data");
                var instance = MyJSON.Parse(Template, json);
                return instance;
            }
            Template.Retrieve = Retrieve;
            class Region {
                constructor($template, $index, instance) {
                    this.$template = $template;
                    this.$index = $index;
                    this.name = instance.name;
                    this.levels = [];
                    for (let i = 0; i < instance.levels.length; i++) {
                        let copy = new Region.Level(this, i, instance.levels[i]);
                        this.levels.push(copy);
                    }
                }
                get template() { return this.$template; }
                get index() { return this.$index; }
                get size() { return this.levels.length; }
                Find(index) {
                    if (index < 0)
                        return null;
                    if (index + 1 > this.levels.length)
                        return null;
                    return this.levels[index];
                }
                get previous() {
                    if (this.index == 0)
                        return null;
                    return this.template.regions[this.index - 1];
                }
                get next() {
                    if (this.index + 1 == this.template.size)
                        return null;
                    return this.template.regions[this.index + 1];
                }
                get isFirst() { return this.index == 0; }
                get isLast() { return this.index + 1 >= this.template.size; }
            }
            Template.Region = Region;
            (function (Region) {
                class Level {
                    constructor($region, $index, source) {
                        this.$region = $region;
                        this.$index = $index;
                        this.stars = [];
                        for (let i = 0; i < source.stars.length; i++) {
                            let instance = new Level.Star(i + 1, source.stars[i]);
                            this.stars.push(instance);
                        }
                        this.rewards = [];
                        for (let i = 0; i < source.rewards.length; i++) {
                            let instance = new Level.Reward(source.rewards[i]);
                            this.rewards.push(instance);
                        }
                    }
                    GetApplicableRewards(args, playerData) {
                        let result = [];
                        for (let i = 0; i < this.rewards.length; i++) {
                            if (this.rewards[i].IsApplicableTo(args, playerData))
                                result.push(this.rewards[i]);
                        }
                        return result;
                    }
                    get region() { return this.$region; }
                    get index() { return this.$index; }
                    get previous() {
                        if (this.index == 0)
                            return null;
                        return this.$region.levels[this.index - 1];
                    }
                    get next() {
                        if (this.index + 1 == this.region.size)
                            return null;
                        return this.region.levels[this.index + 1];
                    }
                    get isFirst() { return this.index == 0; }
                    get isLast() { return this.index + 1 >= this.region.size; }
                }
                Region.Level = Level;
                (function (Level) {
                    class Star {
                        constructor($rank, source) {
                            this.$rank = $rank;
                            this.time = source.time;
                        }
                        get rank() { return this.$rank; }
                        ;
                    }
                    Level.Star = Star;
                    class Reward {
                        constructor(source) {
                            this.data = source.data;
                            if (source.requirements == null)
                                this.requirements = undefined;
                            else
                                this.requirements = new Reward.Requirements(source.requirements);
                        }
                        IsApplicableTo(args, playerData) {
                            if (this.requirements == null)
                                return true;
                            return this.requirements.CompliesWith(args, playerData);
                        }
                    }
                    Level.Reward = Reward;
                    (function (Reward) {
                        class Requirements {
                            constructor(source) {
                                this.occurrence = source.occurrence;
                                this.difficulty = source.difficulty;
                                this.stars = source.stars;
                            }
                            IsValidOccurrence(target) {
                                if (this.occurrence == null)
                                    return true;
                                return this.occurrence.indexOf(target) >= 0;
                            }
                            IsValidDifficulty(target) {
                                if (this.difficulty == null)
                                    return true;
                                return this.difficulty.indexOf(target) >= 0;
                            }
                            IsValidStar(target) {
                                if (this.stars == null)
                                    return true;
                                if (target == null)
                                    return false;
                                return this.stars.indexOf(target.rank) >= 0;
                            }
                            CompliesWith(args, playerData) {
                                if (this.IsValidOccurrence(playerData.occurrence) == false)
                                    return false;
                                if (this.IsValidDifficulty(args.difficulty) == false)
                                    return false;
                                if (this.IsValidStar(playerData.star) == false)
                                    return false;
                                return true;
                            }
                        }
                        Reward.Requirements = Requirements;
                    })(Reward = Level.Reward || (Level.Reward = {}));
                })(Level = Region.Level || (Region.Level = {}));
            })(Region = Template.Region || (Template.Region = {}));
            class Snapshot {
                constructor(data, region, level) {
                    this.data = data;
                    this.region = region;
                    this.level = level;
                }
                static Retrieve(args) {
                    let data = API.World.Template.Retrieve();
                    let region = data.Find(args.region);
                    if (region == null)
                        throw (args.region + " region doesn't exist");
                    let level = region.Find(args.level);
                    if (level == null)
                        throw ("no level with index " + args.level + " defined in " + args.region + " region");
                    return new Snapshot(data, region, level);
                }
            }
            Template.Snapshot = Snapshot;
        })(Template = World.Template || (World.Template = {}));
    })(World = API.World || (API.World = {}));
})(API || (API = {}));
var PlayFab;
(function (PlayFab) {
    class Catalog {
        constructor(items) {
            if (items == null)
                this.items = [];
            else
                this.items = items;
        }
        FindWithID(itemID) {
            for (let i = 0; i < this.items.length; i++)
                if (CompareIgnoreCase(this.items[i].ItemId, itemID))
                    return this.items[i];
            return null;
        }
    }
    PlayFab.Catalog = Catalog;
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
    })(Catalog = PlayFab.Catalog || (PlayFab.Catalog = {}));
})(PlayFab || (PlayFab = {}));
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
                    if (CompareIgnoreCase(this.items[i].ItemId, itemID))
                        return this.items[i];
                return null;
            }
            FindWithInstanceID(itemInstanceID) {
                for (let i = 0; i < this.items.length; i++)
                    if (CompareIgnoreCase(this.items[i].ItemInstanceId, itemInstanceID))
                        return this.items[i];
                return null;
            }
            Contains(itemID) {
                if (this.FindWithID(itemID) == null)
                    return false;
                return true;
            }
            CompliesWith(requirement) {
                if (requirement == null)
                    return true;
                let instance = this.FindWithID(requirement.item);
                if (instance == null)
                    return false;
                if (instance.RemainingUses == null) {
                    if (requirement.count > 1)
                        return false;
                    else
                        return true;
                }
                return instance.RemainingUses >= requirement.count;
            }
            CompliesWithAll(requirements) {
                if (requirements == null || requirements.length == 0)
                    return true;
                for (let i = 0; i < requirements.length; i++)
                    if (this.CompliesWith(requirements[i]) == false)
                        return false;
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
                        log.info("item with ID " + stacks[i].item + " not found in inventory, cannot consume, skipping");
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
    })(Title = PlayFab.Title || (PlayFab.Title = {}));
})(PlayFab || (PlayFab = {}));
