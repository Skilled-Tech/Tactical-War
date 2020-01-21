namespace API
{
    export namespace NewPlayerReward
    {
        export class Template
        {
            token: string;
            items: string[];

            constructor(source: ITemplate)
            {
                this.token = source.token;
                this.items = source.items;
            }

            static Retrieve(): Template
            {
                var json = PlayFab.Title.Data.Retrieve(API.NewPlayerReward.ID);

                if (json == null)
                    throw ("no new player reward template defined");

                var source = JSON.parse(json) as ITemplate;

                var instance = new Template(source);

                return instance;
            }
        }
        export interface ITemplate
        {
            token: string;
            items: string[];
        }
    }
}