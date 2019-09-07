// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)

Constants = {
    Units : {
        Currency: "JL",
        Catalog: "Units",
    },
    Upgrades : {
        Name: "Upgrades",
        Template: {
            Default: "Default",
            Override: "Upgrades_Template",
        },
        Currency: "JL",
    }
}

handlers.UpgradeItem = function(args)
{
    var inventory = GetInventory(currentPlayerId);
    var itemInstance = inventory.Items.find(x => x.ItemInstanceId === args.ItemInstanceId);
    
    if(itemInstance == null)
        return FormatError("Invalid Instance ID");

    var catalog = GetCatalog(itemInstance.CatalogVersion);
    var catalogItem = catalog.Items.find(x => x.ItemId == itemInstance.ItemId);

    var upgradeTemplate = GetUpgradeTemplate(catalogItem);

    if(upgradeTemplate.Types[args.UpgradeType] == null)
        return FormatError("Upgrade Type Isn't Defined Within The Upgrade Template");

    var upgradesData = GetItemInstanceUpgradeData(itemInstance, args.UpgradeType);

    if(upgradesData[args.UpgradeType] >= upgradeTemplate.Types[args.UpgradeType].Ranks.length)
        return FormatError("Maximum Upgrade Level Achieved");
    
    var rank = upgradeTemplate.Types[args.UpgradeType].Ranks[upgradesData[args.UpgradeType]];

    if(inventory.VirtualCurrency[Constants.Upgrades.Currency] < rank.Cost)
        return FormatError("Insufficient Funds");

    SubtractCurrency(currentPlayerId, Constants.Upgrades.Currency, rank.Cost);

    upgradesData[args.UpgradeType]++;

    UpdateUserInventoryItemData(currentPlayerId, itemInstance.ItemInstanceId, Constants.Upgrades.Name, JSON.stringify(upgradesData));

    return { message: "Success" }
}

function GetInventory(playerID)
{
    var request = server.GetUserInventory(
        {
            PlayFabId: playerID,
        }
    );

    return {
        Items : request.Inventory,
        VirtualCurrency : request.VirtualCurrency
    };
}

function GetCatalog(version)
{
    var request = server.GetCatalogItems({
        CatalogVersion: version,
    });

    return {
        Items: request.Catalog,
    }
}

function GetUpgradeTemplate(catalogItem)
{
    var request = server.GetTitleInternalData({
        keys: [Constants.Upgrades.Name]
    });

    var templates = JSON.parse(request.Data[Constants.Upgrades.Name]);

    if(catalogItem.CustomData != null)
    {
        if(catalogItem.CustomData[Constants.Upgrades.Template.Override] != null)
        {
            return templates[catalogItem.CustomData[Constants.Upgrades.Template.Override]];
        }
    }

    return templates[Constants.Upgrades.Template.Default];
}

function GetItemInstanceUpgradeData(itemInstance, type)
{
    var json = itemInstance.CustomData[Constants.Upgrades.Name];
    
    var upgrades = json == null ? {} : JSON.parse(json);

    if(upgrades[type] == null)
        upgrades[type] = 0;

    return upgrades;
}

function SubtractCurrency(playerID, currency, ammout)
{
    var request = server.SubtractUserVirtualCurrency({
        PlayFabId: playerID,
        VirtualCurrency: currency,
        Amount: ammout
    });
}

function UpdateUserInventoryItemData(playerID, itemInstanceID, key, value)
{
    var data = {};
    
    data[key] = value;

    var request = server.UpdateUserInventoryItemCustomData({
        PlayFabId: playerID,
        ItemInstanceId: itemInstanceID,
        Data: data
    });
}

function FormatError(message)
{
    return message;
}