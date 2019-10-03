namespace API
{
    export namespace DailyReward
    {
        export const ID = "daily reward";

        export class Template
        {
            list: API.Reward[];

            public get max(): number { return this.list.length; }

            public Get(index: number): Reward
            {
                if (index < 0 || index + 1 > this.max)
                    throw "no rewars defined for index: " + index;

                return this.list[index];
            }

            constructor(list: Array<API.Reward>)
            {
                this.list = list;
            }
        }
        export namespace Template
        {
            export function Retrieve(): Template
            {
                var json = PlayFab.Title.Data.Retrieve(Reward.ID);

                if (json == null) throw "no rewards template defined";

                var object = <ITemplate>JSON.parse(json);

                var instance = new Template(object);

                return instance;
            }
        }
    }

    export class PlayerData
    {
        timestamp: string;
        progress: number;

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
            PlayFab.Player.Data.ReadOnly.Write(playerID, Reward.ID, JSON.stringify(data));
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
    export namespace PlayerData
    {




        export class Daily
        {

        }
        export namespace Daily
        {
            export function Incremenet(data: PlayerData, templates: API.Reward.Template)
            {
                data.daily.progress++;

                if (data.daily.progress >= templates.daily.length)
                    data.daily.progress = 0;
            }

            export function Create(): Daily
            {
                var instance = new Daily(new Date().toJSON(), 0);

                return instance;
            }
        }
    }

    export class Result
    {
        progress: number;
        items: string[];

        constructor(progress: number, items: string[])
        {
            this.progress = progress;
            this.items = items;
        }
    }
}
}