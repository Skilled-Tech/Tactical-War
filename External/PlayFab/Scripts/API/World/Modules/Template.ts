namespace API
{
    export namespace World
    {
        export class Template
        {
            regions: Array<Template.Region>;

            public get size(): number { return this.regions.length; }

            public Find(name: string): Template.Region | null
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return this.regions[i];

                return null;
            }

            constructor(instance: Template)
            {
                this.regions = [];
                for (let i = 0; i < instance.regions.length; i++)
                {
                    let copy = new Template.Region(this, i, instance.regions[i]);

                    this.regions.push(copy);
                }
            }
        }
        export namespace Template
        {
            export function Retrieve(): Template
            {
                var json = PlayFab.Title.Data.Retrieve(API.World.ID);

                if (json == null)
                    throw "no World Template data defined within PlayFab Title Data";

                var instance = MyJSON.Read(Template, json);

                return instance;
            }

            export class Region
            {
                name: string;
                levels: Array<Region.Level>

                public get template(): Template { return this.$template; }
                public get index(): number { return this.$index; }

                public get size(): number { return this.levels.length; }

                public Find(index: number): Region.Level | null
                {
                    if (index < 0) return null;
                    if (index + 1 > this.levels.length) return null;

                    return this.levels[index];
                }

                public get previous(): Region | null
                {
                    if (this.index == 0) return null;

                    return this.template.regions[this.index - 1];
                }

                public get next(): Region | null
                {
                    if (this.index + 1 == this.template.size) return null;

                    return this.template.regions[this.index + 1];
                }

                public get isFirst(): boolean { return this.index == 0; }
                public get isLast(): boolean { return this.index + 1 >= this.template.size; }

                constructor(private $template: Template, private $index: number, instance: Region)
                {
                    this.name = instance.name;

                    this.levels = [];
                    for (let i = 0; i < instance.levels.length; i++)
                    {
                        let copy = new Region.Level(this, i, instance.levels[i]);

                        this.levels.push(copy);
                    }
                }
            }
            export namespace Region
            {
                export class Level
                {
                    reward: Level.Rewards;

                    public get region(): Region { return this.$region; }
                    public get index(): number { return this.$index; }

                    public get previous(): Level | null
                    {
                        if (this.index == 0) return null;

                        return this.$region.levels[this.index - 1];
                    }

                    public get next(): Level | null
                    {
                        if (this.index + 1 == this.region.size) return null;

                        return this.region.levels[this.index + 1];
                    }

                    public get isFirst(): boolean { return this.index == 0; }
                    public get isLast(): boolean { return this.index + 1 >= this.region.size; }

                    constructor(private $region: Region, private $index: number, instance: Level)
                    {
                        this.reward = instance.reward;
                    }
                }
                export namespace Level
                {
                    export class Rewards
                    {
                        initial: API.Reward;
                        recurring: API.Reward;

                        constructor(initial: API.Reward, constant: API.Reward)
                        {
                            this.initial = initial;
                            this.recurring = constant;
                        }
                    }
                }
            }
        }
    }
}