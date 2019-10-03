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
    function FormatTemplate(): ITemplateSnapshot
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
        var template = FormatTemplate();
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    function FormatPlayerData(template: ITemplateSnapshot): IPlayerDataSnapshot
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
                    throw "trying to index region " + args.region + " without unlocking the previous region of : " + template.region.previous.name;

                if (previous.progress < template.region.previous.size)
                    throw "trying to index region " + args.region + " without finishing the previous region of : " + template.region.previous.name;
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
        var playerData = FormatPlayerData(template);
    }
    catch (error)
    {
        log.error(error);
        return;
    }

    log.info(MyJSON.Write(template));
    log.info(MyJSON.Write(playerData));
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