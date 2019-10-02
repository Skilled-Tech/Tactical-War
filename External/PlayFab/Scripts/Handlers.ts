handlers.LoginReward = function (args?: any, context?: IPlayFabContext | undefined)
{
    AwardIfAdmin();

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
    let world: API.World.Template.SnapShot;
    try
    {
        world = new API.World.Template.SnapShot(args.region, args.level);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let playerData: API.World.PlayerData.SnapShot;
    try
    {
        playerData = new API.World.PlayerData.SnapShot(currentPlayerId, world, args.region);
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

    let template: API.Upgrades.Template.SnapShot;
    try
    {
        template = new API.Upgrades.Template.SnapShot(itemData, args.upgradeType);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let instanceData: API.Upgrades.InstanceData.SnapShot;
    try
    {
        instanceData = new API.Upgrades.InstanceData.SnapShot(itemInstance, itemData, args);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    if (instanceData.rank >= template.element.ranks.length)
    {
        log.error("cannot upgrade " + catalogItem.ItemId + "'s " + args.upgradeType + " any more");
        return;
    }

    let rank = template.element.ranks[instanceData.rank];

    if (rank == null)
    {
        log.error("no rank data found");
        return;
    }

    if (inventory.CompliesWith(rank.requirements) == false)
    {
        log.error("upgrade requirements not met");
        return;
    }

    PlayFab.Player.Currency.Subtract(currentPlayerId, rank.cost.type, rank.cost.value);

    PlayFab.Player.Inventory.ConsumeAll(inventory, rank.requirements);

    instanceData.Increment(args.upgradeType);

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