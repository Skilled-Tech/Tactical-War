namespace API
{
    export namespace Upgrades
    {
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

            constructor(source: ITemplate)
            {
                this.name = source.name;

                this.elements = [];
                for (let i = 0; i < source.elements.length; i++)
                {
                    let instance = new Template.Element(this, i, source.elements[i]);

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

            export class Element
            {
                type: string;
                cost: FactorialValue;
                percentage: FactorialValue;
                requirements: ItemStack[][];

                public get template(): Template { return this.$template; }
                public get index(): number { return this.$index; }

                constructor(private $template: Template, private $index: number, source: IElement)
                {
                    this.type = source.type;

                    this.cost = FactorialValue.Create(source.cost);

                    this.percentage = FactorialValue.Create(source.percentage);

                    this.requirements = [];
                    for (let x = 0; x < source.requirements.length; x++)
                    {
                        this.requirements.push([]);
                        for (let y = 0; y < source.requirements[x].length; y++)
                        {
                            var stack = ItemStack.FromText(source.requirements[x][y]);

                            this.requirements[x].push(stack);
                        }
                    }
                }
            }
            export interface IElement
            {
                type: string;

                cost: IFactorialValue;
                percentage: IFactorialValue;

                requirements: string[][];
            }
            export namespace Element
            {

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