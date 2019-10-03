namespace API
{
    export namespace World
    {
        export const Name = "world";

        export class PlayerData
        {
            regions: PlayerData.Region[];

            Add(region: PlayerData.Region)
            {
                this.regions.push(region);
            }

            Contains(name: string): boolean
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return true;

                return false;
            }

            Find(name: string): PlayerData.Region | null
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return this.regions[i];

                return null;
            }

            constructor(object: IPlayerData)
            {
                this.regions = object.regions;
            }
        }
        interface IPlayerData
        {
            regions: PlayerData.Region[];
        }
        export namespace PlayerData
        {
            export function Retrieve(playerID: string): PlayerData
            {
                let json = PlayFab.Player.Data.ReadOnly.Read(playerID, Name);

                let object: IPlayerData;

                if (json == null)
                    object = { regions: [] };
                else
                    object = JSON.parse(json);

                let instance = new PlayerData(object);

                return instance;
            }

            export function Save(playerID: string, data: PlayerData)
            {
                let json = JSON.stringify(data);

                PlayFab.Player.Data.ReadOnly.Write(playerID, Name, json);
            }

            export class SnapShot
            {
                data: PlayerData;

                region: Region;
                GetRegion(world: Template.SnapShot, name: string): Region
                {
                    if (this.data.Contains(name))
                    {

                    }
                    else
                    {
                        if (this.HasAccessToRegion(world, name))
                        {
                            let instance = new Region(name, 0);

                            this.data.Add(instance);
                        }
                        else
                        {
                            throw "Trying to index " + name + " region without having access to that region";
                        }
                    }

                    var result = this.data.Find(name);

                    if (result == null)
                        throw "No " + name + " region found in player data";

                    return result;
                }

                HasAccessToRegion(world: Template.SnapShot, currentRegion: string): boolean
                {
                    var index = world.template.IndexOf(currentRegion);

                    if (index == null)
                        throw "No region index for " + name + " was found in world template";

                    if (index == 0)
                        return true;

                    var previousRegion = world.template.regions[index - 1];

                    var previousRegionData = this.data.Find(previousRegion.name);

                    if (previousRegionData == null) return false;

                    return previousRegionData.progress >= previousRegion.levels.length;
                }

                get progress(): number { return this.region.progress; }
                set progress(value: number) { this.region.progress = value; }

                Increment()
                {
                    this.progress += 1;
                }

                constructor(playerID: string, instance: Template.SnapShot, region: string)
                {
                    this.data = Retrieve(playerID);

                    this.region = this.GetRegion(instance, region);
                }
            }

            export class Region
            {
                name: string;
                progress: number;

                constructor(name: string, progress: number)
                {
                    this.name = name;
                    this.progress = progress;
                }
            }
        }

        export class Template
        {
            regions: Template.Region[];


            public get Last(): Template.Region
            {
                return this.regions[this.regions.length - 1];
            }


            public Find(name: string): Template.Region | null
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return this.regions[i];

                return null;
            }

            public Contains(name: string): boolean
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return true;

                return false;
            }

            public IndexOf(name: string): number | null
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return i;

                return null;
            }

            constructor(object: ITemplate)
            {
                this.regions = object.regions;
            }
        }
        interface ITemplate
        {
            regions: Template.Region[];
        }
        export namespace Template
        {
            export function Retrieve(): Template
            {
                let json = PlayFab.Title.Data.Retrieve(Name);

                if (json == null)
                    throw "No world template defined";

                let object = JSON.parse(json);

                var data = new Template(object);

                return data;
            }

            export function Validate(data: Template, args: IFinishLevelArguments)
            {
                let region = data.Find(args.region);

                if (region == null)
                {
                    throw args.region + " region doesn't exist";
                }
                else
                {
                    if (args.level >= 0 && args.level < region.levels.length)
                    {
                        return;
                    }
                    else
                    {
                        throw "Level " + args.level + " on " + args.region + " region doesn't exist";
                    }
                }
            }

            export class SnapShot
            {
                template: Template;

                region: Region;
                GetRegion(name: string): Region
                {
                    let result = this.template.Find(name);

                    if (result == null)
                        throw "No region named " + name + " found in world template";

                    return result;
                }

                level: Level;
                GetLevel(index: number): Level
                {
                    let result = this.region.levels[index];

                    if (result == null)
                        throw "No level indexed " + index + " found in " + this.region.name + " world region template";

                    return result;
                }

                constructor(region: string, level: number)
                {
                    this.template = API.World.Template.Retrieve();

                    this.region = this.GetRegion(region);

                    this.level = this.GetLevel(level);
                }
            }

            export class Region
            {
                name: string;
                levels: Level[];

                constructor(name: string, levels: Level[])
                {
                    this.name = name;
                    this.levels = levels;
                }
            }

            export class Level
            {
                reward: Rewards;

                constructor(reward: Rewards)
                {
                    this.reward = reward;
                }
            }

            export class Rewards
            {
                initial: API.Rewards.Type;
                constant: API.Rewards.Type;

                constructor(initial: API.Rewards.Type, constant: API.Rewards.Type)
                {
                    this.initial = initial;
                    this.constant = constant;
                }
            }
        }
    }
}

namespace API2
{
    namespace World
    {
        export namespace PlayerData
        {
            export class World
            {
                regions: Array<World.Region>

                public Find(name: string): World.Region | null
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return this.regions[i];

                    return null;
                }

                constructor(instance: World)
                {
                    this.regions = [];

                    for (let i = 0; i < instance.regions.length; i++)
                    {
                        let copy = new World.Region(this, i, instance.regions[i]);

                        this.regions.push(copy);
                    }
                }
            }
            export namespace World
            {
                export function Retrieve()
                {

                }

                export class Region
                {
                    name: string;
                    progress: number;

                    public get world(): World { return this.$world; }
                    public get index(): number { return this.$index; }

                    constructor(private $world: World, private $index: number, instance: Region)
                    {
                        this.name = instance.name;
                        this.progress = instance.progress;
                    }
                }
            }
        }

        export namespace Template
        {
            export class World
            {
                regions: Array<World.Region>;

                public get size(): number { return this.regions.length; }

                public Find(name: string): World.Region | null
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return this.regions[i];

                    return null;
                }

                constructor(instance: World)
                {
                    this.regions = [];
                    for (let i = 0; i < instance.regions.length; i++)
                    {
                        let copy = new World.Region(this, i, instance.regions[i]);

                        this.regions.push(copy);
                    }
                }
            }
            export namespace World
            {
                export class Region
                {
                    name: string;
                    levels: Array<Region.Level>

                    public Find(index: number): Region.Level | null
                    {
                        if (index < 0) return null;
                        if (index + 1 > this.levels.length) return null;

                        return this.levels[index];
                    }

                    public get size(): number { return this.levels.length; }

                    public get index(): number { return this.$index; }
                    public get world(): World { return this.$world; }

                    public get previous(): Region | null
                    {
                        if (this.index == 0) return null;

                        return this.world.regions[this.index - 1];
                    }

                    public get next(): Region | null
                    {
                        if (this.index + 1 == this.world.size) return null;

                        return this.world.regions[this.index + 1];
                    }

                    public get isLast(): boolean { return this.index + 1 >= this.world.size; }


                    constructor(private $world: World, private $index: number, instance: Region)
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
                        public get index(): number { return this.$index; }
                        public get region(): Region { return this.$region; }

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

                        public get isLast(): boolean { return this.index + 1 >= this.region.size; }

                        constructor(private $region: Region, private $index: number, instance: Level)
                        {

                        }
                    }
                }
            }
        }
    }
}