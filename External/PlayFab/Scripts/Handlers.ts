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
        var playerData = FormatPlayerData(template);
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
        log.error("no catalog item relating to itemID " + itemInstance.ItemId + " was found in catalog version " + itemInstance.CatalogVersion);
        return;
    }

    function FormatTemplate(itemData: API.Upgrades.ItemData): ITemplateSnapshot
    {
        let data: API.Upgrades.Template;

        if (itemData.template == null)
        {
            data = API.Upgrades.Template.GetDefault();
        }
        else
        {
            let result = API.Upgrades.Template.Find(itemData.template);

            if (result == null)
                throw "no " + itemData.template + " upgrade template defined";

            data = result;
        }


        return {
            data: data
        };
    }
    interface ITemplateSnapshot
    {
        data: API.Upgrades.Template,
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
        var playerData = FormatPlayerData(template);
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

function AwardIfAdmin()
{
    if (currentPlayerId == "56F63F9E4A7E88D")
    {
        PlayFab.Title.Catalog.Item.Grant(currentPlayerId, "Wood_Sword", 5, "Admin Bonus");
        PlayFab.Title.Catalog.Item.Grant(currentPlayerId, "Wood_Shield", 5, "Admin Bonus");
    }
}