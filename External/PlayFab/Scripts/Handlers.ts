handlers.LoginReward = function (args?: any, context?: IPlayFabContext | undefined)
{
    let template = API.Rewards.Template.Retrieve();

    let playerData = API.Rewards.PlayerData.Retrieve(currentPlayerId);

    let itemIDs = new Array<string>();

    if (playerData == null) //Signup
    {
        playerData = API.Rewards.PlayerData.Create();

        var signupItems = API.Rewards.Grant(currentPlayerId, template.signup, "Signup Reward");

        itemIDs = itemIDs.concat(signupItems);
    }
    else //Recurring
    {
        let daysFromLastReward = Utility.Dates.DaysFrom(Date.parse(playerData.daily.timestamp));

        if (daysFromLastReward < 1)
        {
            return;
        }
        if (daysFromLastReward >= 2)
        {
            playerData.daily.progress = 0;
        }
        else
        {

        }
    }

    let dailyItems = API.Rewards.Grant(currentPlayerId, template.daily[playerData.daily.progress], "Daily Reward");

    itemIDs = itemIDs.concat(dailyItems);

    var result = new API.Rewards.Result(playerData.daily.progress, itemIDs);

    API.Rewards.PlayerData.Daily.Incremenet(playerData, template);

    API.Rewards.PlayerData.Update(currentPlayerId, playerData);

    return result;
}

handlers.FinishLevel = function (args: IFinishLevelArguments)
{
    try
    {
        var world = new API.World.Template.Instance(args.region, args.level);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    try
    {
        var playerData = new API.World.PlayerData.Instance(currentPlayerId, world, args.region);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    if (args.level >= playerData.progress) //cheating... probably
    {
        log.error("trying to finish level " + args.level + " without finishing the previous level ");
        return;
    }

    let itemIDs = new Array<string>();

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
        log.error("no catalog item relating to " + itemInstance.ItemId + " was found in catalog version " + itemInstance.CatalogVersion);
        return;
    }

    var itemData = API.Upgrades.ItemData.Load(catalogItem);

    if (itemData == null)
    {
        log.error(catalogItem.ItemId + " catalog item has no upgrade data");
        return;
    }

    try
    {
        var template = new API.Upgrades.Template.Instance(itemData, args.upgradeType);
    }
    catch (error)
    {

    }

    try
    {
        var instanceData = new API.Upgrades.InstanceData.Instance(itemInstance, args);
    }
    catch (error)
    {
        log.error(error);
        return;
    }
}
interface IUpgradeItemArguments
{
    itemInstanceId: string;
    upgradeType: string;
}