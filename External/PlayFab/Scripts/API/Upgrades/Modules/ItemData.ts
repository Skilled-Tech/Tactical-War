namespace API
{
    export namespace Upgrades
    {
        export class ItemData
        {
            template?: string;
            applicable: string[];

            public IsApplicable(type: string): boolean
            {
                for (let i = 0; i < this.applicable.length; i++)
                    if (this.applicable[i] == type)
                        return true;

                return false;
            }

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