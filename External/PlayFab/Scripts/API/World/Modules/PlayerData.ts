namespace API
{
    export namespace World
    {
        export class PlayerData implements IPlayerData
        {
            regions: Array<PlayerData.Region>

            public get size(): number { return this.regions.length; }

            public Contains(name: string): boolean
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return true;

                return false;
            }

            public Find(name: string): PlayerData.Region | null
            {
                for (let i = 0; i < this.regions.length; i++)
                    if (this.regions[i].name == name)
                        return this.regions[i];

                return null;
            }

            public Add(name: string): PlayerData.Region
            {
                var region = PlayerData.Region.Create(this, this.size, name, 0);

                this.regions.push(region);

                return region;
            }

            constructor(source: IPlayerData)
            {
                this.regions = [];

                for (let i = 0; i < source.regions.length; i++)
                {
                    let copy = new PlayerData.Region(this, i, source.regions[i]);

                    this.regions.push(copy);
                }
            }

            //Static
            static Retrieve(playerID: string): PlayerData | null
            {
                var json = PlayFab.Player.Data.ReadOnly.Read(playerID, API.World.ID);

                if (json == null) return null;

                var instance = MyJSON.Read(PlayerData, json);

                return instance;
            }

            static Create(): PlayerData
            {
                let source: IPlayerData =
                {
                    regions: []
                };

                var instance = new PlayerData(source);

                return instance;
            }

            static Save(playerID: string, data: PlayerData)
            {
                let json = MyJSON.Write(data);

                PlayFab.Player.Data.ReadOnly.Write(playerID, API.World.ID, json);
            }
        }
        export interface IPlayerData
        {
            regions: Array<PlayerData.Region>
        }
        export namespace PlayerData
        {
            export class Region implements IRegion
            {
                name: string;
                progress: Region.Progress;

                Increment()
                {
                    this.progress.count += 1;
                }

                public get playerData(): PlayerData { return this.$playerData; }
                public get index(): number { return this.$index; }

                constructor(private $playerData: PlayerData, private $index: number, source: IRegion)
                {
                    this.name = source.name;
                    this.progress = new Region.Progress(source.progress);
                }

                public static Create(playerData: PlayerData, index: number, name: string, progress: number): Region
                {
                    let source: IRegion =
                    {
                        name: name,
                        progress: {
                            count: progress,
                            difficulty: API.Difficulty.Normal,
                        }
                    }

                    var instance = new Region(playerData, index, source);

                    return instance;
                }
            }
            export interface IRegion
            {
                name: string;
                progress: Region.IProgress;
            }
            export namespace Region
            {
                export class Progress implements IProgress
                {
                    count: number;
                    difficulty: API.Difficulty;

                    public To(difficulty: API.Difficulty)
                    {
                        this.count = 0;
                        this.difficulty = difficulty;
                    }

                    constructor(source: IProgress)
                    {
                        this.count = source.count;
                        this.difficulty = source.difficulty;
                    }
                }
                export interface IProgress
                {
                    count: number;
                    difficulty: API.Difficulty;
                }
            }

            export class Snapshot
            {
                data: API.World.PlayerData
                region: API.World.PlayerData.Region;
                occurrence: API.World.Level.Finish.Occurrence;

                constructor(data: API.World.PlayerData,
                    region: API.World.PlayerData.Region,
                    occurrence: API.World.Level.Finish.Occurrence)
                {
                    this.data = data;
                    this.region = region;
                    this.occurrence = occurrence;
                }

                static Retrieve(args: IFinishLevelArguments, template: Template.Snapshot): Snapshot
                {
                    let data = API.World.PlayerData.Retrieve(currentPlayerId);
                    let firstTime = false;
                    if (data == null) //first time for the player finishing any level
                    {
                        data = API.World.PlayerData.Create();

                        firstTime = true;
                    }

                    let region = data.Find(args.region);
                    if (region == null)
                    {
                        if (template.region.previous != null)
                        {
                            let previous = data.Find(template.region.previous.name);

                            if (previous == null)
                                throw ("trying to index region " + args.region + " without unlocking the previous region: " + template.region.previous.name);
                            else
                            {
                                if (previous.progress.count < template.region.previous.size)
                                {
                                    if (previous.progress.difficulty > API.Difficulty.Normal)
                                    {

                                    }
                                    else
                                        throw ("trying to index region " + args.region + " without finishing the previous region: " + template.region.previous.name);
                                }
                            }
                        }

                        region = data.Add(args.region);
                    }

                    if (args.difficulty > region.progress.difficulty) //player sending different difficulty than the one we have saved
                    {
                        if (region.progress.difficulty + 1 == args.difficulty) //this is directly the next difficulty
                        {
                            if (region.progress.count < template.region.size)
                                throw ("can't progress difficulty, region not completed at difficulty " + region.progress.difficulty + " yet");

                            region.progress.To(args.difficulty);
                        }
                        else //player trying to jump difficulty
                        {
                            throw ("can't change difficulty from " + region.progress.difficulty + " to " + args.difficulty);
                        }
                    }

                    if (args.level > region.progress.count)
                    {
                        if (args.difficulty >= region.progress.difficulty)
                        {
                            throw ("trying to complete level of index " + args.level + " without completing the previous levels");
                        }
                    }

                    let occurrence: API.World.Level.Finish.Occurrence;

                    if (region.progress.count == args.level && args.difficulty == region.progress.difficulty)
                        occurrence = Level.Finish.Occurrence.Initial;
                    else
                        occurrence = Level.Finish.Occurrence.Recurring;

                    return new Snapshot(data, region, occurrence);
                }
            }
        }
    }
}