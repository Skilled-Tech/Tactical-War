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

    let progress = playerData.progress;
    let itemIDs: Array<string>;

    if (reward == null)
    {
        itemIDs = [];
    }
    else
    {
        playerData.Progress(template);
        API.DailyReward.PlayerData.Save(currentPlayerId, playerData);

        itemIDs = API.Reward.Grant(currentPlayerId, reward, "Daily Login Reward");

        let result = new API.DailyReward.Result(progress, itemIDs);

        return result;
    }

    let result = new API.DailyReward.Result(progress, itemIDs);

    return result;
}

handlers.FinishLevel = function (args?: IFinishLevelArguments)
{
    if (args == null)
    {
        log.error("no arguments specified");
        return;
    }

    if (args.region == null)
    {
        log.error("no region argument specified");
        return;
    }

    if (args.level == null)
    {
        log.error("no level argument specified");
        return;
    }

    if (args.difficulty == null)
    {
        log.error("no difficulty argument specified");
        return;
    }

    if (args.difficulty in API.Difficulty == false)
    {
        log.error("no difficulty: " + args.difficulty + " defined");
        return;
    }

    let template: API.World.Template.Snapshot;
    try
    {
        template = API.World.Template.Snapshot.Retrieve(args);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let playerData: API.World.PlayerData.Snapshot;
    try
    {
        playerData = API.World.PlayerData.Snapshot.Retrieve(args, template);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    if (playerData.occurrence == API.World.Level.Finish.Occurrence.Initial) //First Time Completing Level
    {
        playerData.region.Increment();
        API.World.PlayerData.Save(currentPlayerId, playerData.data);
    }

    let rewards = template.level.GetApplicableRewards(args, playerData);
    let rewardsIDs = Array<string>();
    for (let i = 0; i < rewards.length; i++)
    {
        let itemIDs = API.Reward.Grant(currentPlayerId, rewards[i].data, "Level Completion " + playerData.occurrence + " Reward");

        rewardsIDs = rewardsIDs.concat(itemIDs);
    }

    let result: API.World.Level.Finish.Result = {
        rewards: rewardsIDs
    };

    return result;
}
interface IFinishLevelArguments
{
    region: string;
    level: number;
    difficulty: API.Difficulty;
}

handlers.UpgradeItem = function (args?: IUpgradeItemArguments)
{
    if (args == null)
    {
        log.error("no arguments specified");
        return;
    }

    if (args.upgradeType == null)
    {
        log.error("no upgrade type argument specified");
        return;
    }

    if (args.itemInstanceId == null)
    {
        log.error("no itemInstanceID argument specified");
        return;
    }

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

    let catalog = PlayFab.Catalog.Retrieve(itemInstance.CatalogVersion);

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

    let template: API.Upgrades.Template.Snapshot;
    try
    {
        template = API.Upgrades.Template.Snapshot.Retrieve(args, itemData);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let instanceData: API.Upgrades.InstanceData.Snapshot;
    try
    {
        instanceData = API.Upgrades.InstanceData.Snapshot.Retrieve(args, itemData, itemInstance, template);
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

    let result = new API.Upgrades.Result(true);

    return result;
}
interface IUpgradeItemArguments
{
    itemInstanceId: string;
    upgradeType: string;
}

handlers.Reward = function (args?: Array<string>)
{
    if (args == null)
    {
        log.error("no arguments specified");
        return;
    }

    PlayFab.Catalog.Item.GrantAll(currentPlayerId, args, "Reward");
}