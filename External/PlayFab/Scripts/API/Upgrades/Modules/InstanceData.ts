namespace API
{
    export namespace Upgrades
    {
        export class InstanceData
        {
            list: Array<InstanceData.Element>;

            public Add(type: string): InstanceData.Element
            {
                var element = new InstanceData.Element(type, 0);

                this.list.push(element);

                return element;
            }

            public Contains(type: string): boolean
            {
                for (let i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return true;

                return false;
            }

            public Find(type: string): InstanceData.Element | null
            {
                for (let i = 0; i < this.list.length; i++)
                    if (this.list[i].type == type)
                        return this.list[i];

                return null;
            }

            public ToJson(): string
            {
                return JSON.stringify(this.list);
            }

            constructor(list: Array<InstanceData.Element>)
            {
                this.list = list;
            }
        }
        export namespace InstanceData
        {
            export function Load(itemInstance: PlayFabServerModels.ItemInstance, upgradeType: string): InstanceData
            {
                let data: InstanceData | null = null;

                if (itemInstance.CustomData == null)
                {

                }
                else
                {
                    var json = itemInstance.CustomData[Upgrades.ID];

                    if (json == null)
                    {

                    }
                    else
                    {
                        var object = JSON.parse(json);

                        data = new InstanceData(object);
                    }
                }

                if (data == null)
                    data = new InstanceData([]);

                return data;
            }

            export function Save(playerID: string, itemInstance: PlayFabServerModels.ItemInstance, data: InstanceData)
            {
                var itemInstanceID = itemInstance.ItemInstanceId;

                if (itemInstanceID == null)
                {
                    log.debug("No Instance ID defined for item instance");
                    return;
                }

                PlayFab.Player.Inventory.UpdateItemData(playerID, itemInstanceID, API.Upgrades.ID, data.ToJson());
            }

            export class SnapShot
            {
                data: InstanceData;

                element: Element;
                GetElement(upgradeType: string, itemData: ItemData): Element
                {
                    if (this.data.Contains(upgradeType))
                    {

                    }
                    else
                    {
                        function isApplicable(): boolean
                        {
                            for (let i = 0; i < itemData.applicable.length; i++)
                                if (upgradeType == itemData.applicable[i])
                                    return true;

                            return false;
                        }

                        if (isApplicable())
                            this.data.Add(upgradeType);
                        else
                            throw "upgrade type " + upgradeType + " not applicable";
                    }

                    let result = this.data.Find(upgradeType);

                    if (result == null)
                        throw "Upgrade type " + upgradeType + " not defined in itemInstanceData";

                    return result;
                }

                public get rank(): number { return this.element.value; }
                public set rank(value: number) { this.element.value = value; }

                Increment(upgradeType: string)
                {
                    this.rank += 1;
                }

                constructor(itemInstance: PlayFabServerModels.ItemInstance, itemData: ItemData, args: IUpgradeItemArguments)
                {
                    this.data = Load(itemInstance, args.upgradeType);

                    this.element = this.GetElement(args.upgradeType, itemData);
                }
            }

            export class Element
            {
                type: string;
                value: number;

                constructor(name: string, value: number)
                {
                    this.type = name;
                    this.value = value;
                }
            }
        }
    }
}