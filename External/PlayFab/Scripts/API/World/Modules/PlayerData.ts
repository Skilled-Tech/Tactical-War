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
                progress: number;

                Increment()
                {
                    this.progress += 1;
                }

                public get playerData(): PlayerData { return this.$playerData; }
                public get index(): number { return this.$index; }

                constructor(private $playerData: PlayerData, private $index: number, source: IRegion)
                {
                    this.name = source.name;
                    this.progress = source.progress;
                }

                public static Create(playerData: PlayerData, index: number, name: string, progress: number): Region
                {
                    let source: IRegion =
                    {
                        name: name,
                        progress: progress,
                    }

                    var instance = new Region(playerData, index, source);

                    return instance;
                }
            }
            export interface IRegion
            {
                name: string;
                progress: number;
            }

            export class Snapshot
            {
                data: API.World.PlayerData
                region: API.World.PlayerData.Region;

                constructor(data: API.World.PlayerData,
                    region: API.World.PlayerData.Region)
                {
                    this.data = data;
                    this.region = region;
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
                        if (template.region.previous == null) //this is the first level
                        {

                        }
                        else
                        {
                            var previous = data.Find(template.region.previous.name);

                            if (previous == null)
                                throw "trying to index region " + args.region + " without unlocking the previous region: " + template.region.previous.name;

                            if (previous.progress < template.region.previous.size)
                                throw "trying to index region " + args.region + " without finishing the previous region: " + template.region.previous.name;
                        }

                        region = data.Add(args.region);
                    }

                    if (args.level > region.progress)
                        throw "trying to complete level of index " + args.level + " without completing the previous levels";

                    return {
                        data: data,
                        region: region
                    };
                }
            }
        }
    }
}