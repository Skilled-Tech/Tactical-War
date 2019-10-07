namespace API
{
    export namespace Upgrades
    {
        export class Template implements ITemplate
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

            constructor(object: ITemplate)
            {
                this.name = object.name;

                this.elements = [];
                for (let i = 0; i < object.elements.length; i++)
                {
                    let instance = new Template.Element(this, i, object.elements[i]);

                    this.elements.push(instance);
                }
            }
        }
        export interface ITemplate
        {
            name: string;
            elements: Template.IElement[];
        }
        export namespace Template
        {
            export const Default = "Default";

            export function GetDefault(): Template
            {
                var result = Find(Default);

                if (result == null)
                    throw ("no " + Default + " upgrade template defined");

                return result;
            }

            export function Find(name: string): Template | null
            {
                let list = GetAll();

                for (let i = 0; i < list.length; i++)
                    if (list[i].name == name)
                        return new Template(list[i]);

                return null;
            }
            export function GetAll(): Array<ITemplate>
            {
                let json = PlayFab.Title.Data.Retrieve(API.Upgrades.ID);

                if (json == null)
                    throw ("no upgrades templates defined");

                var object = <ITemplate[]>JSON.parse(json);

                return object;
            }

            export class Element implements IElement
            {
                type: string;
                ranks: Element.Rank[];

                public get template(): Template { return this.$template; }
                public get index(): number { return this.$index; }

                constructor(private $template: Template, private $index: number, source: IElement)
                {
                    this.type = source.type;

                    this.ranks = [];
                    for (let i = 0; i < source.ranks.length; i++)
                    {
                        let instance = new Element.Rank(this, i, source.ranks[i]);

                        this.ranks.push(instance);
                    }
                }
            }
            export interface IElement
            {
                type: string;
                ranks: Element.IRank[];
            }
            export namespace Element
            {
                export class Rank implements IRank
                {
                    cost: API.Cost;
                    percentage: number;
                    requirements: API.ItemStack[]

                    public get element(): Element { return this.$element; }
                    public get index(): number { return this.$index; }

                    public get previous(): Rank | null
                    {
                        if (this.index == 0) return null;

                        return this.$element.ranks[this.index - 1];
                    }

                    public get next(): Rank | null
                    {
                        if (this.index + 1 == this.element.ranks.length) return null;

                        return this.element.ranks[this.index + 1];
                    }

                    public get isFirst(): boolean { return this.index == 0; }
                    public get isLast(): boolean { return this.index + 1 >= this.element.ranks.length; }

                    constructor(private $element: Element, private $index: number, source: IRank)
                    {
                        this.cost = source.cost;
                        this.percentage = source.percentage;
                        this.requirements = source.requirements;
                    }
                }
                export interface IRank
                {
                    cost: API.Cost;
                    percentage: number;
                    requirements: API.ItemStack[]
                }
            }

            export class Snapshot
            {
                data: API.Upgrades.Template;
                element: API.Upgrades.Template.Element;

                constructor(data: API.Upgrades.Template,
                    element: API.Upgrades.Template.Element)
                {
                    this.data = data;
                    this.element = element;
                }

                static Retrieve(args: IUpgradeItemArguments, itemData: API.Upgrades.ItemData): Snapshot
                {
                    let data: API.Upgrades.Template | null;

                    if (itemData.template == null)
                    {
                        data = API.Upgrades.Template.GetDefault();
                    }
                    else
                    {
                        data = API.Upgrades.Template.Find(itemData.template);

                        if (data == null)
                            throw ("no " + itemData.template + " upgrade template defined");
                    }

                    let element = data.Find(args.upgradeType);

                    if (element == null)
                        throw ("upgrade type " + args.upgradeType + " not defined within " + data.name + " upgrade template");

                    return {
                        data: data,
                        element: element,
                    };
                }
            }
        }
    }
}