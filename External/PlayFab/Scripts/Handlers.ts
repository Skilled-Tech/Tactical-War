handlers.LoginReward = function (args, context: IPlayFabContext)
{
    let template = API.Rewards.Template.Retrieve();

    let playerData = API.Rewards.PlayerData.Retrieve(currentPlayerId);

    let itemIDs = new Array<string>();

    if (playerData == null) //Signup
    {
        playerData = new API.Rewards.PlayerData.Instance();

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

    let playerData = API.World.PlayerData.Retrieve(currentPlayerId);
    try
    {
        API.World.PlayerData.Validate(playerData, world, args);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    let progress = playerData.Find(args.region).progress;

    if (args.level >= progress) //cheating... probably
    {
        log.error("trying to finish level " + args.level + " without finishing the previous level ");
        return;
    }

    let itemIDs = new Array<string>();

    var level = world.Find(args.region).levels[args.level];

    if (args.level == progress - 1) //Initial
    {
        log.info("Initial Completion");

        API.World.PlayerData.Incremenet(playerData, world, args);
        API.World.PlayerData.Save(currentPlayerId, playerData);

        let rewardItemIDs = API.Rewards.Grant(currentPlayerId, level.reward.initial, "Level Completion Award");
        itemIDs = itemIDs.concat(rewardItemIDs);
    }

    if (args.level < progress - 1) //Recurring
    {
        log.info("Recurring Completion");

        let rewardItemIDs = API.Rewards.Grant(currentPlayerId, level.reward.constant, "Level Completion Award");
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

    let catalog = PlayFab.Title.Catalog.Retrieve(itemInstance.CatalogVersion);
    let catalogItem = catalog.FindWithID(itemInstance.ItemId);

    let itemData = API.Upgrades.ItemData.Load(catalogItem);

    if (itemData == null)
    {
        log.error("Current Item Can't Be Upgraded");
        return;
    }

    let titleData = PlayFab.Title.Data.RetrieveAll([API.Upgrades.ID]);

    let template = API.Upgrades.Template.Find(titleData.Data[API.Upgrades.ID], itemData.template);

    if (template == null)
    {
        log.error(itemData.template + " Upgrades Template Not Defined");
        return;
    }

    if (template.Find(args.upgradeType) == null)
    {
        log.error(args.upgradeType + " Upgrade Type Not Defined");
        return;
    }

    let instanceData = API.Upgrades.InstanceData.Load(itemInstance);
    if (instanceData.Contains(args.upgradeType) == false) instanceData.Add(args.upgradeType);

    if (instanceData.Find(args.upgradeType).value >= template.Find(args.upgradeType).ranks.length)
    {
        log.error("Maximum Upgrade Level Achieved");
        return;
    }

    let rank = template.Match(args.upgradeType, instanceData);

    if (rank.requirements != null)
    {
        if (inventory.CompliesWithRequirements(rank.requirements) == false)
        {
            log.error("Player Doesn't Have The Required Items For the Upgrade");
            return;
        }
    }

    if (inventory.virtualCurrency[rank.cost.type] < rank.cost.value)
    {
        log.error("Insufficient Funds");
        return;
    }

    //Validation Completed, Start Processing Request
    PlayFab.Player.Currency.Subtract(currentPlayerId, rank.cost.type, rank.cost.value);

    PlayFab.Player.Inventory.ConsumeAll(inventory, rank.requirements);

    instanceData.Increment(args.upgradeType);

    API.Upgrades.InstanceData.Save(currentPlayerId, itemInstance, instanceData);

    return "Success";
}
interface IUpgradeItemArguments
{
    itemInstanceId: string;
    upgradeType: string;
}