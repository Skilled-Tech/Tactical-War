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

        export namespace Template
        {
            export function Retrieve(): Data
            {
                var json = PlayFab.Title.Data.Retrieve(Rewards.ID);

                var object = JSON.parse(json);

                var instance = <Data>Object.assign(new Data(), object);

                return instance;
            }

            export class Data
            {
                signup: API.Rewards.Type;
                daily: API.Rewards.Type[];
            }
        }

        export namespace PlayerData
        {
            export function Retrieve(playerID: string): Instance
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

                        let object = JSON.parse(json);

                        let instance = Object.assign(new Instance(), object);

                        return instance;
                    }
                }

                return null;
            }

            export function Save(playerID: string, data: Instance)
            {
                PlayFab.Player.Data.ReadOnly.Write(playerID, Rewards.ID, JSON.stringify(data));
            }

            export function Update(playerID: string, data: Instance)
            {
                data.daily.timestamp = new Date().toJSON();

                Save(playerID, data);
            }

            export class Instance
            {
                daily: Daily.Data;

                constructor()
                {
                    this.daily = new Daily.Data();
                }
            }

            export namespace Daily
            {
                export function Incremenet(data: Instance, templates: API.Rewards.Template.Data)
                {
                    data.daily.progress++;

                    if (data.daily.progress >= templates.daily.length)
                        data.daily.progress = 0;
                }

                export class Data
                {
                    timestamp: string;
                    progress: number;

                    constructor()
                    {
                        this.timestamp = new Date().toJSON();
                        this.progress = 0;
                    }
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
        }
    }
}