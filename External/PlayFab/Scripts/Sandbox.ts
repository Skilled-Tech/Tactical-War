export namespace Sandbox
{
    export class Data implements Data.Interface
    {
        property: Data.Property;

        ToJSON(): string
        {
            var object = this.ToInterface();

            return JSON.stringify(object);
        }
        ToInterface(): Data.Interface
        {
            return this;
        }

        constructor(object: Data.Interface)
        {
            this.property = new Data.Property(object.property);
        }
    }
    export namespace Data
    {
        export interface Interface
        {
            property: Data.Property.Interface;
        }

        export class Property implements Property.Interface
        {
            text: string;

            call()
            {
                console.log(this.text);
            }

            constructor(object: Property.Interface)
            {
                this.text = object.text;
            }
        }
        export namespace Property
        {
            export interface Interface
            {
                text: string;
            }
        }
    }

    var data = new Data({
        property: {
            text: "Hello World"
        }
    });

    data.property.call();
}