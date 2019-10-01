namespace API
{
    export namespace Upgrades
    {
        export const ID = "upgrades";

        export namespace InstanceData
        {
            export function Load(itemInstance: PlayFabServerModels.ItemInstance): Instance
            {
                let instance = new Instance;
                if (itemInstance.CustomData == null)
                {

                }
                else
                {
                    if (itemInstance.CustomData[Upgrades.ID] == null)
                    {

                    }
                    else
                    {
                        instance.FromJSON(itemInstance.CustomData[Upgrades.ID]);
                    }
                }

                return instance;
            }

            export function Save(playerID: string, itemInstance: PlayFabServerModels.ItemInstance, instance: Instance)
            {
                let json = instance.ToJson();

                PlayFab.Player.Inventory.UpdateItemData(playerID, itemInstance.ItemInstanceId, API.Upgrades.ID, json);
            }

            export class Instance
            {
                list: Element[];

                public Add(type: string)
                {
                    this.list.push(new Element(type, 0));
                }

                public Contains(type: string): boolean
                {
                    for (let i = 0; i < this.list.length; i++)
                        if (this.list[i].type == type)
                            return true;

                    return false;
                }

                public Find(type: string): Element
                {
                    for (let i = 0; i < this.list.length; i++)
                        if (this.list[i].type == type)
                            return this.list[i];

                    return null;
                }

                public Increment(type: string)
                {
                    let element = this.Find(type);

                    element.value++;
                }

                public FromJSON(text: string)
                {
                    var object = JSON.parse(text);

                    this.list = Object.assign([], object);
                }
                public ToJson(): string
                {
                    return JSON.stringify(this.list);
                }

                constructor()
                {
                    this.list = [];
                }
            }

            class Element
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

        export namespace ItemData
        {
            export const Default = "Default";

            export function Load(catalogItem: PlayFabServerModels.CatalogItem): Instance
            {
                if (catalogItem == null) return null;

                if (catalogItem.CustomData == null) return null;

                let object = JSON.parse(catalogItem.CustomData);

                if (object[ID] == null)
                {

                }

                let data = Object.assign(new Instance(), object[ID]) as Instance;

                if (data.template == null) data.template = Default;

                return data;
            }

            export class Instance
            {
                template: string;
                applicable: string[];
            }
        }

        export namespace Template
        {
            export function Find(json: string, name: string): Instance
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

            export class Instance
            {
                name: string;
                elements: Element[];

                Find(name: string): Element
                {
                    for (let i = 0; i < this.elements.length; i++)
                        if (this.elements[i].type == name)
                            return this.elements[i];

                    return null;
                }

                Match(name: string, data: Upgrades.InstanceData.Instance): Rank
                {
                    return this.Find(name).ranks[data.Find(name).value];
                }
            }

            export class Element
            {
                type: string;
                ranks: Rank[];
            }

            export class Rank
            {
                cost: API.Cost;
                percentage: number;
                requirements: API.ItemStack[]
            }
        }
    }
}