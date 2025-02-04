function ClaimDailyReward(args?: any, context?: IPlayFabContext | undefined): API.DailyReward.Result
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

function FinishLevel(args?: IFinishLevelArguments)
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
        rewards: rewardsIDs,
        stars: playerData.star == null ? 0 : playerData.star.rank
    };

    return result;
}
interface IFinishLevelArguments
{
    region: string;
    level: number;
    difficulty: API.Difficulty;
    time: number;
}

function UpgradeItem(args?: IUpgradeItemArguments)
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

    let rank = {
        requirements: template.element.requirements[instanceData.element.value],
        cost: {
            value: template.element.cost.Calculate(instanceData.element.value + 1),
            type: "GD",
        }
    }

    log.info("rank:" + MyJSON.Stringfy(rank));

    if (inventory.CompliesWithAll(rank.requirements) == false)
    {
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
interface IUpgradeItemArguments
{
    itemInstanceId: string;
    upgradeType: string;
}

function Reward(args?: Array<string>)
{
    if (args == null)
    {
        log.error("no arguments specified");
        return;
    }

    PlayFab.Catalog.Item.GrantAll(currentPlayerId, args, "Reward");
}

function WelcomeNewPlayer(args?: IWelcomeNewPlayerArguments)
{
    var inventory = PlayFab.Player.Inventory.Retrieve(currentPlayerId);

    var template = API.NewPlayerReward.Template.Retrieve();

    if (inventory.Contains(template.token))
    {
        log.error("player " + currentPlayerId + " has already been rewarded a " + template.token);
        return;
    }

    var rewards = template.items.slice();
    rewards.push(template.token);

    PlayFab.Catalog.Item.GrantAll(currentPlayerId, rewards, "New Player Welcome Reward");

    var result: API.NewPlayerReward.Result = {
        items: rewards
    }

    return result;
}
interface IWelcomeNewPlayerArguments
{

}

function Register()
{
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