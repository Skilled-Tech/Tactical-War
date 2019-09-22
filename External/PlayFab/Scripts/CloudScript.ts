// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)

handlers.ProcessDailyReward = function (args, context: IPlayFabContext)
{
    let templates = API.DailyRewards.Templates.Retrieve();

    let data = API.DailyRewards.Data.Retrieve(currentPlayerId);

    if (data == null)
    {
        data = new API.DailyRewards.Data.Instance();
    }
    else
    {
        let daysFromLastReward = Utility.Dates.DaysFrom(Date.parse(data.lastLogin));

        if (daysFromLastReward < 1)
        {
            return;
        }
        if (daysFromLastReward >= 2)
        {
            data.progress = 0;
        }
        else
        {

        }
    }

    data.lastLogin = new Date().toJSON();

    let items = API.Reward.Grant(currentPlayerId, templates[data.progress], "Daily Reward");

    var result = new API.DailyRewards.Result(data.progress, items);

    API.DailyRewards.Data.Incremenet(data, templates);

    API.DailyRewards.Data.Save(currentPlayerId, data);

    return result;
}

handlers.FinishLevel = function (args: IFinishLevelArguments)
{
    let world = API.World.Template.Retrieve();
    try
    {
        API.World.Template.Validate(world, args);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let data = API.World.Data.Retrieve(currentPlayerId);
    try
    {
        API.World.Data.Validate(data, world, args);
    }
    catch (error)
    {
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

    let IDs: string[] = [];

    var level = world.Find(args.region).levels[args.level];

    if (args.level == progress - 1) //Initial
    {
        log.info("Initial Completion");

        API.World.Data.Incremenet(data, world, args);
        PlayFab.Player.Data.ReadOnly.Write(currentPlayerId, API.World.Name, JSON.stringify(data));

        let items = API.Reward.Grant(currentPlayerId, level.reward.initial, "Level Completion Award");
        IDs = IDs.concat(items);
    }

    if (args.level < progress - 1) //Recurring
    {
        log.info("Recurring Completion");

        let items = API.Reward.Grant(currentPlayerId, level.reward.constant, "Level Completion Award");
        IDs = IDs.concat(items);
    }

    return IDs;
}
interface IFinishLevelArguments
{
    region: string;
    level: number;
}

handlers.UpgradeItem = function (args: IUpgradeItemArguments)
{
    let inventory = PlayFab.Player.Inventory.Retrieve(currentPlayerId);
    let itemInstance = inventory.FindWithInstanceID(args.itemInstanceId);

    if (itemInstance == null)
    {
        log.error(args.itemInstanceId + " is an Invalid Instance ID");
        return;
    }

    let catalog = PlayFab.Title.Catalog.Retrieve(itemInstance.CatalogVersion);
    let catalogItem = catalog.FindWithID(itemInstance.ItemId);

    let arguments = API.Upgrades.Arguments.Load(catalogItem);

    if (arguments == null)
    {
        log.error("Current Item Can't Be Upgraded");
        return;
    }

    let titleData = PlayFab.Title.Data.RetrieveAll([API.Upgrades.Name]);

    let template = API.Upgrades.Template.Find(titleData.Data[API.Upgrades.Name], arguments.template);

    if (template == null)
    {
        log.error(arguments.template + " Upgrades Template Not Defined");
        return;
    }

    if (template.Find(args.upgradeType) == null)
    {
        log.error(args.upgradeType + " Upgrade Type Not Defined");
        return;
    }

    let data = API.Upgrades.Data.Load(itemInstance);
    if (data.Contains(args.upgradeType) == false) data.Add(args.upgradeType);

    if (data.Find(args.upgradeType).value >= template.Find(args.upgradeType).ranks.length)
    {
        log.error("Maximum Upgrade Level Achieved");
        return;
    }

    let rank = template.Match(args.upgradeType, data);

    if (rank.requirements != null)
    {
        if (inventory.CompliesWithRequirements(rank.requirements) == false)
        {
            log.error("Player Doesn't The Required Items For the Upgrade");
            return;
        }
    }

    if (inventory.virtualCurrency[rank.cost.type] < rank.cost.value)
    {
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
}
interface IUpgradeItemArguments
{
    itemInstanceId: string;
    upgradeType: string;
}

namespace API
{
    export namespace DailyRewards
    {
        export const Name = "daily-rewards";

        export namespace Templates
        {
            export function Retrieve(): API.Reward.Data[]
            {
                var json = PlayFab.Title.Data.Retrieve(DailyRewards.Name);

                var object = JSON.parse(json);

                var instance = Object.assign([], object);

                return instance;
            }
        }

        export namespace Data
        {
            export function Retrieve(playerID: string): Instance
            {
                let result = PlayFab.Player.Data.ReadOnly.Read(playerID, DailyRewards.Name);

                if (result.Data == null)
                {

                }
                else
                {
                    if (result.Data[DailyRewards.Name] == null)
                    {

                    }
                    else
                    {
                        let json = result.Data[DailyRewards.Name].Value;

                        let object = JSON.parse(json);

                        let instance = Object.assign(new Instance(), object);

                        return instance;
                    }
                }

                return null;
            }

            export function Incremenet(data: Instance, templates: API.Reward.Data[])
            {
                data.progress++;

                if (data.progress >= templates.length)
                    data.progress = 0;
            }

            export function Save(playerID: string, data: Instance)
            {
                PlayFab.Player.Data.ReadOnly.Write(playerID, DailyRewards.Name, JSON.stringify(data));
            }

            export class Instance
            {
                lastLogin: string;
                progress: number;

                constructor()
                {
                    this.lastLogin = new Date().toJSON();

                    this.progress = 0;
                }
            }
        }

        export class Result
        {
            progress: number;
            items: string[];

            constructor(progress: number, items: string[])
            {
                this.progress = progress;
                this.items = items;
            }
        }
    }

    export namespace World
    {
        export const Name = "world";

        export namespace Data
        {
            export function Retrieve(playerID: string): Instance
            {
                let playerData = PlayFab.Player.Data.ReadOnly.ReadAll(playerID, [Name]);

                if (playerData.Data[Name] == null)
                    return new Instance();

                let json = playerData.Data[Name].Value;

                let object = JSON.parse(json);

                let instance = Object.assign(new Instance(), object);

                return instance;
            }

            export function Validate(data: Instance, world: Template.Data, args: IFinishLevelArguments)
            {
                if (data.Contains(args.region))
                {

                }
                else
                {
                    if (args.region == world.regions[0].name)
                    {
                        let instance = new Region(args.region, 1);

                        data.Add(instance);
                    }
                    else
                    {
                        throw "Can't begin data indexing from " + args.region + " region";
                    }
                }
            }

            export function Incremenet(data: Instance, world: Template.Data, args: IFinishLevelArguments)
            {
                let progress = data.Find(args.region).progress++;

                let region = world.Find(args.region);

                if (progress == region.levels.length) //Completed All Levels
                {
                    let index = world.IndexOf(region.name);

                    if (index >= world.regions.length - 1) //Completed All Regions
                    {

                    }
                    else
                    {
                        let next = world.regions[index + 1];

                        let instance = new Region(next.name, 1);

                        data.Add(instance);
                    }
                }
            }

            export class Instance
            {
                regions: Region[];

                Add(region: Region)
                {
                    this.regions.push(region);
                }

                Contains(name: string): boolean
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return true;

                    return false;
                }

                Find(name: string): Region
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return this.regions[i];

                    return null;
                }

                constructor()
                {
                    this.regions = [];
                }
            }

            export class Region
            {
                name: string;
                progress: number;

                constructor(name: string, progress: number)
                {
                    this.name = name;
                    this.progress = progress;
                }
            }
        }

        export namespace Template
        {
            export function Retrieve(): Data
            {
                let titleData = PlayFab.Title.Data.RetrieveAll([Name]);

                let json = titleData.Data[Name];

                let object = JSON.parse(json);

                let data = Object.assign(new Data(), object);

                return data;
            }

            export function Validate(data: Data, args: IFinishLevelArguments)
            {
                let region = data.Find(args.region);

                if (region == null)
                {
                    throw args.region + " region doesn't exist";
                }
                else
                {
                    if (args.level >= 0 && args.level < region.levels.length)
                    {
                        return;
                    }
                    else
                    {
                        throw "Level " + args.level + " on " + args.region + " region doesn't exist";
                    }
                }
            }

            export class Data
            {
                regions: Region[];

                public Find(name: string)
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return this.regions[i];

                    return null;
                }

                public Contains(name: string): boolean
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return true;

                    return false;
                }

                public IndexOf(name: string): number
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return i;

                    return null;
                }
            }

            export class Region
            {
                name: string;
                levels: Level[];
            }

            export class Level
            {
                reward: Rewards;
            }

            export class Rewards
            {
                initial: API.Reward.Data;
                constant: API.Reward.Data;
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
                let instance = new Instance;
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
                        let object = JSON.parse(itemInstance.CustomData[Upgrades.Name]);

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
                    for (let i = 0; i < this.list.length; i++)
                        if (this.list[i].type == type)
                            return true;

                    return false;
                }

                public Find(type: string): Element
                {
                    for (let i = 0; i < this.list.length; i++)
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

                let object = JSON.parse(catalogItem.CustomData);

                if (object[Name] == null)
                {

                }

                let data = Object.assign(new Instance(), object[Name]) as Instance;

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

                let object = JSON.parse(json);

                let target = object.find(x => x.name == name);

                let template = Object.assign(new Instance(), target);

                return template;
            }
            export function Parse(json: string): Instance
            {
                let object = JSON.parse(json);

                let instance = Object.assign(new Instance(), object);

                return instance;
            }

            export class Instance
            {
                name: string;
                elements: Element[];

                Find(name: string): Element
                {
                    for (let i = 0; i < this.elements.length; i++)
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
                cost: API.Cost.Data;
                percentage: number;
                requirements: API.ItemRequirement.Data[]
            }
        }
    }

    export namespace Reward
    {
        export function Grant(playerID: string, data: Data, annotation: string): string[]
        {
            let IDs = Array<string>();

            if (data.items == null)
            {

            }
            else
            {
                IDs = IDs.concat(data.items);
            }

            if (data.droptable == null)
            {

            }
            else
            {
                let result = PlayFab.Title.Catalog.Tables.Process(data.droptable);
                if (result != null)
                    IDs = IDs.concat(result);
            }

            PlayFab.Title.Catalog.Item.GrantAll(playerID, IDs, annotation);

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

    export namespace Cost
    {
        export class Data
        {
            type: string;
            value: number;
        }
    }

    export namespace ItemRequirement
    {
        export function ConsumeAll(inventory: PlayFab.Player.Inventory.Data, requirements: Data[])
        {
            if (requirements == null) return;

            for (let i = 0; i < requirements.length; i++)
            {
                let itemInstance = inventory.FindWithID(requirements[i].item);

                PlayFab.Player.Inventory.Consume(currentPlayerId, itemInstance.ItemInstanceId, requirements[i].count);
            }
        }

        export class Data
        {
            item: string;
            count: number;
        }
    }
}

namespace Utility
{
    export namespace Dates
    {
        export function DaysFrom(date: any): number
        {
            return DaysBetween(date, new Date());
        }

        export function DaysBetween(date1: Date, date2: Date): number
        {
            return Math.round((date2.valueOf() - date1.valueOf()) / (86400000));
        }
    }
}

namespace PlayFab
{
    export namespace Player
    {
        export namespace Inventory
        {
            export function Retrieve(playerID: string): Data
            {
                let result = server.GetUserInventory(
                    {
                        PlayFabId: playerID,
                    }
                );

                return new Data(result.Inventory, result.VirtualCurrency);
            }

            export function Consume(playerID: string, itemInstanceID, count: number)
            {
                let result = server.ConsumeItem({
                    PlayFabId: playerID,
                    ItemInstanceId: itemInstanceID,
                    ConsumeCount: count,
                });
            }

            export function UpdateItemData(playerID, itemInstanceID, key, value)
            {
                let data = {};

                data[key] = value;

                let request = server.UpdateUserInventoryItemCustomData({
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

                public CompliesWithRequirements(requirements: API.ItemRequirement.Data[]): boolean
                {
                    for (let i = 0; i < requirements.length; i++)
                    {
                        let instance = this.FindWithID(requirements[i].item);

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

        export namespace Currency
        {
            export function Subtract(playerID, currency: string, ammout: number)
            {
                let request = server.SubtractUserVirtualCurrency({
                    PlayFabId: playerID,
                    VirtualCurrency: currency,
                    Amount: ammout
                });
            }
        }

        export namespace Data
        {
            export namespace ReadOnly
            {
                export function ReadAll(playerID: string, keys: string[]): PlayFabServerModels.GetUserDataResult
                {
                    let result = server.GetUserReadOnlyData({
                        PlayFabId: playerID,
                        Keys: keys
                    });

                    return result;
                }

                export function Read(playerID: string, key: string): PlayFabServerModels.GetUserDataResult
                {
                    var result = ReadAll(playerID, [key]);

                    return result;
                }

                export function Write(playerID: string, key: string, value: string)
                {
                    let data = {};

                    data[key] = value;

                    server.UpdateUserReadOnlyData({
                        PlayFabId: playerID,
                        Data: data,
                    })
                }
            }
        }
    }

    export namespace Title
    {
        export namespace Data
        {
            export function RetrieveAll(keys: string[]): PlayFabServerModels.GetTitleDataResult
            {
                let result = server.GetTitleData({
                    Keys: keys,
                })

                return result;
            }

            export function Retrieve(key: string): string
            {
                var result = RetrieveAll([key]);

                return result.Data[key];
            }
        }

        export namespace Catalog
        {
            export const Default = "Default";

            export function Retrieve(version: string): Data
            {
                let result = server.GetCatalogItems(
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

            export namespace Item
            {
                export function Grant(playerID: string, itemID, ammount: number, annotation: string, ): Array<PlayFabServerModels.ItemInstance>
                {
                    let items = [];

                    for (let i = 0; i < ammount; i++)
                        items.push(itemID);

                    return GrantAll(playerID, items, annotation);
                }

                export function GrantAll(playerID: string, itemIDs: string[], annotation: string): Array<PlayFabServerModels.ItemInstance>
                {
                    if (itemIDs == null || itemIDs.length == 0) return [];

                    let result = server.GrantItemsToUser({
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
                    let result = server.EvaluateRandomResultTable({
                        CatalogVersion: Catalog.Default,
                        TableId: tableID
                    });

                    return result.ResultItemId;
                }

                export function Process(table: API.Reward.DropTable): Array<string>
                {
                    let items = Array<string>();

                    for (let i = 0; i < table.iterations; i++)
                    {
                        let item = Evaluate(table.ID);

                        items.push(item);
                    }

                    return items;
                }
            }
        }
    }
}