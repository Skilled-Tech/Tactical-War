namespace API
{
    export namespace Upgrades
    {
        export class ItemData
        {
            template?: string;
            applicable: string[];

            constructor(source: ItemData)
            {
                this.template = source.template;
                this.applicable = source.applicable;
            }

            public static Load(catalogItem: PlayFabServerModels.CatalogItem): ItemData | null
            {
                if (catalogItem == null) return null;

                if (catalogItem.CustomData == null) return null;

                let object = JSON.parse(catalogItem.CustomData);

                var element = object[ID];

                if (element == null) return null;

                let data = new ItemData(element);

                return data;
            }
        }
    }
}