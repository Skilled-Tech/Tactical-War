namespace API
{
    export namespace Upgrades
    {
        export class InstanceData
        {
            list: Array<InstanceData.Element>;

            public Find(name: string): InstanceData.Element | null
            {
                for (let i = 0; i < this.list.length; i++)
                    if (this.list[i].type == name)
                        return this.list[i];

                return null;
            }

            public Add(name: string): InstanceData.Element
            {
                let source: InstanceData.IElement =
                {
                    type: name,
                    value: 0,
                };

                let instance = new InstanceData.Element(source);

                return instance;
            }

            public toJSON(): string
            {
                return MyJSON.Write(this.list);
            }

            constructor(source: Array<InstanceData.IElement>)
            {
                this.list = [];
                for (let i = 0; i < source.length; i++)
                {
                    let instance = new InstanceData.Element(source[i]);

                    this.list.push(instance);
                }
            }

            static Load(itemInstance: PlayFabServerModels.ItemInstance): InstanceData | null
            {
                let data: InstanceData | null = null;

                if (itemInstance == null)
                    throw "itemInstance is null, can't load upgrade instance data";

                if (itemInstance.CustomData == null) return null;

                let json = itemInstance.CustomData[API.Upgrades.ID];

                if (json == null) return null;

                var list = JSON.parse(json) as Array<InstanceData.Element>;

                var instance = new InstanceData(list);

                return instance;
            }

            static Create(): InstanceData
            {
                let source: Array<InstanceData.IElement> = [];

                let instance = new InstanceData(source);

                return instance;
            }

            static Save(playerID: string, itemInstance: PlayFabServerModels.ItemInstance, data: InstanceData)
            {
                var itemInstanceID = itemInstance.ItemInstanceId;

                if (itemInstanceID == null)
                {
                    log.debug("No Instance ID defined for item instance");
                    return;
                }

                PlayFab.Player.Inventory.UpdateItemData(playerID, itemInstanceID, API.Upgrades.ID, data.toJSON());
            }
        }
        export namespace InstanceData
        {
            export class Element implements IElement
            {
                type: string;
                value: number;

                public Increment()
                {
                    this.value += 1;
                }

                constructor(source: IElement)
                {
                    this.type = source.type;
                    this.value = source.value;
                }
            }
            export interface IElement
            {
                type: string;
                value: number;
            }
        }
    }
}