handlers.ClaimDailyReward = function (args?: any, context?: IPlayFabContext | undefined): API.DailyReward.Result
{
    let template = API.DailyReward.Template.Retrieve();

    let playerData = API.DailyReward.PlayerData.Retrieve(currentPlayerId);

    let reward: API.Reward | null = null;

    if (playerData == null) //First time claiming daily reward
    {
        playerData = API.DailyReward.PlayerData.Create();

        reward = template.Get(0);
    }
    else
    {
        var daysFromLastReward = Utility.Dates.DaysFrom(playerData.datestamp);

        if (daysFromLastReward >= 1)
        {
            if (daysFromLastReward >= 2)
                playerData.progress = 0;

            reward = template.Get(playerData.progress);
        }
    }

    let itemIDs = Array<string>()

    if (reward == null)
    {
        return new API.DailyReward.Result(playerData.progress, []);
    }
    else
    {
        let progress = playerData.progress;

        playerData.Progress(template);
        API.DailyReward.PlayerData.Update(currentPlayerId, playerData);

        let itemIDs = API.Reward.Grant(currentPlayerId, reward, "Daily Login Reward");

        let result = new API.DailyReward.Result(progress, itemIDs);

        return result;
    }
}

handlers.FinishLevel = function (args?: IFinishLevelArguments)
{
    if (args == null)
        throw "no arguments specified";

    if (args.region == null)
        throw "no level argument specified";

    if (args.level == null)
        throw "no level argument specified";

    function FormatTemplate(args: IFinishLevelArguments): ITemplateSnapshot
    {
        let data = API.World.Template.Retrieve();

        let region = data.Find(args.region);

        if (region == null)
            throw args.region + " region doesn't exist";

        let level = region.Find(args.level);

        if (level == null)
            throw "no level with index " + args.level + " defined in " + args.region + " region";

        return {
            data: data,
            region: region,
            level: level,
        };
    }
    interface ITemplateSnapshot
    {
        data: API.World.Template,
        region: API.World.Template.Region,
        level: API.World.Template.Region.Level,
    }

    try
    {
        var template = FormatTemplate(args);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    function FormatPlayerData(args: IFinishLevelArguments, template: ITemplateSnapshot): IPlayerDataSnapshot
    {
        let data = API.World.PlayerData.Retrieve(currentPlayerId);
        let firstTime = false;
        if (data == null) //first time for the player finishing any level
        {
            data = API.World.PlayerData.Create();

            firstTime = true;
        }

        let region = data.Find(args.region);
        if (region == null)
        {
            if (template.region.previous == null) //this is the first level
            {

            }
            else
            {
                var previous = data.Find(template.region.previous.name);

                if (previous == null)
                    throw "trying to index region " + args.region + " without unlocking the previous region: " + template.region.previous.name;

                if (previous.progress < template.region.previous.size)
                    throw "trying to index region " + args.region + " without finishing the previous region: " + template.region.previous.name;
            }

            region = data.Add(args.region);
        }

        if (args.level > region.progress)
            throw "trying to complete level of index " + args.level + " without completing the previous levels";

        return {
            data: data,
            region: region
        };
    }
    interface IPlayerDataSnapshot
    {
        data: API.World.PlayerData;
        region: API.World.PlayerData.Region;
    }

    try
    {
        var playerData = FormatPlayerData(args, template);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let rewardItemIDs: Array<string> = [];

    if (playerData.region.progress == args.level) //Initial Completion
    {
        playerData.region.progress += 1;
        API.World.PlayerData.Save(currentPlayerId, playerData.data);

        let IDs = API.Reward.Grant(currentPlayerId, template.level.reward.initial, "Level Completion Reward");

        rewardItemIDs = rewardItemIDs.concat(IDs);
    }
    else //Recurring Completion
    {
        let IDs = API.Reward.Grant(currentPlayerId, template.level.reward.recurring, "Level Completion Reward");

        rewardItemIDs = rewardItemIDs.concat(IDs);
    }

    return rewardItemIDs;
}
interface IFinishLevelArguments
{
    region: string;
    level: number;
}

handlers.UpgradeItem = function (args?: IUpgradeItemArguments)
{
    if (args == null)
        throw "no arguments specified";

    if (args.upgradeType == null)
        throw "no upgrade type argument specified";

    if (args.itemInstanceId == null)
        throw "no itemInstanceID argument specified"

    let inventory = PlayFab.Player.Inventory.Retrieve(currentPlayerId);
    let itemInstance = inventory.FindWithInstanceID(args.itemInstanceId);

    if (itemInstance == null)
    {
        log.error("no inventory item found with instanceID: " + args.itemInstanceId);
        return;
    }

    if (itemInstance.CatalogVersion == null)
    {
        log.error("itemInstance has no catalog version defined");
        return;
    }

    if (itemInstance.ItemId == null)
    {
        log.error("itemInstance has no itemID value defined");
        return;
    }

    let catalog = PlayFab.Title.Catalog.Retrieve(itemInstance.CatalogVersion);
    let catalogItem = catalog.FindWithID(itemInstance.ItemId);

    if (catalogItem == null)
    {
        log.error("no catalog item relating to itemID " + itemInstance.ItemId + " was found in catalog version " + itemInstance.CatalogVersion);
        return;
    }

    let itemData = API.Upgrades.ItemData.Load(catalogItem);

    if (itemData == null)
    {
        log.error("item: " + catalogItem.ItemId + " Cannot be upgraded");
        return;
    }

    //Template
    function FormatTemplate(args: IUpgradeItemArguments, itemData: API.Upgrades.ItemData): ITemplateSnapshot
    {
        let data: API.Upgrades.Template | null;

        if (itemData.template == null)
        {
            data = API.Upgrades.Template.GetDefault();
        }
        else
        {
            data = API.Upgrades.Template.Find(itemData.template);

            if (data == null)
                throw "no " + itemData.template + " upgrade template defined";
        }

        let element = data.Find(args.upgradeType);

        if (element == null)
            throw "upgrade type " + args.upgradeType + " not defined within " + data.name + " upgrade template";

        return {
            data: data,
            element: element,
        };
    }
    interface ITemplateSnapshot
    {
        data: API.Upgrades.Template,
        element: API.Upgrades.Template.Element;
    }

    try
    {
        var template = FormatTemplate(args, itemData);
    }
    catch (error)
    {
        log.error(error);
        return;
    }


    //Instance Data
    function FormatInstanceData(args: IUpgradeItemArguments, itemData: API.Upgrades.ItemData, itemInstance: PlayFabServerModels.ItemInstance, template: ITemplateSnapshot): IInstanceDataSnapshot
    {
        let data = API.Upgrades.InstanceData.Load(itemInstance);

        if (data == null) //First time the player is upgrading this item
        {
            data = API.Upgrades.InstanceData.Create();
        }

        if (itemData.IsApplicable(args.upgradeType) == false)
            throw args.upgradeType + " upgrade is not applicable to " + itemInstance.ItemId;

        let element = data.Find(args.upgradeType);

        if (element == null) //First time the player is upgrading this property
        {
            element = data.Add(args.upgradeType);
        }

        if (element.value >= template.element.ranks.length)
            throw "can't upgrade " + itemInstance.ItemId + " any further";

        return {
            data: data,
            element: element
        };
    }
    interface IInstanceDataSnapshot
    {
        data: API.Upgrades.InstanceData;
        element: API.Upgrades.InstanceData.Element;
    }

    try
    {
        var instanceData = FormatInstanceData(args, itemData, itemInstance, template);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let rank = template.element.ranks[instanceData.element.value];

    if (rank == null)
    {
        log.error("rank of index " + instanceData.element.value + " can't be null");
        return;
    }

    if (inventory.CompliesWith(rank.requirements) == false)
    {
        log.error("inventory doesn't comply with upgrade requirements");
        return;
    }

    PlayFab.Player.Currency.Subtract(currentPlayerId, rank.cost.type, rank.cost.value);

    PlayFab.Player.Inventory.ConsumeAll(inventory, rank.requirements);

    instanceData.element.Increment();

    API.Upgrades.InstanceData.Save(currentPlayerId, itemInstance, instanceData.data);
}
interface IUpgradeItemArguments
{
    itemInstanceId: string;
    upgradeType: string;
}

function AwardIfAdmin()
{
    if (currentPlayerId == "56F63F9E4A7E88D")
    {
        PlayFab.Title.Catalog.Item.Grant(currentPlayerId, "Wood_Sword", 5, "Admin Bonus");
        PlayFab.Title.Catalog.Item.Grant(currentPlayerId, "Wood_Shield", 5, "Admin Bonus");
    }
}