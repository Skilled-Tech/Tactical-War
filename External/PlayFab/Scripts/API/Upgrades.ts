namespace API
{
    export namespace Upgrades
    {
        export const ID = "upgrades";

        export class InstanceData
        {
            list: Array<InstanceData.Element>;

            public Add(type: string): InstanceData.Element
            {
                var element = new InstanceData.Element(type, 0);

                this.list.push(element);

                return element;
            }

            public Contains(type: string): boolean
            {
                for (let i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return true;

                return false;
            }

            public Find(type: string): InstanceData.Element | null
            {
                for (let i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return this.list[i];

                return null;
            }

            public Increment(type: string): InstanceData.Element | null
            {
                let element = this.Find(type);

                if (element == null)
                    return null;
                else
                {
                    element.value++;
                    return element;
                }
            }

            public ToJson(): string
            {
                return JSON.stringify(this.list);
            }

            constructor(list: Array<InstanceData.Element>)
            {
                this.list = list;
            }
        }
        export namespace InstanceData
        {
            export function Load(itemInstance: PlayFabServerModels.ItemInstance): InstanceData | null
            {
                if (itemInstance.CustomData == null)
                {

                }
                else
                {
                    var json = itemInstance.CustomData[Upgrades.ID];

                    if (json == null)
                    {

                    }
                    else
                    {
                        var object = JSON.parse(json);

                        return new InstanceData(object);
                    }
                }

                return null;
            }

            export function Save(playerID: string, itemInstance: PlayFabServerModels.ItemInstance, data: InstanceData)
            {
                let json = data.ToJson();

                PlayFab.Player.Inventory.UpdateItemData(playerID, itemInstance.ItemInstanceId, API.Upgrades.ID, json);
            }

            export class Element
            {
                type: string;
                value: number;

                constructor(name: string, value: number)
                {
                    this.type = name;
                    this.value = value;
                }
            }
        }

        export class ItemData
        {
            template: string;
            applicable: string[];

            constructor(object: IItemData)
            {
                this.template = object.template;
                this.applicable = object.applicable;
            }
        }
        interface IItemData
        {
            template: string;
            applicable: string[];
        }
        export namespace ItemData
        {
            export const Default = "Default";

            export function Load(catalogItem: PlayFabServerModels.CatalogItem): ItemData | null
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

        export class Template
        {
            name: string;
            elements: Template.Element[];

            Find(name: string): Template.Element | null
            {
                for (let i = 0; i < this.elements.length; i++)
                    if (this.elements[i].type == name)
                        return this.elements[i];

                return null;
            }

            Match(name: string, data: InstanceData): Template.Rank | null
            {
                let element = this.Find(name);
                if (element == null) return null;

                let dataElement = data.Find(name);
                if (dataElement == null) return null;

                return element.ranks[dataElement.value];
            }

            constructor(object: ITemplate)
            {
                this.name = object.name;
                this.elements = object.elements;
            }
        }
        interface ITemplate
        {
            name: string;
            elements: Template.Element[];
        }
        export namespace Template
        {
            export function Find(json: string, name: string): ItemData | null
            {
                if (json == null) return null;

                if (name == null) return null;

                let object = JSON.parse(json);

                let target = object.find(x => x.name == name);

                let template = Object.assign(new Instance(), target);

                return template;
            }
            export function Parse(json: string): Instance
            {
                let object = JSON.parse(json);

                let instance = Object.assign(new Instance(), object);

                return instance;
            }
            function ParseAll(json: string)
            {
                var titleData = PlayFab.Title.Data.RetrieveAll([API.Upgrades.ID]);

                if (titleData.Data[API.Upgrades.ID])
            }

            export class Element
            {
                type: string;
                ranks: Rank[];

                constructor(type: string, ranks: Rank[])
                {
                    this.type = type;
                    this.ranks = ranks;
                }
            }

            export class Rank
            {
                cost: API.Cost;
                percentage: number;
                requirements: API.ItemStack[]

                constructor(cost: API.Cost, percentage: number, requirements: API.ItemStack[])
                {
                    this.cost = cost;
                    this.percentage = percentage;
                    this.requirements = requirements;
                }
            }
        }
    }
}