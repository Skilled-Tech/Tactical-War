// (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
// (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)

handlers.UpgradeItem = function(args)
{
    var arg = {
        itemInstanceID: args.ItemInstanceId,
        upgradeType: args.UpgradeType,
    }

    var inventory = GetInventory(currentPlayerId);
    var itemInstance = inventory.Items.find(x => x.ItemInstanceId === arg.itemInstanceID);
    
    if(itemInstance == null)
        return FormatError("Invalid Instance ID");

    var catalog = GetCatalog(itemInstance.CatalogVersion);
    var catalogItem = catalog.Items.find(x => x.ItemId == itemInstance.ItemId);

    var arguments = Upgrades.Arguments.Load(catalogItem);

    if(arguments == null)
        return FormatError("Current Item Can't Be Upgraded");

    var titleData = GetTitleData();

    var template = Upgrades.Template.Find(titleData[Upgrades.Name], arguments.Template);

    if(template == null)
        return FormatError(arguments.Template + " Upgrades Template Not Defined");

    if(template.Find(arg.upgradeType) == null)
        return FormatError(arg.upgradeType + " Upgrade Type Not Defined");

    var data = Upgrades.Data.Load(itemInstance);
    if(data.Contains(arg.upgradeType) == false) data.Add(arg.upgradeType);

    if(data.Find(arg.upgradeType).Value >= template.Find(arg.upgradeType).Ranks.length)
        return FormatError("Maximum Upgrade Level Achieved");

    var rank = template.Match(arg.upgradeType, data);

    if(inventory.VirtualCurrency[Upgrades.Currency] < rank.Cost)
        return FormatError("Insufficient Funds");

    SubtractCurrency(currentPlayerId, Upgrades.Currency, rank.Cost);

    data.Find(arg.upgradeType).Value++;

    UpdateUserInventoryItemData(currentPlayerId, itemInstance.ItemInstanceId, Upgrades.Name, data.ToJson());

    return { message: "Success" }
}

namespace Upgrades
{
    export const Name = "Upgrades";

    export const Currency = "JL";

    export namespace Data
    {
        export function Load(itemInstance : PlayFabServerModels.ItemInstance) : Instance
        {
            if(itemInstance.CustomData == null)
            {
                return new Instance();
            }
            else
            {
                if(itemInstance.CustomData[Upgrades.Name] == null)
                {
                    return new Instance();
                }
                else
                {
                    var object = JSON.parse(itemInstance.CustomData[Upgrades.Name]);

                    var instance = new Instance();

                    instance.Load(object);

                    return instance;
                }
            }
        }

        export class Instance
        {
            List : Element[];

            public Add(type : string)
            {
                this.List.push(new Element(type, 0));
            }

            public Contains(type : string) : boolean
            {
                for (var i = 0; i < this.List.length; i++)
                    if(this.List[i].Type == type)
                        return true;

                return false;
            }

            public Find(type : string) : Element
            {
                for (var i = 0; i < this.List.length; i++)
                    if(this.List[i].Type == type)
                        return this.List[i];

                return null;
            }

            public Load(object : object)
            {
                this.List = Object.assign([], object);
            }

            public ToJson() : string
            {
                return JSON.stringify(this.List);
            }

            constructor()
            {
                this.List = [];
            }
        }

        class Element
        {
            Type : string;
            Value : number;

            public Increament() : void
            {
                this.Value++;
            }

            constructor(name : string, value : number)
            {
                this.Type = name;
                this.Value = value;
            }
        }
    }

    export namespace Arguments
    {
        export const Default = "Default";

        export function Load(catalogItem : PlayFabServerModels.CatalogItem) : Instance
        {
            if(catalogItem == null) return null;

            if(catalogItem.CustomData == null) return null;

            var object = JSON.parse(catalogItem.CustomData);

            if(object[Name] == null)
            {

            }

            var data = Object.assign(new Instance(), object[Name]) as Instance;

            if(data.Template == null) data.Template = Default;

            return data;
        }

        export class Instance
        {
            Template: string;
            Applicable: string[];
        }
    }

    export namespace Template
    {
        export function Find(json : string, name : string) : Instance
        {
            if(json == null) return null;

            if(name == null) return null;

            var object = JSON.parse(json);
            
            var target = object.find(x => x.Name == name);
            
            var template = Object.assign(new Instance(), target);

            return template;
        }
        export function Parse(json : string) : Instance
        {
            var object = JSON.parse(json);

            var instance = Object.assign(new Instance(), object);

            return instance;
        }

        export class Instance
        {
            Name : string;
            Elements : Element[];

            Find(name : string) : Element
            {
                for (var i = 0; i < this.Elements.length; i++)
                    if(this.Elements[i].Type == name)
                        return this.Elements[i];
                
                return null;
            }

            Match(name : string, data : Upgrades.Data.Instance) : Rank
            {
                return this.Find(name).Ranks[data.Find(name).Value];
            }
        }
        
        export class Element
        {
            Type : string;
            Ranks : Rank[];
        }
        
        export class Rank
        {
            Cost : number;
            Percentage : number;
        }
    }
}

function GetInventory(playerID)
{
    var result = server.GetUserInventory(
        {
            PlayFabId: playerID,
        }
    );

    return {
        Items : result.Inventory,
        VirtualCurrency : result.VirtualCurrency
    };
}

function GetCatalog(version)
{
    var result = server.GetCatalogItems({
        CatalogVersion: version,
    });

    return {
        Items: result.Catalog,
    }
}

function GetTitleData()
{
    var result = server.GetTitleData({
        Keys : [Upgrades.Name],
    })

    return result.Data;
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