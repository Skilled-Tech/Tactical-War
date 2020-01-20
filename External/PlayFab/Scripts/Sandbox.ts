class Sandbox
{
    static Init()
    {
        if (IsOnPlayFab())
        {

        }
        else
        {
            setTimeout(this.Execute, 50);
        }
    }

    static Execute()
    {
        var json = "{ \"name\": \"Default\", \"elements\": [ { \"type\": \"Power\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] }, { \"type\": \"Defense\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] }, { \"type\": \"Range\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] }, { \"type\": \"Speed\", \"cost\": { \"initial\": 100, \"multiplier\": 100 }, \"percentage\": { \"initial\": 10, \"multiplier\": 5 }, \"requirements\": [ [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ], [ \"[2]Wood_Sword\", \"[2]Wood_Shield\" ] ] } ] }";

        var instance = JSON.parse(json);

        var template = new API.Upgrades.Template(instance);

        console.log(MyJSON.Stringfy(template));
    }
}

Sandbox.Init();