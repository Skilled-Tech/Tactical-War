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

            public Add(name: string)
            {
                var region = PlayerData.Region.Create(this, this.size, name, 0);
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
        }
    }
}