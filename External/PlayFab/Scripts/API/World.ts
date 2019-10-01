namespace API
{
    export namespace World
    {
        export const Name = "world";

        export namespace PlayerData
        {
            export function Retrieve(playerID: string): Instance
            {
                let playerData = PlayFab.Player.Data.ReadOnly.ReadAll(playerID, [Name]);

                if (playerData.Data[Name] == null)
                    return new Instance();

                let json = playerData.Data[Name].Value;

                let object = JSON.parse(json);

                let instance = Object.assign(new Instance(), object);

                return instance;
            }

            export function Validate(data: Instance, world: Template.Data, args: IFinishLevelArguments)
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

            export function Incremenet(data: Instance, world: Template.Data, args: IFinishLevelArguments)
            {
                let progress = data.Find(args.region).progress++;

                let region = world.Find(args.region);

                if (progress == region.levels.length) //Completed All Levels
                {
                    let index = world.IndexOf(region.name);

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

            export function Save(playerID: string, data: Instance)
            {
                let json = JSON.stringify(data);

                PlayFab.Player.Data.ReadOnly.Write(playerID, Name, json);
            }

            export class Instance
            {
                regions: Region[];

                Add(region: Region)
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

                Find(name: string): Region
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return this.regions[i];

                    return null;
                }

                constructor()
                {
                    this.regions = [];
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

        export namespace Template
        {
            export function Retrieve(): Data
            {
                let titleData = PlayFab.Title.Data.RetrieveAll([Name]);

                let json = titleData.Data[Name];

                let object = JSON.parse(json);

                let data = Object.assign(new Data(), object);

                return data;
            }

            export function Validate(data: Data, args: IFinishLevelArguments)
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

            export class Data
            {
                regions: Region[];

                public Find(name: string)
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

                public IndexOf(name: string): number
                {
                    for (let i = 0; i < this.regions.length; i++)
                        if (this.regions[i].name == name)
                            return i;

                    return null;
                }
            }

            export class Region
            {
                name: string;
                levels: Level[];
            }

            export class Level
            {
                reward: Rewards;
            }

            export class Rewards
            {
                initial: API.Rewards.Type;
                constant: API.Rewards.Type;
            }
        }
    }
}