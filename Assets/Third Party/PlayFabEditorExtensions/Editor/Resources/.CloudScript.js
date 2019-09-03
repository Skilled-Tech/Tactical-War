Catalogs = {
    Units : "Units"
}

handlers.PurchaseUnit = function(args, context)
{
    return {"Hello" : "World"};
}

handlers.OnItemPurchased = function (args, context)
{
    // (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
    var event = context.playStreamEvent;

    // (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)
    var profile = context.playerProfile;

    // Auto Grant Characters
    if(event.CatalogVersion == Catalogs.Units)
    {
        var request = {
            PlayFabId: profile.PlayerId,
            ItemId: event.ItemId,
            CatalogVersion: event.CatalogVersion,
            CharacterName: event.ItemId,
            CharacterType: "Unit",
        };
    
        var reponse = server.GrantCharacterToUser(request);

        return { name : request.CharacterName };
    }
};