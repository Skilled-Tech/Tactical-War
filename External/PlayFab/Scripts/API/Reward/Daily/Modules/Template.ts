namespace API
{
    export namespace DailyReward
    {
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

            //Static
            static Retrieve(): Template
            {
                var json = PlayFab.Title.Data.Retrieve(Reward.ID);

                if (json == null) throw "no rewards template defined";

                var list = JSON.parse(json) as Array<API.Reward>;

                var instance = new Template(list);

                return instance;
            }
        }
    }
}