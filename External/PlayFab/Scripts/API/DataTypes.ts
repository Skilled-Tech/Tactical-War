namespace API
{
    export class DropTable
    {
        ID: string;
        iterations: number;

        constructor(ID: string, iterations: number)
        {
            this.ID = ID;
            this.iterations = iterations;
        }
    }

    export class Cost
    {
        type: string;
        value: number;

        constructor(type: string, value: number)
        {
            this.type = type;
            this.value = value;
        }
    }

    export class ItemStack
    {
        item: string;
        count: number;

        constructor(item: string, count: number)
        {
            this.item = item;
            this.count = count;
        }

        static Grant(playerID: string, stack: ItemStack, annotation: string)
        {
            PlayFab.Catalog.Item.Grant(playerID, stack.item, stack.count, annotation);
        }
        static GrantAll(playerID: string, stacks: Array<ItemStack>, annotation: string)
        {
            let IDs = [];

            for (let x = 0; x < stacks.length; x++)
                for (let y = 0; y < stacks[x].count; y++)
                    IDs.push(stacks[x].item);

            PlayFab.Catalog.Item.GrantAll(playerID, IDs, annotation);
        }
    }

    export enum Difficulty
    {
        Normal, Hard, Skilled
    }
}