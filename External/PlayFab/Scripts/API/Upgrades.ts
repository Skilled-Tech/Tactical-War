namespace API
{
    export namespace Upgrades
    {
        export const ID = "upgrades";

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
            export function Load(itemInstance: PlayFabServerModels.ItemInstance, upgradeType: string): InstanceData
            {
                let data: InstanceData | null = null;

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

                        data = new InstanceData(object);
                    }
                }

                if (data == null)
                    data = new InstanceData([]);

                return data;
            }

            export function Save(playerID: string, itemInstance: PlayFabServerModels.ItemInstance, data: InstanceData)
            {
                var itemInstanceID = itemInstance.ItemInstanceId;

                if (itemInstanceID == null)
                {
                    log.debug("No Instance ID defined for item instance");
                    return;
                }

                PlayFab.Player.Inventory.UpdateItemData(playerID, itemInstanceID, API.Upgrades.ID, data.ToJson());
            }

            export class Instance
            {
                data: InstanceData;

                element: Element;
                GetElement(upgradeType: string): Element
                {
                    if (this.data.Contains(upgradeType))
                    {

                    }
                    else
                    {
                        this.data.Add(upgradeType);
                    }

                    let result = this.data.Find(upgradeType);

                    if (result == null)
                        throw "Upgrade type " + upgradeType + " not defined in itemInstanceData";

                    return result;
                }

                public get rank(): number { return this.element.value; }
                public set rank(value: number) { this.element.value = value; }

                constructor(itemInstance: PlayFabServerModels.ItemInstance, args: IUpgradeItemArguments)
                {
                    this.data = Load(itemInstance, args.upgradeType);

                    this.element = this.GetElement(args.upgradeType);
                }
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
            export function Find(name: string): Template | null
            {
                let list = GetAll();

                if (list == null) return null;

                for (let i = 0; i < list.length; i++)
                    if (list[i].name == name)
                        return new Template(list[i]);

                return null;
            }
            export function GetAll(): ITemplate[] | null
            {
                let json = PlayFab.Title.Data.Retrieve(API.Upgrades.ID);

                if (json == null) return null;

                var object = <ITemplate[]>JSON.parse(json);

                return object;
            }

            export class Instance
            {
                data: Template;
                GetTemplate(name: string): Template
                {
                    let result = Template.Find(name);

                    if (result == null)
                        throw "no " + name + " upgrade template found";

                    return result;
                }

                element: Element;
                GetElement(upgradeType: string): Element
                {
                    let result = this.data.Find(upgradeType);

                    if (result == null)
                        throw "no " + upgradeType + " upgrade type found in " + this.data.name + " upgrade template";

                    return result;
                }

                constructor(itemData: ItemData, upgradeType: string)
                {
                    this.data = this.GetTemplate(itemData.template);

                    this.element = this.GetElement(upgradeType);
                }
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