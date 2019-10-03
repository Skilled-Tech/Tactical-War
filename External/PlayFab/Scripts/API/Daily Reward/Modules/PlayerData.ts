namespace API
{
    export namespace DailyReward
    {
        export class PlayerData implements IPlayerData
        {
            timestamp: string;

            public get datestamp(): number { return Date.parse(this.timestamp); }

            progress: number;
            public Progress(template: Template)
            {
                this.timestamp = new Date().toJSON();

                this.progress += 1;

                if (this.progress >= template.max)
                    this.progress = 0;
            }

            constructor(source: IPlayerData)
            {
                this.timestamp = source.timestamp;
                this.progress = source.progress;
            }

            //Static
            static Retrieve(playerID: string): PlayerData | null
            {
                let json = PlayFab.Player.Data.ReadOnly.Read(playerID, API.DailyReward.ID);

                if (json == null) return null;

                let instance = MyJSON.Read(PlayerData, json);

                return instance;
            }

            static Create(): PlayerData
            {
                let source: IPlayerData =
                {
                    progress: 0,
                    timestamp: new Date().toJSON()
                };

                let instance = new PlayerData(source);

                return instance;
            }

            static Save(playerID: string, data: PlayerData)
            {
                PlayFab.Player.Data.ReadOnly.Write(playerID, API.DailyReward.ID, JSON.stringify(data));
            }

            static Update(playerID: string, data: PlayerData)
            {
                data.timestamp = new Date().toJSON();

                PlayerData.Save(playerID, data);
            }
        }
        interface IPlayerData
        {
            timestamp: string;
            progress: number;
        }
    }
}