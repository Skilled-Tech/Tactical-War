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
    }
}