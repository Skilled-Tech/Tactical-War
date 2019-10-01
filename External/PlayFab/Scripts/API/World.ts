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
            export function Retrieve(playerID: string): PlayerData | null
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

            export function Validate(data: PlayerData, world: Template, args: IFinishLevelArguments)
            {
                if (data.Contains(args.region))
                {

                }
                else
                {
                    if (args.region == world.regions[0].name)
                    {
                        let instance = new Region(args.region, 1);

                        data.Add(instance);
                    }
                    else
                    {
                        throw "Can't begin data indexing from " + args.region + " region";
                    }
                }
            }

            export function Incremenet(data: PlayerData, world: Template, args: IFinishLevelArguments)
            {
                let region = data.Find(args.region);

                if (region == null) return;

                region.progress += 1;

                Progress(data, world, region.progress, args);
            }

            export function Progress(data: PlayerData, world: Template, progress: number, args: IFinishLevelArguments)
            {
                let region = world.Find(args.region);

                if (region == null) return;

                if (progress == region.levels.length) //Completed All Levels
                {
                    let index = world.IndexOf(region.name);

                    if (index == null) return;

                    if (index >= world.regions.length - 1) //Completed All Regions
                    {

                    }
                    else
                    {
                        let next = world.regions[index + 1];

                        if (data.Contains(next.name)) //Region Already Unlocked
                        {

                        }
                        else
                        {
                            let instance = new Region(next.name, 1);

                            data.Add(instance);
                        }
                    }
                }
            }

            export function Save(playerID: string, data: PlayerData)
            {
                let json = JSON.stringify(data);

                PlayFab.Player.Data.ReadOnly.Write(playerID, Name, json);
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