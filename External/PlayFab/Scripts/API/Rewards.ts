namespace API
{
    export namespace Rewards
    {
        export const ID = "rewards";

        export function Grant(playerID: string, data: Type, annotation: string): string[]
        {
            let IDs = Array<string>();

            if (data.items == null)
            {

            }
            else
            {
                IDs = IDs.concat(data.items);
            }

            if (data.droptable == null)
            {

            }
            else
            {
                let result = PlayFab.Title.Catalog.Tables.Process(data.droptable);
                if (result != null)
                    IDs = IDs.concat(result);
            }

            PlayFab.Title.Catalog.Item.GrantAll(playerID, IDs, annotation);

            return IDs;
        }

        export class Template
        {
            signup: API.Rewards.Type;
            daily: API.Rewards.Type[];

            constructor(object: ITemplate)
            {
                this.signup = object.signup;
                this.daily = object.daily;
            }
        }
        interface ITemplate
        {
            signup: API.Rewards.Type;
            daily: API.Rewards.Type[]
        }
        export namespace Template
        {
            export function Retrieve(): Template | null
            {
                var json = PlayFab.Title.Data.Retrieve(Rewards.ID);

                if (json == null) return null;

                var object = <Template>JSON.parse(json);

                var instance = new Template(object);

                return instance;
            }
        }

        export class PlayerData
        {
            daily: PlayerData.Daily;

            constructor(object: IPlayerData)
            {
                this.daily = object.daily;
            }
        }
        interface IPlayerData
        {
            daily: PlayerData.Daily;
        }
        export namespace PlayerData
        {
            export function Retrieve(playerID: string): PlayerData | null
            {
                let result = PlayFab.Player.Data.ReadOnly.Read(playerID, Rewards.ID);

                if (result.Data == null)
                {

                }
                else
                {
                    if (result.Data[Rewards.ID] == null)
                    {

                    }
                    else
                    {
                        let json = result.Data[Rewards.ID].Value;

                        if (json == null)
                        {

                        }
                        else
                        {
                            let object = JSON.parse(json);

                            let instance = new PlayerData(object);

                            return instance;
                        }
                    }
                }

                return null;
            }

            export function Save(playerID: string, data: PlayerData)
            {
                PlayFab.Player.Data.ReadOnly.Write(playerID, Rewards.ID, JSON.stringify(data));
            }

            export function Update(playerID: string, data: PlayerData)
            {
                data.daily.timestamp = new Date().toJSON();

                Save(playerID, data);
            }

            export class Daily
            {
                timestamp: string;
                progress: number;

                constructor()
                {
                    this.timestamp = new Date().toJSON();
                    this.progress = 0;
                }
            }
            export namespace Daily
            {
                export function Incremenet(data: PlayerData, templates: API.Rewards.Template)
                {
                    data.daily.progress++;

                    if (data.daily.progress >= templates.daily.length)
                        data.daily.progress = 0;
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

        export class Type
        {
            items: [string];
            droptable: DropTable;

            constructor(items: [string], droptable: DropTable)
            {
                this.items = items;
                this.droptable = droptable;
            }
        }
    }
}