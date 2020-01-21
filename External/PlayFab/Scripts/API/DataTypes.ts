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

        static FromText(text: string): ItemStack
        {
            let start = text.indexOf('[');

            if (start < 0)
                return new ItemStack(text, 1);

            let end = text.indexOf(']');

            if (end < 0)
                return new ItemStack(text, 1);

            let count = Number.parseInt(text.substring(start + 1, end));
            let id = text.substring(end + 1, text.length);

            return new ItemStack(id, count);
        }
    }

    export enum Difficulty
    {
        Normal = 1, Hard = 2, Skilled = 3
    }

    export class FactorialValue
    {
        initial: number;
        multiplier: number;

        Calculate(value: number)
        {
            if (value == 0) return 0;

            return this.initial + (this.multiplier * (value - 1));
        }

        constructor(initial: number, multiplier: number)
        {
            this.initial = initial;
            this.multiplier = multiplier;
        }

        public static Create(source: IFactorialValue): FactorialValue
        {
            return new FactorialValue(source.initial, source.multiplier);
        }
    }
    export interface IFactorialValue
    {
        initial: number;
        multiplier: number;
    }
}